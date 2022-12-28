Imports CoreAudio
Imports System.Collections.Generic

Partial Public Class CCoreAudio
    ''' <summary>
    ''' A session is an audio stream created by an application which can be controlled independently of the volume and mute states of the different <see cref="CLine">lines</see> provided by the sound card as well as from other sessions. 
    ''' </summary>
    ''' <remarks>
    ''' Sessions provide a limited functionality over what can be manipulated and, so far, it is only possible to control their volume, their mute state and query their peak meter.
    ''' </remarks>
    Public Class CSession
        Implements IDisposable

        Private mParent As CMixer
        Private mSession As AudioSessionControl2
        Private mLine As CLine

        ''' <summary>
        ''' Occurs when the name of the session has changed.
        ''' </summary>
        ''' <param name="sender">The session that triggered the event</param>
        ''' <param name="newName">The new name for the session</param>
        Public Event NameChanged(sender As CSession, newName As String)

        ''' <summary>
        ''' Occurs when the session closes the audio stream and disconnects from the current mixer.
        ''' </summary>
        ''' <param name="sender">The session that triggered the event</param>
        ''' <param name="reason">One of the possible values from the <see cref="AudioSessionDisconnectReason">AudioSessionDisconnectReason</see> enumeration.</param>
        Public Event Disconnected(sender As CSession, reason As AudioSessionDisconnectReason)

        ''' <summary>
        ''' Occurs when the state of the session changes.
        ''' </summary>
        ''' <param name="sender">The session that triggered the event</param>
        ''' <param name="newState">One of the possible values from the <see cref="AudioSessionState">AudioSessionState</see> enumeration.</param>
        Public Event StateChanged(sender As CSession, newState As AudioSessionState)

        Public Sub New(parent As CMixer, session As AudioSessionControl2)
            mParent = parent
            mSession = session
            mLine = New CLine(parent, session)

            AddHandler mSession.OnDisplayNameChanged, Sub(sender As Object, newDisplayName As String) RaiseEvent NameChanged(Me, newDisplayName)
            AddHandler mSession.OnSessionDisconnected, Sub(sender As Object, disconnectReason As AudioSessionDisconnectReason) RaiseEvent Disconnected(Me, disconnectReason)
            AddHandler mSession.OnStateChanged, Sub(sender As Object, newState As AudioSessionState) RaiseEvent StateChanged(Me, newState)
        End Sub

        ''' <summary>
        ''' Returns the name of the current session.
        ''' </summary>
        ''' <remarks>
        ''' <p>Under some circumstances, the name property will return an empty string.</p>
        ''' <p>This is not a bug in MixerProNET, instead, sessions need to be named by the application that creates them and if the developer of that application
        ''' "forgot" or simply neglected to set the name, the session will remain nameless.</p>
        ''' <p>In the case where the name is missing, MixerProNET will try to obtain an alternate name representation for the session, such as for example, it will try to obtain the name of the process.
        ''' Unfortunately, this will not always work so if the return value is empty, your application should handle this situation on its own.</p>
        ''' </remarks>
        Public ReadOnly Property Name As String
            Get
                Return CSession.GetSessionName(mSession, "")
            End Get
        End Property

        Public ReadOnly Property ProcessId As Integer
            Get
                Return mSession.ProcessID()
            End Get
        End Property

        'Protected Friend Shared Function GetSessionName(session As AudioSessionControl2, errorChar As String) As String
        '    Dim procName As String = ""
        '    Dim p As Process
        '    Dim ps() As Process

        '    Try
        '        Dim sID As UInteger = session.GetProcessID()
        '        ps = Process.GetProcesses()
        '        For i As Integer = 0 To ps.Length - 1
        '            If ps(i).Id = sID Then
        '                p = ps(i)
        '                Exit For
        '            End If
        '        Next

        '        If p Is Nothing Then
        '            'RaiseEvent Disconnected(Me, AudioSessionDisconnectReason.DisconnectReasonDeviceRemoval)
        '        Else
        '            If session.IsSystemSoundsSession Then
        '                procName = "System Sounds"
        '            Else
        '                If session.DisplayName = "" Then
        '                    procName = p.MainWindowTitle
        '                Else
        '                    procName = session.DisplayName
        '                End If
        '                If procName = "" Then procName = p.ProcessName
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'RaiseEvent Disconnected(Me, AudioSessionDisconnectReason.DisconnectReasonDeviceRemoval)
        '        procName = errorChar
        '    End Try

        '    For i As Integer = 0 To ps.Length - 1
        '        ps(i).Dispose()
        '    Next

        '    Return procName
        'End Function

        Protected Friend Shared Function GetSessionName(session As AudioSessionControl2, errorChar As String) As String
            Dim procName As String = ""
            Dim p As Process
            'Dim ps() As Process

            Try
                p = Process.GetProcessById(session.ProcessID)
                If p Is Nothing Then
                    'RaiseEvent Disconnected(Me, AudioSessionDisconnectReason.DisconnectReasonDeviceRemoval)
                Else
                    If session.IsSystemSoundsSession Then
                        procName = "System Sounds"
                    Else
                        If session.DisplayName = "" Then
                            procName = p.MainWindowTitle
                        Else
                            procName = session.DisplayName
                        End If
                        If procName = "" Then procName = p.ProcessName
                    End If
                End If
            Catch ex As Exception
                'RaiseEvent Disconnected(Me, AudioSessionDisconnectReason.DisconnectReasonDeviceRemoval)
                Debug.WriteLine("Unknown Process with ID = " + session.ProcessID.ToString())
                procName = errorChar
            End Try

            If p IsNot Nothing Then p.Dispose()

            Return procName
        End Function

        ''' <summary>
        ''' Provides access to a <see cref="CControl.CControlMute">CControlMute</see> object to allow querying and manipulating the mute state of the session.
        ''' </summary>
        Public ReadOnly Property ControlMute As CControl.CControlMute
            Get
                Return mLine.Controls(0).ControlMute
            End Get
        End Property

        ''' <summary>
        ''' Provides access to a <see cref="CControl.CControlVolume">CControlVolume</see> object to allow querying and manipulating the volume level of the session.
        ''' </summary>
        Public ReadOnly Property ControlVolume As CControl.CControlVolume
            Get
                Return mLine.Controls(0).ControlVolume
            End Get
        End Property

        ''' <summary>
        ''' Provides access to a <see cref="CControl.CControlPeakMeter">CControlPeakMeter</see> object to allow querying session's audio levels.
        ''' </summary>
        Public ReadOnly Property ControlPeakMeter As CControl.CControlPeakMeter
            Get
                Return mLine.Controls(0).ControlPeakMeter
            End Get
        End Property

        ''' <summary>
        ''' Represents a line where this session contains all its controls.
        ''' </summary>
        ''' <remarks>
        ''' <p>In reality, sessions do not not expose any lines, but one is provided for consistency purposes so instead of exposing all the controls without a line, CSession encapsulates its controls inside a virtual line.</p>
        ''' <p>Of course, if you wish, you can access all the session's <see cref="CControl.CControlObject">CControlObject</see> objects directly through the <see cref="ControlMute">ControlMute</see>, <see cref="ControlVolume">ControlVolume</see> and <see cref="ControlPeakMeter">ControlPeakMeter</see> properties.</p>
        ''' </remarks>
        Public ReadOnly Property Line As CLine
            Get
                Return mLine
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Name
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If mLine IsNot Nothing Then mLine.Dispose()
                    ' If mSession IsNot Nothing Then mSession.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
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
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Class
