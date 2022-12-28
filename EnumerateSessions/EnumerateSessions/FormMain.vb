Imports NMixerProNET
Imports NMixerProNET.CCoreAudio
Imports System.Threading

Public Class FormMain
    Private mxp As CMixerPro
    Private ca As CCoreAudio
    Private selectedSession As CSession

    Private autoRefresh As Timer

    Private Sub FormMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        autoRefresh.Dispose()

        If ca IsNot Nothing Then ca.Dispose()
        If mxp IsNot Nothing Then mxp.Dispose()
    End Sub

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If CCoreAudio.RequiresCoreAudio Then
            mxp = New CMixerPro()
            ca = New CCoreAudio(mxp)

            autoRefresh = New Timer(New TimerCallback(Sub() Me.Invoke(New MethodInvoker(AddressOf EnumSessions))),
                                                        Nothing, 500, 3000)
        Else
            LvSessions.Enabled = False
            VfSessionVolume.Enabled = False
            CbSessionMute.Enabled = False
            MsgBox("Sessions are only supported under Windows Vista, 7 and 8")
        End If
    End Sub

    Private Sub EnumSessions()
        ' Get Active Sessions
        Dim sessions As New List(Of CSession)
        For Each mixer In ca.Mixers
            If mixer.Selected And mixer.DataFlow = CoreAudio.DataFlow.Render Then
                For Each session In mixer.Sessions
                    sessions.Add(session)
                Next
            End If
        Next

        ' Remove listview items for which their sessions have been destroyed
        Dim itemsToRemove As New List(Of ListViewItem)
        For Each item As ListViewItem In LvSessions.Items

            Dim exists As Boolean = False
            For Each session In sessions
                If session.ProcessId = CType(item.Tag, CSession).ProcessId Then
                    If item.Text <> session.Name Then item.Text = session.Name
                    exists = True
                    Exit For
                End If
            Next

            If Not exists Then itemsToRemove.Add(item)
        Next

        For Each item As ListViewItem In itemsToRemove
            LvSessions.Items.Remove(item)
        Next

        ' Add new sessions
        For Each session In sessions

            Dim exists As Boolean = False
            For Each item As ListViewItem In LvSessions.Items
                If session.ProcessId = CType(item.Tag, CSession).ProcessId Then
                    exists = True
                    Exit For
                End If
            Next

            If Not exists Then
                Dim newItem As ListViewItem = LvSessions.Items.Add(session.Name)
                newItem.Tag = session

                If selectedSession IsNot Nothing AndAlso (selectedSession.ProcessId = session.ProcessId) Then
                    newItem.Selected = True
                    newItem.EnsureVisible()
                End If
            End If
        Next

        ' *** Lazy version
        'LvSessions.Items.Clear()

        'For Each mixer In ca.Mixers
        '    ' Enumerate the sessions from the default playback mixer only
        '    If mixer.Selected And mixer.DataFlow = CoreAudio.DataFlow.Render Then
        '        For Each session In mixer.Sessions
        '            Dim newItem As ListViewItem = LvSessions.Items.Add(session.Name)
        '            newItem.Tag = session

        '            If selectedSession IsNot Nothing AndAlso (selectedSession.ProcessId = session.ProcessId) Then
        '                newItem.Selected = True
        '                newItem.EnsureVisible()
        '            End If
        '        Next
        '    End If
        'Next

        LvSessions.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
        'UpdateControls()
    End Sub

    Private Sub LvSessions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LvSessions.SelectedIndexChanged
        UpdateControls()
    End Sub

    Private Sub UpdateControls()
        If LvSessions.SelectedItems.Count = 1 Then
            selectedSession = CType(LvSessions.SelectedItems(0).Tag, CSession)

            VfSessionVolume.CoreAudioControl = selectedSession
            'VfSessionVolume.Minimum = 0
            'VfSessionVolume.Maximum = 100
            'VfSessionVolume.TickFrequency = 1
            'VfSessionVolume.Value = selectedSession.ControlVolume.UniformValue
            selectedSession.ControlMute.Binding.Define(CbSessionMute, "Checked", "CheckStateChanged")

            VfSessionVolume.Enabled = True
            CbSessionMute.Enabled = True
        Else
            If selectedSession IsNot Nothing Then
                selectedSession.ControlMute.Binding.Remove()
                selectedSession = Nothing

                VfSessionVolume.CoreAudioControl = Nothing
            End If

            VfSessionVolume.Enabled = False
            CbSessionMute.Enabled = False
        End If
    End Sub

    Private Sub VfSessionVolume_Scroll(sender As Object, e As EventArgs)
        If selectedSession IsNot Nothing Then
            selectedSession.ControlVolume.UniformValue = VfSessionVolume.Value
        End If
    End Sub
End Class
