namespace GoldenTubes
{
    partial class TargetingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TargetingForm));
            this.TargetButton = new System.Windows.Forms.Button();
            this.TargetName = new System.Windows.Forms.TextBox();
            this.EstimateTime = new System.Windows.Forms.Label();
            this.CurrentTime = new System.Windows.Forms.Label();
            this.UserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.StopButton = new System.Windows.Forms.Button();
            this.ManualRegion = new System.Windows.Forms.Label();
            this.TriggerTime = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TargetButton
            // 
            this.TargetButton.Location = new System.Drawing.Point(12, 50);
            this.TargetButton.Name = "TargetButton";
            this.TargetButton.Size = new System.Drawing.Size(75, 23);
            this.TargetButton.TabIndex = 0;
            this.TargetButton.Text = "Lock On";
            this.TargetButton.UseVisualStyleBackColor = true;
            this.TargetButton.Click += new System.EventHandler(this.TargetButton_Click);
            // 
            // TargetName
            // 
            this.TargetName.Location = new System.Drawing.Point(93, 50);
            this.TargetName.Name = "TargetName";
            this.TargetName.Size = new System.Drawing.Size(179, 20);
            this.TargetName.TabIndex = 1;
            // 
            // EstimateTime
            // 
            this.EstimateTime.AutoSize = true;
            this.EstimateTime.Location = new System.Drawing.Point(12, 34);
            this.EstimateTime.Name = "EstimateTime";
            this.EstimateTime.Size = new System.Drawing.Size(49, 13);
            this.EstimateTime.TabIndex = 2;
            this.EstimateTime.Text = "00:00:00";
            // 
            // CurrentTime
            // 
            this.CurrentTime.AutoSize = true;
            this.CurrentTime.Location = new System.Drawing.Point(223, 34);
            this.CurrentTime.Name = "CurrentTime";
            this.CurrentTime.Size = new System.Drawing.Size(49, 13);
            this.CurrentTime.TabIndex = 3;
            this.CurrentTime.Text = "00:00:00";
            // 
            // UserName
            // 
            this.UserName.Location = new System.Drawing.Point(104, 6);
            this.UserName.Name = "UserName";
            this.UserName.Size = new System.Drawing.Size(168, 20);
            this.UserName.TabIndex = 4;
            this.UserName.TextChanged += new System.EventHandler(this.UserName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Arwing Callsign";
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(12, 111);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(260, 23);
            this.StopButton.TabIndex = 6;
            this.StopButton.Text = "Disengage Engines";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // ManualRegion
            // 
            this.ManualRegion.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ManualRegion.AutoSize = true;
            this.ManualRegion.Location = new System.Drawing.Point(59, 88);
            this.ManualRegion.Name = "ManualRegion";
            this.ManualRegion.Size = new System.Drawing.Size(115, 13);
            this.ManualRegion.TabIndex = 7;
            this.ManualRegion.Text = "Manual Trigger Region";
            this.ManualRegion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TriggerTime
            // 
            this.TriggerTime.Location = new System.Drawing.Point(12, 85);
            this.TriggerTime.Name = "TriggerTime";
            this.TriggerTime.Size = new System.Drawing.Size(41, 20);
            this.TriggerTime.TabIndex = 8;
            this.TriggerTime.Text = "0";
            // 
            // TargetingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::GoldenTubes.Properties.Resources._20XXBackground;
            this.ClientSize = new System.Drawing.Size(284, 137);
            this.Controls.Add(this.TriggerTime);
            this.Controls.Add(this.ManualRegion);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UserName);
            this.Controls.Add(this.CurrentTime);
            this.Controls.Add(this.EstimateTime);
            this.Controls.Add(this.TargetName);
            this.Controls.Add(this.TargetButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TargetingForm";
            this.Text = "ADR-20XX";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TargetingForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button TargetButton;
        private System.Windows.Forms.TextBox TargetName;
        private System.Windows.Forms.Label EstimateTime;
        private System.Windows.Forms.Label CurrentTime;
        private System.Windows.Forms.TextBox UserName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Label ManualRegion;
        private System.Windows.Forms.TextBox TriggerTime;
    }
}