''' <summary>
''' The MixerProNET engine provides a series of classes and collections that effectively provide an interface to all the sound cards available in the system.
''' </summary>
''' <remarks>
''' <p>Each sound card is interfaced through the <see cref="CMixer">CMixer</see> class. This class then provides a collection of lines through the <see cref="Lines">Lines</see> collection.</p>
''' <p>A line, interfaced through the <see cref="CLine">CLine</see> class, is the combination of controls that control a specific part of the sound card. For example, the "Microphone" line controls the volume and mute state of the Microphone line in the sound card. Each line provides a collection of controls through the <see cref="Controls">Controls</see> collection.</p>
''' <p>Each control, interfaced through the <see cref="CControl">CControl</see> class, provides access to a specific setting in a line. For example, the "Microphone" usually has two controls: a volume fader and a mute switch.</p>
''' <p>Some controls will provide a list of sub controls known as items so each CControl class also exposes a collection of <see cref="CtrlItems">CtrlItems</see>.</p>
''' <p>A control item, interfaced through the <see cref="CCtrlItem">CCtrlItem</see> class, provides access to a sub control in a control.</p>
''' <p>For example, most sound cards provide a line, known as the "Record Master" which includes a special control with a series of sub controls or items. These items are switches used to select the recording source.</p>
''' <p></p>
''' <p>In order to support the new Core Audio set of APIs, introduced in Windows Vista, MixerProNET now offers a new class called <see cref="CCoreAudio">CCoreAudio</see>.</p>
''' <p><b>Requirements</b></p>
''' <list type="bullet">
''' <item><description>Microsoft .NET 4.0</description></item>
''' <item><description>At least one available sound card</description></item>
''' <item><description>A version of Windows that supports the .NET 4.0 framework</description></item>
''' <item><description>In order to use the new <see cref="CCoreAudio">CCoreAudio</see> class, Windows Vista or later is required</description></item>
''' </list>
''' If you are using a registered version of MixerProNET, you should pass your registration information to the control when you instantiate it:
''' <code lang="vbnet">
'''     Dim mxp As CMixerPro = New CMixerPro("A1025CNF19475D33D1AH1E024719FA01", "72361119")
''' </code>
''' </remarks>
Public Class CMixerPro
    ' CMixerPro
    '   Mixers
    '       CMixer
    '           Lines
    '           LinesByComponentType
    '           LinesByConnection
    '           LinesByLineType
    '               CLine
    '                   Controls
    '                   ControlsByClass
    '                       CControl
    '                           Items
    '                               CItem
    Private mvarMixers As Mixers
    Private mvarHWND As Integer
    Private frmMsgHnd As MsgHnd
    Private mvarTag As String
    'Private licenseKey As String

    Private Const MM_MIXM_CONTROL_CHANGE As Short = &H3D1S
    Private Const MM_MIXM_LINE_CHANGE As Short = &H3D0S
    Private Const WM_TIMER As Short = &H113S

    ''' <summary>
    ''' This event is fired every time the <see cref="CControl.Value">value</see> of a <see cref="CControl">control</see> changes.
    ''' </summary>
    ''' <param name="mixerIndex">The index of the <see cref="CMixer">mixer</see> containing the <see cref="CControl">control</see> whose value has changed</param>
    ''' <param name="lineIndex">The index of the <see cref="CLine">line</see> containing the <see cref="CControl">control</see> whose value has changed</param>
    ''' <param name="controlIndex">The index of the <see cref="CControl">control</see> whose value has changed</param>
    ''' <remarks>
    ''' <p>This event will also be fired when a <see cref="CCtrlItem">control item</see> changes its <see cref="CCtrlItem.Value">value</see>but because Windows
    ''' does not provide any means to detect which control item changed, the event will only report the parent control and it is up to the host application to
    ''' try to identify which control item triggered the event.</p>
    ''' <p>For control items that are <see cref="CCtrlItem.Binding">bound</see>, MixerProNET will correctly update the bound controls but just because it will actually update all the bound control items for the control that triggered the event.</p>
    ''' </remarks>
    Public Event Changed(ByVal mixerIndex As Short, ByVal lineIndex As Short, ByVal controlIndex As Short)
    Public Event AudioDevicesChange()

    ''' <summary>
    ''' Return the version number for the MixerProNET library.
    ''' </summary>
    Public ReadOnly Property Version() As String
        Get
            Return System.Diagnostics.FileVersionInfo.GetVersionInfo(GetType(CMixerPro).Assembly.Location).FileVersion.ToString
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the object that contains data about the control
    ''' </summary>
    ''' <returns>A System.Object that contains data about the control. The default is null.</returns>
    Public Property Tag() As String
        Get
            Return mvarTag
        End Get
        Set(ByVal Value As String)
            mvarTag = Value
        End Set
    End Property

    ''' <summary>
    ''' This collection provides a series of <see cref="CMixer">CMixer</see> objects that interface each available sound card represented by the CMixerPRO base class
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Mixers() As Mixers
        Get
            If mvarMixers Is Nothing Then mvarMixers = New Mixers
            Mixers = mvarMixers
        End Get
        Set(ByVal Value As Mixers)
            mvarMixers = Value
        End Set
    End Property

    Private Function CheckForDeviceChanges() As Boolean
        Dim i As Short
        Dim mc As Short
        Dim mCap As MIXERCAPS

        mc = mixerGetNumDevs()
        If mc <> mvarMixers.Count Then
            Return True
        Else
            For i = 1 To mc
                mixerGetDevCaps(i - 1, mCap, Len(mCap))
                If mvarMixers(i).DeviceName <> StripNull(mCap.szPname) Or mvarMixers(i).DriverVersion <> StripNull(CStr(mCap.vDriverVersion)) Or mvarMixers(i).ManufacturerID <> CDbl(StripNull(CStr(mCap.wMid))) Or mvarMixers(i).ProductID <> CDbl(StripNull(CStr(mCap.wPid))) Then
                    Return True
                End If
            Next i
        End If

        Return False
    End Function

    ''' <summary>
    ''' Instantiates a new instance of a CMixerPro class.
    ''' </summary>
    ''' <param name="LicenseKey">The provided license key. If you have not purchased the control, leave this field empty.</param>
    ''' <param name="SerialNumber">The provided serial number. If you have not purchased the control, leave this field empty.</param>
    ''' <remarks>If this function is called with the incorrect parameters, the control may crash</remarks>
    Public Sub New()
        frmMsgHnd = New MsgHnd With {
            .mxp = Me
        }
        mvarHWND = frmMsgHnd.Handle.ToInt32

        Init()
    End Sub

    Private Sub Init()
        If mvarMixers IsNot Nothing Then mvarMixers.Dispose()
        mvarMixers = New Mixers()
        For i As Integer = 0 To mixerGetNumDevs() - 1
            mvarMixers.Add(CStr(i), i, mvarHWND)
        Next i

        KillTimer(mvarHWND, &H1000000)
        SetTimer(mvarHWND, &H1000000, 1000, 0)
    End Sub

    ''' <summary>
    ''' Releases all resources used by the System.ComponentModel.Component.
    ''' </summary>
    Public Sub Dispose()
        frmMsgHnd.Close()
        KillTimer(mvarHWND, &H1000000)
        mvarMixers.Dispose()
    End Sub

    Friend Sub wProc(ByRef msg As Message)
        Dim m As Short
        Dim l As Short
        Dim c As Short

        Select Case msg.Msg
            Case WM_TIMER
                If msg.WParam.ToInt32 = &H1000000 Then
                    If CheckForDeviceChanges() Then
                        Init()
                        RaiseEvent AudioDevicesChange()
                    End If
                Else
                    m = CShort(msg.WParam.ToInt32 And &HFF0000) / &H10000
                    l = CShort((msg.WParam.ToInt32 - m * &H10000) And &HFF00) / &H100
                    c = (msg.WParam.ToInt32 - m * &H10000 - l * &H100)
                    RaiseEvent Changed(m, l, c)
                End If
            Case MM_MIXM_LINE_CHANGE
                For m = 1 To mvarMixers.Count
                    If mvarMixers(m).mixerHandler = msg.WParam.ToInt32 Then
                        For l = 1 To mvarMixers(m).Lines.Count
                            If mvarMixers(m).Lines(l).ID = msg.LParam.ToInt32 Then
                                For c = 0 To mvarMixers(m).Lines(l).Controls.Count - 1
                                    RaiseEvent Changed(m, l, c)
                                Next c
                                Exit Sub
                            End If
                        Next l
                    End If
                Next m
            Case MM_MIXM_CONTROL_CHANGE
                For m = 1 To mvarMixers.Count
                    If mvarMixers(m).mixerHandler = msg.WParam.ToInt32 Then
                        For l = 1 To mvarMixers(m).Lines.Count
                            For c = 1 To mvarMixers(m).Lines(l).Controls.Count
                                If mvarMixers(m).Lines(l).Controls(c).ID = msg.LParam.ToInt32 Then
                                    RaiseEvent Changed(m, l, c)

                                    Dim ct As CControl = mvarMixers(m).Lines(l).Controls(c)
                                    If ct.CtrlItems.Count > 0 Then
                                        For Each cti As CCtrlItem In ct.CtrlItems
                                            cti.Binding.UpdateBoundObjValue()
                                        Next cti
                                    Else
                                        ct.Binding.UpdateBoundObjValue()
                                    End If
                                    Exit Sub
                                End If
                            Next c
                        Next l
                    End If
                Next m
        End Select
    End Sub
End Class