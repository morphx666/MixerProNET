''' <summary>
''' This collection provides a series of <see cref="CMixer">CMixer</see> objects that interface each available sound card represented by the <see cref="CMixerPRO">CMixerPRO</see> base class.
''' </summary>
Public Class Mixers
    Implements ICollection, IDisposable

    Private mCol As ArrayList

    Protected Friend Function Add(ByVal Key As String, ByVal ID As Integer, ByVal mvarHWND As Integer, Optional ByVal sKey As String = "") As CMixer
        Dim objNewMember As CMixer = New CMixer()

        objNewMember.Key = Key
        'If Len(sKey) = 0 Then
        mCol.Add(objNewMember)
        'Else
        '	mCol.Add(objNewMember, sKey)
        'End If

        Dim mCap As MIXERCAPS
        mixerGetDevCaps(ID, mCap, Len(mCap))
        objNewMember.DeviceName = StripNull(mCap.szPname)
        objNewMember.DriverVersion = StripNull(CStr(mCap.vDriverVersion))
        objNewMember.ManufacturerID = mCap.wMid
        objNewMember.ProductID = mCap.wPid
        objNewMember.Destinations = mCap.cDestinations
        objNewMember.WinHWND = mvarHWND
        objNewMember.Index = mCol.Count()
        objNewMember.ID = ID

        Return objNewMember
    End Function

    ''' <summary>
    ''' Returns the mixer specified by the <paramref name="index">index</paramref> parameter.
    ''' </summary>
    ''' <param name="index">A positive integer value that specifies the <see cref="CMixer">mixer</see> to be retrieved.</param>
    ''' <returns>One of the available mixes in the host computer, where each mixer represents each one of the installed and available sound cards.</returns>
    Default Public ReadOnly Property Item(ByVal index As Integer) As CMixer
        Get
            Return mCol.Item(index - 1)
        End Get
    End Property

    Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return mCol.GetEnumerator
    End Function

    Private Sub Remove(ByVal obj As Object)
        mCol.Remove(obj)
    End Sub

    Protected Friend Sub New()
        MyBase.New()
        mCol = New ArrayList()
    End Sub

    Protected Overrides Sub Finalize()
        mCol = Nothing
        MyBase.Finalize()
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

    Public Sub Dispose() Implements IDisposable.Dispose
        While mCol.Count
            CType(mCol(0), CMixer).Dispose()
            mCol.Remove(mCol(0))
        End While
        mCol = Nothing
    End Sub
End Class