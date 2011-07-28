namespace GoogleContact
{
    partial class SyncInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SyncInfo));
            this.label1 = new System.Windows.Forms.Label();
            this.lbContacts = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbStep = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbWorkOn = new System.Windows.Forms.Label();
            this.pbWork = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.pbSum = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lbContacts
            // 
            resources.ApplyResources(this.lbContacts, "lbContacts");
            this.lbContacts.Name = "lbContacts";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lbStep
            // 
            resources.ApplyResources(this.lbStep, "lbStep");
            this.lbStep.Name = "lbStep";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // lbWorkOn
            // 
            resources.ApplyResources(this.lbWorkOn, "lbWorkOn");
            this.lbWorkOn.Name = "lbWorkOn";
            // 
            // pbWork
            // 
            resources.ApplyResources(this.pbWork, "pbWork");
            this.pbWork.Name = "pbWork";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // pbSum
            // 
            resources.ApplyResources(this.pbSum, "pbSum");
            this.pbSum.Name = "pbSum";
            // 
            // SyncInfo
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.pbSum);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pbWork);
            this.Controls.Add(this.lbWorkOn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbStep);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbContacts);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SyncInfo";
            this.Load += new System.EventHandler(this.SyncInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbContacts;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbStep;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbWorkOn;
        private System.Windows.Forms.ProgressBar pbWork;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar pbSum;
    }
}