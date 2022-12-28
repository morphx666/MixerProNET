''' <summary>
''' This class provides an interface to one of the lines available in the parent <see cref="CMixer">CMixer</see> object.
''' </summary>
''' <remarks>
''' Lines are the containers that group controls for a particular section of a sound card, such as the CD, the Microphone, the Speakers, etc... 
''' </remarks>
Public Class CLine
    Public Key As String
    Private mvarControls As Controls
    Private mvarID As Integer

    ''' <summary>
    ''' A list of all the component types supported by MixerProNET
    ''' </summary>
    Public Enum ComponentTypeConstants
        ctcDST_FIRST = &H0
        ctcDST_DIGITAL = (ComponentTypeConstants.ctcDST_FIRST + 1)
        ctcDST_HEADPHONES = (ComponentTypeConstants.ctcDST_FIRST + 5)
        ctcDST_LAST = (ComponentTypeConstants.ctcDST_FIRST + 8)
        ctcDST_LINE = (ComponentTypeConstants.ctcDST_FIRST + 2)
        ctcDST_MONITOR = (ComponentTypeConstants.ctcDST_FIRST + 3)
        ctcDST_SPEAKERS = (ComponentTypeConstants.ctcDST_FIRST + 4)
        ctcDST_TELEPHONE = (ComponentTypeConstants.ctcDST_FIRST + 6)
        ctcDST_UNDEFINED = (ComponentTypeConstants.ctcDST_FIRST + 0)
        ctcDST_VOICEIN = (ComponentTypeConstants.ctcDST_FIRST + 8)
        ctcDST_WAVEIN = (ComponentTypeConstants.ctcDST_FIRST + 7)
        ctcSRC_FIRST = &H1000
        ctcSRC_DIGITAL = (ComponentTypeConstants.ctcSRC_FIRST + 1)
        ctcSRC_LINE = (ComponentTypeConstants.ctcSRC_FIRST + 2)
        ctcSRC_MICROPHONE = (ComponentTypeConstants.ctcSRC_FIRST + 3)
        ctcSRC_SYNTHESIZER = (ComponentTypeConstants.ctcSRC_FIRST + 4)
        ctcSRC_COMPACTDISC = (ComponentTypeConstants.ctcSRC_FIRST + 5)
        ctcSRC_TELEPHONE = (ComponentTypeConstants.ctcSRC_FIRST + 6)
        ctcSRC_PCSPEAKER = (ComponentTypeConstants.ctcSRC_FIRST + 7)
        ctcSRC_WAVEOUT = (ComponentTypeConstants.ctcSRC_FIRST + 8)
        ctcSRC_AUXILIARY = (ComponentTypeConstants.ctcSRC_FIRST + 9)
        ctcSRC_ANALOG = (ComponentTypeConstants.ctcSRC_FIRST + 10)
        ctcSRC_LAST = (ComponentTypeConstants.ctcSRC_FIRST + 10)
        ctcSRC_UNDEFINED = (ComponentTypeConstants.ctcSRC_FIRST + 0)
    End Enum

    Private mvarComponentType As ComponentTypeConstants
    Private mvarChannels As Short
    Private mvarShortName As String
    Private mvarLongName As String
    Private mvarlineinfo As MIXERLINE

    ''' <summary>
    ''' Lines can only be of types, Destination or Source
    ''' </summary>
    ''' <remarks>
    ''' A Destination line represents the end point of a sound card, such as the Speakers.
    ''' A Source line represents a source from where the audio is originated, such as the Microphone.
    ''' </remarks>
    Public Enum LineTypeConstants
        ltcDestination
        ltcSource
    End Enum

    Private mvarLineType As LineTypeConstants
    Private mvarConnectedTo As Integer
    Private mvarDestinationID As Integer
    Private mvarIndex As Short
    Private mvarphmx As Integer
    Private mvarParent As CMixer
    Private mvarTag As String

    ''' <summary>
    ''' This collection provides a series of <see cref="CControl">CControl</see> objects that interface each available control in the current CLine object that matches the specified <see cref="CControl.ControlClassConstants">Control Class</see>.
    ''' </summary>
    ''' <param name="controlClass">One of the available values from the <see cref="CControl.ControlClassConstants">ControlClassConstants</see> enumeration.</param>
    Public Function ControlsByClass(ByVal controlClass As CControl.ControlClassConstants) As Controls
        Dim cc As New Controls

        For Each c As CControl In mvarControls
            If (c.ControlClass And controlClass) = controlClass Then cc.Add(c)
        Next c

        Return cc
    End Function

    ''' <summary>
    ''' This collection provides a series of <see cref="CControl">CControl</see> objects that interface each available control in the parent CLine object that matches the specified <see cref="CControl.ControlType">Control Type</see>.
    ''' </summary>
    ''' <param name="controlType">One of the available values from the <see cref="CControl.ControlTypeConstants">ControlTypeConstants</see> enumeration.</param>
    Public Function ControlsByType(ByVal controlType As CControl.ControlTypeConstants) As Controls
        Dim cc As New Controls

        For Each c As CControl In mvarControls
            If c.ControlType = controlType Then cc.Add(c)
        Next c

        Return cc
    End Function

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
    ''' Returns a reference to the parent <see cref="CMixer">CMixer</see> object.
    ''' </summary>
    Public Property Parent() As CMixer
        Get
            Parent = mvarParent
        End Get
        Protected Friend Set(ByVal Value As CMixer)
            mvarParent = Value
        End Set
    End Property

    Protected Friend WriteOnly Property phmx() As Integer
        Set(ByVal Value As Integer)
            mvarphmx = Value
        End Set
    End Property

    ''' <summary>
    ''' The index of the current line.
    ''' </summary>
    ''' <returns>A positive number indicating the index of the current line in the <see cref="Lines">Lines</see> collection.</returns>
    Public Property Index() As Short
        Get
            Return mvarIndex
        End Get
        Protected Friend Set(ByVal value As Short)
            mvarIndex = value
        End Set
    End Property

    ''' <summary>
    ''' Returns the line <see cref="ID">ID</see> of the line to which this line is connected.
    ''' </summary>
    Public Property DestinationID() As Integer
        Get
            Return mvarDestinationID
        End Get
        Protected Friend Set(ByVal value As Integer)
            mvarDestinationID = value
        End Set
    End Property

    ''' <summary>
    ''' Returns the ID of the destination line that this line is connected.
    ''' </summary>
    ''' <remarks>
    ''' A mixer has two types of lines: destinations and sources.
    ''' For destination lines this property will always be zero. For sources lines this property will have the ID of the destination line to which it's connected.
    ''' </remarks>
    Public Property ConnectedTo() As Integer
        Get
            Return mvarConnectedTo
        End Get
        Protected Friend Set(ByVal value As Integer)
            mvarConnectedTo = value
        End Set
    End Property

    ''' <summary>
    ''' The type of line, destination or source, as described by one of the values from the <see cref="LineTypeConstants">LineTypeConstants</see> enumeration.
    ''' </summary>
    Public Property LineType() As LineTypeConstants
        Get
            Return mvarLineType
        End Get
        Protected Friend Set(ByVal value As LineTypeConstants)
            mvarLineType = value
        End Set
    End Property

    Friend WriteOnly Property LineInfo() As MIXERLINE
        Set(ByVal Value As MIXERLINE)
            mvarlineinfo = Value
        End Set
    End Property

    ''' <summary>
    ''' Returns the long name of the line as reported by the sound card's driver.
    ''' </summary>
    Public Property LongName() As String
        Get
            Return mvarLongName
        End Get
        Protected Friend Set(ByVal Value As String)
            mvarLongName = Value
        End Set
    End Property

    ''' <summary>
    ''' Returns the short name of the line as reported by the sound card's driver.
    ''' </summary>
    Public Property ShortName() As String
        Get
            Return mvarShortName
        End Get
        Set(ByVal Value As String)
            mvarShortName = Value
        End Set
    End Property

    ''' <summary>
    ''' Returns the number of channels for the line.
    ''' </summary>
    Public Property Channels() As Short
        Get
            Return mvarChannels
        End Get
        Protected Friend Set(ByVal Value As Short)
            mvarChannels = Value
        End Set
    End Property

    ''' <summary>
    ''' Returns the type of component that this line is interfacing.
    ''' </summary>
    ''' <returns>One of the possible values from the <see cref="ComponentTypeConstants">ComponentTypeConstants</see> enumeration.</returns>
    Public Property ComponentType() As ComponentTypeConstants
        Get
            Return mvarComponentType
        End Get
        Protected Friend Set(ByVal Value As ComponentTypeConstants)
            mvarComponentType = Value
        End Set
    End Property

    ''' <summary>
    ''' The returns the unique identifier for the current line.
    ''' </summary>
    Public Property ID() As Integer
        Get
            Return mvarID
        End Get
        Protected Friend Set(ByVal value As Integer)
            mvarID = value
            GetControls(, IIf(mvarControls.Count > 0, 1, 0))
        End Set
    End Property

    Public Property Controls() As Controls
        Get
            If mvarControls Is Nothing Then mvarControls = New Controls
            Return mvarControls
        End Get
        Protected Set(ByVal Value As Controls)
            mvarControls = Value
        End Set
    End Property

    Protected Friend Sub New()
        mvarControls = New Controls
    End Sub

    ''' <summary>
    ''' Releases all resources used by the System.ComponentModel.Component.
    ''' </summary>
    Public Sub Dispose()
        mvarControls.Dispose()
    End Sub

    Protected Friend Function GetControls(Optional ByRef col As Controls = Nothing, Optional ByVal FlagsOnlyForIndex As Integer = 0, Optional ByVal CtrlType As CControl.ControlTypeConstants = CControl.ControlTypeConstants.ctrltcNULL) As Integer
        Dim i As Integer
        Dim mixerLineCtrl As MIXERLINECONTROLS
        Dim mixerCtrl() As MIXERCONTROL
        Dim ForceOffset As Integer
        Dim hMem As Integer
        Dim IsValid As Boolean

        If mvarlineinfo.cControls = 0 Then Exit Function
        If col Is Nothing Then col = mvarControls

        ReDim mixerCtrl(mvarlineinfo.cControls - 1)
        With mixerLineCtrl
            .cbmxctrl = Marshal.SizeOf(mixerCtrl(0))
            .cbStruct = Marshal.SizeOf(mixerLineCtrl)
            .cControls = mvarlineinfo.cControls
            .dwLineID = mvarlineinfo.dwLineID

            hMem = GlobalAlloc(&H40S, .cbmxctrl * .cControls)
            .pamxctrl = GlobalLock(hMem)
        End With
        If mixerGetLineControls(mvarphmx, mixerLineCtrl, MIXER_GETLINECONTROLSF_ALL) = 0 Then
            For i = 0 To mvarlineinfo.cControls - 1
                'ForceOffset = (mixerLineCtrl.cbmxctrl - Marshal.SizeOf(mixerCtrl(0))) * i
                mixerCtrl(i) = CType(Marshal.PtrToStructure(New IntPtr(mixerLineCtrl.pamxctrl + Marshal.SizeOf(mixerCtrl(0)) * i + ForceOffset), GetType(MIXERCONTROL)), MIXERCONTROL)

                IsValid = False

                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_CLASS_FADER) = MIXERCONTROL_CT_CLASS_FADER
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_CLASS_LIST) = MIXERCONTROL_CT_CLASS_LIST
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_CLASS_MASK) = MIXERCONTROL_CT_CLASS_MASK
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_CLASS_METER) = MIXERCONTROL_CT_CLASS_METER
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_CLASS_NUMBER) = MIXERCONTROL_CT_CLASS_NUMBER
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_CLASS_SLIDER) = MIXERCONTROL_CT_CLASS_SLIDER
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_CLASS_SWITCH) = MIXERCONTROL_CT_CLASS_SWITCH
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_CLASS_TIME) = MIXERCONTROL_CT_CLASS_TIME
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_SC_LIST_MULTIPLE) = MIXERCONTROL_CT_SC_LIST_MULTIPLE
                'IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_SC_LIST_SINGLE) = MIXERCONTROL_CT_SC_LIST_SINGLE
                'IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_SC_METER_POLLED) = MIXERCONTROL_CT_SC_METER_POLLED
                'IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_SC_SWITCH_BOOLEAN) = MIXERCONTROL_CT_SC_SWITCH_BOOLEAN
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_SC_SWITCH_BUTTON) = MIXERCONTROL_CT_SC_SWITCH_BUTTON
                'IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_SC_TIME_MICROSECS) = MIXERCONTROL_CT_SC_TIME_MICROSECS
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_SC_TIME_MILLISECS) = MIXERCONTROL_CT_SC_TIME_MILLISECS
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_UNITS_BOOLEAN) = MIXERCONTROL_CT_UNITS_BOOLEAN
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_UNITS_DECIBELS) = MIXERCONTROL_CT_UNITS_DECIBELS
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_UNITS_MASK) = MIXERCONTROL_CT_UNITS_MASK
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_UNITS_PERCENT) = MIXERCONTROL_CT_UNITS_PERCENT
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_UNITS_SIGNED) = MIXERCONTROL_CT_UNITS_SIGNED
                IsValid = IsValid Or (mixerCtrl(i).dwControlType And MIXERCONTROL_CT_UNITS_UNSIGNED) = MIXERCONTROL_CT_UNITS_UNSIGNED

                If FlagsOnlyForIndex <> 0 Then
                    If FlagsOnlyForIndex = i Then
                        GetControls = mixerCtrl(i).fdwControl
                        Exit For
                    End If
                Else
                    If (mixerCtrl(i).dwControlType = CtrlType Or CtrlType = -1) Then
                        col.Add("C" & CStr(i + 1), mixerCtrl(i), mvarlineinfo, mvarphmx, IsValid, Me)
                    End If
                End If
            Next i
        End If

        GlobalUnlock(hMem)
        GlobalFree(hMem)

    End Function

    ''' <summary>
    ''' Returns a string that represents the current object.
    ''' </summary>
    ''' <returns>Returns the same value as <see cref="LongName">LongName</see>.</returns>
    ''' <remarks></remarks>
    Public Overrides Function ToString() As String
        Return mvarLongName
    End Function
End Class