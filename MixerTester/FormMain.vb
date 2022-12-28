Imports NMixerProNET
Imports NMixerProNET.CLine.LineTypeConstants
Imports NMixerProNET.CControl.ControlClassConstants

Public Class FormMain
    Inherits Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(disposing As Boolean)
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
    Friend WithEvents tvMix As TreeView
    <DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.tvMix = New TreeView()
        Me.SuspendLayout()
        '
        'tvMix
        '
        Me.tvMix.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), AnchorStyles)
        Me.tvMix.Font = New Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tvMix.HideSelection = False
        Me.tvMix.Location = New Point(11, 16)
        Me.tvMix.Name = "tvMix"
        Me.tvMix.Size = New Size(786, 385)
        Me.tvMix.TabIndex = 0
        '
        'FormMain
        '
        Me.AutoScaleBaseSize = New Size(7, 17)
        Me.ClientSize = New Size(811, 530)
        Me.Controls.Add(Me.tvMix)
        Me.Font = New Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "FormMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MixerPro.NET Tester"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private WithEvents cAudio As CCoreAudio
    Private WithEvents mxp As CMixerPro

    Private Delegate Sub SafeSessionsUpdateDelegate(mixer As CCoreAudio.CMixer, node As TreeNode)

    Private chkOp() As CheckBox
    Private ucsCtrl As TrackBar
    Private caFader As VolumeFader

    Private Sub FormMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If cAudio IsNot Nothing Then cAudio.Dispose()
    End Sub

    Private Sub FormMain_Load(sender As System.Object, e As EventArgs) Handles MyBase.Load
        Dim n1 As TreeNode
        Dim n2 As TreeNode

        mxp = New CMixerPro()

        If CCoreAudio.RequiresCoreAudio Then
            cAudio = New CCoreAudio(mxp)

            Dim l1 As CCoreAudio.CLine

            For Each m As CCoreAudio.CMixer In cAudio.Mixers
                n1 = tvMix.Nodes.Add(m.DeviceName + " (" + m.Name + ")")

                If Not m.Enabled Then n1.ForeColor = Color.FromKnownColor(KnownColor.ControlDark)
                If m.Selected Then n1.NodeFont = New Font(tvMix.Font.FontFamily, tvMix.Font.Size, FontStyle.Bold, tvMix.Font.Unit)
                n1.Tag = m

                If m.Line IsNot Nothing Then AddControls(m.Line, n1)

                n2 = n1.Nodes.Add("Lines")
                For Each l1 In m.Lines
                    AddControls(l1, n2)
                Next l1

                n1 = n1.Nodes.Add(m.DeviceName, "Sessions")
                n1.Tag = m
                DisplaySessions(m, n1)
                AddHandler m.SessionChanged, Sub(mixer As CCoreAudio.CMixer, session As CCoreAudio.CSession)
                                                 DisplaySessions(mixer, FindSessionsNode(mixer, Nothing))
                                             End Sub
            Next

            For Each n1 In tvMix.Nodes
                UpdateChildColors(n1)
            Next
        Else
            Dim l1 As CLine
            Dim l2 As CLine

            For Each m As CMixer In mxp.Mixers
                n1 = tvMix.Nodes.Add(m.DeviceName)
                n1 = n1.Nodes.Add("Lines")
                For Each l1 In m.LinesByLineType(ltcDestination)
                    n2 = AddControls(l1, n1, m.Index)
                    n2 = n2.Nodes.Add("Source Lines")
                    For Each l2 In m.LinesByConnection(l1.ID)
                        AddControls(l2, n2, m.Index)
                    Next l2
                Next l1
            Next
        End If

        Me.Text += " (Based on MixerProNET " + mxp.Version + ")"

        For Each m As CMixer In mxp.Mixers
            For Each l As CLine In m.Lines
                Dim peakMeters As Controls = l.ControlsByType(CControl.ControlTypeConstants.ctrltcPEAKMETER)
                If peakMeters.Count > 0 Then
                    ' Sound card has peak meters!
                End If
            Next
        Next
    End Sub

    Private Function FindSessionsNode(mixer As CCoreAudio.CMixer, parentNode As TreeNode) As TreeNode
        Dim nodes As TreeNodeCollection
        If parentNode Is Nothing Then
            nodes = tvMix.Nodes
        Else
            nodes = parentNode.Nodes
        End If

        For Each n As TreeNode In nodes
            If TypeOf n.Tag Is CCoreAudio.CMixer Then
                If CType(n.Tag, CCoreAudio.CMixer).Equals(mixer) Then Return n
            Else
                Dim foundNode As TreeNode = FindSessionsNode(mixer, n)
                If foundNode IsNot Nothing Then Return foundNode
            End If
        Next

        Return Nothing
    End Function

    Private Sub DisplaySessions(mixer As CCoreAudio.CMixer, parentNode As TreeNode)
        If Me.InvokeRequired Then
            Me.Invoke(New SafeSessionsUpdateDelegate(AddressOf SafeSessionsUpdate), mixer, parentNode)
        Else
            SafeSessionsUpdate(mixer, parentNode)
        End If
    End Sub

    Private Sub SafeSessionsUpdate(mixer As CCoreAudio.CMixer, parentNode As TreeNode)
        parentNode.Nodes.Clear()
        For Each s As CCoreAudio.CSession In mixer.Sessions
            AddControls(s, parentNode)
        Next
    End Sub

    Private Sub UpdateChildColors(parentNode As TreeNode)
        For Each childNode As TreeNode In parentNode.Nodes
            childNode.ForeColor = parentNode.ForeColor
            UpdateChildColors(childNode)
        Next
    End Sub

    Private Function AddControls(l As CLine, n As TreeNode, mIdx As Integer) As TreeNode
        Dim c As CControl
        Dim n1 As TreeNode

        n = n.Nodes.Add(l.LongName)
        For Each c In l.Controls
            n1 = n.Nodes.Add(c.LongName)
            n1.Tag = c
            n1.ForeColor = IIf(c.IsValid And c.Enabled, Color.Black, Color.DimGray)
        Next

        Return n
    End Function

    Private Function AddControls(s As CCoreAudio.CSession, n As TreeNode) As TreeNode
        n = n.Nodes.Add(s.Name)
        n.Nodes.Add("Volume").Tag = s.ControlVolume
        n.Nodes.Add("Mute").Tag = s.ControlMute

        Return n
    End Function

    Private Function AddControls(l As CCoreAudio.CLine, n As TreeNode) As TreeNode
        Dim n1 As TreeNode

        n = n.Nodes.Add(l.Name)
        For Each c As CCoreAudio.CControl In l.Controls
            For Each sc In c.SubControls
                If Not TypeOf sc Is CCoreAudio.CControl.CControlPeakMeter Then
                    If n.Text = "Master Controls" Then
                        n1 = n.Nodes.Add(sc.Name)
                    Else
                        n1 = n.Nodes.Add(c.Name)
                    End If
                    n1.Tag = sc
                End If
            Next
        Next

        Return n
    End Function

    Private Sub tvMix_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvMix.AfterSelect
        If e.Node.Tag IsNot Nothing Then
            If TypeOf e.Node.Tag Is CCoreAudio.CControl Then
                DisplayControls(CType(e.Node.Tag, CCoreAudio.CControl))
            ElseIf TypeOf e.Node.Tag Is CControl Then
                DisplayControls(CType(e.Node.Tag, CControl))
            ElseIf TypeOf e.Node.Tag Is CCoreAudio.CControl.CControlObject Then
                DisplayControls(CType(e.Node.Tag, CCoreAudio.CControl.CControlObject))
            ElseIf TypeOf e.Node.Tag Is CCoreAudio.CMixer Then
                DisplayControls(CType(e.Node.Tag, CCoreAudio.CMixer))
            End If
        Else
            DeleteUIControls()
        End If
    End Sub

    Private Sub DeleteUIControls()
        If ucsCtrl IsNot Nothing Then Controls.Remove(ucsCtrl)
        If caFader IsNot Nothing Then Controls.Remove(caFader)

        If Not chkOp Is Nothing Then
            For i As Integer = UBound(chkOp) To 0 Step -1
                Controls.Remove(chkOp(i))
            Next i
        End If
    End Sub

    Private Sub DisplayControls(selCtrl As CCoreAudio.CControl)
        If selCtrl.ControlMute IsNot Nothing Then DisplayControls(selCtrl.ControlMute)
        If selCtrl.ControlVolume IsNot Nothing Then DisplayControls(selCtrl.ControlVolume)
        If selCtrl.ControlPeakMeter IsNot Nothing Then DisplayControls(selCtrl.ControlPeakMeter)
        If selCtrl.ControlLoudness IsNot Nothing Then DisplayControls(selCtrl.ControlLoudness)
    End Sub

    Private Sub DisplayControls(selCtrl As CCoreAudio.CControl.CControlObject)
        Dim topPos As Integer = tvMix.Top + tvMix.Height + 8
        Dim x As Integer = tvMix.Left
        Dim y As Single = topPos
        Dim isEnabled As Boolean = (tvMix.SelectedNode.ForeColor <> Color.FromKnownColor(KnownColor.ControlDark))

        DeleteUIControls()

        If TypeOf selCtrl Is CCoreAudio.CControl.CControlMute Then
            ReDim Preserve chkOp(0)
            chkOp(0) = New CheckBox()
            With chkOp(0)
                .Text = selCtrl.Name
                .Width = tvMix.Width
                .Location = New Point(x, y)
                .Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
                y += (.Height + 4)
            End With
            Controls.Add(chkOp(0))
            selCtrl.Binding.Define(chkOp(0), "Checked", "CheckStateChanged")
            chkOp(0).Enabled = isEnabled
        ElseIf TypeOf selCtrl Is CCoreAudio.CControl.CControlVolume Then
            caFader = New VolumeFader()
            With caFader
                .Width = tvMix.Width
                .Location = New Point(x, y)
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
                .CoreAudioControl = selCtrl.Parent
                .IntegralChanges = False
                y += (.Height + 4)
            End With
            Controls.Add(caFader)
            caFader.Enabled = isEnabled
        ElseIf TypeOf selCtrl Is CCoreAudio.CControl.CControlLoudness Then
            ReDim Preserve chkOp(0)
            chkOp(0) = New CheckBox()
            With chkOp(0)
                .Text = selCtrl.Name
                .Width = tvMix.Width
                .Location = New Point(x, y)
                .Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
                y += (.Height + 4)
            End With
            Controls.Add(chkOp(0))
            selCtrl.Binding.Define(chkOp(0), "Checked", "CheckStateChanged")
            chkOp(0).Enabled = isEnabled
        End If
    End Sub

    Private Sub DisplayControls(selCtrl As CControl)
        Dim topPos As Integer = tvMix.Top + tvMix.Height + 8
        Dim x As Integer = tvMix.Left
        Dim y As Single = topPos

        DeleteUIControls()

        If (Not selCtrl.IsValid) Or (Not selCtrl.Enabled) Then Exit Sub

        If selCtrl.CtrlItems.Count Then
            For i As Integer = 0 To selCtrl.CtrlItems.Count - 1
                ReDim Preserve chkOp(i)
                chkOp(i) = New CheckBox()
                With chkOp(i)
                    .Text = selCtrl.CtrlItems(i + 1).ItemName
                    .Width = tvMix.Width / 4
                    .Location = New Point(x, y)
                    .Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
                    x = x + .Width + 4
                    If x + .Width > Width Then
                        x = tvMix.Left
                        y += (.Height + 4)
                    End If
                End With
                Controls.Add(chkOp(i))
                selCtrl.CtrlItems(i + 1).Binding.Define(chkOp(i), "Checked", "CheckStateChanged")
            Next i
        Else
            If ((selCtrl.ControlClass And cccCLASS_FADER) = cccCLASS_FADER) Or
                ((selCtrl.ControlClass And cccCLASS_NUMBER) = cccCLASS_NUMBER) Or
                ((selCtrl.ControlClass And cccCLASS_METER) = cccCLASS_METER) Or
                ((selCtrl.ControlClass And cccCLASS_SLIDER) = cccCLASS_SLIDER) Then
                ucsCtrl = New TrackBar()
                With ucsCtrl
                    .Minimum = selCtrl.Min
                    .Maximum = selCtrl.Max
                    .TickFrequency = .Maximum / 10
                    .TickStyle = TickStyle.BottomRight
                    .Width = tvMix.Width
                    .Location = New Point(x, y)
                    .Anchor = AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
                    y += (.Height + 4)
                End With
                Controls.Add(ucsCtrl)
                selCtrl.Binding.Define(ucsCtrl, "Value", "ValueChanged")
            Else
                ReDim Preserve chkOp(0)
                chkOp(0) = New CheckBox
                With chkOp(0)
                    .Text = selCtrl.LongName
                    .Width = tvMix.Width
                    .Location = New Point(x, y)
                    .Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
                    y += (.Height + 4)
                End With
                Controls.Add(chkOp(0))
                selCtrl.Binding.Define(chkOp(0), "Checked", "CheckStateChanged")
            End If
        End If
    End Sub

    Private Sub DisplayControls(selCtrl As CCoreAudio.CMixer)
        Dim topPos As Integer = tvMix.Top + tvMix.Height + 8
        Dim x As Integer = tvMix.Left
        Dim y As Single = topPos

        DeleteUIControls()

        ReDim Preserve chkOp(0)
        chkOp(0) = New CheckBox()
        With chkOp(0)
            .Text = "Default Device"
            .Width = tvMix.Width
            .Location = New Point(x, y)
            .Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
            .Checked = selCtrl.Selected
            .Enabled = Not selCtrl.Selected
            y += (.Height + 4)
        End With
        Controls.Add(chkOp(0))

        AddHandler chkOp(0).Click, Sub()
                                       selCtrl.Selected = True
                                       chkOp(0).Enabled = False
                                   End Sub
    End Sub
End Class