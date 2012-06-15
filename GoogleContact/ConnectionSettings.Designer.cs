namespace GoogleContact
{
    partial class ConnectionSettings
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbBoth = new System.Windows.Forms.RadioButton();
            this.rbGo2Ou = new System.Windows.Forms.RadioButton();
            this.rbOu2Go = new System.Windows.Forms.RadioButton();
            this.chFirstSync = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbBothSync = new System.Windows.Forms.RadioButton();
            this.rbGo2OuSync = new System.Windows.Forms.RadioButton();
            this.rbOut2GoSync = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ddLogLevel = new System.Windows.Forms.ComboBox();
            this.btnSelectDirectory = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txDirectory = new System.Windows.Forms.TextBox();
            this.fbSelectLogDirectory = new System.Windows.Forms.FolderBrowserDialog();
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 214);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(93, 214);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbBoth);
            this.groupBox1.Controls.Add(this.rbGo2Ou);
            this.groupBox1.Controls.Add(this.rbOu2Go);
            this.groupBox1.Controls.Add(this.chFirstSync);
            this.groupBox1.Location = new System.Drawing.Point(12, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 118);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "First synchronize";
            // 
            // rbBoth
            // 
            this.rbBoth.AutoSize = true;
            this.rbBoth.Enabled = false;
            this.rbBoth.Location = new System.Drawing.Point(7, 90);
            this.rbBoth.Name = "rbBoth";
            this.rbBoth.Size = new System.Drawing.Size(69, 17);
            this.rbBoth.TabIndex = 3;
            this.rbBoth.Text = "Both side";
            this.rbBoth.UseVisualStyleBackColor = true;
            // 
            // rbGo2Ou
            // 
            this.rbGo2Ou.AutoSize = true;
            this.rbGo2Ou.Location = new System.Drawing.Point(7, 67);
            this.rbGo2Ou.Name = "rbGo2Ou";
            this.rbGo2Ou.Size = new System.Drawing.Size(137, 17);
            this.rbGo2Ou.TabIndex = 2;
            this.rbGo2Ou.TabStop = true;
            this.rbGo2Ou.Text = "From Google to Outlook";
            this.rbGo2Ou.UseVisualStyleBackColor = true;
            // 
            // rbOu2Go
            // 
            this.rbOu2Go.AutoSize = true;
            this.rbOu2Go.Location = new System.Drawing.Point(7, 43);
            this.rbOu2Go.Name = "rbOu2Go";
            this.rbOu2Go.Size = new System.Drawing.Size(137, 17);
            this.rbOu2Go.TabIndex = 1;
            this.rbOu2Go.TabStop = true;
            this.rbOu2Go.Text = "From Outlook to Google";
            this.rbOu2Go.UseVisualStyleBackColor = true;
            // 
            // chFirstSync
            // 
            this.chFirstSync.AutoSize = true;
            this.chFirstSync.Location = new System.Drawing.Point(7, 19);
            this.chFirstSync.Name = "chFirstSync";
            this.chFirstSync.Size = new System.Drawing.Size(115, 17);
            this.chFirstSync.TabIndex = 0;
            this.chFirstSync.Text = "Is First synchronize";
            this.chFirstSync.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbBothSync);
            this.groupBox2.Controls.Add(this.rbGo2OuSync);
            this.groupBox2.Controls.Add(this.rbOut2GoSync);
            this.groupBox2.Location = new System.Drawing.Point(218, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(186, 118);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Standard synchronize setup";
            // 
            // rbBothSync
            // 
            this.rbBothSync.AutoSize = true;
            this.rbBothSync.Location = new System.Drawing.Point(7, 67);
            this.rbBothSync.Name = "rbBothSync";
            this.rbBothSync.Size = new System.Drawing.Size(114, 17);
            this.rbBothSync.TabIndex = 2;
            this.rbBothSync.TabStop = true;
            this.rbBothSync.Text = "Outlook <> Google";
            this.rbBothSync.UseVisualStyleBackColor = true;
            // 
            // rbGo2OuSync
            // 
            this.rbGo2OuSync.AutoSize = true;
            this.rbGo2OuSync.Location = new System.Drawing.Point(7, 43);
            this.rbGo2OuSync.Name = "rbGo2OuSync";
            this.rbGo2OuSync.Size = new System.Drawing.Size(135, 17);
            this.rbGo2OuSync.TabIndex = 1;
            this.rbGo2OuSync.TabStop = true;
            this.rbGo2OuSync.Text = "Only Google to Outlook";
            this.rbGo2OuSync.UseVisualStyleBackColor = true;
            // 
            // rbOut2GoSync
            // 
            this.rbOut2GoSync.AutoSize = true;
            this.rbOut2GoSync.Location = new System.Drawing.Point(7, 20);
            this.rbOut2GoSync.Name = "rbOut2GoSync";
            this.rbOut2GoSync.Size = new System.Drawing.Size(135, 17);
            this.rbOut2GoSync.TabIndex = 0;
            this.rbOut2GoSync.TabStop = true;
            this.rbOut2GoSync.Text = "Only Outlook to Google";
            this.rbOut2GoSync.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.ddLogLevel);
            this.groupBox3.Controls.Add(this.btnSelectDirectory);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.txDirectory);
            this.groupBox3.Location = new System.Drawing.Point(12, 134);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(392, 74);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Logging";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Log level";
            // 
            // ddLogLevel
            // 
            this.ddLogLevel.FormattingEnabled = true;
            this.ddLogLevel.Items.AddRange(new object[] {
            "None",
            "Fatal",
            "Error",
            "Warning",
            "Debug"});
            this.ddLogLevel.Location = new System.Drawing.Point(100, 42);
            this.ddLogLevel.Name = "ddLogLevel";
            this.ddLogLevel.Size = new System.Drawing.Size(205, 21);
            this.ddLogLevel.TabIndex = 3;
            // 
            // btnSelectDirectory
            // 
            this.btnSelectDirectory.Location = new System.Drawing.Point(311, 14);
            this.btnSelectDirectory.Name = "btnSelectDirectory";
            this.btnSelectDirectory.Size = new System.Drawing.Size(75, 23);
            this.btnSelectDirectory.TabIndex = 2;
            this.btnSelectDirectory.Text = "...";
            this.btnSelectDirectory.UseVisualStyleBackColor = true;
            this.btnSelectDirectory.Click += new System.EventHandler(this.btnSelectDirectory_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Logging directory";
            // 
            // txDirectory
            // 
            this.txDirectory.Location = new System.Drawing.Point(100, 16);
            this.txDirectory.Name = "txDirectory";
            this.txDirectory.ReadOnly = true;
            this.txDirectory.Size = new System.Drawing.Size(205, 20);
            this.txDirectory.TabIndex = 0;
            // 
            // fbSelectLogDirectory
            // 
            this.fbSelectLogDirectory.Description = "Select directory for log files";
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Location = new System.Drawing.Point(272, 213);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(131, 23);
            this.btnAdvanced.TabIndex = 11;
            this.btnAdvanced.Text = "Advanced settings ...";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
            // 
            // ConnectionSettings
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(428, 246);
            this.Controls.Add(this.btnAdvanced);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(434, 274);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(434, 274);
            this.Name = "ConnectionSettings";
            this.ShowInTaskbar = false;
            this.Text = "Google Connection Settings";
            this.Load += new System.EventHandler(this.ConnectionSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbGo2Ou;
        private System.Windows.Forms.RadioButton rbOu2Go;
        private System.Windows.Forms.CheckBox chFirstSync;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbBothSync;
        private System.Windows.Forms.RadioButton rbGo2OuSync;
        private System.Windows.Forms.RadioButton rbOut2GoSync;
        private System.Windows.Forms.RadioButton rbBoth;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox ddLogLevel;
        private System.Windows.Forms.Button btnSelectDirectory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txDirectory;
        private System.Windows.Forms.FolderBrowserDialog fbSelectLogDirectory;
        private System.Windows.Forms.Button btnAdvanced;
    }
}