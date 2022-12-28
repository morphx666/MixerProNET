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
        Me.LvSessions = New System.Windows.Forms.ListView()
        Me.chName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.CbSessionMute = New System.Windows.Forms.CheckBox()
        Me.VfSessionVolume = New NMixerProNET.LineContainer()
        Me.SuspendLayout()
        '
        'LvSessions
        '
        Me.LvSessions.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LvSessions.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chName})
        Me.LvSessions.FullRowSelect = True
        Me.LvSessions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.LvSessions.HideSelection = False
        Me.LvSessions.Location = New System.Drawing.Point(14, 14)
        Me.LvSessions.Name = "LvSessions"
        Me.LvSessions.Size = New System.Drawing.Size(425, 320)
        Me.LvSessions.TabIndex = 0
        Me.LvSessions.UseCompatibleStateImageBehavior = False
        Me.LvSessions.View = System.Windows.Forms.View.Details
        '
        'chName
        '
        Me.chName.Text = "Name"
        '
        'CbSessionMute
        '
        Me.CbSessionMute.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CbSessionMute.AutoSize = True
        Me.CbSessionMute.Location = New System.Drawing.Point(14, 396)
        Me.CbSessionMute.Name = "CbSessionMute"
        Me.CbSessionMute.Size = New System.Drawing.Size(54, 19)
        Me.CbSessionMute.TabIndex = 3
        Me.CbSessionMute.Text = "Mute"
        Me.CbSessionMute.UseVisualStyleBackColor = True
        '
        'VfSessionVolume
        '
        Me.VfSessionVolume.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.VfSessionVolume.CoreAudioControl = Nothing
        Me.VfSessionVolume.Location = New System.Drawing.Point(14, 340)
        Me.VfSessionVolume.Name = "VfSessionVolume"
        Me.VfSessionVolume.Size = New System.Drawing.Size(425, 47)
        Me.VfSessionVolume.TabIndex = 4
        Me.VfSessionVolume.Value = 0
        '
        'FormMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.ClientSize = New System.Drawing.Size(454, 427)
        Me.Controls.Add(Me.VfSessionVolume)
        Me.Controls.Add(Me.CbSessionMute)
        Me.Controls.Add(Me.LvSessions)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "FormMain"
        Me.Text = "Enumerate Sessions"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents LvSessions As System.Windows.Forms.ListView
    Friend WithEvents chName As System.Windows.Forms.ColumnHeader
    Friend WithEvents CbSessionMute As System.Windows.Forms.CheckBox
    Friend WithEvents VfSessionVolume As NMixerProNET.LineContainer

End Class
