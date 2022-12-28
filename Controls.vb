''' <summary>
''' This collection provides a series of <see cref="CControl">CControl</see> objects that interface each available control in the parent <see cref="CLine">line</see> object.
''' </summary>
''' <remarks></remarks>
Public Class Controls
    Implements ICollection, IDisposable

    Private mCol As ArrayList

    Protected Friend Sub Add(ByVal c As CControl)
        mCol.Add(c)
    End Sub

    Friend Function Add(ByVal Key As String, ByVal ctrlinfo As MIXERCONTROL, ByVal lineinfo As MIXERLINE, ByVal mixerHandler As Integer, ByVal IsValid As Boolean, ByVal pCtrl As CLine, Optional ByVal sKey As String = "") As CControl
        Dim objNewMember As New CControl

        Key = Key & mCol.Count()
        sKey = Key

        objNewMember.Key = Key
        'If Len(sKey) = 0 Then
        mCol.Add(objNewMember)
        'Else
        '    mCol.Add(objNewMember, sKey)
        'End If
        objNewMember.Parent = pCtrl
        objNewMember.ControlType = ctrlinfo.dwControlType
        objNewMember.LongName = StripNull(ctrlinfo.szName)
        'If (ctrlinfo.dwControlType And MIXERCONTROL_CT_UNITS_SIGNED) = MIXERCONTROL_CONTROLTYPE_SIGNED Then
        objNewMember.Max = ctrlinfo.lMaximum
        objNewMember.Min = ctrlinfo.lMinimum
        'End If
        'If (ctrlinfo.dwControlType And MIXERCONTROL_CT_UNITS_UNSIGNED) = MIXERCONTROL_CT_UNITS_UNSIGNED Then
        '    objNewMember.Max = ctrlinfo.dwMaximum
        '    objNewMember.Min = ctrlinfo.dwMinimum
        'End If
        objNewMember.Steps = ctrlinfo.cSteps
        objNewMember.ShortName = StripNull(ctrlinfo.szShortName)
        objNewMember.ControlInfo = ctrlinfo
        objNewMember.LineInfo = lineinfo
        objNewMember.Index = mCol.Count()
        objNewMember.phmx = mixerHandler
        objNewMember.IsValid = IsValid
        objNewMember.ID = ctrlinfo.dwControlID

        Return objNewMember

    End Function

    ''' <summary>
    ''' Returns the control specified by the <paramref name="index">index</paramref> parameter.
    ''' </summary>
    ''' <param name="index">A positive integer value that specifies the control to be retrieved.</param>
    ''' <returns>One of the controls in the parent <see cref="CLine">line</see>.</returns>
    Default Public ReadOnly Property Item(ByVal index As Integer) As CControl
        Get
            Return mCol.Item(index - 1)
        End Get
    End Property

    ''' <summary>
    ''' Retrieves a reference to an enumerator object that is used to iterate over a Controls collection.
    ''' </summary>
    Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return mCol.GetEnumerator
    End Function

    Private Sub Remove(ByVal obj As Object)
        mCol.Remove(obj)
    End Sub

    Protected Friend Sub New()
        mCol = New ArrayList
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        While mCol.Count
            CType(mCol(0), CControl).Dispose()
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