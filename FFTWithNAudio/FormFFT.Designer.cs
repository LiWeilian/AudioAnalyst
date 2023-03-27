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
            this.panel2 = new System.Windows.Forms.Panel();
            this.gbFrequency = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOpenWavFile = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
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
            // panel2
            // 
            this.panel2.Controls.Add(this.gbFrequency);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(333, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(920, 640);
            this.panel2.TabIndex = 2;
            // 
            // gbFrequency
            // 
            this.gbFrequency.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbFrequency.Location = new System.Drawing.Point(0, 0);
            this.gbFrequency.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbFrequency.Name = "gbFrequency";
            this.gbFrequency.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbFrequency.Size = new System.Drawing.Size(920, 640);
            this.gbFrequency.TabIndex = 1;
            this.gbFrequency.TabStop = false;
            this.gbFrequency.Text = "频率分布";
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
            // groupBox1
            // 
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(333, 583);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "文件";
            // 
            // btnOpenWavFile
            // 
            this.btnOpenWavFile.Location = new System.Drawing.Point(13, 13);
            this.btnOpenWavFile.Name = "btnOpenWavFile";
            this.btnOpenWavFile.Size = new System.Drawing.Size(75, 30);
            this.btnOpenWavFile.TabIndex = 0;
            this.btnOpenWavFile.Text = "打开";
            this.btnOpenWavFile.UseVisualStyleBackColor = true;
            // 
            // FormFFT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1253, 640);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormFFT";
            this.Text = "FFT With NAudio";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnOpenWavFile;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox gbFrequency;
    }
}

