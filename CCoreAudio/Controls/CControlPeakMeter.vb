Imports CoreAudio
Imports System.Collections.Generic

Partial Public Class CCoreAudio
    Partial Public Class CControl
        ''' <summary>
        ''' This class allows for querying controls that expose a peak meter.
        ''' </summary>
        Public Class CControlPeakMeter
            Inherits CControlObject

            Private mAudioPeakMeter As AudioPeakMeter
            Private mAudioMeterInformation As AudioMeterInformation
            Private mChannels As Integer = 0

            Protected Friend Sub New(ByVal parent As CControl, ByVal control As Object, ByVal controlType As ControlTypeConstants)
                MyBase.New(parent, control, controlType)

                Select Case MyBase.ControlType
                    Case ControlTypeConstants.Part
                        mAudioPeakMeter = CType(control, AudioPeakMeter)
                    Case ControlTypeConstants.Session
                        mAudioMeterInformation = CType(control, AudioSessionControl2).AudioMeterInformation
                    Case ControlTypeConstants.AudioEndPoint
                        mAudioMeterInformation = CType(control, MMDevice).AudioMeterInformation
                        If mAudioMeterInformation IsNot Nothing Then mChannels = mAudioMeterInformation.PeakValues.Count
                End Select
            End Sub

            ''' <summary>
            ''' This property is the same as <see cref="PeakLevel">PeakLevel</see> and is only provided for compatibility purposes with the <see cref="Binding">Binding</see> support.
            ''' </summary>
            Public ReadOnly Property UniformValue As Single
                Get
                    Return Me.PeakLevel
                End Get
            End Property

            ''' <summary>
            ''' Allows querying the peak value of the audio being streamed through a control
            ''' </summary>
            Public ReadOnly Property PeakLevel As Single
                Get
                    Select Case MyBase.ControlType
                        Case ControlTypeConstants.Part
                            Dim channels = mAudioPeakMeter.ChannelCount
                            Dim avg As Single = 0
                            For c As Integer = 0 To channels - 1
                                avg += mAudioPeakMeter.Level(c)
                            Next
                            Return avg / channels * 100
                        Case ControlTypeConstants.Session
                            Return mAudioMeterInformation.MasterPeakValue * 100
                        Case ControlTypeConstants.AudioEndPoint
                            Return mAudioMeterInformation.MasterPeakValue * 100
                    End Select
                End Get
            End Property

            ''' <summary>
            ''' Allows querying the peak value on a per channel basis. Only supported for Master (from CMixer)
            ''' </summary>
            Public ReadOnly Property PeakLevel(ByVal channel As Integer) As Single
                Get
                    If MyBase.ControlType = ControlTypeConstants.AudioEndPoint Then
                        Return mAudioMeterInformation.PeakValues(channel)
                    End If
                End Get
            End Property

            ''' <summary>
            ''' Returns the number of channels in the current CControlPeakMeter object. Only supported for Master (from CMixer)
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public ReadOnly Property Channels As Integer
                Get
                    Return mChannels
                End Get
            End Property
        End Class
    End Class
End Class
