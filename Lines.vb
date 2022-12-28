''' <summary>
''' This collection provides a series of <see cref="CLine">CLine</see> objects that interface each available line in the parent <see cref="CMixer">CMixer</see> object.
''' </summary>
Public Class Lines
    Implements ICollection, IDisposable

    Private mCol As ArrayList

    Protected Friend Sub Add(ByVal l As CLine)
        mCol.Add(l)
    End Sub

    Friend Function Add(ByVal Key As String, ByRef lineinfo As MIXERLINE, ByVal lType As CLine.LineTypeConstants, ByVal ConnTo As Integer, ByVal mixerHandler As Integer, ByRef pCtrl As CMixer, Optional ByVal sKey As String = "") As CLine
        Dim objNewMember As New CLine()

        Key = Key & mCol.Count()
        objNewMember.Key = Key
        sKey = Key
        'If Len(sKey) = 0 Then
        mCol.Add(objNewMember)
        'Else
        '	mCol.Add(objNewMember, sKey)
        'End If

        objNewMember.Index = mCol.Count()
        objNewMember.Parent = pCtrl
        objNewMember.Channels = lineinfo.cChannels
        objNewMember.ComponentType = lineinfo.dwComponentType
        objNewMember.LongName = StripNull(lineinfo.szName)
        objNewMember.ShortName = StripNull(lineinfo.szShortName)
        objNewMember.LineInfo = lineinfo
        objNewMember.LineType = lType
        objNewMember.DestinationID = lineinfo.dwDestination
        objNewMember.LineInfo = lineinfo
        objNewMember.ConnectedTo = ConnTo
        objNewMember.phmx = mixerHandler
        objNewMember.ID = lineinfo.dwLineID

        Return objNewMember
    End Function

    ''' <summary>
    ''' Returns the line specified by the <paramref name="index">index</paramref> parameter.
    ''' </summary>
    ''' <param name="index">A positive integer value that specifies the line to be retrieved.</param>
    ''' <returns>One of the lines in the parent <see cref="CMixer">mixer</see>.</returns>
    Default Public ReadOnly Property Item(ByVal index As Integer) As CLine
        Get
            Return mCol.Item(index - 1)
        End Get
    End Property

    ''' <summary>
    ''' Retrieves a reference to an enumerator object that is used to iterate over a Lines collection.
    ''' </summary>
    Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return mCol.GetEnumerator
    End Function

    Protected Friend Sub Remove(ByVal obj As Object)
        mCol.Remove(obj)
    End Sub

    Protected Friend Sub New()
        mCol = New ArrayList
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        While mCol.Count
            CType(mCol(0), CLine).Dispose()
            mCol.Remove(mCol(0))
        End While
        mCol = Nothing
    End Sub

    Public ReadOnly Property Count() As Integer Implements ICollection.Count
        Get
            Return mCol.Count()
        End Get
    End Property

    Public ReadOnly Property IsSynchronized() As Boolean Implements ICollection.IsSynchronized
        Get
            Return False
        End Get
    End Property

    Public ReadOnly Property SyncRoot() As Object Implements ICollection.SyncRoot
        Get
            Return Me
        End Get
    End Property

    Public Sub CopyTo(ByVal array As Array, ByVal index As Integer) Implements ICollection.CopyTo
        Dim i As Integer
        For Each i In mCol
            array(index) = i
            index = index + 1
        Next
    End Sub
End Class