Imports CoreAudio
Imports System.Collections.Generic

Partial Public Class CCoreAudio
    ''' <summary>
    ''' Provides access to a mixer (sound card)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CMixer
        Implements IDisposable

        Private mLines As New List(Of CLine)
        Private mSessions As New List(Of CSession)
        Private mMixer As MMDevice
        Private mDeviceName As String
        Private mDataFlow As DataFlow
        Private mIsEnabled As Boolean
        Private mName As String
        Private mMasterLine As CLine
        Private isFirst As Boolean = True
        Private processesChache As New List(Of Integer)

        ''' <summary>
        ''' This event is triggered when one (or more) <see cref="CSession">sessions</see> associated with this mixer change.
        ''' </summary>
        ''' <param name="sender">The mixer that triggered the event</param>
        ''' <param name="session">The session associated with this mixer that triggered the event</param>
        ''' <remarks></remarks>
        Public Event SessionChanged(ByVal sender As CMixer, ByVal session As CSession)

        'Public Event MasterMuteChanged(ByVal sender As CMixer)
        'Public Event MasterVolumeChanged(ByVal sender As CMixer)
        'Public Event MasterPeakMeterChanged(ByVal sender As CMixer)

        Protected Friend Sub New(mixer As MMDevice, flow As DataFlow)
            mMixer = mixer
            mDataFlow = flow
            mName = mMixer.DeviceFriendlyName

            If mMixer.Properties.Contains(PKey.DeviceDescription) Then
                mDeviceName = mMixer.Properties(PKey.DeviceDescription).Value.ToString()
            End If

            mMasterLine = New CLine(Me, mMixer)
            'If mMasterControl.ControlMute IsNot Nothing Then AddHandler mMasterControl.ControlMute.ControlChanged, Sub() RaiseEvent MasterMuteChanged(Me)
            'If mMasterControl.ControlVolume IsNot Nothing Then AddHandler mMasterControl.ControlVolume.ControlChanged, Sub() RaiseEvent MasterVolumeChanged(Me)
            'If mMasterControl.ControlPeakMeter IsNot Nothing Then AddHandler mMasterControl.ControlPeakMeter.ControlChanged, Sub() RaiseEvent MasterPeakMeterChanged(Me)

            Dim deviceTopology = mMixer.DeviceTopology
            Dim connectorEndpoint = deviceTopology.GetConnector(0)
            Dim connectorDevice = connectorEndpoint.ConnectedTo
            Dim part = connectorDevice.Part

            GetLines(part, flow)
            'GetSessions()
        End Sub

        Private Sub GetSessions()
            For Each s In mSessions
                s.Dispose()
            Next
            mSessions.Clear()

            Dim session As AudioSessionControl2
            Dim asm2 As AudioSessionManager2
            Try
                asm2 = mMixer.AudioSessionManager2
            Catch ex As Exception
                Exit Sub
            End Try

            asm2.RefreshSessions()

            For i As Integer = 0 To asm2.Sessions.Count - 1
                session = asm2.Sessions.Item(i)

                If session.State <> AudioSessionState.AudioSessionStateExpired Then
                    Dim caSession As CSession = New CSession(Me, session)

                    If caSession.Name <> "" Then
                        mSessions.Add(caSession)

                        If Not processesChache.Contains(caSession.ProcessId) Then
                            processesChache.Add(caSession.ProcessId)

                            AddHandler caSession.NameChanged, Sub(sender As CSession, newDisplayName As String) RaiseEvent SessionChanged(Me, sender)
                            AddHandler caSession.Disconnected, Sub(sender As CSession, disconnectReason As AudioSessionDisconnectReason) RaiseEvent SessionChanged(Me, sender)
                            AddHandler caSession.StateChanged, Sub(sender As CSession, newState As AudioSessionState) RaiseEvent SessionChanged(Me, sender)
                        End If
                    End If
                End If
            Next

            Dim pcsToRemove As New List(Of Integer)
            For Each pc In processesChache
                Dim exists As Boolean = False

                For i As Integer = 0 To asm2.Sessions.Count - 1
                    If pc = asm2.Sessions.Item(i).ProcessID Then
                        exists = True
                        Exit For
                    End If
                Next

                If Not exists Then pcsToRemove.Add(pc)
            Next
            For Each pctr In pcsToRemove
                processesChache.Remove(pctr)
            Next

            If isFirst Then
                isFirst = False

                AddHandler asm2.OnSessionCreated, Sub(sender As Object, newSession As Interfaces.IAudioSessionControl2)
                                                      GetSessions()
                                                      RaiseEvent SessionChanged(Me, Nothing)
                                                  End Sub
            End If
        End Sub

        Private lastControl As CControl
        Private Sub GetLines(part As Part, flow As DataFlow)
            If Part.PartType = PartType.Connector Then
                Dim newLine As CLine = New CLine(Me, part, flow, lastControl)
                If newLine.Controls.Count > 0 Then
                    mLines.Add(newLine)
                    If lastControl Is Nothing AndAlso newLine.Controls.Count > 0 Then
                        lastControl = newLine.Controls(newLine.Controls.Count - 1)
                    End If
                End If
            End If

            Dim parts = part.EnumPartsIncoming
            If parts IsNot Nothing Then
                For i As Integer = 0 To parts.Count - 1
                    Dim iPart = parts.Part(i)
                    GetLines(iPart, flow)
                Next
            End If
        End Sub

        ''' <summary>
        ''' Use to query the status of a mixer
        ''' </summary>
        ''' <returns>Returns True if the mixer is Active (or enabled) or False if the mixer is disabled.</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Enabled As Boolean
            Get
                Return (mMixer.State And DeviceState.Active) = DeviceState.Active
            End Get
        End Property

        ''' <summary>
        ''' Returns the name of the mixer as reported by the driver.
        ''' </summary>
        Public ReadOnly Property Name As String
            Get
                Return mName
            End Get
        End Property

        ''' <summary>
        ''' Returns the name of the device as reported by the driver.
        ''' </summary>
        Public ReadOnly Property DeviceName As String
            Get
                Return mDeviceName
            End Get
        End Property

        ''' <summary>
        ''' Indicates the direction in which the data (or audio) flows within the mixer.
        ''' </summary>
        ''' <returns>Returns one of the values available in the <see cref="CMixer.DataFlow">EDataFlow</see> enumeration.</returns>
        ''' <remarks>
        ''' <p>Mixers returning DataFlow.Render indicate that are used for playback.</p>
        ''' <p>Mixers returning DataFlow.Capture indicate that are used for recording or capturing audio.</p>
        ''' </remarks>
        Public ReadOnly Property DataFlow As DataFlow
            Get
                Return mDataFlow
            End Get
        End Property

        ''' <summary>
        ''' Returns the list of <see cref="CLine">CLine</see> objects (lines) exposed by the current mixer.
        ''' </summary>
        Public ReadOnly Property Lines As List(Of CLine)
            Get
                Return mLines
            End Get
        End Property

        ''' <summary>
        ''' Returns a list of <see cref="CSession">CSession</see> objects (sessions) associated with the current mixer.
        ''' </summary>
        ''' <remarks>
        ''' Note that this list is constantly changing as applications open and close audio streams in the system.
        ''' </remarks>
        Public ReadOnly Property Sessions As List(Of CSession)
            Get
                If mSessions Is Nothing OrElse mSessions.Count = 0 Then GetSessions()
                'GetSessions()
                Return mSessions
            End Get
        End Property

        ''' <summary>
        ''' Provides access to the Master Volume exposed by this mixer object.
        ''' </summary>
        Public ReadOnly Property Volume As CControl.CControlVolume
            Get
                If mMasterLine.Controls.Count > 0 AndAlso mMasterLine.Controls(0) IsNot Nothing Then
                    Return mMasterLine.Controls(0).ControlVolume
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Provides access to the Master Mute exposed by this mixer object.
        ''' </summary>
        Public ReadOnly Property Mute As CControl.CControlMute
            Get
                If mMasterLine.Controls.Count > 0 AndAlso mMasterLine.Controls(0) IsNot Nothing Then
                    Return mMasterLine.Controls(0).ControlMute
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Provides access to the Master Peak Meter exposed by this mixer object.
        ''' </summary>
        Public ReadOnly Property PeakMeter As CControl.CControlPeakMeter
            Get
                If mMasterLine.Controls.Count > 0 AndAlso mMasterLine.Controls(0) IsNot Nothing Then
                    Return mMasterLine.Controls(0).ControlPeakMeter
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Represents a line where this mixer contains all its master controls.
        ''' </summary>
        ''' <remarks>
        ''' <p>In reality, mixers do not not expose any lines, but one is provided for consistency purposes so instead of exposing all the controls without a line, CMixer encapsulates its controls inside a virtual line.</p>
        ''' <p>Of course, if you wish, you can access all the session's <see cref="CControl.CControlObject">CControlObject</see> objects directly through the <see cref="Mute">MasterMute</see>, <see cref="Volume">MasterVolume</see> and <see cref="PeakMeter">MasterPeakMeter</see> properties.</p>
        ''' </remarks>
        Public ReadOnly Property Line As CLine
            Get
                Return mMasterLine
            End Get
        End Property

        ''' <summary>
        ''' Use to query wether the device is the default audio device, systemwide.
        ''' Setting it to true, will set the device as the default audio device.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Public Property Selected As Boolean
            Get
                Return mMixer.Selected
            End Get
            Set(value As Boolean)
                If value = True Then mMixer.Selected = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0} ({1})", mName, mDeviceName)
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
                For Each l As CLine In mLines
                    l.Dispose()
                Next
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
