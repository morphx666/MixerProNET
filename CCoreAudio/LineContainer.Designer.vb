<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LineContainer
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.VfControl = New NMixerProNET.VolumeFader()
        CType(Me.VfControl, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'VfControl
        '
        Me.VfControl.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.VfControl.AutoSize = False
        Me.VfControl.BackColor = System.Drawing.Color.Transparent
        Me.VfControl.CoreAudioControl = Nothing
        Me.VfControl.IntegralChanges = True
        Me.VfControl.Location = New System.Drawing.Point(0, 0)
        Me.VfControl.Margin = New System.Windows.Forms.Padding(0)
        Me.VfControl.Name = "VfControl"
        Me.VfControl.PeakLevel = 0
        Me.VfControl.Size = New System.Drawing.Size(420, 41)
        Me.VfControl.TabIndex = 0
        Me.VfControl.TickStyle = System.Windows.Forms.TickStyle.TopLeft
        '
        'LineContainer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.VfControl)
        Me.Name = "LineContainer"
        Me.Size = New System.Drawing.Size(420, 41)
        CType(Me.VfControl, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents VfControl As NMixerProNET.VolumeFader

End Class
