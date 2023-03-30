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
            this.btnRemoveWavFile = new System.Windows.Forms.Button();
            this.btnOpenWavFile = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.gbPhase = new System.Windows.Forms.GroupBox();
            this.gbPower = new System.Windows.Forms.GroupBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.trackWavPcmPos = new System.Windows.Forms.TrackBar();
            this.panel5 = new System.Windows.Forms.Panel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.trackColoramp = new System.Windows.Forms.TrackBar();
            this.btnCreateSpectrogram = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbMaxPowerY = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbFFTSize = new System.Windows.Forms.ComboBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackWavPcmPos)).BeginInit();
            this.panel5.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackColoramp)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(333, 641);
            this.panel1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstWavFiles);
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(333, 641);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "文件";
            // 
            // lstWavFiles
            // 
            this.lstWavFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstWavFiles.FormattingEnabled = true;
            this.lstWavFiles.ItemHeight = 20;
            this.lstWavFiles.Location = new System.Drawing.Point(3, 65);
            this.lstWavFiles.Name = "lstWavFiles";
            this.lstWavFiles.Size = new System.Drawing.Size(327, 573);
            this.lstWavFiles.TabIndex = 0;
            this.lstWavFiles.SelectedIndexChanged += new System.EventHandler(this.lstWavFiles_SelectedIndexChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnRemoveWavFile);
            this.panel3.Controls.Add(this.btnOpenWavFile);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(3, 23);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(327, 42);
            this.panel3.TabIndex = 1;
            // 
            // btnRemoveWavFile
            // 
            this.btnRemoveWavFile.Location = new System.Drawing.Point(90, 6);
            this.btnRemoveWavFile.Name = "btnRemoveWavFile";
            this.btnRemoveWavFile.Size = new System.Drawing.Size(75, 30);
            this.btnRemoveWavFile.TabIndex = 1;
            this.btnRemoveWavFile.Text = "移除";
            this.btnRemoveWavFile.UseVisualStyleBackColor = true;
            this.btnRemoveWavFile.Click += new System.EventHandler(this.btnRemoveWavFile_Click);
            // 
            // btnOpenWavFile
            // 
            this.btnOpenWavFile.Location = new System.Drawing.Point(9, 6);
            this.btnOpenWavFile.Name = "btnOpenWavFile";
            this.btnOpenWavFile.Size = new System.Drawing.Size(75, 30);
            this.btnOpenWavFile.TabIndex = 0;
            this.btnOpenWavFile.Text = "打开";
            this.btnOpenWavFile.UseVisualStyleBackColor = true;
            this.btnOpenWavFile.Click += new System.EventHandler(this.btnOpenWavFile_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gbPhase);
            this.panel2.Controls.Add(this.gbPower);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(552, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(770, 641);
            this.panel2.TabIndex = 2;
            this.panel2.SizeChanged += new System.EventHandler(this.panel2_SizeChanged);
            // 
            // gbPhase
            // 
            this.gbPhase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbPhase.Location = new System.Drawing.Point(0, 278);
            this.gbPhase.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbPhase.Name = "gbPhase";
            this.gbPhase.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbPhase.Size = new System.Drawing.Size(770, 278);
            this.gbPhase.TabIndex = 3;
            this.gbPhase.TabStop = false;
            this.gbPhase.Text = "频率 - 相位";
            // 
            // gbPower
            // 
            this.gbPower.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbPower.Location = new System.Drawing.Point(0, 0);
            this.gbPower.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbPower.Name = "gbPower";
            this.gbPower.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbPower.Size = new System.Drawing.Size(770, 278);
            this.gbPower.TabIndex = 1;
            this.gbPower.TabStop = false;
            this.gbPower.Text = "频率 - 幅度";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.groupBox2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 556);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(770, 85);
            this.panel4.TabIndex = 2;
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
            this.trackWavPcmPos.Location = new System.Drawing.Point(3, 23);
            this.trackWavPcmPos.Name = "trackWavPcmPos";
            this.trackWavPcmPos.Size = new System.Drawing.Size(764, 59);
            this.trackWavPcmPos.TabIndex = 1;
            this.trackWavPcmPos.Scroll += new System.EventHandler(this.trackWavPcmPos_Scroll);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.groupBox5);
            this.panel5.Controls.Add(this.groupBox4);
            this.panel5.Controls.Add(this.groupBox3);
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(333, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(219, 641);
            this.panel5.TabIndex = 4;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.trackColoramp);
            this.groupBox5.Controls.Add(this.btnCreateSpectrogram);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox5.Location = new System.Drawing.Point(0, 141);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(219, 137);
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "频谱图";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(187, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "弱           渲染效果           强";
            // 
            // trackColoramp
            // 
            this.trackColoramp.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.trackColoramp.Location = new System.Drawing.Point(3, 78);
            this.trackColoramp.Maximum = 20;
            this.trackColoramp.Name = "trackColoramp";
            this.trackColoramp.Size = new System.Drawing.Size(213, 56);
            this.trackColoramp.TabIndex = 1;
            this.trackColoramp.Value = 17;
            // 
            // btnCreateSpectrogram
            // 
            this.btnCreateSpectrogram.Location = new System.Drawing.Point(7, 26);
            this.btnCreateSpectrogram.Name = "btnCreateSpectrogram";
            this.btnCreateSpectrogram.Size = new System.Drawing.Size(206, 30);
            this.btnCreateSpectrogram.TabIndex = 0;
            this.btnCreateSpectrogram.Text = "生成";
            this.btnCreateSpectrogram.UseVisualStyleBackColor = true;
            this.btnCreateSpectrogram.Click += new System.EventHandler(this.btnCreateSpectrogram_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbMaxPowerY);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox4.Location = new System.Drawing.Point(0, 65);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(219, 76);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "频谱图";
            // 
            // cbMaxPowerY
            // 
            this.cbMaxPowerY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMaxPowerY.FormattingEnabled = true;
            this.cbMaxPowerY.Items.AddRange(new object[] {
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
            this.cbMaxPowerY.Location = new System.Drawing.Point(98, 26);
            this.cbMaxPowerY.Name = "cbMaxPowerY";
            this.cbMaxPowerY.Size = new System.Drawing.Size(103, 28);
            this.cbMaxPowerY.TabIndex = 9;
            this.cbMaxPowerY.SelectedIndexChanged += new System.EventHandler(this.cbMaxPowerY_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "最大幅度Y值";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.cbFFTSize);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(219, 65);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "FFT设置";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 20);
            this.label1.TabIndex = 6;
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
            this.cbFFTSize.Location = new System.Drawing.Point(80, 26);
            this.cbFFTSize.Name = "cbFFTSize";
            this.cbFFTSize.Size = new System.Drawing.Size(121, 28);
            this.cbFFTSize.TabIndex = 5;
            this.cbFFTSize.SelectedIndexChanged += new System.EventHandler(this.cbFFTSize_SelectedIndexChanged);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.chkAll);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 563);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(219, 78);
            this.panel6.TabIndex = 8;
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(20, 16);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(181, 24);
            this.chkAll.TabIndex = 6;
            this.chkAll.Text = "显示完整音频时间分布";
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // FormFFT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1322, 641);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormFFT";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FFT With NAudio";
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackWavPcmPos)).EndInit();
            this.panel5.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackColoramp)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox gbPower;
        private System.Windows.Forms.ListBox lstWavFiles;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar trackWavPcmPos;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cbMaxPowerY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbFFTSize;
        private System.Windows.Forms.GroupBox gbPhase;
        private System.Windows.Forms.Button btnCreateSpectrogram;
        private System.Windows.Forms.TrackBar trackColoramp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnRemoveWavFile;
        private System.Windows.Forms.Button btnOpenWavFile;
    }
}

