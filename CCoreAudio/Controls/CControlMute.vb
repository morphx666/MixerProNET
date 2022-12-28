Imports CoreAudio
Imports System.Collections.Generic

Partial Public Class CCoreAudio
    Partial Public Class CControl
        ''' <summary>
        ''' This class allows for querying and manipulating controls that expose muting functionality.
        ''' </summary>
        Public Class CControlMute
            Inherits CControlObject

            Private mAudioMute As AudioMute
            Private mSimpleAudioVolume As SimpleAudioVolume
            Private mAudioEndpointVolume As AudioEndpointVolume

            'Private masterMute As CControlMute

            Private lastState As Boolean

            Protected Friend Sub New(ByVal parent As CControl, ByVal control As Object, ByVal controlType As ControlTypeConstants)
                MyBase.New(parent, control, controlType)

                Select Case MyBase.ControlType
                    Case ControlTypeConstants.Part
                        mAudioMute = CType(control, AudioMute)
                    Case ControlTypeConstants.Session
                        mSimpleAudioVolume = CType(control, AudioSessionControl2).SimpleAudioVolume

                        'Try
                        '    For Each c In parent.Parent.Parent.Line.Controls
                        '        If c.ControlMute IsNot Nothing Then
                        '            masterMute = c.ControlMute
                        '            AddHandler masterMute.ControlChanged, Sub()
                        '                                                      'DoRaiseEvent() <--- why isn't this working????
                        '                                                      mSimpleAudioVolume.Mute = Not mSimpleAudioVolume.Mute
                        '                                                      mSimpleAudioVolume.Mute = Not mSimpleAudioVolume.Mute
                        '                                                  End Sub

                        '            Exit For
                        '        End If
                        '    Next
                        'Catch
                        'End Try
                    Case ControlTypeConstants.AudioEndPoint
                        Dim device As MMDevice = CType(control, MMDevice)
                        Try
                            mAudioEndpointVolume = device.AudioEndpointVolume
                        Catch
                        End Try
                End Select
            End Sub

            ''' <summary>
            ''' This property is the same as <see cref="Mute">Mute</see> and is only provided for compatibility purposes with the <see cref="Binding">Binding</see> support.
            ''' </summary>
            Public Property UniformValue As Boolean
                Get
                    Return Me.Mute
                End Get
                Set(ByVal value As Boolean)
                    If Me.Mute <> value Then Me.Mute = value
                End Set
            End Property

            ''' <summary>
            ''' Allows querying and manipulating the mute state of a control
            ''' </summary>
            Public Property Mute As Boolean
                Get
                    Select Case MyBase.ControlType
                        Case ControlTypeConstants.Part
                            Return mAudioMute.Mute
                        Case ControlTypeConstants.Session
                            'If masterMute IsNot Nothing Then
                            '    Return mSimpleAudioVolume.Mute Or masterMute.Mute
                            'Else
                            Return mSimpleAudioVolume.Mute
                            'End If
                        Case ControlTypeConstants.AudioEndPoint
                            Return mAudioEndpointVolume.Mute
                    End Select
                End Get
                Set(ByVal value As Boolean)
                    Select Case MyBase.ControlType
                        Case ControlTypeConstants.Part
                            mAudioMute.Mute = value
                        Case ControlTypeConstants.Session
                            'If masterMute IsNot Nothing AndAlso masterMute.Mute AndAlso Not value Then masterMute.Mute = False
                            mSimpleAudioVolume.Mute = value
                        Case ControlTypeConstants.AudioEndPoint
                            mAudioEndpointVolume.Mute = value
                    End Select
                End Set
            End Property
        End Class
    End Class
End Class
