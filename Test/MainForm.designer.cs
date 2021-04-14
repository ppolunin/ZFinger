namespace TestZFinger
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            if (disposing)
                storage.Dispose();
        }

        #region Windows 

        private void InitializeComponent()
        {
            this.bnOpen = new System.Windows.Forms.Button();
            this.bnClose = new System.Windows.Forms.Button();
            this.picFPImg = new System.Windows.Forms.PictureBox();
            this.btnOutput = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnImport = new System.Windows.Forms.Button();
            this.textRes = new System.Windows.Forms.RichTextBox();
            this.cbSessionType = new System.Windows.Forms.ComboBox();
            this.cbxCloseSession = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picFPImg)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnOpen
            // 
            this.bnOpen.Location = new System.Drawing.Point(14, 12);
            this.bnOpen.Name = "bnOpen";
            this.bnOpen.Size = new System.Drawing.Size(75, 25);
            this.bnOpen.TabIndex = 1;
            this.bnOpen.Text = "Open";
            this.bnOpen.UseVisualStyleBackColor = true;
            this.bnOpen.Click += new System.EventHandler(this.bnOpen_Click);
            // 
            // bnClose
            // 
            this.bnClose.Enabled = false;
            this.bnClose.Location = new System.Drawing.Point(14, 43);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(75, 25);
            this.bnClose.TabIndex = 5;
            this.bnClose.Text = "Close";
            this.bnClose.UseVisualStyleBackColor = true;
            this.bnClose.Click += new System.EventHandler(this.bnClose_Click);
            // 
            // picFPImg
            // 
            this.picFPImg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picFPImg.BackColor = System.Drawing.SystemColors.Info;
            this.picFPImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.picFPImg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picFPImg.Location = new System.Drawing.Point(6, 7);
            this.picFPImg.Name = "picFPImg";
            this.picFPImg.Size = new System.Drawing.Size(287, 291);
            this.picFPImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picFPImg.TabIndex = 8;
            this.picFPImg.TabStop = false;
            // 
            // btnOutput
            // 
            this.btnOutput.Enabled = false;
            this.btnOutput.Location = new System.Drawing.Point(162, 304);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(131, 25);
            this.btnOutput.TabIndex = 12;
            this.btnOutput.Text = "Output BMP";
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btCaptureBmp_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(301, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(307, 364);
            this.tabControl1.TabIndex = 16;
            this.tabControl1.Tag = "S";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnImport);
            this.tabPage1.Controls.Add(this.picFPImg);
            this.tabPage1.Controls.Add(this.btnOutput);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(299, 338);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Image Convert(Charge)";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnImport
            // 
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(6, 304);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(131, 25);
            this.btnImport.TabIndex = 13;
            this.btnImport.Text = "Import BMP";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // textRes
            // 
            this.textRes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textRes.Location = new System.Drawing.Point(14, 74);
            this.textRes.Name = "textRes";
            this.textRes.Size = new System.Drawing.Size(281, 303);
            this.textRes.TabIndex = 17;
            this.textRes.Text = "";
            // 
            // cbSessionType
            // 
            this.cbSessionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSessionType.FormattingEnabled = true;
            this.cbSessionType.Location = new System.Drawing.Point(95, 14);
            this.cbSessionType.Name = "cbSessionType";
            this.cbSessionType.Size = new System.Drawing.Size(200, 21);
            this.cbSessionType.TabIndex = 18;
            // 
            // cbxCloseSession
            // 
            this.cbxCloseSession.AutoSize = true;
            this.cbxCloseSession.Location = new System.Drawing.Point(95, 48);
            this.cbxCloseSession.Name = "cbxCloseSession";
            this.cbxCloseSession.Size = new System.Drawing.Size(114, 17);
            this.cbxCloseSession.TabIndex = 19;
            this.cbxCloseSession.Text = "Auto close session";
            this.cbxCloseSession.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 389);
            this.Controls.Add(this.cbxCloseSession);
            this.Controls.Add(this.cbSessionType);
            this.Controls.Add(this.textRes);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.bnClose);
            this.Controls.Add(this.bnOpen);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "ZFinger Demo";
            ((System.ComponentModel.ISupportInitialize)(this.picFPImg)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bnOpen;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.PictureBox picFPImg;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.RichTextBox textRes;
        private System.Windows.Forms.ComboBox cbSessionType;
        private System.Windows.Forms.CheckBox cbxCloseSession;
    }
}

