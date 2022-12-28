Imports CoreAudio
Imports System.Collections.Generic
Imports System.Threading

Partial Public Class CCoreAudio
    Partial Public Class CControl
        ''' <summary>
        ''' This is the base class from which the classes that allow querying and manipulating controls derive.
        ''' </summary>
        ''' <remarks></remarks>
        Public MustInherit Class CControlObject
            Private mControlType As ControlTypeConstants
            Private mTimer As Timer
            Private mName As String
            Private mParent As CControl

            ''' <summary>
            ''' Use to bind a <see cref="Windows.Forms.Control">Windows Forms Control</see> to a CControlObject to allow for automatic manipulation.
            ''' </summary>
            ''' <remarks></remarks>
            Public Binding As New CBinding(Me)

            ''' <summary>
            ''' Occurs when a control changes its value
            ''' </summary>
            ''' <param name="sender">The control that triggered the change</param>
            Public Event ControlChanged(ByVal sender As CControlObject)

            Protected Friend Sub New(ByVal parent As CControl, ByVal control As Object, ByVal controlType As ControlTypeConstants)
                mParent = parent
                mControlType = controlType

#If NET40 Then
                Select Case Me.GetType()
                    Case GetType(CControlMute) : mName = "Mute"
                    Case GetType(CControlVolume) : mName = "Volume"
                    Case GetType(CControlLoudness) : mName = "Loudness"
                    Case GetType(CControlPeakMeter) : mName = "PeakMeter"
                    Case Else : mName = "Undefined"
                End Select
#Else
                Dim t = Me.GetType()
                If t Is GetType(CControlMute) Then mName = "Mute"
                If t Is GetType(CControlVolume) Then mName = "Volume"
                If t Is GetType(CControlLoudness) Then mName = "Loudness"
                If t Is GetType(CControlPeakMeter) Then mName = "PeakMeter"
#End If
            End Sub

            Public ReadOnly Property Parent As CControl
                Get
                    Return mParent
                End Get
            End Property

            ''' <summary>
            ''' Returns a description of the control that this object represents.
            ''' </summary>
            ''' <remarks>At this moment, the names are only returned in English and are hardcoded into the library.</remarks>
            Public ReadOnly Property Name As String
                Get
                    Return mName
                End Get
            End Property

            Protected Friend Sub DoRaiseEvent()
                RaiseEvent ControlChanged(Me)
            End Sub

            Protected ReadOnly Property ControlType As ControlTypeConstants
                Get
                    Return mControlType
                End Get
            End Property
        End Class
    End Class
End Class
