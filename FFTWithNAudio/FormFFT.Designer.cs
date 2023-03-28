namespace FFTWithNAudio
{
    partial class FormFFT
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstWavFiles = new System.Windows.Forms.ListBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnOpenWavFile = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.gbFrequency = new System.Windows.Forms.GroupBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.cbFFTSize = new System.Windows.Forms.ComboBox();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbMaxY = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.trackWavPcmPos = new System.Windows.Forms.TrackBar();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackWavPcmPos)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(333, 640);
            this.panel1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstWavFiles);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(333, 583);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "文件";
            // 
            // lstWavFiles
            // 
            this.lstWavFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstWavFiles.FormattingEnabled = true;
            this.lstWavFiles.ItemHeight = 20;
            this.lstWavFiles.Location = new System.Drawing.Point(3, 23);
            this.lstWavFiles.Name = "lstWavFiles";
            this.lstWavFiles.Size = new System.Drawing.Size(327, 557);
            this.lstWavFiles.TabIndex = 0;
            this.lstWavFiles.SelectedIndexChanged += new System.EventHandler(this.lstWavFiles_SelectedIndexChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnOpenWavFile);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(333, 57);
            this.panel3.TabIndex = 0;
            // 
            // btnOpenWavFile
            // 
            this.btnOpenWavFile.Location = new System.Drawing.Point(13, 13);
            this.btnOpenWavFile.Name = "btnOpenWavFile";
            this.btnOpenWavFile.Size = new System.Drawing.Size(75, 30);
            this.btnOpenWavFile.TabIndex = 0;
            this.btnOpenWavFile.Text = "打开";
            this.btnOpenWavFile.UseVisualStyleBackColor = true;
            this.btnOpenWavFile.Click += new System.EventHandler(this.btnOpenWavFile_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gbFrequency);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(552, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(770, 640);
            this.panel2.TabIndex = 2;
            // 
            // gbFrequency
            // 
            this.gbFrequency.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbFrequency.Location = new System.Drawing.Point(0, 0);
            this.gbFrequency.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbFrequency.Name = "gbFrequency";
            this.gbFrequency.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbFrequency.Size = new System.Drawing.Size(770, 555);
            this.gbFrequency.TabIndex = 1;
            this.gbFrequency.TabStop = false;
            this.gbFrequency.Text = "频率分布";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.groupBox2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 555);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(770, 85);
            this.panel4.TabIndex = 2;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.cbMaxY);
            this.panel5.Controls.Add(this.label2);
            this.panel5.Controls.Add(this.chkAll);
            this.panel5.Controls.Add(this.label1);
            this.panel5.Controls.Add(this.cbFFTSize);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(333, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(219, 640);
            this.panel5.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "FFT点数";
            // 
            // cbFFTSize
            // 
            this.cbFFTSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFFTSize.FormattingEnabled = true;
            this.cbFFTSize.Items.AddRange(new object[] {
            "32",
            "64",
            "128",
            "256",
            "512",
            "1024",
            "2048",
            "4096",
            "8192",
            "16384",
            "32768",
            "65536"});
            this.cbFFTSize.Location = new System.Drawing.Point(92, 15);
            this.cbFFTSize.Name = "cbFFTSize";
            this.cbFFTSize.Size = new System.Drawing.Size(121, 28);
            this.cbFFTSize.TabIndex = 3;
            this.cbFFTSize.SelectedIndexChanged += new System.EventHandler(this.cbFFTSize_SelectedIndexChanged);
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Checked = true;
            this.chkAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAll.Location = new System.Drawing.Point(11, 578);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(181, 24);
            this.chkAll.TabIndex = 5;
            this.chkAll.Text = "显示完整音频频率分布";
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "最大Y值";
            // 
            // cbMaxY
            // 
            this.cbMaxY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMaxY.FormattingEnabled = true;
            this.cbMaxY.Items.AddRange(new object[] {
            "100",
            "200",
            "500",
            "1000",
            "2000",
            "3000",
            "4000",
            "5000",
            "8000",
            "10000",
            "12000",
            "15000",
            "20000"});
            this.cbMaxY.Location = new System.Drawing.Point(92, 53);
            this.cbMaxY.Name = "cbMaxY";
            this.cbMaxY.Size = new System.Drawing.Size(121, 28);
            this.cbMaxY.TabIndex = 7;
            this.cbMaxY.SelectedIndexChanged += new System.EventHandler(this.cbMaxY_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.trackWavPcmPos);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(770, 85);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "时间轴";
            // 
            // trackWavPcmPos
            // 
            this.trackWavPcmPos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackWavPcmPos.Enabled = false;
            this.trackWavPcmPos.Location = new System.Drawing.Point(3, 23);
            this.trackWavPcmPos.Name = "trackWavPcmPos";
            this.trackWavPcmPos.Size = new System.Drawing.Size(764, 59);
            this.trackWavPcmPos.TabIndex = 1;
            this.trackWavPcmPos.Scroll += new System.EventHandler(this.trackWavPcmPos_Scroll);
            // 
            // FormFFT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1322, 640);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFFT";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FFT With NAudio";
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackWavPcmPos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnOpenWavFile;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox gbFrequency;
        private System.Windows.Forms.ListBox lstWavFiles;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ComboBox cbMaxY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbFFTSize;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar trackWavPcmPos;
    }
}

