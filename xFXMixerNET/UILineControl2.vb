Imports NMixerProNET

Public Class UILineControl2
    Inherits System.Windows.Forms.UserControl

    Private mText As String
    Private mTextSize As SizeF = New SizeF(0, 0)
    Private mLineControl As Object
    Private mFader As TrackBar
    Private mSwitch As CheckBox
    Private mEditMode As Boolean
    Private mIsDragging As Boolean
    Private mPos As Point
    Private mMouseAction As MouseActionConstants = MouseActionConstants.None
    Private mUIControlType As UIControlTypeConstants

    Public Enum UIControlTypeConstants
        Fader
        Switch
    End Enum

    Private Enum MouseActionConstants
        None
        ResizeTop
        ResizeRight
        ResizeBottom
        ResizeLeft
        ResizeTopLeft
        ResizeTopRight
        ResizeBottomLeft
        ResizeBottomRight
        Move
        Invalid
    End Enum

    Public Event Editing()

    Public Property UIControlType() As UIControlTypeConstants
        Get
            Return mUIControlType
        End Get
        Set(ByVal Value As UIControlTypeConstants)
            mUIControlType = Value
        End Set
    End Property

    Public Property EditMode() As Boolean
        Get
            Return mEditMode
        End Get
        Set(ByVal Value As Boolean)
            If Value <> mEditMode Then
                mEditMode = Value
                If mEditMode Then
                    Me.Left -= 8
                    Me.Top -= 8
                Else
                    Me.Left += 8
                    Me.Top += 8
                    Me.Cursor = Cursors.Default
                End If
                Select Case mUIControlType
                    Case UIControlTypeConstants.Fader
                        mFader.Enabled = Not mEditMode
                    Case UIControlTypeConstants.Switch
                        mSwitch.Enabled = Not mEditMode
                End Select
                AutoResize()
            End If
        End Set
    End Property

    Public Property LineControl() As Object
        Get
            Return mLineControl
        End Get
        Set(ByVal Value As Object)
            mLineControl = Value
            Try
                Text = mLineControl.LongName
            Catch
            End Try
            Select Case mUIControlType
                Case UIControlTypeConstants.Fader
                    With mFader
                        .Minimum = mLineControl.ControlVolume.Min
                        .Maximum = mLineControl.ControlVolume.Max
                        .TickFrequency = mLineControl.ControlVolume.Steps
                        .Value = mLineControl.ControlVolume.UniformValue
                    End With
                    mLineControl.ControlVolume.Binding.Define(mFader, "Value", "ValueChanged")
                Case UIControlTypeConstants.Switch
                    mSwitch.Checked = mLineControl.ControlMute.UniformValue
                    mLineControl.ControlMute.Binding.Define(mSwitch, "Checked", "CheckedChanged")
            End Select

            Me.Refresh()
        End Set
    End Property

    Public Property Orientation() As Orientation
        Get
            If mUIControlType = UIControlTypeConstants.Fader Then
                Return mFader.Orientation
            Else
                Return Orientation.Vertical
            End If
        End Get
        Set(ByVal Value As Orientation)
            If mUIControlType = UIControlTypeConstants.Fader Then
                mFader.Orientation = Value
                AutoResize()
                RaiseEvent Editing()
            End If
        End Set
    End Property

    Public Property UIControlWidth() As Integer
        Get
            Select Case mUIControlType
                Case UIControlTypeConstants.Fader
                    Return mFader.Width
                Case UIControlTypeConstants.Switch
                    Return mSwitch.Width
            End Select
        End Get
        Set(ByVal Value As Integer)
            Select Case mUIControlType
                Case UIControlTypeConstants.Fader
                    mFader.Width = Value
                Case UIControlTypeConstants.Switch
                    mSwitch.Width = Value
            End Select
        End Set
    End Property

    Public Property UIControlHeight() As Integer
        Get
            Select Case mUIControlType
                Case UIControlTypeConstants.Fader
                    Return mFader.Height
                Case UIControlTypeConstants.Switch
                    Return mSwitch.Height
            End Select
        End Get
        Set(ByVal Value As Integer)
            Select Case mUIControlType
                Case UIControlTypeConstants.Fader
                    mFader.Height = Value
                Case UIControlTypeConstants.Switch
                    mSwitch.Height = Value
            End Select
        End Set
    End Property

    Public Overrides Property Text() As String
        Get
            Return mText
        End Get
        Set(ByVal Value As String)
            mText = Value

            Dim g As Graphics = Me.CreateGraphics
            mTextSize = g.MeasureString(mText, Me.Font)
            g.Dispose()

            If mUIControlType = UIControlTypeConstants.Switch Then
                mSwitch.Text = Value
            End If

            AutoResize()
        End Set
    End Property

    Private Sub UIControl_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
        Dim g As Graphics = e.Graphics
        Dim m As Integer = IIf(mEditMode, 8, 0)

        g.Clear(Color.FromKnownColor(KnownColor.Control))

        If mEditMode Then
            Dim r1 As Rectangle = New Rectangle(1, 1, Width - 1, Height - 1)
            Dim r2 As Rectangle = New Rectangle(6, 6, Width - 13, Height - 13)
            ControlPaint.DrawSelectionFrame(g, True, r1, r2, Color.White)

            ControlPaint.DrawGrabHandle(g, New Rectangle(0, 0, 8, 8), True, mUIControlType <> UIControlTypeConstants.Fader)
            ControlPaint.DrawGrabHandle(g, New Rectangle(Width / 2 - 4, 0, 8, 8), True, mUIControlType = UIControlTypeConstants.Switch Or Orientation = Orientation.Vertical)
            ControlPaint.DrawGrabHandle(g, New Rectangle(Width - 8, 0, 8, 8), True, mUIControlType <> UIControlTypeConstants.Fader)

            ControlPaint.DrawGrabHandle(g, New Rectangle(0, Height / 2 - 4, 8, 8), True, mUIControlType = UIControlTypeConstants.Switch Or Orientation = Orientation.Horizontal)
            ControlPaint.DrawGrabHandle(g, New Rectangle(Width - 8, Height / 2 - 4, 8, 8), True, mUIControlType = UIControlTypeConstants.Switch Or Orientation = Orientation.Horizontal)

            ControlPaint.DrawGrabHandle(g, New Rectangle(0, Height - 8, 8, 8), True, mUIControlType <> UIControlTypeConstants.Fader)
            ControlPaint.DrawGrabHandle(g, New Rectangle(Width / 2 - 4, Height - 8, 8, 8), True, mUIControlType = UIControlTypeConstants.Switch Or Orientation = Orientation.Vertical)
            ControlPaint.DrawGrabHandle(g, New Rectangle(Width - 8, Height - 8, 8, 8), True, mUIControlType <> UIControlTypeConstants.Fader)
        End If

        Select Case mUIControlType
            Case UIControlTypeConstants.Fader
                g.DrawString(mText, Me.Font, New SolidBrush(Color.Black), m, m)
        End Select
    End Sub

    Private Sub AutoResize()
        Dim m As Integer = IIf(mEditMode, 8, 0)

        Select Case mUIControlType
            Case UIControlTypeConstants.Fader
                For i As Integer = 1 To 2
                    Me.Size = New Size(Math.Max(mFader.Width, mTextSize.Width) + 2 * m, mFader.Top + mFader.Height + m)
                    Select Case mFader.Orientation
                        Case Orientation.Vertical
                            mFader.Location = New Point(((Width - 2 * m) - mFader.Width) / 2 + m, mTextSize.Height + m)
                        Case Orientation.Horizontal
                            mFader.Location = New Point(m, m)
                    End Select
                Next i
            Case UIControlTypeConstants.Switch
                mSwitch.Location = New Point(m, m)
                Me.Size = New Size(mSwitch.Width + 2 * m, mSwitch.Height + 2 * m)
        End Select

        Me.Refresh()
    End Sub

    Public Sub New(ByVal lc As CCoreAudio.CControl, ByVal ct As UIControlTypeConstants)
        mUIControlType = ct

        Select Case mUIControlType
            Case UIControlTypeConstants.Fader
                mFader = New TrackBar
                Me.Controls.Add(mFader)
                With mFader
                    .Orientation = Orientation.Vertical
                    .TickStyle = TickStyle.Both
                    .Height = 100
                    .Visible = True
                End With
            Case UIControlTypeConstants.Switch
                mSwitch = New CheckBox
                Me.Controls.Add(mSwitch)
                With mSwitch
                    .Location = New Point(0, 0)
                    .Visible = True
                End With
        End Select

        LineControl = lc
        AutoResize()
    End Sub

    Private Sub ChangeOrientation()
        If mEditMode And mUIControlType = UIControlTypeConstants.Fader Then
            Orientation = IIf(Orientation = Orientation.Horizontal, Orientation.Vertical, Orientation.Horizontal)
        End If
    End Sub

    Protected Overrides Sub Finalize()
        mFader = Nothing
        mSwitch = Nothing
        mLineControl = Nothing
        MyBase.Finalize()
    End Sub

    Private Function GetMouseAction(ByVal x As Integer, ByVal y As Integer) As MouseActionConstants
        Dim c As Cursor = Cursors.Default
        Dim m As MouseActionConstants = MouseActionConstants.None

        If mEditMode Then
            'If y <= 8 Then
            '    If x <= 8 Then m = MouseActionConstants.ResizeTopLeft
            '    If x > 8 And x < Width - 8 Then m = MouseActionConstants.ResizeTop
            '    If x >= Width - 8 Then m = MouseActionConstants.ResizeTopRight
            'End If
            'If y > 8 And y < Height - 8 Then
            '    If x <= 8 Then m = MouseActionConstants.ResizeLeft
            '    If x > 8 And x < Width - 8 Then m = MouseActionConstants.Move
            '    If x >= Width - 8 Then m = MouseActionConstants.ResizeRight
            'End If
            'If y >= Height - 8 Then
            '    If x <= 8 Then m = MouseActionConstants.ResizeBottomLeft
            '    If x > 8 And x < Width - 8 Then m = MouseActionConstants.ResizeBottom
            '    If x >= Width - 8 Then m = MouseActionConstants.ResizeBottomRight
            'End If

            If y <= 8 Then
                m = MouseActionConstants.Move
                If x <= 8 Then m = MouseActionConstants.ResizeTopLeft
                If x >= Width / 2 - 4 And x <= Width / 2 + 4 Then m = MouseActionConstants.ResizeTop
                If x >= Width - 8 Then m = MouseActionConstants.ResizeTopRight
            End If
            If x <= 8 Or x >= Width - 8 Then
                m = MouseActionConstants.Move
                If x <= 8 And y >= Height / 2 - 4 And y <= Height / 2 + 4 Then m = MouseActionConstants.ResizeLeft
                If x >= Width - 8 And y >= Height / 2 - 4 And y <= Height / 2 + 4 Then m = MouseActionConstants.ResizeRight
            End If
            If y >= Height - 8 Then
                m = MouseActionConstants.Move
                If x <= 8 Then m = MouseActionConstants.ResizeBottomLeft
                If x >= Width / 2 - 4 And x <= Width / 2 + 4 Then m = MouseActionConstants.ResizeBottom
                If x >= Width - 8 Then m = MouseActionConstants.ResizeBottomRight
            End If

            Select Case m
                Case MouseActionConstants.Move
                    c = Cursors.SizeAll
                Case MouseActionConstants.ResizeTop, MouseActionConstants.ResizeBottom
                    If UIControlType = UIControlTypeConstants.Fader Then
                        Select Case Orientation
                            Case Orientation.Vertical
                                c = Cursors.SizeNS
                            Case Orientation.Horizontal
                                c = Cursors.Default
                        End Select
                    Else
                        c = Cursors.SizeNS
                    End If
                Case MouseActionConstants.ResizeTopLeft, MouseActionConstants.ResizeBottomRight
                    c = IIf(UIControlType = UIControlTypeConstants.Fader, Cursors.Default, Cursors.SizeNWSE)
                Case MouseActionConstants.ResizeTopRight, MouseActionConstants.ResizeBottomLeft
                    c = IIf(UIControlType = UIControlTypeConstants.Fader, Cursors.Default, Cursors.SizeNESW)
                Case MouseActionConstants.ResizeLeft, MouseActionConstants.ResizeRight
                    If UIControlType = UIControlTypeConstants.Fader Then
                        Select Case Orientation
                            Case Orientation.Vertical
                                c = Cursors.Default
                            Case Orientation.Horizontal
                                c = Cursors.SizeWE
                        End Select
                    Else
                        c = Cursors.SizeWE
                    End If
            End Select

            If Not mIsDragging Then m = MouseActionConstants.None
        End If

        Me.Cursor = c

        Return m
    End Function

    Private Sub UILineControl_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
        If mEditMode Then
            mPos = New Point(e.X, e.Y)
            mIsDragging = True
            mMouseAction = GetMouseAction(e.X, e.Y)
        End If
    End Sub

    Private Sub UILineControl_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseUp
        mMouseAction = MouseActionConstants.None
        mIsDragging = False
    End Sub

    Private Sub UILineControl_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
        UILineControlResize(e.X, e.Y)
    End Sub

    Private Sub UILineControlResize(ByVal x As Integer, ByVal y As Integer, Optional ByVal m As MouseActionConstants = MouseActionConstants.Invalid, Optional ByVal IgnoreInitPos As Boolean = False)
        If m = MouseActionConstants.Invalid Then m = IIf(mIsDragging, mMouseAction, GetMouseAction(x, y))
        If IgnoreInitPos Then mPos = New Point(0, 0)

        If m <> MouseActionConstants.None Then
            Dim c As Control
            Select Case mUIControlType
                Case UIControlTypeConstants.Fader
                    c = mFader
                Case UIControlTypeConstants.Switch
                    c = mSwitch
            End Select
            Select Case m
                Case MouseActionConstants.Move
                    Me.Location = New Point(Me.Left + x - mPos.X, Me.Top + y - mPos.Y)
                Case MouseActionConstants.ResizeTop
                    Me.Top += y - mPos.Y
                    Me.UIControlHeight += -y + mPos.Y
                Case MouseActionConstants.ResizeBottom
                    Me.UIControlHeight += y - mPos.Y
                    mPos.Y = y
                Case MouseActionConstants.ResizeRight
                    Me.UIControlWidth += x - mPos.X
                    mPos.X = x
                Case MouseActionConstants.ResizeLeft
                    Me.Left += x - mPos.X
                    Me.UIControlWidth += -x + mPos.X
                Case MouseActionConstants.ResizeBottomLeft
                    UILineControlResize(x, y, MouseActionConstants.ResizeBottom)
                    UILineControlResize(x, y, MouseActionConstants.ResizeLeft)
                Case MouseActionConstants.ResizeBottomRight
                    UILineControlResize(x, y, MouseActionConstants.ResizeBottom)
                    UILineControlResize(x, y, MouseActionConstants.ResizeRight)
                Case MouseActionConstants.ResizeTopLeft
                    UILineControlResize(x, y, MouseActionConstants.ResizeTop)
                    UILineControlResize(x, y, MouseActionConstants.ResizeLeft)
                Case MouseActionConstants.ResizeTopRight
                    UILineControlResize(x, y, MouseActionConstants.ResizeTop)
                    UILineControlResize(x, y, MouseActionConstants.ResizeRight)
            End Select

            AutoResize()
            RaiseEvent Editing()
        End If
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Const WM_KEYDOWN As Integer = &H100

        If mEditMode And msg.Msg = WM_KEYDOWN Then
            If (keyData And Keys.Control) = Keys.Control Then
                Select Case keyData Xor Keys.Control
                    Case Keys.Up
                        Me.UILineControlResize(0, -1, UILineControl2.MouseActionConstants.Move, True)
                        Return True
                    Case Keys.Down
                        Me.UILineControlResize(0, 1, UILineControl2.MouseActionConstants.Move, True)
                        Return True
                    Case Keys.Left
                        Me.UILineControlResize(-1, 0, UILineControl2.MouseActionConstants.Move, True)
                        Return True
                    Case Keys.Right
                        Me.UILineControlResize(1, 0, UILineControl2.MouseActionConstants.Move, True)
                        Return True
                End Select
            ElseIf (keyData And Keys.Shift) = Keys.Shift Then
                Select Case keyData Xor Keys.Shift
                    Case Keys.Up
                        Me.UILineControlResize(0, -1, UILineControl2.MouseActionConstants.ResizeBottom, True)
                        Return True
                    Case Keys.Down
                        Me.UILineControlResize(0, 1, UILineControl2.MouseActionConstants.ResizeBottom, True)
                        Return True
                    Case Keys.Left
                        Me.UILineControlResize(-1, 0, UILineControl2.MouseActionConstants.ResizeRight, True)
                        Return True
                    Case Keys.Right
                        Me.UILineControlResize(1, 0, UILineControl2.MouseActionConstants.ResizeRight, True)
                        Return True
                End Select
            End If
        End If

        MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub InitializeComponent()
        '
        'UILineControl
        '
        Me.Name = "UILineControl"

    End Sub

    Private Sub UILineControl_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.DoubleClick
        ChangeOrientation()
    End Sub
End Class
