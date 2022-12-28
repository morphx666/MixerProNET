''' <summary>
''' <p>This control mimics, as much as possible, the volume fader control included with Windows Vista and later versions of Windows.</p>
''' <p>The control inherits from the <see cref="TrackBar">TrackBar</see> control, but implements several new features only available sndvol.exe and mmsys.cpl applications from post-Vista versions of Windows.</p>
''' </summary>
''' <remarks></remarks>
Public Class VolumeFader
    Private Enum Modes
        DisabledOrSession = 0
        Normal = 1
    End Enum

    Private thumbRect As Rectangle = New Rectangle(0, 0, 19, 19)
    Private isOverThumb As Boolean
    Private rangeRect As Rectangle = Rectangle.Empty
    Private cachedValue As Integer
    Private hasPeakMeter As Boolean

    Private sessionControl As CCoreAudio.CSession
    Private parentContainer As LineContainer

    Private sessionControlHasMute As Boolean

    Private ctrlMax As Integer
    Private ctrlMin As Integer
    Private ctrlSteps As Integer

    Private Enum ControlTypes
        CControlVolume
        CSession
    End Enum

    Private mIntegralChanges As Boolean = True
    Private mPeakLevel As Integer
    Private mControl As Object
    Private mControlType As ControlTypes = ControlTypes.CControlVolume

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
    End Sub

    ''' <summary>
    ''' Used to instantiate a new instance of the VolumeFader control from a <see cref="CCoreAudio.CControl.CControlVolume">CControlVolume</see> object.
    ''' </summary>
    ''' <param name="coreAudioControl">A reference to a valid <see cref="CCoreAudio.CControl.CControlVolume">CControlVolume</see> object</param>
    ''' <remarks>
    ''' You can also set the CoreAudioControl through the <paramref name="CoreAudioControl">CoreAudioControl</paramref> property.
    ''' </remarks>
    Public Sub New(coreAudioControl As CCoreAudio.CControl.CControlVolume)
        Me.New()
        Me.CoreAudioControl = coreAudioControl
    End Sub

    ''' <summary>
    ''' Used to instantiate a new instance of the VolumeFader control from a <see cref="CCoreAudio.CSession">CSession</see> object.
    ''' </summary>
    ''' <param name="coreAudioControl">A reference to a valid <see cref="CCoreAudio.CSession">CSession</see> object</param>
    ''' <remarks>
    ''' You can also set the CoreAudioControl through the <paramref name="CoreAudioControl">CoreAudioControl</paramref> property.
    ''' </remarks>
    Public Sub New(coreAudioControl As CCoreAudio.CSession)
        Me.New()
        Me.CoreAudioControl = coreAudioControl
    End Sub

    ''' <summary>
    ''' This property is ignored and changing it has no effect as the control changes it automatically based on the <see cref="TrackBar.Orientation">Orientation</see> property.
    ''' </summary>
    <Browsable(False)>
    Public Overloads Property TickStyle As TickStyle
        Get
            If MyBase.Orientation = Orientation.Horizontal Then
                Return TickStyle.TopLeft
            Else
                Return TickStyle.BottomRight
            End If
        End Get
        Set(ByVal value As TickStyle)
        End Set
    End Property

    ''' <summary>
    ''' Determines if the slider should respect the resolution of the fader as specified by the <paramref cref="TrackBar.TickFrequency">TickFrequency</paramref> property.
    ''' </summary>
    ''' <remarks>When enabled, the control will only allow changes to its value that fall inside increments as specified by the <see cref="TrackBar.TickFrequency">TickFrequency</see> property.</remarks>
    Public Property IntegralChanges As Boolean
        Get
            Return mIntegralChanges
        End Get
        Set(ByVal value As Boolean)
            mIntegralChanges = value
        End Set
    End Property

    ''' <summary>
    ''' Use to query the current peak level for the control.
    ''' </summary>
    ''' <remarks>The setter should not be used by the host application since it is only provided for <see cref="CBinding">binding</see> purposes.</remarks>
    <Browsable(False)>
    Public Property PeakLevel As Integer
        Get
            Return mPeakLevel
        End Get
        Set(ByVal value As Integer)
            If value <> mPeakLevel Then
                mPeakLevel = value
                Me.Invalidate()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Used to define the CCoreAudio control to which this VolumeMeter object is bound to.
    ''' </summary>
    ''' <remarks>
    ''' <p>The VolumeMeter control can be bound to either a <see cref="CCoreAudio.CSession">CSession</see> or <see cref="CCoreAudio.CControl.CControlVolume">CControlVolume</see> object.</p>
    ''' <p>Based on the type of object that is passed, the control will automatically determine its characteristic and provide a peak meter display if the control supports it.</p>
    ''' </remarks>
    <Browsable(False)>
    Public Property CoreAudioControl As Object
        Get
            Return mControl
        End Get
        Set(ByVal value As Object)
            If mControl IsNot Nothing Then
                If mControlType = ControlTypes.CSession Then CType(mControl, CCoreAudio.CSession).ControlPeakMeter.Binding.Remove()
                If mControlType = ControlTypes.CControlVolume Then CType(mControl, CCoreAudio.CControl).ControlPeakMeter.Binding.Remove()
                mControl.ControlVolume.Binding.Remove()

                If parentContainer IsNot Nothing Then parentContainer = Nothing
            End If

            mControl = value
            hasPeakMeter = False
            sessionControlHasMute = False

            If mControl IsNot Nothing Then
                If TypeOf mControl Is CCoreAudio.CSession Then
                    mControlType = ControlTypes.CSession
                    sessionControl = CType(mControl, CCoreAudio.CSession)
                    With sessionControl
                        MyBase.Minimum = .ControlVolume.Min
                        MyBase.Maximum = .ControlVolume.Max
                        MyBase.TickFrequency = .ControlVolume.Steps

                        .ControlPeakMeter.Binding.Define(Me, "PeakLevel")
                    End With

                    If TypeOf Me.Parent Is LineContainer Then parentContainer = CType(Me.Parent, LineContainer)
                    hasPeakMeter = True
                    sessionControlHasMute = sessionControl.ControlMute IsNot Nothing
                Else
                    mControlType = ControlTypes.CControlVolume
                    With CType(mControl, CCoreAudio.CControl)
                        If .ControlPeakMeter IsNot Nothing Then
                            .ControlPeakMeter.Binding.Define(Me, "PeakLevel")
                            hasPeakMeter = True
                        End If
                    End With
                End If

                ctrlMax = mControl.ControlVolume.Max
                ctrlMin = mControl.ControlVolume.Min
                ctrlSteps = mControl.ControlVolume.Steps

                MyBase.Minimum = 0 ' mControl.ControlVolume.Min
                MyBase.Maximum = 100 ' mControl.ControlVolume.Max
                MyBase.TickFrequency = 1
                MyBase.SmallChange = MyBase.TickFrequency
                MyBase.LargeChange = MyBase.SmallChange

                Dim scale As CBinding.Scale
                If mControl.ControlVolume.Min = 0 AndAlso mControl.ControlVolume.Max = 100 AndAlso mControl.ControlVolume.Steps = 1 Then
                    scale = New CBinding.Scale(CBinding.Scale.ScaleMode.Linear, mControl.ControlVolume.Min, mControl.ControlVolume.Max, mControl.ControlVolume.Steps)
                Else
                    scale = New CBinding.Scale(CBinding.Scale.ScaleMode.Logarithmic, mControl.ControlVolume.Min, mControl.ControlVolume.Max, mControl.ControlVolume.Steps)
                End If

                mControl.ControlVolume.Binding.Define(Me, "Value", "ValueChanged", scale)
            End If

            Me.Invalidate()
        End Set
    End Property

    Private Sub ForeceRefresh(sender As Object)
        Me.Invalidate()
    End Sub

    Public Overloads Property Orientation As Orientation
        Get
            Return MyBase.Orientation
        End Get
        Set(ByVal value As Orientation)
            MyBase.Orientation = value
            MyBase.TickStyle = Me.TickStyle
        End Set
    End Property

    Protected Overrides Sub OnGotFocus(e As EventArgs)
        MyBase.OnGotFocus(e)
        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnEnabledChanged(e As EventArgs)
        MyBase.OnEnabledChanged(e)
        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        MyBase.OnPaintBackground(pevent)

        Dim g As Graphics = pevent.Graphics
        Dim r As Rectangle = Me.DisplayRectangle

        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        rangeRect = DrawPeakMeter(g, r)
        DrawScale(g, r, rangeRect)
        DrawVolumeIndicator(g, r, rangeRect)
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        Dim r As Rectangle = Me.DisplayRectangle

        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        DrawThumb(g, r, rangeRect)

        MyBase.OnPaint(e)
    End Sub

    Private Sub DrawThumb(g As Graphics, r As Rectangle, r1 As Rectangle)
        Dim thumb As VisualStyles.VisualStyleElement

        Select Case MyBase.Orientation
            Case Orientation.Horizontal
                If MyBase.Enabled Then
                    If isOverThumb Then
                        thumb = VisualStyles.VisualStyleElement.TrackBar.ThumbTop.Hot
                    Else
                        If MyBase.Focused Then
                            thumb = VisualStyles.VisualStyleElement.TrackBar.ThumbTop.Focused
                        Else
                            thumb = VisualStyles.VisualStyleElement.TrackBar.ThumbTop.Normal
                        End If
                    End If
                Else
                    thumb = VisualStyles.VisualStyleElement.TrackBar.ThumbTop.Disabled
                End If
            Case Orientation.Vertical
                If MyBase.Enabled Then
                    If isOverThumb Then
                        thumb = VisualStyles.VisualStyleElement.TrackBar.ThumbRight.Hot
                    Else
                        If MyBase.Focused Then
                            thumb = VisualStyles.VisualStyleElement.TrackBar.ThumbRight.Focused
                        Else
                            thumb = VisualStyles.VisualStyleElement.TrackBar.ThumbRight.Normal
                        End If
                    End If
                Else
                    thumb = VisualStyles.VisualStyleElement.TrackBar.ThumbRight.Disabled
                End If
        End Select

        Dim valuePercentage As Single = ValueToPercentage(cachedValue)
        Dim vsr = New VisualStyles.VisualStyleRenderer(thumb)
        thumbRect.Size = vsr.GetPartSize(g, VisualStyles.ThemeSizeType.Draw)

        Dim pos As Integer
        Select Case MyBase.Orientation
            Case Orientation.Horizontal
                pos = valuePercentage * rangeRect.Width
                thumbRect.Location = New Point(pos + thumbRect.Width \ 2 + 3, r.Top + thumbRect.Height / 2)
            Case Orientation.Vertical
                pos = valuePercentage * rangeRect.Height
                thumbRect.Location = New Point(2, pos + thumbRect.Height / 2 + 3)
        End Select

        vsr.DrawBackground(g, thumbRect)
    End Sub

    Private Function ValueToPercentage(value As Integer) As Single
        Dim w As Integer = MyBase.Maximum - MyBase.Minimum
        Dim min = MyBase.Minimum
        Dim max = MyBase.Maximum

        If MyBase.Orientation = Orientation.Horizontal Then
            Return (value - min) / (max - min)
        Else
            Return 1 - (value - min) / (max - min)
        End If
    End Function

    Private Sub DrawScale(g As Graphics, r As Rectangle, r1 As Rectangle)
        Using p = New Pen(Color.FromArgb(231, 231, 231), 2)
            Select Case MyBase.Orientation
                Case Orientation.Horizontal
                    g.DrawLine(p, r1.Left, r1.Top, r1.Left, r1.Bottom - thumbRect.Height - 1)
                    g.DrawLine(p, r1.Left + r1.Width \ 2, r1.Top, r1.Left + r1.Width \ 2, r1.Bottom - thumbRect.Height - 1)
                    g.DrawLine(p, r1.Right, r1.Top, r1.Right, r1.Bottom - thumbRect.Height - 1)
                Case Orientation.Vertical
                    g.DrawLine(p, r1.Right, r1.Bottom, r1.Left + thumbRect.Width + 1, r1.Bottom)
                    g.DrawLine(p, r1.Right, r1.Top + r1.Height \ 2, r1.Left + thumbRect.Width + 1, r1.Top + r1.Height \ 2)
                    g.DrawLine(p, r1.Right, r1.Top, r1.Left + thumbRect.Width + 1, r1.Top)
            End Select
        End Using
    End Sub

    Public Sub DrawVolumeIndicator(g As Graphics, r As Rectangle, r1 As Rectangle, Optional state As Boolean = True)
        Dim points() As Point
        Dim factor As Single = If(state AndAlso parentContainer IsNot Nothing, Me.Width / parentContainer.Width, 1)

        Select Case MyBase.Orientation
            Case Orientation.Horizontal
                points = New Point() {
                                        New Point(r1.Left, r1.Top),
                                        New Point(r1.Right, r1.Bottom - thumbRect.Height * 0.8 * factor),
                                        New Point(r1.Right, r1.Top)
                                    }
            Case Orientation.Vertical
                points = New Point() {
                                        New Point(r1.Right, r1.Bottom),
                                        New Point(r1.Right, r1.Top),
                                        New Point(r1.Left + thumbRect.Width * 0.8, r1.Top)
                                    }
        End Select

        Using b = New SolidBrush(If(state, Color.FromArgb(213, 223, 229), Color.FromArgb(238, 242, 245)))
            g.FillPolygon(b, points)
        End Using
    End Sub

    Public Function DrawPeakMeter(g As Graphics, r As Rectangle, Optional state As Boolean = True) As Rectangle
        Dim r1 As Rectangle
        Dim peakMeterSize As Integer

        If hasPeakMeter Then
            peakMeterSize = 4
        Else
            peakMeterSize = 1
        End If

        Select Case MyBase.Orientation
            Case Orientation.Horizontal
                r1 = New Rectangle(r.X + 14, r.Top + thumbRect.Height, r.Width - 29, peakMeterSize)
            Case Orientation.Vertical
                r1 = New Rectangle(r.X + 5, r.Y + 14, peakMeterSize, r.Height - 29)
            Case Else
                Throw New ArgumentException(String.Format("Invalid Orientation: '{0}'", MyBase.Orientation))
        End Select

        ' Erase
        Using b As New SolidBrush(If(state, Color.FromArgb(231, 234, 234), Color.FromArgb(245, 247, 247)))
            g.FillRectangle(b, r1)
        End Using

        If state AndAlso mPeakLevel > 0 Then
            Dim v As Single = mPeakLevel / 100
            Using b As New SolidBrush(Color.FromArgb(195, 197, 197))
                Select Case MyBase.Orientation
                    Case Orientation.Horizontal
                        g.FillRectangle(b, New Rectangle(r1.Left, r1.Top, r1.Width * v, r1.Height))
                    Case Orientation.Vertical
                        g.FillRectangle(b, New Rectangle(r1.Left, r1.Top + r1.Height - r1.Height * v, r1.Width, r1.Height * v))
                End Select
            End Using

            If (Not sessionControlHasMute) OrElse (sessionControlHasMute AndAlso (Not sessionControl.ControlMute.Mute)) Then
                v = (mPeakLevel * cachedValue / 100) / 100
                Using b As New Drawing2D.LinearGradientBrush(New Point(r1.Left, r1.Bottom), New Point(r1.Right, r1.Top), Color.FromArgb(128, 51, 153, 51), Color.FromArgb(51, 255, 51))
                    Select Case MyBase.Orientation
                        Case Orientation.Horizontal
                            g.FillRectangle(b, New Rectangle(r1.Left, r1.Top, r1.Width * v, r1.Height))
                        Case Orientation.Vertical
                            g.FillRectangle(b, New Rectangle(r1.Left, r1.Top + r1.Height - r1.Height * v, r1.Width, r1.Height * v))
                    End Select
                End Using
            End If
        End If

        r1.Inflate(1, 1)
        ControlPaint.DrawBorder3D(g, r1, Border3DStyle.SunkenOuter)

        Return r1
    End Function

    Private Sub VolumeFader_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If thumbRect.IntersectsWith(New Rectangle(e.Location, New Size(1, 1))) Then
            isOverThumb = True
            Me.Invalidate()
        ElseIf isOverThumb Then
            isOverThumb = False
            Me.Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnValueChanged(e As EventArgs)
        If mIntegralChanges Then
            If MyBase.TickFrequency > 0 Then MyBase.Value -= (MyBase.Value Mod MyBase.TickFrequency)
        End If
        MyBase.OnValueChanged(e)
    End Sub

    Private Sub VolumeFader_ValueChanged(sender As Object, e As EventArgs) Handles Me.ValueChanged
        cachedValue = MyBase.Value
        Me.Invalidate()
    End Sub
End Class
