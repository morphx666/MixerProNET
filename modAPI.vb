Imports System.Runtime.InteropServices
Imports System.ComponentModel

<Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
Friend Module modAPI
    Public Const MAXPNAMELEN As Integer = 32
    Public Const MIXER_SHORT_NAME_CHARS As Integer = 16
    Public Const MIXER_LONG_NAME_CHARS As Integer = 64

    Public Const MIXER_OBJECTF_AUX As Integer = &H50000000
    Public Const MIXER_OBJECTF_HANDLE As Integer = &H80000000
    Public Const MIXER_OBJECTF_MIXER As Integer = &H0
    Public Const MIXER_OBJECTF_WAVEIN As Integer = &H20000000
    Public Const MIXER_OBJECTF_WAVEOUT As Integer = &H10000000
    Public Const MIXER_OBJECTF_HMIXER As Integer = (MIXER_OBJECTF_HANDLE Or MIXER_OBJECTF_MIXER)
    Public Const MIXER_OBJECTF_HWAVEIN As Integer = (MIXER_OBJECTF_HANDLE Or MIXER_OBJECTF_WAVEIN)
    Public Const MIXER_OBJECTF_HWAVEOUT As Integer = (MIXER_OBJECTF_HANDLE Or MIXER_OBJECTF_WAVEOUT)

    Public Const MIXERCONTROL_CT_CLASS_CUSTOM As Integer = &H0
    Public Const MIXERCONTROL_CT_CLASS_FADER As Integer = &H50000000
    Public Const MIXERCONTROL_CT_CLASS_LIST As Integer = &H70000000
    Public Const MIXERCONTROL_CT_CLASS_MASK As Integer = &HF0000000
    Public Const MIXERCONTROL_CT_CLASS_METER As Integer = &H10000000
    Public Const MIXERCONTROL_CT_CLASS_NUMBER As Integer = &H30000000
    Public Const MIXERCONTROL_CT_CLASS_SLIDER As Integer = &H40000000
    Public Const MIXERCONTROL_CT_CLASS_SWITCH As Integer = &H20000000
    Public Const MIXERCONTROL_CT_CLASS_TIME As Integer = &H60000000
    Public Const MIXERCONTROL_CT_UNITS_BOOLEAN As Integer = &H10000
    Public Const MIXERCONTROL_CT_UNITS_CUSTOM As Integer = &H0
    Public Const MIXERCONTROL_CT_UNITS_DECIBELS As Integer = &H40000
    Public Const MIXERCONTROL_CT_UNITS_MASK As Integer = &HFF0000
    Public Const MIXERCONTROL_CT_UNITS_PERCENT As Integer = &H50000
    Public Const MIXERCONTROL_CT_UNITS_SIGNED As Integer = &H20000
    Public Const MIXERCONTROL_CT_UNITS_UNSIGNED As Integer = &H30000
    Public Const MIXERCONTROL_CT_SC_LIST_MULTIPLE As Integer = &H1000000
    Public Const MIXERCONTROL_CT_SC_LIST_SINGLE As Integer = &H0
    Public Const MIXERCONTROL_CT_SC_METER_POLLED As Integer = &H0
    Public Const MIXERCONTROL_CT_SC_SWITCH_BOOLEAN As Integer = &H0
    Public Const MIXERCONTROL_CT_SC_SWITCH_BUTTON As Integer = &H1000000
    Public Const MIXERCONTROL_CT_SC_TIME_MICROSECS As Integer = &H0
    Public Const MIXERCONTROL_CT_SC_TIME_MILLISECS As Integer = &H1000000

    Public Const MIXERCONTROL_CONTROLF_DISABLED As Integer = &H80000000
    Public Const MIXERCONTROL_CONTROLF_MULTIPLE As Integer = &H2
    Public Const MIXERCONTROL_CONTROLF_UNIFORM As Integer = &H1

    Public Const MIXERCONTROL_CONTROLTYPE_FADER As Integer = (MIXERCONTROL_CT_CLASS_FADER Or MIXERCONTROL_CT_UNITS_UNSIGNED)
    Public Const MIXERCONTROL_CONTROLTYPE_BASS As Integer = MIXERCONTROL_CONTROLTYPE_FADER + 2
    Public Const MIXERCONTROL_CONTROLTYPE_TREBLE As Integer = MIXERCONTROL_CONTROLTYPE_FADER + 3
    Public Const MIXERCONTROL_CONTROLTYPE_BOOLEAN As Integer = (MIXERCONTROL_CT_CLASS_SWITCH Or MIXERCONTROL_CT_SC_SWITCH_BOOLEAN Or MIXERCONTROL_CT_UNITS_BOOLEAN)
    Public Const MIXERCONTROL_CONTROLTYPE_BOOLEANMETER As Integer = (MIXERCONTROL_CT_CLASS_METER Or MIXERCONTROL_CT_SC_METER_POLLED Or MIXERCONTROL_CT_UNITS_BOOLEAN)
    Public Const MIXERCONTROL_CONTROLTYPE_BUTTON As Integer = (MIXERCONTROL_CT_CLASS_SWITCH Or MIXERCONTROL_CT_SC_SWITCH_BUTTON Or MIXERCONTROL_CT_UNITS_BOOLEAN)
    Public Const MIXERCONTROL_CONTROLTYPE_CUSTOM As Integer = (MIXERCONTROL_CT_CLASS_CUSTOM Or MIXERCONTROL_CT_UNITS_CUSTOM)
    Public Const MIXERCONTROL_CONTROLTYPE_DECIBELS As Integer = (MIXERCONTROL_CT_CLASS_NUMBER Or MIXERCONTROL_CT_UNITS_DECIBELS)
    Public Const MIXERCONTROL_CONTROLTYPE_EQUALIZER As Integer = MIXERCONTROL_CONTROLTYPE_FADER + 4
    Public Const MIXERCONTROL_CONTROLTYPE_LOUDNESS As Integer = MIXERCONTROL_CONTROLTYPE_BOOLEAN + 4
    Public Const MIXERCONTROL_CONTROLTYPE_MICROTIME As Integer = (MIXERCONTROL_CT_CLASS_TIME Or MIXERCONTROL_CT_SC_TIME_MICROSECS Or MIXERCONTROL_CT_UNITS_UNSIGNED)
    Public Const MIXERCONTROL_CONTROLTYPE_MILLITIME As Integer = (MIXERCONTROL_CT_CLASS_TIME Or MIXERCONTROL_CT_SC_TIME_MILLISECS Or MIXERCONTROL_CT_UNITS_UNSIGNED)
    Public Const MIXERCONTROL_CONTROLTYPE_MULTIPLESELECT As Integer = (MIXERCONTROL_CT_CLASS_LIST Or MIXERCONTROL_CT_SC_LIST_MULTIPLE Or MIXERCONTROL_CT_UNITS_BOOLEAN)
    Public Const MIXERCONTROL_CONTROLTYPE_MIXER As Integer = MIXERCONTROL_CONTROLTYPE_MULTIPLESELECT + 1
    Public Const MIXERCONTROL_CONTROLTYPE_MONO As Integer = MIXERCONTROL_CONTROLTYPE_BOOLEAN + 3
    Public Const MIXERCONTROL_CONTROLTYPE_MUTE As Integer = MIXERCONTROL_CONTROLTYPE_BOOLEAN + 2
    Public Const MIXERCONTROL_CONTROLTYPE_SINGLESELECT As Integer = (MIXERCONTROL_CT_CLASS_LIST Or MIXERCONTROL_CT_SC_LIST_SINGLE Or MIXERCONTROL_CT_UNITS_BOOLEAN)
    Public Const MIXERCONTROL_CONTROLTYPE_MUX As Integer = MIXERCONTROL_CONTROLTYPE_SINGLESELECT + 1
    Public Const MIXERCONTROL_CONTROLTYPE_ONOFF As Integer = MIXERCONTROL_CONTROLTYPE_BOOLEAN + 1
    Public Const MIXERCONTROL_CONTROLTYPE_SLIDER As Integer = (MIXERCONTROL_CT_CLASS_SLIDER Or MIXERCONTROL_CT_UNITS_SIGNED)
    Public Const MIXERCONTROL_CONTROLTYPE_PAN As Integer = MIXERCONTROL_CONTROLTYPE_SLIDER + 1
    Public Const MIXERCONTROL_CONTROLTYPE_SIGNEDMETER As Integer = (MIXERCONTROL_CT_CLASS_METER Or MIXERCONTROL_CT_SC_METER_POLLED Or MIXERCONTROL_CT_UNITS_SIGNED)
    Public Const MIXERCONTROL_CONTROLTYPE_PEAKMETER As Integer = MIXERCONTROL_CONTROLTYPE_SIGNEDMETER + 1
    Public Const MIXERCONTROL_CONTROLTYPE_PERCENT As Integer = (MIXERCONTROL_CT_CLASS_NUMBER Or MIXERCONTROL_CT_UNITS_PERCENT)
    Public Const MIXERCONTROL_CONTROLTYPE_QSOUNDPAN As Integer = MIXERCONTROL_CONTROLTYPE_SLIDER + 2
    Public Const MIXERCONTROL_CONTROLTYPE_SIGNED As Integer = (MIXERCONTROL_CT_CLASS_NUMBER Or MIXERCONTROL_CT_UNITS_SIGNED)
    Public Const MIXERCONTROL_CONTROLTYPE_STEREOENH As Integer = MIXERCONTROL_CONTROLTYPE_BOOLEAN + 5
    Public Const MIXERCONTROL_CONTROLTYPE_UNSIGNED As Integer = (MIXERCONTROL_CT_CLASS_NUMBER Or MIXERCONTROL_CT_UNITS_UNSIGNED)
    Public Const MIXERCONTROL_CONTROLTYPE_UNSIGNEDMETER As Integer = (MIXERCONTROL_CT_CLASS_METER Or MIXERCONTROL_CT_SC_METER_POLLED Or MIXERCONTROL_CT_UNITS_UNSIGNED)
    Public Const MIXERCONTROL_CONTROLTYPE_VOLUME As Integer = MIXERCONTROL_CONTROLTYPE_FADER + 1

    Public Const MIXERLINE_TARGETTYPE_AUX As Integer = 5
    Public Const MIXERLINE_TARGETTYPE_MIDIIN As Integer = 4
    Public Const MIXERLINE_TARGETTYPE_MIDIOUT As Integer = 3
    Public Const MIXERLINE_TARGETTYPE_UNDEFINED As Integer = 0
    Public Const MIXERLINE_TARGETTYPE_WAVEIN As Integer = 2
    Public Const MIXERLINE_TARGETTYPE_WAVEOUT As Integer = 1

    Public Const MIXERLINE_COMPONENTTYPE_DST_FIRST As Integer = &H0
    Public Const MIXERLINE_COMPONENTTYPE_DST_DIGITAL As Integer = (MIXERLINE_COMPONENTTYPE_DST_FIRST + 1)
    Public Const MIXERLINE_COMPONENTTYPE_DST_HEADPHONES As Integer = (MIXERLINE_COMPONENTTYPE_DST_FIRST + 5)
    Public Const MIXERLINE_COMPONENTTYPE_DST_LAST As Integer = (MIXERLINE_COMPONENTTYPE_DST_FIRST + 8)
    Public Const MIXERLINE_COMPONENTTYPE_DST_LINE As Integer = (MIXERLINE_COMPONENTTYPE_DST_FIRST + 2)
    Public Const MIXERLINE_COMPONENTTYPE_DST_MONITOR As Integer = (MIXERLINE_COMPONENTTYPE_DST_FIRST + 3)
    Public Const MIXERLINE_COMPONENTTYPE_DST_SPEAKERS As Integer = (MIXERLINE_COMPONENTTYPE_DST_FIRST + 4)
    Public Const MIXERLINE_COMPONENTTYPE_DST_TELEPHONE As Integer = (MIXERLINE_COMPONENTTYPE_DST_FIRST + 6)
    Public Const MIXERLINE_COMPONENTTYPE_DST_UNDEFINED As Integer = (MIXERLINE_COMPONENTTYPE_DST_FIRST + 0)
    Public Const MIXERLINE_COMPONENTTYPE_DST_VOICEIN As Integer = (MIXERLINE_COMPONENTTYPE_DST_FIRST + 8)
    Public Const MIXERLINE_COMPONENTTYPE_DST_WAVEIN As Integer = (MIXERLINE_COMPONENTTYPE_DST_FIRST + 7)

    Public Const MIXERLINE_COMPONENTTYPE_SRC_FIRST As Integer = &H1000
    Public Const MIXERLINE_COMPONENTTYPE_SRC_DIGITAL As Integer = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 1)
    Public Const MIXERLINE_COMPONENTTYPE_SRC_LINE As Integer = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 2)
    Public Const MIXERLINE_COMPONENTTYPE_SRC_MICROPHONE As Integer = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 3)
    Public Const MIXERLINE_COMPONENTTYPE_SRC_SYNTHESIZER As Integer = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 4)
    Public Const MIXERLINE_COMPONENTTYPE_SRC_COMPACTDISC As Integer = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 5)
    Public Const MIXERLINE_COMPONENTTYPE_SRC_TELEPHONE As Integer = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 6)
    Public Const MIXERLINE_COMPONENTTYPE_SRC_PCSPEAKER As Integer = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 7)
    Public Const MIXERLINE_COMPONENTTYPE_SRC_WAVEOUT As Integer = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 8)
    Public Const MIXERLINE_COMPONENTTYPE_SRC_AUXILIARY As Integer = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 9)
    Public Const MIXERLINE_COMPONENTTYPE_SRC_ANALOG As Integer = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 10)
    Public Const MIXERLINE_COMPONENTTYPE_SRC_LAST As Integer = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 10)
    Public Const MIXERLINE_COMPONENTTYPE_SRC_UNDEFINED As Integer = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 0)

    Public Const MIXER_GETCONTROLDETAILSF_VALUE As Integer = &H0
    Public Const MIXER_GETCONTROLDETAILSF_LISTTEXT As Integer = &H1
    Public Const MIXER_GETCONTROLDETAILSF_QUERYMASK As Integer = &HF

    Public Const MIXER_SETCONTROLDETAILSF_VALUE As Integer = &H0
    Public Const MIXER_SETCONTROLDETAILSF_CUSTOM As Integer = &H1
    Public Const MIXER_SETCONTROLDETAILSF_QUERYMASK As Integer = &HF

    Public Const MIXER_GETLINECONTROLSF_ALL As Integer = &H0
    Public Const MIXER_GETLINECONTROLSF_ONEBYID As Integer = &H1
    Public Const MIXER_GETLINECONTROLSF_ONEBYTYPE As Integer = &H2
    Public Const MIXER_GETLINECONTROLSF_QUERYMASK As Integer = &HF

    Public Const MIXER_GETLINEINFOF_COMPONENTTYPE As Integer = &H3
    Public Const MIXER_GETLINEINFOF_DESTINATION As Integer = &H0
    Public Const MIXER_GETLINEINFOF_LINEID As Integer = &H2
    Public Const MIXER_GETLINEINFOF_QUERYMASK As Integer = &HF
    Public Const MIXER_GETLINEINFOF_SOURCE As Integer = &H1
    Public Const MIXER_GETLINEINFOF_TARGETTYPE As Integer = &H4

    Public Const MIXERR_BASE As Integer = 1024
    Public Const MIXERR_INVALCONTROL As Integer = (MIXERR_BASE + 1)
    Public Const MIXERR_INVALLINE As Integer = (MIXERR_BASE + 0)
    Public Const MIXERR_INVALVALUE As Integer = (MIXERR_BASE + 2)
    Public Const MIXERR_LASTERROR As Integer = (MIXERR_BASE + 2)

    Public Const MMSYSERR_BASE As Integer = 0
    Public Const MMSYSERR_ALLOCATED As Integer = (MMSYSERR_BASE + 4)
    Public Const MMSYSERR_BADDEVICEID As Integer = (MMSYSERR_BASE + 2)
    Public Const MMSYSERR_BADERRNUM As Integer = (MMSYSERR_BASE + 9)
    Public Const MMSYSERR_ERROR As Integer = (MMSYSERR_BASE + 1)
    Public Const MMSYSERR_HANDLEBUSY As Integer = (MMSYSERR_BASE + 12)
    Public Const MMSYSERR_INVALFLAG As Integer = (MMSYSERR_BASE + 10)
    Public Const MMSYSERR_INVALHANDLE As Integer = (MMSYSERR_BASE + 5)
    Public Const MMSYSERR_INVALIDALIAS As Integer = (MMSYSERR_BASE + 13)
    Public Const MMSYSERR_INVALPARAM As Integer = (MMSYSERR_BASE + 11)
    Public Const MMSYSERR_LASTERROR As Integer = (MMSYSERR_BASE + 13)
    Public Const MMSYSERR_NODRIVER As Integer = (MMSYSERR_BASE + 6)
    Public Const MMSYSERR_NOERROR As Integer = 0
    Public Const MMSYSERR_NOMEM As Integer = (MMSYSERR_BASE + 7)
    Public Const MMSYSERR_NOTENABLED As Integer = (MMSYSERR_BASE + 3)
    Public Const MMSYSERR_NOTSUPPORTED As Integer = (MMSYSERR_BASE + 8)

    Public Structure MIXERCONTROLDETAILS_UNSIGNED
        Dim dwValue As Integer
    End Structure

    Public Structure MIXERCONTROLDETAILS_SIGNED
        Dim lValue As Long
    End Structure

    Public Structure MIXERCONTROLDETAILS_BOOLEAN
        Dim fValue As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure MIXERCONTROLDETAILS_LISTTEXT
        Dim dwParam1 As Integer
        Dim dwParam2 As Integer
        <VBFixedString(MIXER_LONG_NAME_CHARS), MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MIXER_LONG_NAME_CHARS)> Dim szName As String
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure MIXERCONTROL
        Dim cbStruct As Integer '  size in Byte of MIXERCONTROL
        Dim dwControlID As Integer '  unique control id for mixer device
        Dim dwControlType As Integer '  MIXERCONTROL_CONTROLTYPE_xxx
        Dim fdwControl As Integer '  MIXERCONTROL_CONTROLF_xxx
        Dim cMultipleItems As Integer '  if MIXERCONTROL_CONTROLF_MULTIPLE set
        <VBFixedString(MIXER_SHORT_NAME_CHARS), MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MIXER_SHORT_NAME_CHARS)> Dim szShortName As String ' short name of control
        <VBFixedString(MIXER_LONG_NAME_CHARS), MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MIXER_LONG_NAME_CHARS)> Dim szName As String ' long name of control
        Dim lMinimum As Integer '  Minimum value
        Dim lMaximum As Integer '  Maximum value
        Dim dwMinimum As Integer
        Dim dwMaximum As Integer
        Dim cSteps As Integer
        Dim cbCustomData As Integer
        <VBFixedArray(6), MarshalAs(UnmanagedType.ByValArray, SizeConst:=6, ArraySubType:=UnmanagedType.AsAny)> Dim reserved() As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure MIXERCAPS
        Dim wMid As Short
        Dim wPid As Short
        Dim vDriverVersion As Integer
        <VBFixedString(MAXPNAMELEN), MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MAXPNAMELEN)> Dim szPname As String
        Dim fdwSupport As Integer
        Dim cDestinations As Integer
    End Structure

    Public Structure MIXERLINECONTROLS
        Dim cbStruct As Integer
        Dim dwLineID As Integer
        Dim dwControl As Integer
        Dim cControls As Integer
        Dim cbmxctrl As Integer
        Dim pamxctrl As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure Target
        Dim dwType As Integer '  MIXERLINE_TARGETTYPE_xxxx
        Dim dwDeviceID As Integer '  target device ID of device type
        Dim wMid As Short '  of target device
        Dim wPid As Short '       "
        Dim vDriverVersion As Integer '       "
        <VBFixedString(MAXPNAMELEN), MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MAXPNAMELEN)> Dim szPname As String
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure MIXERLINE
        Dim cbStruct As Integer '  size of MIXERLINE structure
        Dim dwDestination As Integer '  zero based destination index
        Dim dwSource As Integer '  zero based source index (if source)
        Dim dwLineID As Integer '  unique line id for mixer device
        Dim fdwLine As Integer '  state/information about line
        Dim dwUser As Integer '  driver specific information
        Dim dwComponentType As Integer '  component type line connects to
        Dim cChannels As Integer '  number of channels line supports
        Dim cConnections As Integer '  number of connections (possible)
        Dim cControls As Integer '  number of controls at this line
        <VBFixedString(MIXER_SHORT_NAME_CHARS), MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MIXER_SHORT_NAME_CHARS)> Dim szShortName As String
        <VBFixedString(MIXER_LONG_NAME_CHARS), MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MIXER_LONG_NAME_CHARS)> Dim szName As String
        Dim lpTarget As Target
    End Structure

    Public Structure MIXERCONTROLDETAILS
        Dim cbStruct As Integer '  size in Byte of MIXERCONTROLDETAILS
        Dim dwControlID As Integer '  control id to get/set details on
        Dim cChannels As Integer '  number of channels in paDetails array
        Dim Item As Integer ' hwndOwner or cMultipleItems
        Dim cbDetails As Integer '  size of _one_ details_XX struct
        Dim paDetails As Integer '  pointer to array of details_XX structs
    End Structure

    Declare Function mixerOpen Lib "winmm.dll" (ByRef phmx As Integer, ByVal uMxId As Integer, ByVal dwCallback As Integer, ByVal dwInstance As Integer, ByVal fdwOpen As Integer) As Integer
    Declare Function mixerClose Lib "winmm.dll" (ByVal hmx As Integer) As Integer
    Declare Function mixerGetID Lib "winmm.dll" (ByVal hmxobj As Integer, ByRef puMxId As Integer, ByVal fdwId As Integer) As Integer
    Declare Function mixerGetLineControls Lib "winmm.dll" Alias "mixerGetLineControlsA" (ByVal hmxobj As Integer, ByRef pmxlc As MIXERLINECONTROLS, ByVal fdwControls As Integer) As Integer
    Declare Function mixerGetDevCaps Lib "winmm.dll" Alias "mixerGetDevCapsA" (ByVal uMxId As Integer, ByRef pmxcaps As MIXERCAPS, ByVal cbmxcaps As Integer) As Integer
    Declare Function mixerGetLineInfo Lib "winmm.dll" Alias "mixerGetLineInfoA" (ByVal hmxobj As Integer, ByRef pmxl As MIXERLINE, ByVal fdwInfo As Integer) As Integer
    Declare Function mixerGetControlDetails Lib "winmm.dll" Alias "mixerGetControlDetailsA" (ByVal hmxobj As Integer, ByRef pmxcd As MIXERCONTROLDETAILS, ByVal fdwDetails As Integer) As Integer
    Declare Function mixerSetControlDetails Lib "winmm.dll" (ByVal hmxobj As Integer, ByRef pmxcd As MIXERCONTROLDETAILS, ByVal fdwDetails As Integer) As Integer
    Declare Function mixerGetNumDevs Lib "winmm.dll" () As Integer
    Declare Function GlobalAlloc Lib "kernel32" (ByVal wFlags As Integer, ByVal dwBytes As Integer) As Integer
    Declare Function GlobalLock Lib "kernel32" (ByVal hMem As Integer) As Integer
    Declare Function GlobalFree Lib "kernel32" (ByVal hMem As Integer) As Integer
    Public Declare Function GlobalUnlock Lib "kernel32" (ByVal hMem As Integer) As Integer
    Public Declare Function SetTimer Lib "user32" (ByVal hWnd As Integer, ByVal nIDEvent As Integer, ByVal uElapse As Integer, ByVal lpTimerFunc As Integer) As Integer
    Public Declare Function KillTimer Lib "user32" (ByVal hWnd As Integer, ByVal nIDEvent As Integer) As Integer

    Public Function StripNull(ByVal s As String) As String
        Dim p As Integer = InStr(s, Chr(0))
        If p > 0 Then
            s = Left(s, p - 1)
        End If
        Return s.Trim
    End Function

    Public Function VarPtr(ByVal o As Object) As Integer
        Dim GC As GCHandle = GCHandle.Alloc(o, GCHandleType.Pinned)
        Dim ret As Integer = GC.AddrOfPinnedObject.ToInt32

        GC.Free()

        Return ret
    End Function
End Module