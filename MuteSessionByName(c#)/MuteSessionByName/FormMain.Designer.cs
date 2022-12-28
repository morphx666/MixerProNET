namespace MuteSessionByName {
    partial class FormMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.TextBoxSessionName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonToggleMute = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ListBoxSessions = new System.Windows.Forms.ListBox();
            this.ButtonRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TextBoxSessionName
            // 
            this.TextBoxSessionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxSessionName.Location = new System.Drawing.Point(15, 27);
            this.TextBoxSessionName.Name = "TextBoxSessionName";
            this.TextBoxSessionName.Size = new System.Drawing.Size(399, 23);
            this.TextBoxSessionName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Session Name";
            // 
            // ButtonToggleMute
            // 
            this.ButtonToggleMute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonToggleMute.Location = new System.Drawing.Point(420, 27);
            this.ButtonToggleMute.Name = "ButtonToggleMute";
            this.ButtonToggleMute.Size = new System.Drawing.Size(102, 23);
            this.ButtonToggleMute.TabIndex = 2;
            this.ButtonToggleMute.Text = "Toggle Mute";
            this.ButtonToggleMute.UseVisualStyleBackColor = true;
            this.ButtonToggleMute.Click += new System.EventHandler(this.ButtonToggleMute_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Available Sessions";
            // 
            // ListBoxSessions
            // 
            this.ListBoxSessions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListBoxSessions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ListBoxSessions.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListBoxSessions.FormattingEnabled = true;
            this.ListBoxSessions.ItemHeight = 18;
            this.ListBoxSessions.Location = new System.Drawing.Point(15, 94);
            this.ListBoxSessions.Name = "ListBoxSessions";
            this.ListBoxSessions.Size = new System.Drawing.Size(399, 200);
            this.ListBoxSessions.TabIndex = 3;
            // 
            // ButtonRefresh
            // 
            this.ButtonRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonRefresh.Location = new System.Drawing.Point(420, 94);
            this.ButtonRefresh.Name = "ButtonRefresh";
            this.ButtonRefresh.Size = new System.Drawing.Size(102, 23);
            this.ButtonRefresh.TabIndex = 4;
            this.ButtonRefresh.Text = "Refresh";
            this.ButtonRefresh.UseVisualStyleBackColor = true;
            this.ButtonRefresh.Click += new System.EventHandler(this.ButtonRefresh_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 299);
            this.Controls.Add(this.ButtonRefresh);
            this.Controls.Add(this.ListBoxSessions);
            this.Controls.Add(this.ButtonToggleMute);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBoxSessionName);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mute Session By Name";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxSessionName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonToggleMute;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox ListBoxSessions;
        private System.Windows.Forms.Button ButtonRefresh;
    }
}

