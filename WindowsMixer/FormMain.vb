Imports NMixerProNET

' WindowsMixer 1.0
' xFX JumpStart
' 10/Nov/2004
'
' This application demonstrates the use of the MixerProNET engine to create a fully
' functional mixer.
' This mixer resembles the mixer included with Windows and also shows the use of the
' new "Binding" capability of MixerProNET that allows the binding of Controls and
' ControlItems to Form's Controls such as TrackBars and Checkboxes.

Public Enum ScrollDirConstants
    sdLeft
    sdRight
End Enum

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
    Friend WithEvents cmbDevices As ComboBox
    Friend WithEvents cmbSources As ComboBox
    Friend WithEvents tmrScroll As Timers.Timer
    Private WithEvents lblDevices As Label
    Friend WithEvents lblSources As Label
    <DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(FormMain))
        Me.cmbDevices = New ComboBox()
        Me.lblDevices = New Label()
        Me.lblSources = New Label()
        Me.cmbSources = New ComboBox()
        Me.tmrScroll = New Timers.Timer()
        CType(Me.tmrScroll, ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cmbDevices
        '
        Me.cmbDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbDevices.Location = New Point(10, 24)
        Me.cmbDevices.Name = "cmbDevices"
        Me.cmbDevices.Size = New Size(137, 21)
        Me.cmbDevices.TabIndex = 0
        '
        'lblDevices
        '
        Me.lblDevices.Location = New Point(10, 9)
        Me.lblDevices.Name = "lblDevices"
        Me.lblDevices.Size = New Size(53, 13)
        Me.lblDevices.TabIndex = 1
        Me.lblDevices.Text = "Devices"
        '
        'lblSources
        '
        Me.lblSources.Location = New Point(162, 9)
        Me.lblSources.Name = "lblSources"
        Me.lblSources.Size = New Size(53, 13)
        Me.lblSources.TabIndex = 2
        Me.lblSources.Text = "Sources"
        '
        'cmbSources
        '
        Me.cmbSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbSources.Location = New Point(162, 24)
        Me.cmbSources.Name = "cmbSources"
        Me.cmbSources.Size = New Size(166, 21)
        Me.cmbSources.TabIndex = 3
        '
        'tmrScroll
        '
        Me.tmrScroll.Interval = 1000.0R
        Me.tmrScroll.SynchronizingObject = Me
        '
        'frmMain
        '
        Me.AutoScaleBaseSize = New Size(5, 14)
        Me.ClientSize = New Size(646, 347)
        Me.Controls.Add(Me.cmbSources)
        Me.Controls.Add(Me.lblSources)
        Me.Controls.Add(Me.lblDevices)
        Me.Controls.Add(Me.cmbDevices)
        Me.Font = New Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Me.MinimumSize = New Size(400, 310)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "WindowsMixer"
        CType(Me.tmrScroll, ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

    ' Every application that wants to use the MixerProNET engine must declare
    ' a variable of type CMixerPro. This is what will instantiate the class library.
    Dim WithEvents mxp As New CMixerPro()

    Dim LineX As Integer
    Dim ScrollDir As ScrollDirConstants
    Dim ScrollIndex As Integer = 0
    Dim nLines As Integer
    Dim IsAddingLines As Boolean
    Dim fAdv As Form
    Dim DestLineHasCtrls As Boolean

    Dim btnScrollRight As New MyBtn
    Dim btnScrollLeft As New MyBtn

    Private Declare Function LockWindowUpdate Lib "user32" (ByVal hwndLock As Integer) As Integer

    Private Sub FormMain_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        If CCoreAudio.RequiresCoreAudio Then
            MsgBox("This sample application has not yet been updated to take advantage of the new Core Audio support which allows MixerProNET to interface with the new mixer API provided under Windows Vista and later versions", MsgBoxStyle.Information Or MsgBoxStyle.OkOnly)
        End If

        CreateScrollingButtons()
        GetDevices()
        ReposScrollButtons()
    End Sub

    Private Sub CreateScrollingButtons()
        With btnScrollRight
            .Text = "»"
            .Top = cmbDevices.Top
            .Name = "btnScrollRight"
            AddHandler .MouseDown, AddressOf Scroll
            AddHandler .MouseUp, AddressOf StopScrolling
            .ScrollDir = ScrollDirConstants.sdRight
            .Size = New Size(21, 21)
        End With
        Me.Controls.Add(btnScrollRight)

        With btnScrollLeft
            .Text = "«"
            .Top = cmbDevices.Top
            .Name = "btnScrollLeft"
            AddHandler .MouseDown, AddressOf Scroll
            AddHandler .MouseUp, AddressOf StopScrolling
            .ScrollDir = ScrollDirConstants.sdLeft
            .Size = New Size(21, 21)
        End With
        Me.Controls.Add(btnScrollLeft)
    End Sub

    Private Sub GetDevices()
        ' To obtain the list of devices (or sound cards that provide a mixer driver)
        ' we just have to cycle through the Mixers collection
        cmbDevices.Items.Clear()
        For Each m As CMixer In mxp.Mixers
            cmbDevices.Items.Add(m)
        Next m

        If cmbDevices.Items.Count > 0 Then cmbDevices.SelectedIndex = 0
    End Sub

    Private Sub GetSources()
        ' This sub retrieves the destination lines in the currently
        ' selected device
        Dim m As CMixer = CType(cmbDevices.SelectedItem, CMixer)

        ' Destination lines are the target where all other lines are connected to.
        ' For example, the Mute control belongs to a line that is always connected
        ' to the speakers and the speakers are the destination.
        ' Since differencing these lines from the other can be quite complex we
        ' use the LinesByLineType method which returns a collection of destination
        ' lines.
        cmbSources.Items.Clear()
        For Each s As CLine In m.LinesByLineType(CLine.LineTypeConstants.ltcDestination)
            cmbSources.Items.Add(s)
        Next

        If cmbSources.Items.Count > 0 Then cmbSources.SelectedIndex = 0
    End Sub

    Private Sub GetLines(Optional ByVal JustAddNew As Boolean = False)
        ' This routine is used to ontain the list of lines that are
        ' connected to the currently selected destination line.
        If IsAddingLines Then Exit Sub
        IsAddingLines = True

        LockWindowUpdate(Me.Handle.ToInt32)

        Try
            Dim m As CMixer = CType(cmbDevices.SelectedItem, CMixer)
            Dim dl As CLine = CType(cmbSources.SelectedItem, CLine)
            Dim cLines As Integer

            LineX = 5 - IIf(ScrollIndex > 0, 136, 0) - IIf(ScrollIndex > 1, (ScrollIndex - 1) * 100, 0)
            If Not JustAddNew Then
                nLines = 0
                DeleteControls()
            End If

            ' First we add the destination line. This is the line that (usually)
            ' performs global functions such as to control the overall volume
            ' (also known as "Master Volume" and it also contains the Mute All
            ' control which is used to mute all the other lines.
            If AddLine(dl, True, JustAddNew) Then
                cLines += 1
                DestLineHasCtrls = True
            Else
                DestLineHasCtrls = False
            End If
            ' Now, we add all the source lines that are connected to the
            ' selected destination.
            ' Again, we use a specialized method, LinesByConnection, to retrieve
            ' the list of lines.
            For Each l As CLine In m.LinesByConnection(dl.ID)
                If AddLine(l, False, IIf(JustAddNew, cLines < nLines, False)) Then cLines += 1
                If LineX > Width Then Exit For
            Next

            nLines = cLines

            btnScrollLeft.Enabled = (ScrollIndex > 0)
            btnScrollRight.Enabled = (LineX > Width)

            If Not btnScrollRight.Enabled Then
                Me.MaximumSize = New Size(LineX + 3, Screen.PrimaryScreen.WorkingArea.Height)
            Else
                Me.MaximumSize = Screen.FromControl(Me).WorkingArea.Size
            End If
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try

        LockWindowUpdate(0)

        IsAddingLines = False
    End Sub

    Private Sub DeleteControls()
Restart:
        For Each c As Control In Me.Controls
            If c.Name <> "lblDevices" And
                c.Name <> "lblSources" And
                c.Name <> "cmbDevices" And
                c.Name <> "cmbSources" And
                c.Name <> "btnScrollRight" _
                And c.Name <> "btnScrollLeft" Then
                Me.Controls.Remove(c)
                GoTo Restart
            End If
        Next
    End Sub

    Private Function AddLine(ByVal l As CLine, ByVal IsFirst As Boolean, ByVal JustTest As Boolean) As Boolean
        ' This is the main routine of the program and its tasks are to:
        ' 1) Add the FADER control from the line, used to control the volume
        ' 2) Add the SWITCH control, used to control the Mute state for output lines
        ' 3) Add the SWITCH control, used to control the recording source for input lines
        ' 4) Add a button to access any additional controls
        Dim tb As TrackBar = Nothing
        Dim tbb As TrackBar
        Dim ch As CheckBox
        Dim bt As Button
        Dim lb As Label
        Dim cc As Controls
        Dim aw As Integer = IIf(IsFirst, 136, 100)
        Dim h As Integer = Height - 30
        Dim c As CControl = Nothing
        Dim WasAdded As Boolean = False

        If LineX >= 0 Then
            ' First of all we obtain a collection of FAER controls
            cc = l.ControlsByClass(CControl.ControlClassConstants.cccCLASS_FADER)
            If cc.Count = 0 Then Return False

            WasAdded = True

            If Not JustTest Then
                For Each c In cc
                    tb = New TrackBar

                    tb.Orientation = Orientation.Vertical
                    tb.Minimum = c.Min
                    tb.Maximum = c.Max
                    tb.Height = h - 130 - 60
                    tb.TickFrequency = (tb.Maximum - tb.Minimum) / 10
                    tb.TickStyle = TickStyle.Both
                    tb.Anchor = AnchorStyles.Bottom Or AnchorStyles.Top Or AnchorStyles.Left
                    Controls.Add(tb)
                    tb.Location = New Point(LineX + (aw - tb.Width) / 2, 130)

                    ' Now that we have added the TrackBar we bind it to the control
                    ' This will allow the control to adjust the TrackBar's position
                    ' automatically and at the same time will allow the control
                    ' to update its value in case the TrackBar's slider is moved.
                    ' To create a binding we pass the control's reference (tb), the name
                    ' of the property that will hold the control's value (Value) and
                    ' the name of the event that the control should check (ValueChanged)
                    ' for changes.
                    c.Binding.Define(tb, "Value", "ValueChanged")

                    lb = New Label
                    lb.Location = New Point(LineX + 5, 65)
                    lb.Size = New Size(aw - 10, 13)
                    lb.Text = l.LongName
                    lb.FlatStyle = FlatStyle.System
                    Controls.Add(lb)

                    tbb = New TrackBar
                    tbb.Orientation = Orientation.Horizontal
                    tbb.Width = 60
                    tbb.Minimum = -100
                    tbb.Maximum = 100
                    tbb.TickStyle = TickStyle.BottomRight
                    tbb.TickFrequency = 100
                    tbb.Enabled = tb.Enabled And (l.Channels > 1)
                    tbb.Tag = c
                    SetBal(tbb)
                    Controls.Add(tbb)
                    tbb.Location = New Point(tb.Left + tb.Width / 2 - tbb.Width / 2, 95)

                    ' Since almost any sound card provides controls to adjust a line's
                    ' balance we'll have to handle it ourselves
                    AddHandler tbb.ValueChanged, AddressOf BalanceChanged
                    AddHandler tbb.MouseUp, AddressOf ResetBal

                    ' We don't yet support multiple faders... so exit on the first one
                    Exit For
                Next

                ' Now, let's obtain a collection of MUTE controls
                For Each c In l.ControlsByType(CControl.ControlTypeConstants.ctrltcMUTE)
                    ch = New CheckBox
                    ch.Location = New Point(LineX + 5, 130 + tb.Height + 5)
                    ch.Text = "Mute" + IIf(IsFirst, " All", "")
                    ch.Width = 90
                    ch.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
                    ch.FlatStyle = FlatStyle.System
                    Controls.Add(ch)

                    ' Here we bind the checkbox to the control
                    c.Binding.Define(ch, "Checked", "CheckStateChanged")

                    ' We don't yet support multiple switches... so exit on the first one
                    Exit For
                Next

                ' Let's now see if the current mixer has a MUX control connected to
                ' the WAVEIN.
                ' The WAVEIN is the component that all the input lines connect to.
                ' A MUX control is a control that contains one or more subcontrols, known
                ' as ControlItems and are often used to control properties of the sound
                ' card that can effect the whole sound card and all its lines or a group
                ' of them.
                ' One case is the source line selection which is what allows us to select
                ' which line will be used for recording purposes.
                Dim lp As Lines = l.Parent.LinesByComponentType(CLine.ComponentTypeConstants.ctcDST_WAVEIN)
                If lp.Count > 0 Then
                    If l.ConnectedTo = lp(1).ID Then
                        cc = lp(1).ControlsByType(CControl.ControlTypeConstants.ctrltcMUX)
                        If cc.Count > 0 Then
                            Try
                                ' ****************
                                Dim m As CMixer = l.Parent
                                Dim ln As Integer = 0
                                For Each sl As CLine In m.LinesByLineType(CLine.LineTypeConstants.ltcDestination)
                                    ln += (m.LinesByConnection(sl.ID).Count + 1)
                                    If sl.ID = lp(1).ID Then Exit For
                                Next
                                Dim ci As CCtrlItem = cc(1).CtrlItems(ln - l.Index + 1)
                                ' ****************
                                'Dim ci As CCtrlItem = cc(1).CtrlItems(l.Parent.Lines.Count - c.Parent.Index + 1)

                                ch = New CheckBox
                                ch.Location = New Point(LineX + 5, 130 + tb.Height + 5)
                                ch.Text = "Select"
                                ch.Width = 90
                                ch.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
                                ch.FlatStyle = FlatStyle.System
                                Controls.Add(ch)
                                ci.Binding.Define(ch, "Checked", "CheckStateChanged")
                            Catch
                            End Try
                        End If
                    End If
                End If

                ' Finally, let's see if the line has some additional controls that
                ' our application can manage...
                ' If it does, then we'll add a button use to display a dialog with
                ' these controls.
                For Each c In l.Controls
                    If (c.ControlType And CControl.ControlTypeConstants.ctrltcBASS) = CControl.ControlTypeConstants.ctrltcBASS Or
                        (c.ControlType And CControl.ControlTypeConstants.ctrltcTREBLE) = CControl.ControlTypeConstants.ctrltcTREBLE Or
                        (c.ControlType And CControl.ControlTypeConstants.ctrltcBUTTON) = CControl.ControlTypeConstants.ctrltcBUTTON Or
                        (c.ControlType And CControl.ControlTypeConstants.ctrltcLOUDNESS) = CControl.ControlTypeConstants.ctrltcLOUDNESS Or
                        (c.ControlType And CControl.ControlTypeConstants.ctrltcSTEREOENH) = CControl.ControlTypeConstants.ctrltcSTEREOENH Then
                        bt = New Button
                        bt.Text = "Advanced"
                        bt.Left = LineX + 5
                        bt.Top = 130 + tb.Height + 30
                        bt.Width = IIf(IsFirst, 120, 80)
                        bt.Tag = l
                        bt.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
                        bt.FlatStyle = FlatStyle.System
                        Controls.Add(bt)
                        AddHandler bt.Click, AddressOf DisplayAdvCtrls
                        Exit For
                    End If
                Next
            End If
        End If

        LineX += aw

        Return WasAdded
    End Function

    Private Sub SetBal(ByVal tbb As TrackBar)
        Dim c As CControl = tbb.Tag

        If c.Parent.Channels > 1 Then
            If c.Value(0) > c.Value(1) Then
                tbb.Value = -100 + c.Value(1) / c.Value(0) * 100
            End If
            If c.Value(1) > c.Value(0) Then
                tbb.Value = 100 - c.Value(0) / c.Value(1) * 100
            End If
            If c.Value(0) = c.Value(1) Then
                tbb.Value = 0
            End If
        Else
            tbb.Value = 0
        End If

    End Sub

    Private Sub MuteChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim ch As CheckBox = sender
        Dim c As CControl = ch.Tag

        c.UniformValue = IIf(ch.Checked, c.Max, c.Min)
    End Sub

    Private Sub ResetBal(ByVal sender As Object, ByVal e As MouseEventArgs)
        If e.Button = Windows.Forms.MouseButtons.Right Then
            Dim tbb As TrackBar = sender
            Dim c As CControl = tbb.Tag
            Dim v(1) As Integer

            v(0) = c.UniformValue
            v(1) = c.UniformValue
            c.Value = v

            SetBal(tbb)
        End If
    End Sub

    Private Sub DisplayAdvCtrls(ByVal sender As Object, ByVal e As EventArgs)
        fAdv = New FormAdv

        fAdv.Tag = sender.Tag
        fAdv.ShowDialog()
        fAdv = Nothing
    End Sub

    Private Sub BalanceChanged(ByVal sender As Object, ByVal e As EventArgs)

        Dim tbb As TrackBar = sender
        Dim c As CControl = tbb.Tag

        c.Value = GetVol(tbb, c.UniformValue)
    End Sub

    Private Function GetVol(ByVal tbb As TrackBar, ByVal vol As Integer) As Integer()
        Dim v(1) As Integer
        Dim pL As Single
        Dim pR As Single

        If tbb.Value < 0 Then
            pR = 1 - (-tbb.Value / 100)
        Else
            pR = 1
        End If
        If tbb.Value > 0 Then
            pL = 1 - (tbb.Value / 100)
        Else
            pL = 1
        End If

        v(0) = vol * pL
        v(1) = vol * pR
        Return v
    End Function

    Private Sub cmbDevices_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmbDevices.SelectedIndexChanged
        Text = "WindowsMixer - " + cmbDevices.Text
        GetSources()
    End Sub

    Private Sub cmbSources_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmbSources.SelectedIndexChanged
        ScrollIndex = 0
        GetLines()
    End Sub

    Private Sub FormMain_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles MyBase.Paint
        If IsAddingLines Then Exit Sub

        Dim IsFirstVisible As Boolean = ScrollIndex = 0 And DestLineHasCtrls

        Me.SuspendLayout()
        Me.CreateGraphics.Clear(Me.BackColor)

        If IsFirstVisible Then Draw3DVerticalLine(136, 55)

        Dim lc As Integer = 0
        For x As Integer = IIf(IsFirstVisible, 136, 100) To Width Step 100
            lc += 1
            Draw3DVerticalLine(x, 95)

            If lc >= nLines Then Exit For
        Next

        Draw3DHorizontalLine(55, 0, Width)
        If IsFirstVisible Then
            Draw3DHorizontalLine(87, 0, 130)
            Draw3DHorizontalLine(87, 142, Width)
        Else
            Draw3DHorizontalLine(87, 0, Width)
        End If
        Me.ResumeLayout()
    End Sub

    Private Sub Draw3DVerticalLine(ByVal x As Integer, ByVal y1 As Integer)
        Me.CreateGraphics.DrawLine(New Pen(Color.DarkGray), x, y1, x, Height)
        Me.CreateGraphics.DrawLine(New Pen(Color.White), x + 1, y1, x + 1, Height)
    End Sub

    Private Sub Draw3DHorizontalLine(ByVal y As Integer, ByVal x1 As Integer, ByVal x2 As Integer)
        Me.CreateGraphics.DrawLine(New Pen(Color.DarkGray), x1, y, x2, y)
        Me.CreateGraphics.DrawLine(New Pen(Color.White), x1, y + 1, x2, y + 1)
    End Sub

    Private Sub FormMain_Resize(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Resize
        If Not mxp Is Nothing Then
            If btnScrollLeft.Enabled = True Or btnScrollRight.Enabled = True Or LineX > Width Then GetLines(True)

            ReposScrollButtons()
        End If
    End Sub

    Private Sub ReposScrollButtons()
        btnScrollLeft.Left = Width - 58
        btnScrollRight.Left = Width - 35
    End Sub

    Private Shadows Sub Scroll(ByVal sender As Object, ByVal e As MouseEventArgs)
        Dim b As MyBtn = sender
        ScrollDir = b.ScrollDir

        tmrScroll.Interval = 10
        tmrScroll.Enabled = True
    End Sub

    Private Sub tmrScroll_Elapsed(ByVal sender As System.Object, ByVal e As Timers.ElapsedEventArgs) Handles tmrScroll.Elapsed
        tmrScroll.Interval = 400
        ScrollIndex += IIf(ScrollDir = ScrollDirConstants.sdLeft, -1, 1)
        GetLines()
        Select Case ScrollDir
            Case ScrollDirConstants.sdLeft
                If Not btnScrollLeft.Enabled Then StopScrolling(Nothing, New MouseEventArgs(Windows.Forms.MouseButtons.Left, 0, 0, 0, 0))
            Case ScrollDirConstants.sdRight
                If Not btnScrollRight.Enabled Then StopScrolling(Nothing, New MouseEventArgs(Windows.Forms.MouseButtons.Left, 0, 0, 0, 0))
        End Select
    End Sub

    Private Sub StopScrolling(ByVal sender As System.Object, ByVal e As MouseEventArgs)
        tmrScroll.Enabled = False
    End Sub

    Private Sub mxp_AudioDevicesChange() Handles mxp.AudioDevicesChange
        GetDevices()
    End Sub
End Class

Public Class MyBtn
    Inherits Button
    Private IsMouseOver As Boolean
    Private pScrollDir As ScrollDirConstants

    Public Property ScrollDir() As ScrollDirConstants
        Get
            Return pScrollDir
        End Get
        Set(ByVal Value As ScrollDirConstants)
            pScrollDir = Value
        End Set
    End Property

    Private Sub MyBtn_MouseEnter(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.MouseEnter
        IsMouseOver = True
        Me.Refresh()
    End Sub

    Private Sub MyBtn_MouseLeave(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.MouseLeave
        IsMouseOver = False
        Me.Refresh()
    End Sub

    Private Sub MyBtn_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles MyBase.Paint
        With e.Graphics
            .Clear(Color.LightGray)
            Dim r As Rectangle = e.ClipRectangle
            r.Width -= 1
            r.Height -= 1
            If IsMouseOver Then
                .DrawRectangle(New Pen(Color.Black), r)
            Else
                .DrawRectangle(New Pen(Color.Gray), r)
            End If

            Dim f As New Font(Parent.Font, FontStyle.Bold)
            Dim s As SizeF = .MeasureString(Me.Text, f)
            Dim p As New PointF((r.Width - s.Width) / 2, (r.Height - s.Height) / 2)
            .DrawString(Me.Text, f, IIf(Me.Enabled, Brushes.Black, Brushes.Gray), p)

            f.Dispose()
        End With
    End Sub

    Private Sub MyBtn_Move(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Move
        Me.Refresh()
    End Sub
End Class