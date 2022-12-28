Imports CoreAudio
Imports System.Collections.Generic

Partial Public Class CCoreAudio
    Partial Public Class CControl
        ''' <summary>
        ''' This class allow for querying and manipulating controls that expose volume functionality.
        ''' </summary>
        Public Class CControlVolume
            Inherits CControlObject

            Private mAudioVolumeLevel As AudioVolumeLevel
            Private mSimpleAudioVolume As SimpleAudioVolume
            Private mAudioEndpointVolume As AudioEndpointVolume

            Private mMinValue As Single
            Private mMaxValue As Single
            Private mSteps As Single

            'Private masterVolume As CControlVolume
            'Private realSessionVolume As Single

            Private mChannels As Integer

            Protected Friend Sub New(ByVal parent As CControl, ByVal control As Object, ByVal controlType As ControlTypeConstants)
                MyBase.New(parent, control, controlType)

                Select Case MyBase.ControlType
                    Case ControlTypeConstants.Part
                        mAudioVolumeLevel = CType(control, AudioVolumeLevel)

                        mChannels = mAudioVolumeLevel.ChannelCount
                        Dim avgMin As Single = 0
                        Dim avgMax As Single = 0
                        Dim avgStp As Single = 0

                        For c = 0 To mChannels - 1
                            avgMin += mAudioVolumeLevel.GetLevelRange(c).minLevel
                            avgMax += mAudioVolumeLevel.GetLevelRange(c).maxLevel
                            avgStp += mAudioVolumeLevel.GetLevelRange(c).stepping
                        Next
                        mMinValue = avgMin / mChannels
                        mMaxValue = avgMax / mChannels
                        mSteps = avgStp / mChannels
                    Case ControlTypeConstants.Session
                        mSimpleAudioVolume = CType(control, AudioSessionControl2).SimpleAudioVolume

                        mChannels = 0
                        mMinValue = 0
                        mMaxValue = 100
                        mSteps = 1

                        'For Each c In parent.Parent.Parent.Line.Controls
                        '    If c.ControlVolume IsNot Nothing Then
                        '        masterVolume = c.ControlVolume

                        '        AddHandler masterVolume.ControlChanged, Sub()
                        '                                                    'DoRaiseEvent() <--- why isn't this working????
                        '                                                    Me.Volume = Me.Volume
                        '                                                End Sub

                        '        Exit For
                        '    End If
                        'Next
                    Case ControlTypeConstants.AudioEndPoint
                        mAudioEndpointVolume = CType(control, MMDevice).AudioEndpointVolume

                        mChannels = mAudioEndpointVolume.Channels.Count
                        mMinValue = mAudioEndpointVolume.VolumeRange.MindB
                        mMaxValue = mAudioEndpointVolume.VolumeRange.MaxdB
                        mSteps = mAudioEndpointVolume.VolumeRange.IncrementdB
                End Select
            End Sub

            ''' <summary>
            ''' Used to query the number of channels available on the current control
            ''' </summary>
            ''' <remarks>
            ''' <p>When this control is managing a <see cref="CSession">session</see>, the number of channels returned will be 0.</p>
            ''' <p>This is because session objects do not allow the manipulation of the volume level on a per channel basis.</p>
            ''' </remarks>
            Public ReadOnly Property Channels As Integer
                Get
                    Select Case MyBase.ControlType
                        Case ControlTypeConstants.Part
                            Return mChannels
                        Case ControlTypeConstants.Session
                            Return 0
                        Case ControlTypeConstants.AudioEndPoint
                            Return mChannels
                    End Select
                End Get
            End Property

            ''' <summary>
            ''' Allows querying and manipulating the volume level of a control
            ''' </summary>
            Public Property Volume As Single
                Get
                    Select Case MyBase.ControlType
                        Case ControlTypeConstants.Part
                            Dim avg As Single = 0

                            For c = 0 To mChannels - 1
                                avg += mAudioVolumeLevel.GetLevel(c)
                            Next
                            Return avg / mChannels
                        Case ControlTypeConstants.Session
                            Return mSimpleAudioVolume.MasterVolume * 100
                        Case ControlTypeConstants.AudioEndPoint
                            Return mAudioEndpointVolume.MasterVolumeLevel
                    End Select
                End Get
                Set(ByVal value As Single)
                    Select Case MyBase.ControlType
                        Case ControlTypeConstants.Part
                            mAudioVolumeLevel.SetLevelUniform(value)
                        Case ControlTypeConstants.Session
                            'Dim sessionVolume As Single
                            'If masterVolume IsNot Nothing Then
                            '    Dim p As Single = 10 ^ ((masterVolume.UniformValue - masterVolume.Min) / (masterVolume.Max - masterVolume.Min))
                            '    realSessionVolume = value * (1 - (p / 10))
                            'Else
                            '    realSessionVolume = sessionVolume
                            'End If
                            mSimpleAudioVolume.MasterVolume = value / 100
                        Case ControlTypeConstants.AudioEndPoint
                            mAudioEndpointVolume.MasterVolumeLevel = value
                    End Select
                End Set
            End Property

            ''' <summary>
            ''' Allows querying and manipulating the volume level of a control on a per channel basis
            ''' </summary>
            ''' <param name="channel">The channel number for which to query or modify its value</param>
            ''' <remarks>
            ''' <p>Always use the <see cref="Channels">Channels</see> property to query the number of channels supported by the control.</p>
            ''' <p>When the control is managing a session, the <paramref name="channel">channel</paramref> parameter is ignored and it's the same as calling the standard <see cref="Volume">Volume</see> property.</p>
            ''' <seealso cref="Volume">Volume</seealso>
            ''' <seealso cref="Channels">Channels</seealso>
            ''' </remarks>
            Public Property Volume(ByVal channel As Integer) As Single
                Get
                    Select Case MyBase.ControlType
                        Case ControlTypeConstants.Part
                            Return mAudioVolumeLevel.GetLevel(channel)
                        Case ControlTypeConstants.Session
                            Return Me.Volume
                        Case ControlTypeConstants.AudioEndPoint
                            Return mAudioEndpointVolume.Channels(channel).VolumeLevel
                    End Select
                End Get
                Set(ByVal value As Single)
                    Select Case MyBase.ControlType
                        Case ControlTypeConstants.Part
                            mAudioVolumeLevel.SetLevel(channel, value)
                        Case ControlTypeConstants.Session
                            Me.Volume = value
                        Case ControlTypeConstants.AudioEndPoint
                            mAudioEndpointVolume.Channels(channel).VolumeLevel = value
                    End Select
                End Set
            End Property

            ''' <summary>
            ''' This property is the same as <see cref="Volume">Volume</see> and is only provided for compatibility purposes with the <see cref="Binding">Binding</see> support.
            ''' </summary>
            Public Property UniformValue As Single
                Get
                    Return Me.Volume
                End Get
                Set(ByVal value As Single)
                    If Me.Volume <> value Then Me.Volume = value
                End Set
            End Property

            ''' <summary>
            ''' Returns the minimum value supported by the control.
            ''' </summary>
            Public ReadOnly Property Min As Single
                Get
                    Return mMinValue
                End Get
            End Property

            ''' <summary>
            ''' Returns the maximum value supported by the control.
            ''' </summary>
            Public ReadOnly Property Max As Single
                Get
                    Return mMaxValue
                End Get
            End Property

            ''' <summary>
            ''' Returns the increments in which the Volume of the current control changes.
            ''' </summary>
            Public ReadOnly Property Steps As Single
                Get
                    Return mSteps
                End Get
            End Property
        End Class
    End Class
End Class
