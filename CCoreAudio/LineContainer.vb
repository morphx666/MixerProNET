Public Class LineContainer
    Private masterVolume As CCoreAudio.CControl.CControlVolume
    Private sessionControl As CCoreAudio.CSession
    Private isSession As Boolean
    Private rangeRect As Rectangle = Rectangle.Empty

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
    End Sub

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
            Return VfControl.CoreAudioControl
        End Get
        Set(ByVal value As Object)
            If masterVolume IsNot Nothing Then
                RemoveHandler masterVolume.ControlChanged, AddressOf AdjustVolumeFader
            End If

            VfControl.CoreAudioControl = value

            If TypeOf value Is CCoreAudio.CSession Then
                isSession = True

                For Each c In CType(value, CCoreAudio.CSession).ControlVolume.Parent.Parent.Parent.Line.Controls
                    If c.ControlVolume IsNot Nothing Then
                        masterVolume = c.ControlVolume

                        AddHandler masterVolume.ControlChanged, AddressOf AdjustVolumeFader
                        AdjustVolumeFader(masterVolume)
                        Exit For
                    End If
                Next
            End If
        End Set
    End Property

    Public Property Value As Integer
        Get
            Return VfControl.Value
        End Get
        Set(value As Integer)
            VfControl.Value = value
        End Set
    End Property

    Private Function MasterVolumeToLinear() As Single
        If isSession Then
            Dim min As Double = masterVolume.Min
            Dim max As Double = masterVolume.Max
            Dim value As Double = masterVolume.UniformValue + Math.Log10(100)
            Dim size As Double = max - min

            Dim p As Double = Math.Min(10 ^ ((value - min) / size) ^ 2 / 100, 1)
            'Debug.WriteLine(p)
            Return p
        Else
            Return 1 / 100
        End If
    End Function

    Private Sub AdjustVolumeFader(sender As CCoreAudio.CControl.CControlObject)
        If Me.InvokeRequired Then
            Me.Invoke(New MethodInvoker(AddressOf SafeAdjustVolumeFader))
        Else
            SafeAdjustVolumeFader()
        End If
    End Sub

    Private Sub SafeAdjustVolumeFader()
        Select Case VfControl.Orientation
            Case Orientation.Horizontal
                VfControl.Width = Me.Width * MasterVolumeToLinear()
        End Select
    End Sub

    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        MyBase.OnPaintBackground(pevent)

        Dim g As Graphics = pevent.Graphics
        Dim r As Rectangle = Me.DisplayRectangle

        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        rangeRect = VfControl.DrawPeakMeter(g, r, False)
        VfControl.DrawVolumeIndicator(g, r, rangeRect, False)
    End Sub
End Class
