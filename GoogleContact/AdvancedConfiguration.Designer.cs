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
            this.chUseCacheOutlook = new System.Windows.Forms.CheckBox();
            this.chUseCacheGoogle = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chUseCacheOutlook
            // 
            this.chUseCacheOutlook.AutoSize = true;
            this.chUseCacheOutlook.Location = new System.Drawing.Point(13, 13);
            this.chUseCacheOutlook.Name = "chUseCacheOutlook";
            this.chUseCacheOutlook.Size = new System.Drawing.Size(206, 17);
            this.chUseCacheOutlook.TabIndex = 0;
            this.chUseCacheOutlook.Text = "Use cache system for Outlook records";
            this.chUseCacheOutlook.UseVisualStyleBackColor = true;
            // 
            // chUseCacheGoogle
            // 
            this.chUseCacheGoogle.AutoSize = true;
            this.chUseCacheGoogle.Location = new System.Drawing.Point(13, 37);
            this.chUseCacheGoogle.Name = "chUseCacheGoogle";
            this.chUseCacheGoogle.Size = new System.Drawing.Size(203, 17);
            this.chUseCacheGoogle.TabIndex = 1;
            this.chUseCacheGoogle.Text = "Use cache system for Google records";
            this.chUseCacheGoogle.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(141, 60);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // AdvancedConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 101);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.chUseCacheGoogle);
            this.Controls.Add(this.chUseCacheOutlook);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvancedConfiguration";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Advanced Configuration";
            this.Load += new System.EventHandler(this.AdvancedConfiguration_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chUseCacheOutlook;
        private System.Windows.Forms.CheckBox chUseCacheGoogle;
        private System.Windows.Forms.Button btnClose;
    }
}