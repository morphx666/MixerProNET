Imports CoreAudio
Imports System.Collections.Generic

Partial Public Class CCoreAudio
    Partial Public Class CControl
        ''' <summary>
        ''' This class allows for querying and manipulating controls that expose the ability to toggle the loudness equalization setting.
        ''' </summary>
        Public Class CControlLoudness
            Inherits CControlObject

            Private mAudioLoudness As AudioLoudness

            Private lastState As Boolean

            Protected Friend Sub New(ByVal parent As CControl, ByVal control As Object, ByVal controlType As ControlTypeConstants)
                MyBase.New(parent, control, controlType)

                Select Case MyBase.ControlType
                    Case ControlTypeConstants.Part
                        mAudioLoudness = CType(control, AudioLoudness)
                End Select
            End Sub

            ''' <summary>
            ''' This property is the same as <see cref="Loudness">Loudness</see> and is only provided for compatibility purposes with the <see cref="Binding">Binding</see> support.
            ''' </summary>
            Public Property UniformValue As Boolean
                Get
                    Return Me.Loudness
                End Get
                Set(ByVal value As Boolean)
                    If value <> Me.Loudness Then Me.Loudness = value
                End Set
            End Property

            ''' <summary>
            ''' Allows querying and manipulating the loudness equalization state of a control
            ''' </summary>
            Public Property Loudness As Boolean
                Get
                    Select Case MyBase.ControlType
                        Case ControlTypeConstants.Part
                            Return mAudioLoudness.Enabled
                    End Select
                End Get
                Set(ByVal value As Boolean)
                    Select Case MyBase.ControlType
                        Case ControlTypeConstants.Part
                            mAudioLoudness.Enabled = value
                    End Select
                End Set
            End Property
        End Class
    End Class
End Class
