''' <summary>
''' This class provides an interface to one of the controls available in the parent <see cref="CLine">CLine</see> object.
''' </summary>
Public Class CControl
    Public Key As String

    Private timers() As Integer
    Private IsPeakMeter As Boolean

    Private mvarCtrlItems As CtrlItems
    Private mvarID As Integer
    Private mvarShortName As String
    Private mvarLongName As String

    ''' <summary>
    ''' This enumeration lists all the control types supported by MixerProNet.
    ''' </summary>
    Public Enum ControlTypeConstants
        ctrltcNULL = -1
        ctrltcFADER = (MIXERCONTROL_CT_CLASS_FADER Or MIXERCONTROL_CT_UNITS_UNSIGNED)
        ctrltcBASS = MIXERCONTROL_CONTROLTYPE_FADER + 2
        ctrltcTREBLE = MIXERCONTROL_CONTROLTYPE_FADER + 3
        ctrltcBOOLEAN = (MIXERCONTROL_CT_CLASS_SWITCH Or MIXERCONTROL_CT_SC_SWITCH_BOOLEAN Or MIXERCONTROL_CT_UNITS_BOOLEAN)
        ctrltcBOOLEANMETER = (MIXERCONTROL_CT_CLASS_METER Or MIXERCONTROL_CT_SC_METER_POLLED Or MIXERCONTROL_CT_UNITS_BOOLEAN)
        ctrltcBUTTON = (MIXERCONTROL_CT_CLASS_SWITCH Or MIXERCONTROL_CT_SC_SWITCH_BUTTON Or MIXERCONTROL_CT_UNITS_BOOLEAN)
        ctrltcCUSTOM = (MIXERCONTROL_CT_CLASS_CUSTOM Or MIXERCONTROL_CT_UNITS_CUSTOM)
        ctrltcDECIBELS = (MIXERCONTROL_CT_CLASS_NUMBER Or MIXERCONTROL_CT_UNITS_DECIBELS)
        ctrltcEQUALIZER = MIXERCONTROL_CONTROLTYPE_FADER + 4
        ctrltcLOUDNESS = MIXERCONTROL_CONTROLTYPE_BOOLEAN + 4
        ctrltcMICROTIME = (MIXERCONTROL_CT_CLASS_TIME Or MIXERCONTROL_CT_SC_TIME_MICROSECS Or MIXERCONTROL_CT_UNITS_UNSIGNED)
        ctrltcMILLITIME = (MIXERCONTROL_CT_CLASS_TIME Or MIXERCONTROL_CT_SC_TIME_MILLISECS Or MIXERCONTROL_CT_UNITS_UNSIGNED)
        ctrltcMULTIPLESELECT = (MIXERCONTROL_CT_CLASS_LIST Or MIXERCONTROL_CT_SC_LIST_MULTIPLE Or MIXERCONTROL_CT_UNITS_BOOLEAN)
        ctrltcMIXER = MIXERCONTROL_CONTROLTYPE_MULTIPLESELECT + 1
        ctrltcMONO = MIXERCONTROL_CONTROLTYPE_BOOLEAN + 3
        ctrltcMUTE = MIXERCONTROL_CONTROLTYPE_BOOLEAN + 2
        ctrltcSINGLESELECT = (MIXERCONTROL_CT_CLASS_LIST Or MIXERCONTROL_CT_SC_LIST_SINGLE Or MIXERCONTROL_CT_UNITS_BOOLEAN)
        ctrltcMUX = MIXERCONTROL_CONTROLTYPE_SINGLESELECT + 1
        ctrltcONOFF = MIXERCONTROL_CONTROLTYPE_BOOLEAN + 1
        ctrltcSLIDER = (MIXERCONTROL_CT_CLASS_SLIDER Or MIXERCONTROL_CT_UNITS_SIGNED)
        ctrltcPAN = MIXERCONTROL_CONTROLTYPE_SLIDER + 1
        ctrltcSIGNEDMETER = (MIXERCONTROL_CT_CLASS_METER Or MIXERCONTROL_CT_SC_METER_POLLED Or MIXERCONTROL_CT_UNITS_SIGNED)
        ctrltcPEAKMETER = MIXERCONTROL_CONTROLTYPE_SIGNEDMETER + 1
        ctrltcPERCENT = (MIXERCONTROL_CT_CLASS_NUMBER Or MIXERCONTROL_CT_UNITS_PERCENT)
        ctrltcQSOUNDPAN = MIXERCONTROL_CONTROLTYPE_SLIDER + 2
        ctrltcSIGNED = (MIXERCONTROL_CT_CLASS_NUMBER Or MIXERCONTROL_CT_UNITS_SIGNED)
        ctrltcSTEREOENH = MIXERCONTROL_CONTROLTYPE_BOOLEAN + 5
        ctrltcUNSIGNED = (MIXERCONTROL_CT_CLASS_NUMBER Or MIXERCONTROL_CT_UNITS_UNSIGNED)
        ctrltcUNSIGNEDMETER = (MIXERCONTROL_CT_CLASS_METER Or MIXERCONTROL_CT_SC_METER_POLLED Or MIXERCONTROL_CT_UNITS_UNSIGNED)
        ctrltcVOLUME = MIXERCONTROL_CONTROLTYPE_FADER + 1
    End Enum

    Private mvarControlType As ControlTypeConstants
    Private mvarMax As Integer
    Private mvarMin As Integer

    ''' <summary>
    ''' This enumeration lists all the classes supported by MixerProNET.
    ''' </summary>
    Public Enum ControlClassConstants
        cccCLASS_CUSTOM = &H0
        cccCLASS_FADER = &H50000000
        cccCLASS_LIST = &H70000000
        cccCLASS_MASK = &HF0000000
        cccCLASS_METER = &H10000000
        cccCLASS_NUMBER = &H30000000
        cccCLASS_SLIDER = &H40000000
        cccCLASS_SWITCH = &H20000000
        cccCLASS_TIME = &H60000000
    End Enum

    Private mvarctrlinfo As MIXERCONTROL
    Private mvarlineinfo As MIXERLINE
    Private mvarIndex As Short
    Private mvarphmx As Integer
    Private mvarIsValid As Boolean
    Private mvarParent As CLine
    Private mvarSteps As Integer
    Private mvarTag As String

    ''' <summary>
    ''' Use to bind a <see cref="Control">Control</see> to a CControl to allow for automatic manipulation of the controls in a line.
    ''' </summary>
    ''' <remarks></remarks>
    Public Binding As New CBinding(Me)

    ''' <summary>
    ''' Gets or sets the object that contains data about the control
    ''' </summary>
    ''' <returns>A System.Object that contains data about the control. The default is null.</returns>
    Public Property Tag() As String
        Get
            Return mvarTag
        End Get
        Set(value As String)
            mvarTag = value
        End Set
    End Property

    ''' <summary>
    ''' Allows the modification of the value of the current control regardless of the number channels it supports.
    ''' <seealso cref="Value">Value</seealso>
    ''' </summary>
    Public Property UniformValue() As Integer
        Get
            Dim v() As Integer = Value
            Dim i As Short
            For i = 0 To UBound(v)
                If UniformValue < v(i) Then UniformValue = v(i)
            Next i
        End Get
        Set(newValue As Integer)
            Dim v(mvarlineinfo.cChannels - 1) As Integer
            Dim i As Short
            For i = 0 To mvarlineinfo.cChannels - 1
                v(i) = newValue
            Next
            Value = v
        End Set
    End Property

    ''' <summary>
    ''' Returns the increments in which the Value of the current control changes.
    ''' </summary>
    Public Property Steps() As Integer
        Get
            Return mvarSteps
        End Get
        Protected Friend Set(Value As Integer)
            mvarSteps = Value
        End Set
    End Property

    ''' <summary>
    ''' Returns a reference to the parent <see cref="CLine">CLine</see> object.
    ''' </summary>
    Public Property Parent() As CLine
        Get
            Parent = mvarParent
        End Get
        Protected Friend Set(Value As CLine)
            mvarParent = Value
        End Set
    End Property

    ''' <summary>
    ''' Returns whether the control is valid or not.
    ''' </summary>
    ''' <remarks>
    ''' Invalid controls are those that are not supported by the engine.
    ''' Such controls include all those that fall into the <see cref="ControlClassConstants">cccCLASS_CUSTOM</see> class.
    ''' </remarks>
    Public Property IsValid() As Boolean
        Get
            Return mvarIsValid
        End Get
        Protected Friend Set(Value As Boolean)
            mvarIsValid = Value
        End Set
    End Property

    ''' <summary>
    ''' Returns whether the control is enabled or disabled.
    ''' </summary>
    Public ReadOnly Property Enabled() As Boolean
        Get
            Return (mvarParent.GetControls(, CInt(mvarIndex)) And MIXERCONTROL_CONTROLF_DISABLED) <> MIXERCONTROL_CONTROLF_DISABLED
        End Get
    End Property

    Protected Friend WriteOnly Property phmx() As Integer
        Set(Value As Integer)
            mvarphmx = Value
        End Set
    End Property

    ''' <summary>
    ''' The index of the current control.
    ''' </summary>
    ''' <returns>A positive number indicating the index of the current control in the <see cref="Controls">Controls</see> collection.</returns>
    Public Property Index() As Short
        Get
            Return mvarIndex
        End Get
        Protected Friend Set(value As Short)
            mvarIndex = value
        End Set
    End Property

    Friend Property LineInfo() As MIXERLINE
        Get
            Return mvarlineinfo
        End Get
        Set(value As MIXERLINE)
            mvarlineinfo = value
        End Set
    End Property

    Friend Property ControlInfo() As MIXERCONTROL
        Get
            Return mvarctrlinfo
        End Get
        Set(Value As MIXERCONTROL)
            mvarctrlinfo = Value
        End Set
    End Property

    ''' <summary>
    ''' Returns a value indicating a combination of classes that define the type of control as specified by the <see cref="ControlClassConstants">ControlClassConstants</see> enumeration.
    ''' </summary>
    Public ReadOnly Property ControlClass() As ControlClassConstants
        Get
            If (mvarControlType And ControlClassConstants.cccCLASS_FADER) = ControlClassConstants.cccCLASS_FADER Then
                Return ControlClassConstants.cccCLASS_FADER
                Exit Property
            End If

            If (mvarControlType And ControlClassConstants.cccCLASS_LIST) = ControlClassConstants.cccCLASS_LIST Then
                Return ControlClassConstants.cccCLASS_LIST
                Exit Property
            End If

            If (mvarControlType And ControlClassConstants.cccCLASS_MASK) = ControlClassConstants.cccCLASS_MASK Then
                Return ControlClassConstants.cccCLASS_MASK
                Exit Property
            End If

            If (mvarControlType And ControlClassConstants.cccCLASS_METER) = ControlClassConstants.cccCLASS_METER Then
                Return ControlClassConstants.cccCLASS_METER
                Exit Property
            End If

            If (mvarControlType And ControlClassConstants.cccCLASS_NUMBER) = ControlClassConstants.cccCLASS_NUMBER Then
                Return ControlClassConstants.cccCLASS_NUMBER
                Exit Property
            End If

            If (mvarControlType And ControlClassConstants.cccCLASS_SLIDER) = ControlClassConstants.cccCLASS_SLIDER Then
                Return ControlClassConstants.cccCLASS_SLIDER
                Exit Property
            End If

            If (mvarControlType And ControlClassConstants.cccCLASS_SWITCH) = ControlClassConstants.cccCLASS_SWITCH Then
                Return ControlClassConstants.cccCLASS_SWITCH
                Exit Property
            End If

            If (mvarControlType And ControlClassConstants.cccCLASS_TIME) = ControlClassConstants.cccCLASS_TIME Then
                Return ControlClassConstants.cccCLASS_TIME
                Exit Property
            End If

            Return ControlClassConstants.cccCLASS_CUSTOM
        End Get
    End Property

    ''' <summary>
    ''' Returns the minimum value supported by the control.
    ''' </summary>
    Public Property Min() As Integer
        Get
            Return mvarMin
        End Get
        Friend Set(Value As Integer)
            mvarMin = Value
        End Set
    End Property

    ''' <summary>
    ''' Returns the maximum value supported by the control.
    ''' </summary>
    Public Property Max() As Integer
        Get
            Return mvarMax
        End Get
        Friend Set(Value As Integer)
            mvarMax = Value
        End Set
    End Property

    ''' <summary>
    ''' Sets or returns an array of values for all the channels available in the control.
    ''' </summary>
    Public Property Value() As Integer()
        Get
            Value = GetControlValue()
        End Get
        Set(value As Integer())
            If IsPeakMeter Then Exit Property
            SetControlValue(value)
        End Set
    End Property

    ''' <summary>
    ''' Returns the type of control as specified by the <see cref="ControlTypeConstants">ControlTypeConstants</see> enumeration.
    ''' </summary>
    Public Property ControlType() As ControlTypeConstants
        Get
            Return mvarControlType
        End Get
        Protected Friend Set(value As ControlTypeConstants)
            mvarControlType = value
        End Set
    End Property

    ''' <summary>
    ''' Returns the long name of the control as reported by the sound card's driver.
    ''' </summary>
    Public Property LongName() As String
        Get
            Return mvarLongName
        End Get
        Protected Friend Set(value As String)
            mvarLongName = value
        End Set
    End Property

    ''' <summary>
    ''' Returns the short name of the control as reported by the sound card's driver.
    ''' </summary>
    Public Property ShortName() As String
        Get
            Return mvarShortName
        End Get
        Set(value As String)
            mvarShortName = value
        End Set
    End Property

    ''' <summary>
    ''' The returns the unique identifier for the current control.
    ''' </summary>
    Public Property ID() As Integer
        Get
            Return mvarID
        End Get
        Protected Friend Set(value As Integer)
            mvarID = value
            GetItems()
        End Set
    End Property

    ''' <summary>
    ''' Returns a list of all the <see cref="CCtrlItem">CCtrlItem</see> available in the control (if any).
    ''' <seealso cref="CtrlItems">CtrlItems</seealso>
    ''' </summary>
    Public Property CtrlItems() As CtrlItems
        Get
            If mvarCtrlItems Is Nothing Then mvarCtrlItems = New CtrlItems
            Return mvarCtrlItems
        End Get
        Protected Set(Value As CtrlItems)
            mvarCtrlItems = Value
        End Set
    End Property

    Public Sub New()
        mvarCtrlItems = CtrlItems
        ReDim timers(0)
    End Sub

    Public Sub Dispose()
        mvarCtrlItems = Nothing
        KillTimers()
    End Sub

    Private Sub KillTimers()
        For i As Short = 1 To UBound(timers)
            KillTimer(Me.Parent.Parent.WinHWND, timers(i))
        Next i
        ReDim timers(0)
    End Sub

    Private Sub CreateTimer(ByVal tmrEventID As Integer)
        IsPeakMeter = True

        ReDim timers(UBound(timers) + 1)
        timers(UBound(timers)) = tmrEventID

        SetTimer(Me.Parent.Parent.WinHWND, tmrEventID, 50, 0)
    End Sub

    Private Sub GetItems()
        Try
            Dim mxCtrlDetailsLT() As MIXERCONTROLDETAILS_LISTTEXT
            Dim mxCtrlDetails As MIXERCONTROLDETAILS
            Dim hMem As Integer
            Dim sLen As Integer

            If (Me.ControlType And MIXERCONTROL_CT_CLASS_FADER) <> MIXERCONTROL_CT_CLASS_FADER Then
                If (Me.ControlType And MIXERCONTROL_CT_CLASS_METER) = MIXERCONTROL_CT_CLASS_METER Then
                    CreateTimer(Me.Parent.Parent.Index * &H10000 + Me.Parent.Index * &H100 + Me.Index)
                End If
            End If

            If Not mvarIsValid Then Exit Sub
            If mvarctrlinfo.cMultipleItems = 0 Then Exit Sub

            ReDim mxCtrlDetailsLT(mvarctrlinfo.cMultipleItems - 1)
            With mxCtrlDetails
                .cbStruct = Len(mxCtrlDetails)
                .dwControlID = mvarctrlinfo.dwControlID
                .cChannels = mvarlineinfo.cChannels
                .cbDetails = Len(mxCtrlDetailsLT(0))
                .Item = mvarctrlinfo.cMultipleItems

                sLen = .cChannels * .Item * .cbDetails
                hMem = GlobalAlloc(&H40S, sLen)
                .paDetails = GlobalLock(hMem)
            End With
            If mixerGetControlDetails(mvarphmx, mxCtrlDetails, MIXER_GETCONTROLDETAILSF_LISTTEXT) = 0 Then
                For i As Integer = 0 To mvarctrlinfo.cMultipleItems - 1
                    mxCtrlDetailsLT(i) = CType(Marshal.PtrToStructure(New IntPtr(mxCtrlDetails.paDetails + Marshal.SizeOf(mxCtrlDetailsLT(0)) * i), GetType(MIXERCONTROLDETAILS_LISTTEXT)), MIXERCONTROLDETAILS_LISTTEXT)
                    mvarCtrlItems.Add(CStr(i), StripNull(mxCtrlDetailsLT(i).szName), Me)
                Next i
            End If

            GlobalUnlock(hMem)
            GlobalFree(hMem)
        Catch ex As Exception
            Debug.WriteLine("CControl.GetItems()" + vbCrLf + ex.Message)
        End Try
    End Sub

    Friend Function GetControlValue(Optional ByVal IsSubItem As Boolean = False) As Integer()
        Dim mctrlDetails As MIXERCONTROLDETAILS
        Dim mctrlDetailsU() As MIXERCONTROLDETAILS_UNSIGNED
        Dim mctrlDetailsS() As MIXERCONTROLDETAILS_SIGNED
        Dim mctrlDetailsB() As MIXERCONTROLDETAILS_BOOLEAN
        Dim v() As Integer
        Dim i As Integer
        Dim r As Integer
        Dim nChan As Integer
        Dim nItems As Short

        Dim IsSigned As Boolean
        Dim IsUnSigned As Boolean
        Dim IsBoolean As Boolean

        If Not mvarIsValid Then Return Nothing
        If mvarctrlinfo.cMultipleItems > 0 And Not IsSubItem Then Return Nothing

        Select Case mvarctrlinfo.dwControlType
            Case MIXERCONTROL_CONTROLTYPE_UNSIGNEDMETER, MIXERCONTROL_CONTROLTYPE_UNSIGNED, MIXERCONTROL_CONTROLTYPE_BASS, MIXERCONTROL_CONTROLTYPE_EQUALIZER, MIXERCONTROL_CONTROLTYPE_FADER, MIXERCONTROL_CONTROLTYPE_TREBLE, MIXERCONTROL_CONTROLTYPE_VOLUME, MIXERCONTROL_CONTROLTYPE_MICROTIME, MIXERCONTROL_CONTROLTYPE_MILLITIME, MIXERCONTROL_CONTROLTYPE_PERCENT
                IsUnSigned = True
            Case MIXERCONTROL_CONTROLTYPE_PEAKMETER, MIXERCONTROL_CONTROLTYPE_SIGNEDMETER, MIXERCONTROL_CONTROLTYPE_SIGNED, MIXERCONTROL_CONTROLTYPE_DECIBELS, MIXERCONTROL_CONTROLTYPE_PAN, MIXERCONTROL_CONTROLTYPE_QSOUNDPAN, MIXERCONTROL_CONTROLTYPE_SLIDER
                IsSigned = True
            Case MIXERCONTROL_CONTROLTYPE_BOOLEANMETER, MIXERCONTROL_CONTROLTYPE_BOOLEAN, MIXERCONTROL_CONTROLTYPE_BUTTON, MIXERCONTROL_CONTROLTYPE_LOUDNESS, MIXERCONTROL_CONTROLTYPE_MONO, MIXERCONTROL_CONTROLTYPE_MUTE, MIXERCONTROL_CONTROLTYPE_ONOFF, MIXERCONTROL_CONTROLTYPE_STEREOENH, MIXERCONTROL_CONTROLTYPE_MIXER, MIXERCONTROL_CONTROLTYPE_MULTIPLESELECT, MIXERCONTROL_CONTROLTYPE_MUX, MIXERCONTROL_CONTROLTYPE_SINGLESELECT
                IsBoolean = True
        End Select

        If (mvarctrlinfo.fdwControl And MIXERCONTROL_CONTROLF_UNIFORM) = MIXERCONTROL_CONTROLF_UNIFORM Then
            nChan = 1
        Else
            nChan = mvarlineinfo.cChannels
        End If
        If IsSubItem Then
            nItems = mvarctrlinfo.cMultipleItems
        Else
            nItems = 1
        End If

Retry:
        ReDim v(nChan * nItems - 1)

        If (Not IsUnSigned) And (Not IsSigned) And (Not IsBoolean) Then
            'Debug.Print Hex(mvarctrlinfo.dwControlType)
        Else
            ReDim mctrlDetailsU(nChan * nItems - 1)
            ReDim mctrlDetailsS(nChan * nItems - 1)
            ReDim mctrlDetailsB(nChan * nItems - 1)
            With mctrlDetails
                If IsUnSigned Then
                    .cbDetails = Marshal.SizeOf(mctrlDetailsU(0))
                    .paDetails = VarPtr(mctrlDetailsU)
                End If
                If IsSigned Then
                    .cbDetails = Marshal.SizeOf(mctrlDetailsS(0))
                    .paDetails = VarPtr(mctrlDetailsS)
                End If
                If IsBoolean Then
                    .cbDetails = Marshal.SizeOf(mctrlDetailsB(0))
                    .paDetails = VarPtr(mctrlDetailsB)
                End If

                .cbStruct = Marshal.SizeOf(mctrlDetails)
                .cChannels = nChan
                .dwControlID = mvarctrlinfo.dwControlID
                .Item = IIf(IsSubItem, nItems, 0)
            End With
            r = mixerGetControlDetails(mvarphmx, mctrlDetails, MIXER_GETCONTROLDETAILSF_VALUE)
            If r <> 0 And nChan > 1 Then
                nChan = nChan - 1
                GoTo Retry
            End If
            If r = 0 Then
                For i = 0 To nChan * nItems - 1
                    If IsUnSigned Then v(i) = mctrlDetailsU(i).dwValue
                    If IsSigned Then v(i) = mctrlDetailsS(i).lValue
                    If IsBoolean Then v(i) = mctrlDetailsB(i).fValue
                Next i
            Else
                Debug.WriteLine("ERROR")
            End If
        End If

        Return v
    End Function

    Friend Sub SetControlValue(ByRef v() As Integer, Optional ByVal IsSubItem As Boolean = False)
        Dim mctrlDetails As MIXERCONTROLDETAILS
        Dim mctrlDetailsU() As MIXERCONTROLDETAILS_UNSIGNED
        Dim mctrlDetailsS() As MIXERCONTROLDETAILS_SIGNED
        Dim mctrlDetailsB() As MIXERCONTROLDETAILS_BOOLEAN
        Dim i As Integer
        Dim nChan As Integer
        Dim nItems As Short

        Dim IsSigned As Boolean
        Dim IsUnSigned As Boolean
        Dim IsBoolean As Boolean

        If Not mvarIsValid Then Exit Sub
        If mvarctrlinfo.cMultipleItems > 0 And Not IsSubItem Then Exit Sub

        Select Case mvarctrlinfo.dwControlType
            Case MIXERCONTROL_CONTROLTYPE_UNSIGNEDMETER, MIXERCONTROL_CONTROLTYPE_UNSIGNED, MIXERCONTROL_CONTROLTYPE_BASS, MIXERCONTROL_CONTROLTYPE_EQUALIZER, MIXERCONTROL_CONTROLTYPE_FADER, MIXERCONTROL_CONTROLTYPE_TREBLE, MIXERCONTROL_CONTROLTYPE_VOLUME, MIXERCONTROL_CONTROLTYPE_MICROTIME, MIXERCONTROL_CONTROLTYPE_MILLITIME, MIXERCONTROL_CONTROLTYPE_PERCENT
                IsUnSigned = True
            Case MIXERCONTROL_CONTROLTYPE_PEAKMETER, MIXERCONTROL_CONTROLTYPE_SIGNEDMETER, MIXERCONTROL_CONTROLTYPE_SIGNED, MIXERCONTROL_CONTROLTYPE_DECIBELS, MIXERCONTROL_CONTROLTYPE_PAN, MIXERCONTROL_CONTROLTYPE_QSOUNDPAN, MIXERCONTROL_CONTROLTYPE_SLIDER
                IsSigned = True
            Case MIXERCONTROL_CONTROLTYPE_BOOLEANMETER, MIXERCONTROL_CONTROLTYPE_BOOLEAN, MIXERCONTROL_CONTROLTYPE_BUTTON, MIXERCONTROL_CONTROLTYPE_LOUDNESS, MIXERCONTROL_CONTROLTYPE_MONO, MIXERCONTROL_CONTROLTYPE_MUTE, MIXERCONTROL_CONTROLTYPE_ONOFF, MIXERCONTROL_CONTROLTYPE_STEREOENH, MIXERCONTROL_CONTROLTYPE_MIXER, MIXERCONTROL_CONTROLTYPE_MULTIPLESELECT, MIXERCONTROL_CONTROLTYPE_MUX, MIXERCONTROL_CONTROLTYPE_SINGLESELECT
                IsBoolean = True
        End Select

        If mvarctrlinfo.fdwControl And MIXERCONTROL_CONTROLF_UNIFORM Then
            nChan = 1
        Else
            nChan = mvarlineinfo.cChannels
        End If
        If IsSubItem Then
            nItems = mvarctrlinfo.cMultipleItems
        Else
            nItems = 1
        End If

Retry:
        If (Not IsUnSigned) And (Not IsSigned) And (Not IsBoolean) Then
            System.Diagnostics.Debug.WriteLine(Hex(mvarctrlinfo.dwControlType))
        Else
            ReDim mctrlDetailsU(nChan * nItems - 1)
            ReDim mctrlDetailsS(nChan * nItems - 1)
            ReDim mctrlDetailsB(nChan * nItems - 1)

            For i = 0 To nChan * nItems - 1
                If IsUnSigned Then mctrlDetailsU(i).dwValue = v(i)
                If IsSigned Then mctrlDetailsS(i).lValue = v(i)
                If IsBoolean Then mctrlDetailsB(i).fValue = v(i)
            Next i

            With mctrlDetails
                If IsUnSigned Then
                    .cbDetails = Len(mctrlDetailsU(0))
                    .paDetails = VarPtr(mctrlDetailsU)
                End If
                If IsSigned Then
                    .cbDetails = Len(mctrlDetailsS(0))
                    .paDetails = VarPtr(mctrlDetailsS)
                End If
                If IsBoolean Then
                    .cbDetails = Len(mctrlDetailsB(0))
                    .paDetails = VarPtr(mctrlDetailsB)
                End If

                .cbStruct = Len(mctrlDetails)
                .cChannels = nChan
                .dwControlID = mvarctrlinfo.dwControlID
                .Item = IIf(IsSubItem, nItems, 0)
            End With
            mixerSetControlDetails(mvarphmx, mctrlDetails, MIXER_SETCONTROLDETAILSF_VALUE)
        End If
    End Sub

    Public Overrides Function ToString() As String
        Return mvarLongName
    End Function
End Class