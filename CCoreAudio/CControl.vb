Imports CoreAudio
Imports System.Collections.Generic
Imports System.Threading

Partial Public Class CCoreAudio
    ''' <summary>
    ''' Represents a control within a <see cref="CLine">line</see>.
    ''' </summary>
    ''' <remarks>
    ''' <p>In order to simplify the use the Core Audio APIs, <see cref="CCoreAudio">CCoreAudio</see> represents every single control through this class, whether the control represents a volume fader, a mute state or even a peak meter.</p>
    ''' <p>This way you can use this class to access a control regardless of its native Core Audio nature.</p>
    ''' <p>Core Audio can expose several different classes representing the same end-user experience. For example, depending on how you access the volume control for a particular line,
    ''' Core Audio may return a completely different and unique type of object, ranging from an AudioVolumeLevel, SimpleAudioVolume, AudioEndpointVolume, AudioMute, etc...</p>
    ''' <p>Fortunately, you don't have to worry about these complexities since, as explained before, MixerProNET provides an accessible abstraction layer to obfuscate these 
    ''' complexities and provide a simple way to query and manipulate the different controls in all mixers and sessions.</p>
    ''' </remarks>
    Public Class CControl
        Implements IDisposable

        Public Enum ControlTypeConstants
            Part = 1
            Session = 2
            AudioEndPoint = 3
            Unsupported = -1
        End Enum

        Private mName As String
        Private mPart As Part
        Private mParent As CLine
        Private mControl As Object
        Private mControlType As ControlTypeConstants = ControlTypeConstants.Unsupported
        Private mTypeName As String

        Private mSubControls As New List(Of CControlObject)

        Private mControlVolume As CControlVolume
        Private mControlMute As CControlMute
        Private mControlLoudness As CControlLoudness
        Private mControlPeakMeter As CControlPeakMeter

        Private mLastErrorMessage As String

        Private timerRefreshPeakMeter As Timer = New Timer(New TimerCallback(AddressOf UpdatePeakMeter), Nothing, Timeout.Infinite, Timeout.Infinite)

        Private isInit As Boolean = False

        Protected Friend Sub New(parent As CLine, part As Part)
            mParent = parent
            mPart = part

            Select Case Part.SubType
                Case KSNODETYPE.VOLUME
                    mControl = part.AudioVolumeLevel
                Case KSNODETYPE.MUTE
                    mControl = part.AudioMute
                Case KSNODETYPE.PEAKMETER
                    mControl = part.AudioPeakMeter
                Case KSNODETYPE.LOUDNESS
                    mControl = part.AudioLoudness
                Case Else
                    mControlType = ControlTypeConstants.Unsupported
            End Select

            mName = mPart.Name
            mTypeName = mPart.SubTypeName
            If mControl IsNot Nothing Then
                mControlType = ControlTypeConstants.Part
                GenerateControls()
            ElseIf mControlType <> ControlTypeConstants.Unsupported Then
                Throw New Exception(String.Format("Unable to create control for {0}->{1}->{2}", parent.Name, Part.Name, Part.SubTypeName))
            End If
        End Sub

        Protected Friend Sub New(parent As CLine, session As AudioSessionControl2)
            mParent = parent
            mControl = session
            mName = CSession.GetSessionName(session, "")
            mTypeName = "Session"
            mControlType = ControlTypeConstants.Session

            GenerateControls()
        End Sub

        Protected Friend Sub New(device As MMDevice)
            mParent = Nothing
            mControl = device
            mName = "Master Volume"
            mTypeName = "AudioEndPoint"
            mControlType = ControlTypeConstants.AudioEndPoint

            GenerateControls()
        End Sub

        ''' <summary>
        ''' Returns the name of the control.
        ''' </summary>
        ''' <remarks>
        ''' <p>When CControl is interfacing a session, the name may be missing. For more information see the <see cref="CSession.Name">Name</see> property of the <see cref="CSession">CSesion</see> class.</p>
        ''' </remarks>
        Public ReadOnly Property Name As String
            Get
                Return mName
            End Get
        End Property

        ''' <summary>
        ''' Returns a string representation of the type of control that CControl is managing.
        ''' <seealso cref="Control">ControlTypeName</seealso>
        ''' <seealso cref="ControlType">ControlType</seealso>
        ''' </summary>
        Public ReadOnly Property ControlTypeName As String
            Get
                Return mTypeName
            End Get
        End Property

        ''' <summary>
        ''' Returns a reference to the object that CControl is managing.
        ''' </summary>
        ''' <remarks>
        ''' Note that the object returned will be a reference to a COM object representing an instance from one of Core Audio's interfaces.
        ''' <seealso cref="ControlTypeName">ControlTypeName</seealso>
        ''' <seealso cref="ControlType">ControlType</seealso>
        ''' </remarks>
        Public ReadOnly Property Control As Object
            Get
                Return mControl
            End Get
        End Property

        ''' <summary>
        ''' Returns a reference to the parent <see cref="CLine">CLine</see> object
        ''' </summary>
        Public ReadOnly Property Parent As CLine
            Get
                Return mParent
            End Get
        End Property

        Private ReadOnly Property LastErrorMessage As String
            Get
                Return mLastErrorMessage
            End Get
        End Property

        Protected Friend Function IsBasedOnPart(part As Part) As Boolean
            Return mPart IsNot Nothing AndAlso (Part.LocalId = mPart.LocalId)
        End Function

        Private Sub GenerateControls()
            If isInit Then Exit Sub

            mLastErrorMessage = ""

            Try
                Select Case mControlType
                    Case ControlTypeConstants.Part
                        Select Case mPart.SubType
                            Case KSNODETYPE.VOLUME
                                mControlVolume = New CControlVolume(Me, mControl, ControlTypeConstants.Part)
                                AddHandler mPart.OnPartNotification, Sub() UpdateBindings(True)
                            Case KSNODETYPE.MUTE
                                mControlMute = New CControlMute(Me, mControl, ControlTypeConstants.Part)
                                AddHandler mPart.OnPartNotification, Sub() UpdateBindings(, True)
                            Case KSNODETYPE.LOUDNESS
                                mControlLoudness = New CControlLoudness(Me, mControl, ControlTypeConstants.Part)
                                AddHandler mPart.OnPartNotification, Sub() UpdateBindings(, , True)
                            Case KSNODETYPE.PEAKMETER
                                mControlPeakMeter = New CControlPeakMeter(Me, mControl, ControlTypeConstants.Part)
                                AddHandler mPart.OnPartNotification, Sub() UpdateBindings(, , , True)
                        End Select
                    Case ControlTypeConstants.Session
                        mControlMute = New CControlMute(Me, mControl, ControlTypeConstants.Session)
                        mControlVolume = New CControlVolume(Me, mControl, ControlTypeConstants.Session)
                        mControlPeakMeter = New CControlPeakMeter(Me, mControl, ControlTypeConstants.Session)
                        AddHandler CType(mControl, AudioSessionControl2).OnSimpleVolumeChanged, Sub() UpdateBindings(True, True)
                    Case ControlTypeConstants.AudioEndPoint
                        mControlMute = New CControlMute(Me, mControl, ControlTypeConstants.AudioEndPoint)
                        mControlVolume = New CControlVolume(Me, mControl, ControlTypeConstants.AudioEndPoint)
                        mControlPeakMeter = New CControlPeakMeter(Me, mControl, ControlTypeConstants.AudioEndPoint)
                        AddHandler CType(mControl, MMDevice).AudioEndpointVolume.OnVolumeNotification, Sub() UpdateBindings(True, True, , True)
                    Case ControlTypeConstants.Unsupported
                        ' Do nothing...
                End Select
            Catch ex As Exception
                mLastErrorMessage = ex.Message
            End Try

            If mControlMute IsNot Nothing Then mSubControls.Add(mControlMute)
            If mControlVolume IsNot Nothing Then mSubControls.Add(mControlVolume)
            If mControlLoudness IsNot Nothing Then mSubControls.Add(mControlLoudness)
            If mControlPeakMeter IsNot Nothing Then
                mSubControls.Add(mControlPeakMeter)
                timerRefreshPeakMeter.Change(500, 30)
            End If

            isInit = True
        End Sub

        Private Sub UpdateBindings(Optional updateVolume As Boolean = False,
                                   Optional updateMute As Boolean = False,
                                   Optional updateLoudness As Boolean = False,
                                   Optional updatePeakMeter As Boolean = False)
            If updateVolume Then
                If mControlVolume.Binding.IsBound Then mControlVolume.Binding.UpdateBoundObjValue()
                mControlVolume.DoRaiseEvent()
            End If

            If updateMute Then
                If mControlMute.Binding.IsBound Then mControlMute.Binding.UpdateBoundObjValue()
                mControlMute.DoRaiseEvent()
            End If

            If updateLoudness Then
                If mControlLoudness.Binding.IsBound Then mControlLoudness.Binding.UpdateBoundObjValue()
                mControlLoudness.DoRaiseEvent()
            End If

            If updatePeakMeter Then
                If mControlPeakMeter.Binding.IsBound Then mControlPeakMeter.Binding.UpdateBoundObjValue()
                mControlPeakMeter.DoRaiseEvent()
            End If
        End Sub

        Private Sub UpdatePeakMeter(state As Object)
            UpdateBindings(, , , True)
        End Sub

        ''' <summary>
        ''' Returns the type of control as specified by one of the values from the <see cref="ControlTypeConstants">ControlTypeConstants</see> enumeration.
        ''' </summary>
        ''' <remarks>
        ''' This simply indicates the type of instance that CControl is managing.
        ''' <seealso cref="Control">Control</seealso>
        ''' <seealso cref="ControlTypeName">ControlTypeName</seealso>
        ''' </remarks>
        Public ReadOnly Property ControlType As ControlTypeConstants
            Get
                Return mControlType
            End Get
        End Property

        ''' <summary>
        ''' Returns a reference to a <see cref="CControlMute">CControlMute</see> object.
        ''' </summary>
        ''' <remarks>
        ''' The return type for the property may be nothing if the <see cref="CControl">control</see> being managed does not support muting.
        ''' <seealso cref="Control">ControlTypeName</seealso>
        ''' <seealso cref="ControlType">ControlType</seealso>
        ''' <seealso cref="ControlTypeName">ControlTypeName</seealso>
        ''' </remarks>
        Public ReadOnly Property ControlMute As CControlMute
            Get
                Return mControlMute
            End Get
        End Property

        ''' <summary>
        ''' Returns a reference to a <see cref="CControlVolume">CControlVolume</see> object.
        ''' </summary>
        ''' <remarks>
        ''' The return type for the property may be nothing if the <see cref="CControl">control</see> being managed does not support controlling volume.
        ''' <seealso cref="Control">ControlTypeName</seealso>
        ''' <seealso cref="ControlType">ControlType</seealso>
        ''' <seealso cref="ControlTypeName">ControlTypeName</seealso>
        ''' </remarks>
        Public ReadOnly Property ControlVolume As CControlVolume
            Get
                Return mControlVolume
            End Get
        End Property

        ''' <summary>
        ''' Returns a reference to a <see cref="CControlLoudness">CControlLoudness</see> object.
        ''' </summary>
        ''' <remarks>
        ''' The return type for the property may be nothing if the <see cref="CControl">control</see> being managed does not support controlling the loudness equalization setting.
        ''' <seealso cref="Control">ControlTypeName</seealso>
        ''' <seealso cref="ControlType">ControlType</seealso>
        ''' <seealso cref="ControlTypeName">ControlTypeName</seealso>
        ''' </remarks>
        Public ReadOnly Property ControlLoudness As CControlLoudness
            Get
                Return mControlLoudness
            End Get
        End Property

        ''' <summary>
        ''' Returns a reference to a <see cref="CControlPeakMeter">CControlPeakMeter</see> object.
        ''' </summary>
        ''' <remarks>
        ''' The return type for the property may be nothing if the <see cref="CControl">control</see> being managed does not support peak meters.
        ''' <seealso cref="Control">ControlTypeName</seealso>
        ''' <seealso cref="ControlType">ControlType</seealso>
        ''' <seealso cref="ControlTypeName">ControlTypeName</seealso>
        ''' </remarks>
        Public ReadOnly Property ControlPeakMeter As CControlPeakMeter
            Get
                Return mControlPeakMeter
            End Get
        End Property

        ''' <summary>
        ''' This list encapsulates all the <see cref="CControlObject">CControlObject</see> objects supported by this control.
        ''' </summary>
        ''' <remarks>
        ''' <p>You can access all the control's <see cref="CControlObject">CControlObject</see> objects directly through the <see cref="ControlMute">ControlMute</see>, <see cref="ControlVolume">ControlVolume</see> and <see cref="ControlPeakMeter">ControlPeakMeter</see> properties.</p>
        ''' </remarks>
        Public ReadOnly Property SubControls As List(Of CControlObject)
            Get
                Return mSubControls
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return mName
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                If mPart IsNot Nothing Then
                    mPart.Dispose()
                    mParent = Nothing
                End If
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overrides Sub Finalize()
            Dispose(False)
            MyBase.Finalize()
        End Sub
#End Region
    End Class
End Class
