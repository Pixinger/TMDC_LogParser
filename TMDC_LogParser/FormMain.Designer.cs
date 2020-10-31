namespace TMDC_LogParser
{
    partial class FormMain
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstMissions = new System.Windows.Forms.ListBox();
            this.rtxtMission = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstMissions
            // 
            this.lstMissions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstMissions.FormattingEnabled = true;
            this.lstMissions.IntegralHeight = false;
            this.lstMissions.Location = new System.Drawing.Point(12, 27);
            this.lstMissions.Name = "lstMissions";
            this.lstMissions.Size = new System.Drawing.Size(324, 469);
            this.lstMissions.TabIndex = 0;
            this.lstMissions.SelectedIndexChanged += new System.EventHandler(this.lstMissions_SelectedIndexChanged);
            // 
            // rtxtMission
            // 
            this.rtxtMission.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtMission.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtxtMission.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtMission.Location = new System.Drawing.Point(342, 27);
            this.rtxtMission.Name = "rtxtMission";
            this.rtxtMission.Size = new System.Drawing.Size(1062, 469);
            this.rtxtMission.TabIndex = 1;
            this.rtxtMission.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menOpen});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1416, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menOpen
            // 
            this.menOpen.Name = "menOpen";
            this.menOpen.Size = new System.Drawing.Size(48, 20);
            this.menOpen.Text = "Open";
            this.menOpen.Click += new System.EventHandler(this.menOpen_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1416, 508);
            this.Controls.Add(this.rtxtMission);
            this.Controls.Add(this.lstMissions);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "TMDC LogParser";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstMissions;
        private System.Windows.Forms.RichTextBox rtxtMission;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menOpen;
    }
}

