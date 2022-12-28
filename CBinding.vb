''' <summary>
''' Provides an easy way to bind <see cref="Control">Windows Forms Controls</see> with controls in a line.
''' </summary>
''' <remarks>
''' Here's a sample code to bind a <see cref="TrackBar">TrackBar</see> control with a <see cref="CCoreAudio.CControl.CControlVolume">CControlVolume</see> object:
''' <p>Legacy Version (pre-Vista)</p>
''' <code lang="vbnet">
''' myControl.Binding.Define(myTrackBar, "Value", "ValueChanged")
''' </code>
''' <p>Core Audio Version (post-Vista)</p>
''' <p>The only difference with the legacy version is that when using the <see cref="CCoreAudio">CCoreAudio</see> class, you will need to bind objects exposed by the <see cref="CControl">CControl</see> class, such as <see cref="CCoreAudio.CControl">CControlVolume</see>, through either the <see cref="CCoreAudio.CControl.CControlVolume">ControlVolume</see> or the <see cref="CCoreAudio.CControl.CControlMute">ControlMute</see> properties.</p>
''' <code lang="vbnet">
''' myControl.ControlVolume.Binding.Define(myTrackBar, "Value", "ValueChanged")
''' </code>
''' </remarks>
Public Class CBinding
    Public Class Scale
        Public Enum ScaleMode
            Linear
            Logarithmic
        End Enum

        Public Property Mode As ScaleMode
        Public Property Minimum As Single
        Public Property Maximum As Single
        Public Property Steps As Single

        Public Sub New(mode As ScaleMode, min As Single, max As Single, steps As Single)
            Me.Mode = mode
            Me.Minimum = min
            Me.Maximum = max
            Me.Steps = steps
        End Sub
    End Class

    Private pControlObject As Object
    Private pProperty As Reflection.PropertyInfo
    Private pEvent As Reflection.EventInfo
    Private pMixerControl As Object
    Private mIsBound As Boolean
    Private disableEvent As Boolean
    Private pEventHnd As [Delegate]
    Private mScale As Scale
    Private pControlObjectMinimum As Integer
    Private pControlObjectMaximum As Integer

    Private isReady As Boolean

    Private initialValue As Object
    Private delayInitTimer As Threading.Timer

    Private Sub UpdateValueFromBoundObj(sender As Object, e As EventArgs)
        If Not mIsBound Then Exit Sub

        disableEvent = True
        Try
            If Not pMixerControl Is Nothing Then
                If mScale Is Nothing Then
                    Select Case pProperty.GetValue(pControlObject, Nothing).GetType.Name
                        Case "Boolean"
                            pMixerControl.UniformValue = CInt(If(pProperty.GetValue(pControlObject, Nothing), 1, 0))
                        Case Else
                            pMixerControl.UniformValue = pProperty.GetValue(pControlObject, Nothing)
                    End Select
                Else
                    pMixerControl.UniformValue = ConvertFromPropertyValue(pProperty.GetValue(pControlObject, Nothing))
                End If
            End If
        Catch ex As Exception
            Throw New Exception("UpdateValueFromBoundObj: " + ex.Message)
        End Try
        disableEvent = False
    End Sub

    Protected Friend Sub UpdateBoundObjValue(Optional force As Boolean = False)
        If Not force AndAlso (Not mIsBound OrElse disableEvent) Then Exit Sub

        Try
            If TypeOf pControlObject Is Control Then
                Dim ctrl As Control = CType(pControlObject, Control)
                If ctrl.IsDisposed OrElse ctrl.Disposing Then Exit Sub

                If ctrl.InvokeRequired Then
                    ctrl.Invoke(New MethodInvoker(AddressOf SafeUpdateBoundObjValue))
                Else
                    SafeUpdateBoundObjValue()
                End If
            Else
                SafeUpdateBoundObjValue()
            End If
        Catch ex As Exception
            'Throw New Exception("UpdateBoundObjValue: " + ex.Message)
        End Try
    End Sub

    Private Sub SafeUpdateBoundObjValue()
        If Not pProperty Is Nothing Then
            disableEvent = True
            Try
                Select Case pProperty.GetValue(pControlObject, Nothing).GetType.Name
                    Case "Boolean"
                        pProperty.SetValue(pControlObject, CType(pMixerControl.UniformValue, Boolean), Nothing)
                    Case Else
                        Dim value As Single = CType(pMixerControl.UniformValue, Single)
                        If Not isReady Then
                            If value <> initialValue Then Exit Sub
                            isReady = True
                        End If
                        pProperty.SetValue(pControlObject, ConvertToPropertyValue(value), Nothing)
                End Select
            Catch ex As Exception
                Throw New Exception("SafeUpdateBoundObjValue: " + ex.Message)
            End Try
            disableEvent = False
        End If
    End Sub

    Private Function ConvertFromPropertyValue(value As Integer) As Single
        If mScale Is Nothing Then
            Return value
        Else
            Dim p As Single = (value - pControlObjectMinimum) / (pControlObjectMaximum - pControlObjectMinimum)
            Select Case mScale.Mode
                Case Scale.ScaleMode.Linear
                    Return mScale.Minimum + p * (mScale.Maximum - mScale.Minimum)
                Case Scale.ScaleMode.Logarithmic
                    ' Convert from linear to dB
                    If p = 0 Then
                        Return mScale.Minimum
                    Else
                        Dim n = (mScale.Maximum - mScale.Minimum) / 2
                        Return mScale.Maximum + n * Math.Log10(p)
                    End If
            End Select
        End If
    End Function

    Private Function ConvertToPropertyValue(value As Single) As Integer
        If mScale Is Nothing Then
            Return value
        Else
            Dim p As Single
            Select Case mScale.Mode
                Case Scale.ScaleMode.Linear
                    p = (value - mScale.Minimum) / (mScale.Maximum - mScale.Minimum)
                Case Scale.ScaleMode.Logarithmic
                    ' Convert from dB to linear

                    Dim n = (mScale.Maximum - mScale.Minimum) / 2

                    ' http://msdn.microsoft.com/en-us/library/windows/desktop/dd370798%28v=vs.85%29.aspx
                    ' -96dB = 20 * Math.Log10(1/65535)
                    ' n     = x
                    ' x     = n * 20 * Math.Log10(1/65535) / -96
                    ' n = n * 20 * Math.Log10(1 / 65535) / -96
                    p = 10 ^ ((value - mScale.Maximum) / n)
            End Select
            Return pControlObjectMinimum + p * (pControlObjectMaximum - pControlObjectMinimum)
        End If
    End Function

    Public ReadOnly Property IsBound As Boolean
        Get
            Return mIsBound
        End Get
    End Property

    ''' <summary>
    ''' Used to define a binding between a <see cref="Control">Windows Forms Control</see> and control in a line of a mixer.
    ''' </summary>
    ''' <param name="control">The Windows Forms Control</param>
    ''' <param name="propertyName">The name of the property that will be bound to the control</param>
    ''' <param name="eventName">Optional. The name of the event that gets triggered when the user interacts with the control.</param>
    ''' <remarks></remarks>
    Public Function Define(control As Object, propertyName As String, Optional eventName As String = "", Optional scale As Scale = Nothing) As Boolean
        Dim result As Boolean

        Try
            pControlObject = control
            If propertyName <> "" Then pProperty = pControlObject.GetType.GetProperty(propertyName)

            initialValue = pMixerControl.UniformValue

            If eventName <> "" Then
                pEvent = pControlObject.GetType.GetEvent(eventName)
                pEventHnd = New EventHandler(AddressOf UpdateValueFromBoundObj)
                pEvent.AddEventHandler(pControlObject, pEventHnd)
            End If

            mScale = scale
            If mScale IsNot Nothing Then
                pControlObjectMinimum = pControlObject.Minimum
                pControlObjectMaximum = pControlObject.Maximum
            End If

            If TypeOf pMixerControl Is CCoreAudio.CControl.CControlVolume Then
                delayInitTimer = New Threading.Timer(New Threading.TimerCallback(Sub()
                                                                                     If isReady AndAlso mIsBound AndAlso Not disableEvent Then
                                                                                         pMixerControl.UniformValue = initialValue
                                                                                         UpdateBoundObjValue()
                                                                                         delayInitTimer.Dispose()
                                                                                     End If
                                                                                 End Sub),
                                                    Nothing,
                                                    10,
                                                    Threading.Timeout.Infinite)
            End If

            mIsBound = True
            disableEvent = False

            UpdateBoundObjValue()

            result = True
        Catch ex As Exception
            result = False
        End Try

        Return result
    End Function

    ''' <summary>
    ''' Used to remove a previously defined binding
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Remove()
        If pControlObject IsNot Nothing Then
            If pEvent IsNot Nothing Then pEvent.RemoveEventHandler(pControlObject, pEventHnd)
            pEvent = Nothing
            pEventHnd = Nothing
            pControlObject = Nothing
            pProperty = Nothing
        End If
        mIsBound = False
    End Sub

    Friend Sub New(ByVal c As Object)
        pMixerControl = c
    End Sub

    Friend Sub Dispose()
        Remove()
        pMixerControl = Nothing
    End Sub
End Class