Public Class CtrlItems
    Implements ICollection, IDisposable

    Private mCol As ArrayList

    Friend Function Add(ByVal Key As String, ByVal ctrlName As String, ByVal pCtrl As CControl, Optional ByVal sKey As String = "") As CCtrlItem

        Dim objNewMember As New CCtrlItem

        objNewMember.Key = Key
        'If Len(sKey) = 0 Then
        mCol.Add(objNewMember)
        'Else
        '	mCol.Add(objNewMember, sKey)
        'End If
        objNewMember.Parent = pCtrl
        objNewMember.ItemName = ctrlName
        objNewMember.Index = mCol.Count()

        Return objNewMember

    End Function

    Default Public ReadOnly Property Item(ByVal vntIndexKey As Integer) As CCtrlItem
        Get
            Return mCol.Item(vntIndexKey - 1)
        End Get
    End Property

    Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return mCol.GetEnumerator
    End Function

    Public Sub Remove(ByVal obj As Object)
        mCol.Remove(obj)
    End Sub

    Public Sub New()
        MyBase.New()
        mCol = New ArrayList
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        While mCol.Count
            CType(mCol(0), CCtrlItem).Dispose()
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