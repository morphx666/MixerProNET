Imports NMixerProNET

Public Class FormAdv
    Inherits Form

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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents btnClose As Button
    <DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.btnClose = New Button
        Me.SuspendLayout()
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), AnchorStyles)
        Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnClose.Location = New Point(224, 75)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New Size(69, 25)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "Close"
        '
        'frmAdv
        '
        Me.AutoScaleBaseSize = New Size(5, 14)
        Me.ClientSize = New Size(304, 109)
        Me.Controls.Add(Me.btnClose)
        Me.Font = New Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAdv"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmAdv"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Dim l As CLine
    Dim y As Integer = 10

    Private Sub frmAdv_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        l = Me.Tag
        Me.Text = "Advanced Controls for " + l.LongName

        GetAdvControls()

        Me.MinimumSize = New Size(Me.CreateGraphics.MeasureString(Me.Text, New Font(Me.Font, FontStyle.Bold)).Width, y + btnClose.Height * 2)
    End Sub

    Private Sub GetAdvControls()
        Dim tb As TrackBar
        Dim ch As CheckBox

        For Each c As CControl In l.Controls
            If (c.ControlType And CControl.ControlTypeConstants.ctrltcBASS) = CControl.ControlTypeConstants.ctrltcBASS Or
                (c.ControlType And CControl.ControlTypeConstants.ctrltcTREBLE) = CControl.ControlTypeConstants.ctrltcTREBLE Or
                (c.ControlType And CControl.ControlTypeConstants.ctrltcBUTTON) = CControl.ControlTypeConstants.ctrltcBUTTON Or
                (c.ControlType And CControl.ControlTypeConstants.ctrltcLOUDNESS) = CControl.ControlTypeConstants.ctrltcLOUDNESS Or
                (c.ControlType And CControl.ControlTypeConstants.ctrltcSTEREOENH) = CControl.ControlTypeConstants.ctrltcSTEREOENH Then

                Select Case c.ControlClass
                    Case CControl.ControlClassConstants.cccCLASS_FADER
                        AddLabel(c.LongName, y)

                        tb = New TrackBar
                        tb.Location = New Point(30, y)
                        tb.Width = Width - tb.Left * 2 - 15
                        tb.Minimum = c.Min
                        tb.Maximum = c.Max
                        tb.TickFrequency = (tb.Maximum - tb.Minimum) / 10
                        tb.Value = c.UniformValue
                        Controls.Add(tb)
                        c.Binding.Define(tb, "Value", "ValueChanged")

                        y += tb.Height + 2
                    Case CControl.ControlClassConstants.cccCLASS_SWITCH
                        ch = New CheckBox
                        ch.Text = c.LongName
                        ch.Location = New Point(5, y)
                        ch.Width = Width - ch.Left * 2 - 15
                        ch.Checked = (c.UniformValue <> 0)
                        ch.FlatStyle = FlatStyle.System
                        Controls.Add(ch)
                        c.Binding.Define(ch, "Checked", "CheckStateChanged")

                        y += ch.Height + 2
                End Select
            End If
        Next
    End Sub

    Private Sub SwitchChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim ch As CheckBox = sender
        Dim c As CControl = ch.Tag

        c.UniformValue = IIf(ch.Checked, c.Max, c.Min)

        'RefreshAllControls()
    End Sub

    Private Sub AddLabel(ByVal txt As String, ByVal y As Integer)
        Dim lb As New Label

        lb.Text = txt
        lb.Left = 5
        lb.Top = y
        lb.Width = 30
        Controls.Add(lb)
    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnClose.Click
        Close()
    End Sub
End Class
