Imports NMixerProNET

Public Class UILine2
    Inherits System.Windows.Forms.UserControl

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
    End Sub

    'UserControl overrides dispose to clean up the component list.
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
    Friend WithEvents mnuCtx As System.Windows.Forms.ContextMenu
    Friend WithEvents mnuCtxEditMode As System.Windows.Forms.MenuItem
    Friend WithEvents mnuCtxRenameLineName As System.Windows.Forms.MenuItem
    Friend WithEvents mnuCtxVisible As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem3 As System.Windows.Forms.MenuItem
    Friend WithEvents mnuCtxShowDestinationLine As System.Windows.Forms.MenuItem
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.mnuCtx = New System.Windows.Forms.ContextMenu
        Me.mnuCtxEditMode = New System.Windows.Forms.MenuItem
        Me.MenuItem3 = New System.Windows.Forms.MenuItem
        Me.mnuCtxRenameLineName = New System.Windows.Forms.MenuItem
        Me.mnuCtxShowDestinationLine = New System.Windows.Forms.MenuItem
        Me.MenuItem2 = New System.Windows.Forms.MenuItem
        Me.mnuCtxVisible = New System.Windows.Forms.MenuItem
        '
        'mnuCtx
        '
        Me.mnuCtx.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuCtxEditMode, Me.MenuItem3, Me.mnuCtxRenameLineName, Me.mnuCtxShowDestinationLine, Me.MenuItem2, Me.mnuCtxVisible})
        '
        'mnuCtxEditMode
        '
        Me.mnuCtxEditMode.Index = 0
        Me.mnuCtxEditMode.Text = "Edit Mode"
        '
        'MenuItem3
        '
        Me.MenuItem3.Index = 1
        Me.MenuItem3.Text = "-"
        '
        'mnuCtxRenameLineName
        '
        Me.mnuCtxRenameLineName.Index = 2
        Me.mnuCtxRenameLineName.Text = "Rename Line Name"
        '
        'mnuCtxShowDestinationLine
        '
        Me.mnuCtxShowDestinationLine.Index = 3
        Me.mnuCtxShowDestinationLine.Text = "Show Destination Line"
        '
        'MenuItem2
        '
        Me.MenuItem2.Index = 4
        Me.MenuItem2.Text = "-"
        '
        'mnuCtxVisible
        '
        Me.mnuCtxVisible.Index = 5
        Me.mnuCtxVisible.Text = "Visible"
        '
        'UILine
        '
        Me.Name = "UILine"
        Me.Size = New System.Drawing.Size(246, 199)

    End Sub

#End Region

    Private mLine As CCoreAudio.CLine
    Private mText As String
    Private mDestinationText As String
    Private mDeviceName As String
    Private mWidestLabelWidth As Integer
    Private mEditMode As Boolean
    Private mFont1 As Font = New Font(Me.Font, FontStyle.Bold)
    Private mFont2 As Font = New Font(Me.Font.FontFamily, Me.Font.Size - 1)
    Private mMargins As PointF = New PointF(6, 8)
    Dim mH1 As Integer
    Dim mH2 As Integer
    Dim mH2b As Integer
    Dim mH3 As Integer
    Dim mIsDragging As Boolean
    Dim mPos As Point
    Dim mCanDrag As Boolean
    Dim mVisible As Boolean
    Dim mShowDestinationLine As Boolean

    Public Event Editing()

    Public Property ShowDestinationLine() As Boolean
        Get
            Return mShowDestinationLine
        End Get
        Set(ByVal Value As Boolean)
            mShowDestinationLine = Value

            If Not mShowDestinationLine Then
                mH2b = mH2
                mH2 = 0
            Else
                mH2 = mH2b
            End If
            Me.Refresh()
        End Set
    End Property

    Public Property EditMode() As Boolean
        Get
            Return mEditMode
        End Get
        Set(ByVal Value As Boolean)
            mEditMode = Value

            mnuCtxEditMode.Checked = mEditMode

            If mEditMode Then
                Me.Visible = True
            Else
                Me.Visible = mVisible
            End If

            If Not ActiveControl Is Nothing Then
                Dim uilc As UILineControl
                If TypeOf ActiveControl Is UILineControl Then
                    uilc = ActiveControl
                    uilc.EditMode = mEditMode
                    uilc.Focus()
                End If
            End If
            Me.Refresh()
        End Set
    End Property

    Public Sub New(ByVal l As CCoreAudio.CLine)
        Me.New()
        Line = l

        mnuCtxVisible.Checked = True
        mVisible = True

        mnuCtxShowDestinationLine.Checked = True
        mShowDestinationLine = True
    End Sub

    Public Property Line() As CCoreAudio.CLine
        Get
            Return mLine
        End Get
        Set(ByVal value As CCoreAudio.CLine)
            mLine = value
            CreateControls()
            AutoResize()
            Me.Refresh()
        End Set
    End Property

    Public Overrides Property Text() As String
        Get
            Return mText
        End Get
        Set(ByVal Value As String)
            mText = Value
            CalculateWidestLabel()
            AutoResize()
            RaiseEvent Editing()
        End Set
    End Property

    Private Sub CalculateWidestLabel()
        Dim g As Graphics = Me.CreateGraphics
        mH1 = g.MeasureString(mText, mFont1).Height
        mH2 = g.MeasureString(mDeviceName, mFont2).Height
        mH2b = mH2
        mH3 = mH2
        mWidestLabelWidth = Math.Max(Math.Max(g.MeasureString(mText, mFont1).Width, g.MeasureString(mDestinationText, mFont2).Width), g.MeasureString(mDeviceName, mFont2).Width)
        g.Dispose()
    End Sub

    Private Sub CreateControls()
        Dim x As Integer
        Dim y As Integer
        Dim uilc As UILineControl2 = Nothing

        mText = mLine.Name
        DeleteLineControls()

        'mDeviceName = mLine.Name
        'mDestinationText = mLine.Parent.Name

        CalculateWidestLabel()

        x = mMargins.X
        y = mMargins.Y + mH1 + 4 + mH2 + 4 + mH3 + mMargins.Y + mMargins.Y
        For Each c As CCoreAudio.CControl In mLine.Controls
            If c.ControlVolume IsNot Nothing Then
                uilc = AddFader(c, x, y)
                If uilc IsNot Nothing Then x += uilc.Width
            End If
        Next

        x = mMargins.X
        If uilc IsNot Nothing Then y = uilc.Top + uilc.Height + 4
        For Each c As CCoreAudio.CControl In mLine.Controls
            If c.ControlMute IsNot Nothing Then
                uilc = AddSwitch(c, x, y)
                If Not uilc Is Nothing Then y += uilc.Height
            End If
        Next
    End Sub

    Private Function AddFader(ByVal c As CCoreAudio.CControl, ByVal x As Integer, ByVal y As Integer) As UILineControl2
        Dim f As UILineControl2 = New UILineControl2(c, UILineControl2.UIControlTypeConstants.Fader)
        AddHandler f.Enter, AddressOf UILineControlGotFocus
        AddHandler f.Editing, AddressOf UILineControlEditing
        Me.Controls.Add(f)
        With f
            .Location = New Point(x, y)
            .Visible = True
            .Tag = c
        End With

        Return f
    End Function

    Private Function AddSwitch(ByVal c As CCoreAudio.CControl, ByVal x As Integer, ByVal y As Integer) As UILineControl2
        Dim s As UILineControl2 = New UILineControl2(c, UILineControl2.UIControlTypeConstants.Switch)
        AddHandler s.Enter, AddressOf UILineControlGotFocus
        AddHandler s.Editing, AddressOf UILineControlEditing
        Me.Controls.Add(s)
        With s
            .Location = New Point(x, y)
            .Visible = True
            .AutoSize = True
            .Text = c.Name
            .Tag = c
        End With

        Return s
    End Function

    Private Sub UILineControlEditing()
        AutoResize()
        RaiseEvent Editing()
    End Sub

    Private Sub DeleteLineControls()
Restart:
        For Each c As Control In Me.Controls
            If TypeOf c Is UILineControl Then
                Me.Controls.Remove(c)
                GoTo Restart
            End If
        Next
    End Sub

    Private Sub UILine_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
        Dim g As Graphics = e.Graphics

        g.Clear(Color.FromKnownColor(KnownColor.Control))
        If mEditMode And Not mVisible Then
            ControlPaint.DrawGrid(g, New Rectangle(0, 0, Width - 1, Height - 1), New Size(3, 3), Color.FromKnownColor(KnownColor.Control))
        End If

        ControlPaint.DrawBorder3D(g, 0, mMargins.Y + mH1 + 4 + mH2 + 2 + mH3 + mMargins.Y, Width - 1, 2, Border3DStyle.SunkenOuter)
        If mEditMode Then
            ControlPaint.DrawFocusRectangle(g, Me.DisplayRectangle)
            ControlPaint.DrawContainerGrabHandle(g, New Rectangle(Width - 17, 3, 13, 13))
        Else
            ControlPaint.DrawBorder3D(g, 0, 0, Width - 1, Height - 1, Border3DStyle.Etched)
        End If

        g.DrawString(mText, mFont1, New SolidBrush(Color.Black), mMargins)
        If mShowDestinationLine Then
            g.DrawString(mDestinationText, mFont2, New SolidBrush(Color.Black), mMargins.X, mMargins.Y + mH1 + 4)
        End If
        g.DrawString(mDeviceName, mFont2, New SolidBrush(Color.Black), mMargins.X, mMargins.Y + mH1 + 4 + mH2 + 1)
    End Sub

    Private Sub AutoResize()
        Dim w As Integer = mWidestLabelWidth + 10
        Dim h As Integer = 42

        For Each c As Control In Me.Controls
            w = Math.Max(w, c.Left + c.Width)
            h = Math.Max(h, c.Top + c.Height)
        Next

        Dim ns As Size = New Size(w + mMargins.X, h + mMargins.Y)
        If ns.Width <> Me.Width Or ns.Height <> Me.Height Then
            Me.Size = ns
            Me.Refresh()
        End If
    End Sub

    Private Sub UILineControlGotFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim uilc As UILineControl

        If Not mEditMode Then Exit Sub
        For Each c As Control In Me.Controls
            If TypeOf c Is UILineControl Then
                uilc = c
                uilc.EditMode = (c Is sender)
                If uilc.EditMode Then uilc.BringToFront()
            End If
        Next

        Me.Refresh()
    End Sub

    Private Sub UILine_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.DoubleClick
        EditMode = Not EditMode
    End Sub

    Protected Overrides Sub Finalize()
        DeleteLineControls()
        mFont1 = Nothing
        mFont2 = Nothing
        mLine = Nothing
        MyBase.Finalize()
    End Sub

    Private Sub UILine_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
        mPos = New Point(e.X, e.Y)
        If e.Button = Windows.Forms.MouseButtons.Left Then
            If mEditMode And mCanDrag Then mIsDragging = True
        Else
            mnuCtx.Show(Me, mPos)
        End If
    End Sub

    Private Sub UILine_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseUp
        mCanDrag = False
        mIsDragging = False
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub UILine_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
        If mEditMode Then mCanDrag = (e.X >= Width - 17 And e.X <= Width - 17 + 13)
        Me.Cursor = IIf(mCanDrag, Cursors.SizeAll, Cursors.Default)
        If mIsDragging Then
            'Me.Left += e.X - mPos.X
            'Me.Top += e.Y - mPos.Y
            DockIt(e.X, e.Y)

            RaiseEvent Editing()
        End If
    End Sub

    Private Sub DockIt(ByVal x As Integer, ByVal y As Integer)
        With Me
            Dim nx As Integer = .Left + (x - mPos.X)
            Dim ny As Integer = .Top + (y - mPos.Y)

            Dim nr As Integer = nx + .Width
            Dim nb As Integer = ny + .Height

            Dim sm As Integer = 4
            Dim smd As Integer = sm * 3

            If nx <= smd Then nx = sm
            If ny <= smd Then ny = sm

            'If nr > .Parent.Width - smd Then nx = (.Parent.Width - sm) - .Width
            'If nb > .Parent.Height - smd - 15 Then ny = (.Parent.Height - sm) - .Height - 15

            For Each c As Control In .Parent.Controls
                If TypeOf c Is UILine Then
                    If (Not c Is Me) And c.Visible Then

                        ' Snap to right
                        If Math.Abs(nx - (c.Left + c.Width)) <= smd And LeftRightOverlap(ny, c) Then
                            nx = (c.Left + c.Width + sm)
                            If Math.Abs(ny - c.Top) <= smd Then
                                ny = c.Top
                            Else
                                If Math.Abs((ny + .Height) - (c.Top + c.Height)) <= smd Then ny = c.Top + c.Height - .Height
                            End If
                        End If

                        ' Snap to left
                        If Math.Abs((nx + .Width - c.Left)) <= smd And LeftRightOverlap(ny, c) Then
                            nx = (c.Left - .Width - sm)
                            If Math.Abs(ny - c.Top) <= smd Then
                                ny = c.Top
                            Else
                                If Math.Abs((ny + .Height) - (c.Top + c.Height)) <= smd Then ny = c.Top + c.Height - .Height
                            End If
                        End If

                        ' Snap to bottom
                        If Math.Abs(ny - (c.Top + c.Height)) <= smd And TopBottomOverlap(nx, c) Then
                            ny = (c.Top + c.Height + sm)
                            If Math.Abs(nx - c.Left) <= smd And nx > c.Left Then
                                nx = c.Left
                            Else
                                If Math.Abs((nx + .Width) - (c.Left + c.Width)) <= smd Then nx = c.Left + c.Width - .Width
                            End If
                        End If

                        ' Snap to top
                        If Math.Abs((ny + .Height) - c.Top) <= smd And TopBottomOverlap(nx, c) Then
                            ny = c.Top - .Height - sm
                            If Math.Abs(nx - c.Left) <= smd And nx > c.Left Then
                                nx = c.Left
                            Else
                                If Math.Abs((nx + .Width) - (c.Left + c.Width)) <= smd Then nx = c.Left + c.Width - .Width
                            End If
                        End If

                    End If
                End If
            Next
            .Location = New Point(nx, ny)
        End With
    End Sub

    Private Function LeftRightOverlap(ByVal n As Integer, ByVal c As Control) As Boolean

        With Me
            LeftRightOverlap = (((n >= c.Top) And (n <= (c.Top + c.Height))) Or _
                                (((n + .Height) >= c.Top) And ((n + .Height) <= (c.Top + c.Height))))

            If Not LeftRightOverlap Then
                LeftRightOverlap = (((c.Top >= n) And ((c.Top + c.Height) <= n)) Or _
                                    ((c.Top + c.Height >= n) And (c.Top + c.Height <= .Top + .Height)))
            End If
        End With

    End Function

    Private Function TopBottomOverlap(ByVal n As Integer, ByVal c As UILine) As Boolean

        With Me
            TopBottomOverlap = (((n >= c.Left) And (n <= (c.Left + c.Width))) Or _
                                (((n + .Width) >= c.Left) And ((n + .Width) <= (c.Left + c.Width))))

            If Not TopBottomOverlap Then
                TopBottomOverlap = (((c.Left >= n) And (c.Left + c.Width <= n)) Or _
                                    ((c.Left + c.Width >= n) And (c.Left + c.Width <= .Left + .Width)))
            End If
        End With

    End Function

    Private Sub UILine_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Click
        Me.BringToFront()
    End Sub

    Private Sub mnuCtxEditMode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCtxEditMode.Click
        EditMode = Not EditMode
    End Sub

    Private Sub mnuCtxVisible_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCtxVisible.Click
        mVisible = Not mVisible
        mnuCtxVisible.Checked = mVisible
        Me.Refresh()
        If Not mEditMode Then Me.Visible = mVisible
    End Sub

    Private Sub MenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCtxShowDestinationLine.Click
        ShowDestinationLine = Not ShowDestinationLine
        mnuCtxShowDestinationLine.Checked = mShowDestinationLine
    End Sub

    Private Sub mnuCtxRenameLineName_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCtxRenameLineName.Click
        Dim txt As TextBox = New TextBox

        With txt
            .Text = mText
            .SelectAll()
            .Location = New Point(4, 4)
            .Width = Width - 10
            .Visible = True
            AddHandler .LostFocus, AddressOf txtCancelEditing
            'AddHandler .Leave, AddressOf txtCancelEditing
            AddHandler .KeyUp, AddressOf txtKeyUp
        End With
        Me.Controls.Add(txt)
        txt.Focus()
    End Sub

    Private Sub txtKeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.Escape
                txtCancelEditing(Nothing, New System.EventArgs)
            Case Keys.Enter
                Text = CType(sender, TextBox).Text
                txtCancelEditing(Nothing, New System.EventArgs)
        End Select
    End Sub

    Private Sub txtCancelEditing(ByVal sender As Object, ByVal e As System.EventArgs)
        For Each c As Control In Me.Controls
            If TypeOf c Is TextBox Then
                Me.Controls.Remove(c)
                Exit For
            End If
        Next
    End Sub
End Class
