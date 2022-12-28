Imports System.ComponentModel

<Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
Friend Class MsgHnd
    Inherits Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <DebuggerStepThrough()> Private Sub InitializeComponent()
        '
        'MsgHnd
        '
        Me.AutoScaleBaseSize = New Size(5, 13)
        Me.ClientSize = New Size(292, 273)
        Me.Name = "MsgHnd"
        Me.Text = "MsgHnd"

    End Sub

#End Region

    Friend mxp As CMixerPro

    Protected Overrides Sub WndProc(ByRef m As Message)
        MyBase.WndProc(m)
        mxp.wProc(m)
    End Sub
End Class
