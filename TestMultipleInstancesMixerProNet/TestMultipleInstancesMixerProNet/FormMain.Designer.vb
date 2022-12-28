<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.FlowLayoutPanelMixers = New System.Windows.Forms.FlowLayoutPanel()
        Me.ButtonCreate = New System.Windows.Forms.Button()
        Me.TextBoxCount = New System.Windows.Forms.TextBox()
        Me.ButtonDestroy = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'FlowLayoutPanelMixers
        '
        Me.FlowLayoutPanelMixers.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanelMixers.AutoScroll = True
        Me.FlowLayoutPanelMixers.Location = New System.Drawing.Point(12, 12)
        Me.FlowLayoutPanelMixers.Name = "FlowLayoutPanelMixers"
        Me.FlowLayoutPanelMixers.Size = New System.Drawing.Size(733, 454)
        Me.FlowLayoutPanelMixers.TabIndex = 0
        '
        'ButtonCreate
        '
        Me.ButtonCreate.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ButtonCreate.Location = New System.Drawing.Point(62, 472)
        Me.ButtonCreate.Name = "ButtonCreate"
        Me.ButtonCreate.Size = New System.Drawing.Size(101, 35)
        Me.ButtonCreate.TabIndex = 1
        Me.ButtonCreate.Text = "Create"
        Me.ButtonCreate.UseVisualStyleBackColor = True
        '
        'TextBoxCount
        '
        Me.TextBoxCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.TextBoxCount.Location = New System.Drawing.Point(12, 479)
        Me.TextBoxCount.Name = "TextBoxCount"
        Me.TextBoxCount.Size = New System.Drawing.Size(44, 23)
        Me.TextBoxCount.TabIndex = 2
        Me.TextBoxCount.Text = "10"
        Me.TextBoxCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'ButtonDestroy
        '
        Me.ButtonDestroy.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ButtonDestroy.Location = New System.Drawing.Point(169, 472)
        Me.ButtonDestroy.Name = "ButtonDestroy"
        Me.ButtonDestroy.Size = New System.Drawing.Size(101, 35)
        Me.ButtonDestroy.TabIndex = 1
        Me.ButtonDestroy.Text = "Destroy All"
        Me.ButtonDestroy.UseVisualStyleBackColor = True
        '
        'FormMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(759, 519)
        Me.Controls.Add(Me.TextBoxCount)
        Me.Controls.Add(Me.ButtonDestroy)
        Me.Controls.Add(Me.ButtonCreate)
        Me.Controls.Add(Me.FlowLayoutPanelMixers)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "FormMain"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents FlowLayoutPanelMixers As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents ButtonCreate As System.Windows.Forms.Button
    Friend WithEvents TextBoxCount As System.Windows.Forms.TextBox
    Friend WithEvents ButtonDestroy As System.Windows.Forms.Button

End Class
