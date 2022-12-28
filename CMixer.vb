''' <summary>
''' This class provides an interface to one of the installed sound cards.
''' </summary>
Public Class CMixer
    Public Key As String

    Private Const CALLBACK_WINDOW As Integer = &H10000

    Private mvarLines As Lines
    Private mvarDeviceName As String
    Private mvarDriverVersion As String
    Private mvarID As Integer
    Private mvarManufacturerID As Integer
    Private mvarProductID As Integer
    Private mvarDestinations As Integer
    Private mvarIndex As Short
    Private mvarphmx As Integer
    Private mvarHWND As Integer
    Private mvarTag As String

    ''' <summary>
    ''' This collection provides a series of <see cref="CLine">CLine</see> objects that match the specified ID.
    ''' </summary>
    ''' <param name="ID">The unique identifier for a target line so that the function will return all the lines in the mixer that are connected to the line identified by this ID.</param>
    ''' <returns>A list of lines that are connected to a line whose ID matches the ID passed as a parameter.</returns>
    ''' <remarks></remarks>
    Public Function LinesByConnection(ByVal id As Integer) As Lines
        Dim lc As New Lines()

        For Each l As CLine In mvarLines
            If l.ConnectedTo = id Then lc.Add(l)
        Next l

        Return lc
    End Function

    ''' <summary>
    ''' This collection provides a series of CLine objects that match the specified <see cref="CLine.LineTypeConstants">Line Type</see>.
    ''' </summary>
    ''' <param name="lineType">One of values from the <see cref="CLine.LineTypeConstants">LineTypeConstants</see> enumeration.</param>
    ''' <returns>A list of lines whose type matches <paramref name="lineType">lineType</paramref></returns>
    ''' <remarks></remarks>
    Public Function LinesByLineType(ByVal lineType As CLine.LineTypeConstants) As Lines
        Dim lc As New Lines

        For Each l As CLine In mvarLines
            If l.LineType = lineType Then lc.Add(l)
        Next l

        Return lc
    End Function

    ''' <summary>
    ''' This collection provides a series of CLine objects that match the specified <see cref="CLine.ComponentType">Component Type</see>.
    ''' </summary>
    ''' <param name="compType">Numeric expression that is the combination of one or more of the possible values from the <see cref="CLine.ComponentTypeConstants">ComponentTypeConstants</see> enumeration.</param>
    ''' <returns>A list of lines that match one or more of values from the <paramref name="compType">compType</paramref> parameter.</returns>
    ''' <remarks></remarks>
    Public Function LinesByComponentType(ByRef compType As CLine.ComponentTypeConstants) As Lines
        Dim lc As New Lines

        For Each l As CLine In mvarLines
            If l.ComponentType = compType Then lc.Add(l)
        Next l

        Return lc
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

    Friend Property WinHWND() As Integer
        Get
            Return mvarHWND
        End Get
        Set(ByVal Value As Integer)
            mvarHWND = Value
        End Set
    End Property

    ''' <summary>
    ''' The index of the current mixer.
    ''' </summary>
    ''' <returns>A positive number indicating the index of the current mixer in the <see cref="Mixers">Mixers</see> collection.</returns>
    Public Property Index() As Short
        Get
            Return mvarIndex
        End Get
        Protected Friend Set(ByVal value As Short)
            mvarIndex = value
        End Set
    End Property

    Protected Friend WriteOnly Property Destinations() As Object
        Set(ByVal Value As Object)
            mvarDestinations = Value
        End Set
    End Property

    Public Property mixerHandler() As Integer
        Get
            mixerHandler = mvarphmx
        End Get
        Set(ByVal Value As Integer)
            mvarphmx = Value
        End Set
    End Property

    ''' <summary>
    ''' The ID of the mixer as provided by the sound card driver.
    ''' </summary>
    Public Property ProductID() As Integer
        Get
            Return mvarProductID
        End Get
        Protected Friend Set(ByVal Value As Integer)
            mvarProductID = Value
        End Set
    End Property

    ''' <summary>
    ''' The unique identifier of the mixer as provided by the sound card driver.
    ''' </summary>
    Public Property ManufacturerID() As Integer
        Get
            Return mvarManufacturerID
        End Get
        Set(ByVal Value As Integer)
            mvarManufacturerID = Value
        End Set
    End Property

    ''' <summary>
    ''' The unique identifier for the mixer.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ID() As Integer
        Get
            Return mvarID
        End Get
        Protected Friend Set(ByVal Value As Integer)
            mvarID = Value
            GetLines(mvarLines)
        End Set
    End Property

    ''' <summary>
    ''' The driver version as reported by the sound card driver.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DriverVersion() As String
        Get
            Return mvarDriverVersion
        End Get
        Set(ByVal Value As String)
            mvarDriverVersion = Value
        End Set
    End Property

    ''' <summary>
    ''' The device name as reported by the sound card driver.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DeviceName() As String
        Get
            Return mvarDeviceName
        End Get
        Set(ByVal Value As String)
            mvarDeviceName = Value
        End Set
    End Property

    ''' <summary>
    ''' This collection provides a series of <see cref="CLine">CLine</see> objects that interface each available line in the current CMixer object.
    ''' </summary>
    ''' <returns>A list of all the lines provided by the sound card's driver.</returns>
    Public Property Lines() As Lines
        Get
            If mvarLines Is Nothing Then mvarLines = New Lines()
            Lines = mvarLines
        End Get
        Set(ByVal Value As Lines)
            mvarLines = Value
        End Set
    End Property

    Protected Friend Sub New()
        mvarLines = New Lines()
    End Sub

    ''' <summary>
    ''' Releases all resources used by the System.ComponentModel.Component.
    ''' </summary>
    Public Sub Dispose()
        mixerClose(mvarphmx)
        mvarLines.Dispose()
    End Sub

    Private Sub GetLines(ByVal col As Lines, Optional ByVal CompType As CLine.ComponentTypeConstants = -1)
        Dim dstLine As Integer
        Dim srcLine As Integer
        Dim dlineInfo As MIXERLINE
        Dim slineInfo As MIXERLINE
        Dim mh As Integer

        mixerOpen(mh, mvarID, mvarHWND, 0, MIXER_OBJECTF_HMIXER Or CALLBACK_WINDOW)
        mixerHandler = mh

        For dstLine = 0 To mvarDestinations - 1
            With dlineInfo
                .cbStruct = Len(dlineInfo)
                .dwDestination = dstLine
            End With
            If mixerGetLineInfo(mvarphmx, dlineInfo, MIXER_GETLINEINFOF_DESTINATION) = 0 Then
                If dlineInfo.dwComponentType = CompType Or CompType = -1 Then
                    col.Add("L" & CStr(dstLine), dlineInfo, CLine.LineTypeConstants.ltcDestination, 0, mvarphmx, Me)
                End If

                For srcLine = 0 To dlineInfo.cConnections - 1
                    With slineInfo
                        .cbStruct = Len(slineInfo)
                        .dwDestination = dstLine
                        .dwSource = srcLine
                    End With
                    If mixerGetLineInfo(mvarphmx, slineInfo, MIXER_GETLINEINFOF_SOURCE) = 0 Then
                        If slineInfo.dwComponentType = CompType Or CompType = -1 Then
                            col.Add("L" & CStr(srcLine), slineInfo, CLine.LineTypeConstants.ltcSource, dlineInfo.dwLineID, mvarphmx, Me)
                        End If
                    End If
                Next srcLine
            End If
        Next dstLine
    End Sub

    ''' <summary>
    ''' Returns a string that represents the current object.
    ''' </summary>
    ''' <returns>Returns the same value as <see cref="DeviceName">DeviceName</see>.</returns>
    ''' <remarks></remarks>
    Public Overrides Function ToString() As String
        Return DeviceName
    End Function
End Class