namespace GoogleContact
{
    partial class AdvancedConfiguration
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedConfiguration));
            this.chUseCacheOutlook = new System.Windows.Forms.CheckBox();
            this.chUseCacheGoogle = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnClearOutlook = new System.Windows.Forms.Button();
            this.btnClearGoogle = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOutlokSync = new System.Windows.Forms.Button();
            this.btnGoogleSync = new System.Windows.Forms.Button();
            this.btnClose1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.nUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.pBar = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // chUseCacheOutlook
            // 
            this.chUseCacheOutlook.AutoSize = true;
            this.chUseCacheOutlook.Location = new System.Drawing.Point(6, 62);
            this.chUseCacheOutlook.Name = "chUseCacheOutlook";
            this.chUseCacheOutlook.Size = new System.Drawing.Size(206, 17);
            this.chUseCacheOutlook.TabIndex = 0;
            this.chUseCacheOutlook.Text = "Use cache system for Outlook records";
            this.chUseCacheOutlook.UseVisualStyleBackColor = true;
            // 
            // chUseCacheGoogle
            // 
            this.chUseCacheGoogle.AutoSize = true;
            this.chUseCacheGoogle.Location = new System.Drawing.Point(6, 85);
            this.chUseCacheGoogle.Name = "chUseCacheGoogle";
            this.chUseCacheGoogle.Size = new System.Drawing.Size(203, 17);
            this.chUseCacheGoogle.TabIndex = 1;
            this.chUseCacheGoogle.Text = "Use cache system for Google records";
            this.chUseCacheGoogle.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(8, 196);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(432, 253);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.nUpDown);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.btnClearGoogle);
            this.tabPage1.Controls.Add(this.btnClearOutlook);
            this.tabPage1.Controls.Add(this.chUseCacheOutlook);
            this.tabPage1.Controls.Add(this.btnClose);
            this.tabPage1.Controls.Add(this.chUseCacheGoogle);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(424, 227);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Cache";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lblProgress);
            this.tabPage2.Controls.Add(this.pBar);
            this.tabPage2.Controls.Add(this.btnClose1);
            this.tabPage2.Controls.Add(this.btnGoogleSync);
            this.tabPage2.Controls.Add(this.btnOutlokSync);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(424, 227);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Synchronization";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnClearOutlook
            // 
            this.btnClearOutlook.Location = new System.Drawing.Point(6, 138);
            this.btnClearOutlook.Name = "btnClearOutlook";
            this.btnClearOutlook.Size = new System.Drawing.Size(187, 23);
            this.btnClearOutlook.TabIndex = 3;
            this.btnClearOutlook.Text = "Clear Outlook cache";
            this.btnClearOutlook.UseVisualStyleBackColor = true;
            this.btnClearOutlook.Click += new System.EventHandler(this.btnClearOutlook_Click);
            // 
            // btnClearGoogle
            // 
            this.btnClearGoogle.Location = new System.Drawing.Point(6, 167);
            this.btnClearGoogle.Name = "btnClearGoogle";
            this.btnClearGoogle.Size = new System.Drawing.Size(187, 23);
            this.btnClearGoogle.TabIndex = 4;
            this.btnClearGoogle.Text = "Clear Google Cache";
            this.btnClearGoogle.UseVisualStyleBackColor = true;
            this.btnClearGoogle.Click += new System.EventHandler(this.btnClearGoogle_Click);
            // 
            // label1
            // 
            this.label1.CausesValidation = false;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(418, 68);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            this.label1.UseMnemonic = false;
            // 
            // btnOutlokSync
            // 
            this.btnOutlokSync.Location = new System.Drawing.Point(9, 75);
            this.btnOutlokSync.Name = "btnOutlokSync";
            this.btnOutlokSync.Size = new System.Drawing.Size(174, 23);
            this.btnOutlokSync.TabIndex = 1;
            this.btnOutlokSync.Text = "Clear Outlook sync keys";
            this.btnOutlokSync.UseVisualStyleBackColor = true;
            this.btnOutlokSync.Click += new System.EventHandler(this.btnOutlokSync_Click);
            // 
            // btnGoogleSync
            // 
            this.btnGoogleSync.Location = new System.Drawing.Point(9, 105);
            this.btnGoogleSync.Name = "btnGoogleSync";
            this.btnGoogleSync.Size = new System.Drawing.Size(174, 23);
            this.btnGoogleSync.TabIndex = 2;
            this.btnGoogleSync.Text = "Clear Google sync keys";
            this.btnGoogleSync.UseVisualStyleBackColor = true;
            this.btnGoogleSync.Click += new System.EventHandler(this.btnGoogleSync_Click);
            // 
            // btnClose1
            // 
            this.btnClose1.Location = new System.Drawing.Point(9, 196);
            this.btnClose1.Name = "btnClose1";
            this.btnClose1.Size = new System.Drawing.Size(75, 23);
            this.btnClose1.TabIndex = 3;
            this.btnClose1.Text = "Close";
            this.btnClose1.UseVisualStyleBackColor = true;
            this.btnClose1.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(418, 56);
            this.label2.TabIndex = 5;
            this.label2.Text = "Use cache mode, when you want improves speed for reading synchronized data. If yo" +
    "u had any problem when synchronize contact, try disabling synchronization for bo" +
    "th sources and clearing Cache.\r\n";
            // 
            // nUpDown
            // 
            this.nUpDown.Location = new System.Drawing.Point(7, 109);
            this.nUpDown.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nUpDown.Name = "nUpDown";
            this.nUpDown.Size = new System.Drawing.Size(48, 20);
            this.nUpDown.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(61, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Time to Live for cache data";
            // 
            // pBar
            // 
            this.pBar.Location = new System.Drawing.Point(9, 151);
            this.pBar.Name = "pBar";
            this.pBar.Size = new System.Drawing.Size(407, 23);
            this.pBar.TabIndex = 4;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(9, 135);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(159, 13);
            this.lblProgress.TabIndex = 5;
            this.lblProgress.Text = "Synchronize keys clear progress";
            // 
            // AdvancedConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 253);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvancedConfiguration";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Advanced Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AdvancedConfiguration_FormClosing);
            this.Load += new System.EventHandler(this.AdvancedConfiguration_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chUseCacheOutlook;
        private System.Windows.Forms.CheckBox chUseCacheGoogle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnClearGoogle;
        private System.Windows.Forms.Button btnClearOutlook;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnGoogleSync;
        private System.Windows.Forms.Button btnOutlokSync;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClose1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nUpDown;
        private System.Windows.Forms.ProgressBar pBar;
        private System.Windows.Forms.Label lblProgress;
    }
}