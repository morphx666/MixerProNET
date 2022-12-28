Imports CoreAudio
Imports System.Collections.Generic

Partial Public Class CCoreAudio
    ''' <summary>
    ''' This class provides an interface to one of the lines available in the parent <see cref="CMixer">CMixer</see> object.
    ''' </summary>
    ''' <remarks>
    ''' Lines are the containers that group controls for a particular section of a sound card, such as the CD, the Microphone, the Speakers, etc... 
    ''' </remarks>
    Public Class CLine
        Implements IDisposable

        Private mControls As New List(Of CControl)
        Private mPart As Part
        Private mName As String
        Private mSubTypeName As String
        Private lastControl As CControl
        Private mParent As CMixer

        Protected Friend Sub New(parent As CMixer, part As Part, flow As DataFlow, ByVal lastControl As CControl)
            mParent = parent
            mPart = part
            mName = mPart.Name
            mSubTypeName = mPart.SubTypeName
            If mName = "" Then mName = mSubTypeName

            Me.lastControl = lastControl

            GetControls(part, DataFlow.Render)
            GetControls(part, DataFlow.Capture)
        End Sub

        Public Sub New(parent As CMixer, mixer As MMDevice)
            mParent = parent
            mPart = Nothing
            mName = "Master Controls"
            mSubTypeName = "MasterControls"

            mControls.Add(New CControl(mixer))
        End Sub

        Public Sub New(parent As CMixer, session As AudioSessionControl2)
            mParent = parent
            mPart = Nothing
            mName = "Session"
            mSubTypeName = "Session"

            mControls.Add(New CControl(Me, session))
        End Sub

        Private Sub GetControls(part As Part, flow As DataFlow)
            If lastControl IsNot Nothing AndAlso lastControl.IsBasedOnPart(part) Then Exit Sub
            'Debug.WriteLine(mParent.Name + " => " + Part.Name() + " => " + Part.SubTypeName)

            If part.PartType = PartType.Subunit AndAlso part.SubType <> KsNodeType.Sum Then mControls.Add(New CControl(Me, part))

            Dim parts As PartsList
            Select Case flow
                Case DataFlow.Render : parts = part.EnumPartsIncoming
                Case DataFlow.Capture : parts = part.EnumPartsOutgoing
                Case Else : Throw New NotImplementedException("CLine->GetControls for " + flow.ToString() + " is not implemented")
            End Select

            If parts IsNot Nothing Then
                For i As Integer = 0 To parts.Count - 1
                    Dim subPart As Part = parts.Part(i)

                    'Debug.WriteLine("    " + subPart.Name() + " => " + subPart.SubTypeName)

                    If subPart.PartType = PartType.Connector Then Continue For
                    GetControls(subPart, flow)
                Next
            End If
        End Sub

        ''' <summary>
        ''' Returns a reference to the parent <see cref="CMixer">CMixer</see> object
        ''' </summary>
        Public ReadOnly Property Parent As CMixer
            Get
                Return mParent
            End Get
        End Property

        ''' <summary>
        ''' Returns the name of the current line
        ''' </summary>
        Public ReadOnly Property Name As String
            Get
                Return mName
            End Get
        End Property

        ''' <summary>
        ''' Returns a string representing the type of line.
        ''' </summary>
        Public ReadOnly Property LineType As String
            Get
                Return mSubTypeName
            End Get
        End Property

        ''' <summary>
        ''' Returns a list of <see cref="CControl">CControl</see> objects (controls) available for the current line.
        ''' </summary>
        Public ReadOnly Property Controls As List(Of CControl)
            Get
                Return mControls
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return mName
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                For Each c As CControl In mControls
                    c.Dispose()
                Next

                If mPart IsNot Nothing Then
                    mPart.Dispose()
                    mParent = Nothing
                End If
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overrides Sub Finalize()
            Dispose(False)
            MyBase.Finalize()
        End Sub
#End Region
    End Class
End Class
