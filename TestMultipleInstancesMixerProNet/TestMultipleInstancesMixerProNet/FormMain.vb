Imports NMixerProNET

Public Class FormMain
    Private mxps As New List(Of CMixerPro)
    Private cas As New List(Of CCoreAudio)
    Private masterVolumes As New List(Of CCoreAudio.CControl.CControlVolume)

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    End Sub

    Private Sub ButtonCreate_Click(sender As Object, e As EventArgs) Handles ButtonCreate.Click
        For i As Integer = 1 To Integer.Parse(TextBoxCount.Text)
            Dim mxp As CMixerPro
            Dim ca As CCoreAudio
            Dim masterVolume As CCoreAudio.CControl.CControlVolume

            mxp = New CMixerPro("ED3625D12C81B65A5F350FCFAEDC49FE", "ST86703219")
            ca = New CCoreAudio(mxp)

            masterVolume = ca.Mixers.First().Volume

            Dim m As New VolumeFader()
            m.Orientation = Orientation.Vertical
            m.CoreAudioControl = masterVolume.Parent

            FlowLayoutPanelMixers.Controls.Add(m)

            mxps.Add(mxp)
            cas.Add(ca)
            masterVolumes.Add(masterVolume)
        Next
    End Sub

    Private Sub ButtonDestroy_Click(sender As Object, e As EventArgs) Handles ButtonDestroy.Click
        For i As Integer = 0 To mxps.Count() - 1
            FlowLayoutPanelMixers.Controls.RemoveAt(0)

            cas(i).Dispose()
            mxps(i).Dispose()
        Next

        mxps.Clear()
        cas.Clear()
    End Sub
End Class
