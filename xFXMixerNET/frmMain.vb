Imports NMixerProNET
Imports System.xml

Public Class frmMain
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cbMixers As System.Windows.Forms.ComboBox

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cbMixers = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(32, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Mixer"
        '
        'cbMixers
        '
        Me.cbMixers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbMixers.FormattingEnabled = True
        Me.cbMixers.Location = New System.Drawing.Point(50, 12)
        Me.cbMixers.Name = "cbMixers"
        Me.cbMixers.Size = New System.Drawing.Size(265, 21)
        Me.cbMixers.TabIndex = 1
        '
        'frmMain
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(848, 342)
        Me.Controls.Add(Me.cbMixers)
        Me.Controls.Add(Me.Label1)
        Me.Name = "frmMain"
        Me.Text = "xFXMixerNET"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private mixerPro As CMixerPro = New CMixerPro()
    Private coreAudio As CCoreAudio
    Private useCoreAudio As Boolean

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If CCoreAudio.RequiresCoreAudio Then
            coreAudio = New CCoreAudio(mixerPro)
            For i As Integer = 0 To coreAudio.Mixers.Count - 1
                cbMixers.Items.Add(coreAudio.Mixers(i))
            Next
            useCoreAudio = True
        Else
            For i As Integer = 1 To mixerPro.Mixers.Count
                cbMixers.Items.Add(mixerPro.Mixers(i))
            Next
            useCoreAudio = False
        End If

        If cbMixers.Items.Count > 0 Then cbMixers.SelectedItem = cbMixers.Items(0)
    End Sub

    Private Sub CreateMixer(ByVal m As Object)
        Dim x As Integer = 4
        Dim y As Integer = cbMixers.Bottom + cbMixers.Top
        Dim t As Integer = 0

ReStart:
        For Each ctrl As Control In Me.Controls
            If TypeOf ctrl Is UILine Then
                Me.Controls.Remove(ctrl)
                GoTo ReStart
            ElseIf TypeOf ctrl Is UILine2 Then
                Me.Controls.Remove(ctrl)
                GoTo ReStart
            End If
        Next

        Me.Width = Screen.PrimaryScreen.WorkingArea.Width / 2

        For Each l In m.Lines
            Dim c As Control = Nothing

            If useCoreAudio Then
                c = New UILine2(l)
                AddHandler CType(c, UILine2).Editing, AddressOf AutoResize
            Else
                If (l.ControlsByClass(CControl.ControlClassConstants.cccCLASS_FADER).Count > 0 Or
                l.ControlsByClass(CControl.ControlClassConstants.cccCLASS_SWITCH).Count > 0) And
                l.ControlsByType(CControl.ControlTypeConstants.ctrltcMUX).Count = 0 Then
                    c = New UILine(l)
                    AddHandler CType(c, UILine).Editing, AddressOf AutoResize
                End If
            End If

            If c IsNot Nothing Then
                Me.Controls.Add(c)

                If x + c.Width + 4 > Width Then
                    x = 4
                    y += t + 4
                End If

                c.Left = x
                c.Top = y
                x += c.Width + 4
                t = Math.Max(t, c.Height)
            End If
        Next

        AutoResize()
    End Sub

    Private Sub AutoResize()
        Dim w As Integer = 0
        Dim h As Integer = 0

        For Each c As Control In Me.Controls
            If c.Visible Then
                w = Math.Max(w, c.Left + c.Width)
                h = Math.Max(h, c.Top + c.Height)
            End If
        Next

        Dim ns As Size = New Size(w + 12 + 6, h + 12 + 24 + 4)
        If ns.Width <> Me.Width Or ns.Height <> Me.Height Then
            Me.Size = ns
        End If
    End Sub

    Private Sub frmMain_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        Dim l As UILine
        Dim s As String = ""
        Dim xd As XmlDocument = New XmlDocument

        Dim nMixer As XmlNode = xd.AppendChild(xd.CreateNode(XmlNodeType.Element, "MyMixer", ""))
        Dim nLine As XmlNode
        Dim nControl As XmlNode

        For Each lc As Control In Me.Controls
            If TypeOf lc Is UILine Then
                l = lc
                nLine = nMixer.AppendChild(xd.CreateNode(XmlNodeType.Element, "Line", ""))
                nLine.AppendChild(xd.CreateNode(XmlNodeType.Element, "ProductID", "")).InnerText = l.Line.Parent.ProductID.ToString()
                nLine.AppendChild(xd.CreateNode(XmlNodeType.Element, "ID", "")).InnerText = l.Line.ID
                nLine.AppendChild(xd.CreateNode(XmlNodeType.Element, "Text", "")).InnerText = l.Text
                nLine.AppendChild(xd.CreateNode(XmlNodeType.Element, "ShowDestLine", "")).InnerText = l.ShowDestinationLine.ToString()
                nLine.AppendChild(xd.CreateNode(XmlNodeType.Element, "Left", "")).InnerText = l.Left
                nLine.AppendChild(xd.CreateNode(XmlNodeType.Element, "Top", "")).InnerText = l.Top

                For Each cc As UILineControl In l.Controls
                    nControl = nLine.AppendChild(xd.CreateNode(XmlNodeType.Element, "Control", ""))
                    nControl.AppendChild(xd.CreateNode(XmlNodeType.Element, "ID", "")).InnerText = cc.Tag
                    nControl.AppendChild(xd.CreateNode(XmlNodeType.Element, "Text", "")).InnerText = cc.Text
                    nControl.AppendChild(xd.CreateNode(XmlNodeType.Element, "Left", "")).InnerText = cc.Left
                    nControl.AppendChild(xd.CreateNode(XmlNodeType.Element, "Top", "")).InnerText = cc.Top
                    nControl.AppendChild(xd.CreateNode(XmlNodeType.Element, "Width", "")).InnerText = cc.Width
                    nControl.AppendChild(xd.CreateNode(XmlNodeType.Element, "Height", "")).InnerText = cc.Height
                    nControl.AppendChild(xd.CreateNode(XmlNodeType.Element, "Orientation", "")).InnerText = cc.Orientation
                Next
            End If
        Next

        'xd.Save("C:\Documents and Settings\Administrator\Desktop\test.xml")
    End Sub

    Private Sub cbMixers_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbMixers.SelectedIndexChanged
        CreateMixer(cbMixers.SelectedItem)
    End Sub
End Class
