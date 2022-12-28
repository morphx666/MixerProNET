''' <summary>
''' This collection provides a series of <see cref="CCtrlItem">CCtrlItem</see> objects that interface each available control in the parent <see cref="CControl">CControl</see> object.
''' </summary>
''' <remarks></remarks>
Public Class CCtrlItem
    Implements IDisposable

    Public Key As String

    Private mvarControlName As Object
    Private mvarIndex As Short

    Private mvarParent As CControl
    Private mvarTag As String

    ''' <summary>
    ''' Use to bind a <see cref="Control">Control</see> to a CCtrlItem item to allow for automatic manipulation of the control items in a control.
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
        Set(ByVal Value As String)
            mvarTag = Value
        End Set
    End Property

    ''' <summary>
    ''' Returns a reference to the parent <see cref="CControl">CControl</see> object.
    ''' </summary>
    Public Property Parent() As CControl
        Get
            Parent = mvarParent
        End Get
        Protected Friend Set(ByVal Value As CControl)
            mvarParent = Value
        End Set
    End Property

    ''' <summary>
    ''' Sets or returns the values of the current control item.
    ''' </summary>
    Public Property Value() As Integer
        Get
            Value = GetItemValue()
        End Get
        Set(ByVal Value As Integer)
            SetItemValue(Value)
        End Set
    End Property

    ''' <summary>
    ''' This property is the same as <see cref="Value">Value</see> and is only provided for compatibility purposes with the <see cref="Binding">Binding</see> support.
    ''' </summary>
    Public Property UniformValue() As Integer
        Get
            Return Value
        End Get
        Set(ByVal value As Integer)
            Me.Value = value
        End Set
    End Property

    ''' <summary>
    ''' The index of the current control item.
    ''' </summary>
    ''' <returns>A positive number indicating the index of the current control item in the <see cref="CtrlItems">CtrlItems</see> collection.</returns>
    Public Property Index() As Short
        Get
            Return mvarIndex
        End Get
        Protected Friend Set(ByVal value As Short)
            mvarIndex = value
        End Set
    End Property

    ''' <summary>
    ''' Returns the name of the current control item as reported by the sound card's driver.
    ''' </summary>
    Public Property ItemName() As Object
        Get
            If IsReference(mvarControlName) Then
                ItemName = mvarControlName
            Else
                Return mvarControlName
            End If
        End Get
        Protected Friend Set(ByVal value As Object)
            If IsReference(value) And Not TypeOf value Is String Then
                mvarControlName = value
            Else
                mvarControlName = value
            End If
        End Set
    End Property

    Private Function GetItemValue() As Integer
        Dim v() As Integer = mvarParent.GetControlValue(True)
        GetItemValue = v(mvarIndex - 1)
    End Function

    Private Function SetItemValue(ByVal vv As Integer) As Integer
        Dim v(mvarParent.LineInfo.cChannels * mvarParent.ControlInfo.cMultipleItems - 1) As Integer

        v(mvarIndex - 1) = vv
        mvarParent.SetControlValue(v, True)
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        Binding.Dispose()
    End Sub

    Public Overrides Function ToString() As String
        Return mvarControlName
    End Function
End Class