using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace FFTWithNAudio
{
    public partial class FormFFT : Form
    {
        public FormFFT()
        {
            InitializeComponent();

            cbFFTSize.SelectedIndex = 4;
            cbMaxY.SelectedIndex = 7;

            this.InitializeFFTChartControl();
        }

        private (int, double[]) CalcFFT(short[] dataPcm)
        {
            // the PCM size to be analyzed with FFT must be a power of 2
            int fftPoints = 2;
            while (fftPoints * 2 <= dataPcm.Length)
                fftPoints *= 2;

            // apply a Hamming window function as we load the FFT array then calculate the FFT
            NAudio.Dsp.Complex[] fftFull = new NAudio.Dsp.Complex[fftPoints];
            for (int i = 0; i < fftPoints; i++)
                fftFull[i].X = (float)(dataPcm[i] * NAudio.Dsp.FastFourierTransform.HammingWindow(i, fftPoints));
            NAudio.Dsp.FastFourierTransform.FFT(true, (int)Math.Log(fftPoints, 2.0), fftFull);

            // copy the complex values into the double array that will be plotted

            double[] dataFft = null;
            if (dataFft == null)
            {
                dataFft = new double[fftPoints / 2];
            }
            if (dataFftOutPut == null)
            {
                dataFftOutPut = new double[fftPoints / 2];
            }
            for (int i = 0; i < fftPoints / 2; i++)
            {
                double fftLeft = Math.Abs(fftFull[i].X + fftFull[i].Y);
                double fftRight = Math.Abs(fftFull[fftPoints - i - 1].X + fftFull[fftPoints - i - 1].Y);
                dataFft[i] = fftLeft + fftRight;

                dataFftOutPut[i] = Math.Max(dataFftOutPut[i], dataFft[i]);
            }

            return (fftPoints, dataFft);
        }

        private WaveInEvent wvin;
        double[] dataFftOutPut; 
        private ToolTip trackToolTip;

        #region FFT Chart with ZGraph
        private ucFFTChartPanel fftChart;
        private void InitializeFFTChartControl()
        {
            fftChart = new ucFFTChartPanel();
            fftChart.Parent = gbFrequency;
            fftChart.Dock = DockStyle.Fill;
            fftChart.SetTitle("频率");
            fftChart.SetYAxisScale(false, 0, int.Parse(cbMaxY.Text));
        }

        private void Plot(double[] xpoints, double[] dataFft)
        {
            fftChart.ClearData();
            fftChart.InitFrequencyList(xpoints.ToList(), 0);
            fftChart.AddDisplayData("Power", "频率", dataFft.ToList());
        }
        #endregion

        string gbTitle = "频率分布";
        private void PlotWaveData(string wavFile, int fftSize) 
        {
            dataFftOutPut = null;
            gbFrequency.Text = $"{gbTitle} - 计算中...";
            Application.DoEvents();

            WaveFileReader wavReader = new WaveFileReader(wavFile);

            double fftIntval = (double)wavReader.WaveFormat.SampleRate / (double)fftSize;

            double[] XPoints = new double[fftSize / 2];
            for (int i = 0; i < fftSize / 2; i++)
            {
                XPoints[i] = fftIntval * i;
            }

            byte[] allBytes = File.ReadAllBytes(wavFile);
            byte[] pcmBytes = allBytes.Skip(44).ToArray();

            int offSet = 0;
            while (offSet + fftSize <= pcmBytes.Length / 2)
            {
                byte[] currentPcmBytes = pcmBytes.Skip(offSet * 2).Take(fftSize * 2).ToArray();
                short[] dataPcm = new short[fftSize];
                for (int i = 0; i < fftSize; i++)
                {
                    dataPcm[i] = BitConverter.ToInt16(currentPcmBytes.Skip(i * 2).Take(2).ToArray(), 0);
                }
                (int points, double[] dataFft) = CalcFFT(dataPcm);

                offSet += fftSize;
            }

            if (fftSize > 512)
            {
                int sampleRate = fftSize / 512;
                double[] XPoints2 = new double[256];
                double[] dataFft2 = new double[256];
                for (int i = 0; i < 256; i++)
                {
                    XPoints2[i] = XPoints[i * sampleRate];
                    dataFft2[i] = dataFftOutPut[i * sampleRate];
                }
                Plot(XPoints2, dataFft2);
            } else
            {
                Plot(XPoints, dataFftOutPut);
            }

            gbFrequency.Text = $"{gbTitle}";
            Application.DoEvents();
        }

        private void UpdateTrackBar(string wavFile, int fftSize)
        {
            WaveFileReader wavReader = new WaveFileReader(wavFile);
                        
            trackWavPcmPos.Maximum = (int)wavReader.Length / fftSize / 2 - 1;
            trackWavPcmPos.Minimum = 0;
            trackWavPcmPos.Value = 0;
            trackWavPcmPos.TickFrequency = fftSize;
        }

        private void PlotWaveDataOnTrackPos(string wavFile, int fftSize, int pos)
        {
            dataFftOutPut = null;

            WaveFileReader wavReader = new WaveFileReader(wavFile);

            double fftIntval = (double)wavReader.WaveFormat.SampleRate / (double)fftSize;
            double[] XPoints = new double[fftSize / 2];
            for (int i = 0; i < fftSize / 2; i++)
            {
                XPoints[i] = fftIntval * i;
            }

            byte[] allBytes = File.ReadAllBytes(wavFile);
            byte[] pcmBytes = allBytes.Skip(44).ToArray();

            short[] dataPcm = new short[fftSize];
            byte[] currentPcmBytes = pcmBytes.Skip(pos * fftSize * 2).Take(fftSize * 2).ToArray();
            for (int i = 0; i < fftSize; i++)
            {
                dataPcm[i] = BitConverter.ToInt16(currentPcmBytes.Skip(i * 2).Take(2).ToArray(), 0);
            }
            (int points, double[] dataFft) = CalcFFT(dataPcm);

            if (fftSize > 512)
            {
                int sampleRate = fftSize / 512;
                double[] XPoints2 = new double[256];
                double[] dataFft2 = new double[256];
                for (int i = 0; i < 256; i++)
                {
                    XPoints2[i] = XPoints[i * sampleRate];
                    dataFft2[i] = dataFftOutPut[i * sampleRate];
                }
                Plot(XPoints2, dataFft2);
            }
            else
            {
                Plot(XPoints, dataFftOutPut);
            }
        }

        private void UpdateTrackBarToolTip(string wavFile, int trackMax, int trackPos)
        {
            WaveFileReader wavReader = new WaveFileReader(wavFile);
            double secondes = wavReader.Length / 2 / wavReader.WaveFormat.SampleRate;

            double currentSecond = secondes * trackPos / trackMax;

            string s = (currentSecond - Math.Truncate(currentSecond)).ToString("0.###");
            if (s.StartsWith("0."))
            {
                s = s.Substring(2);
            }

            string toolTipText = $"{ConvertIntToTimeFormat((int)Math.Truncate(currentSecond))}.{s}";

            trackToolTip.SetToolTip(trackWavPcmPos, toolTipText);
        }

        public static string ConvertIntToTimeFormat(int value)
        {
            int num = value / 3600;
            int num2 = value % 3600 / 60;
            int num3 = value % 60;
            return string.Format("{0}:{1}:{2}", num.ToString(), num2.ToString().PadLeft(2, '0'), num3.ToString().PadLeft(2, '0'));
        }

        private void btnOpenWavFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Wave音频文件(*.wav)|*.wav";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                lstWavFiles.Items.Add(new FileItem { FileName = openFileDialog.FileName });
                lstWavFiles.SelectedIndex = lstWavFiles.Items.Count - 1;
            }
        }

        private void lstWavFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstWavFiles.SelectedItem != null && lstWavFiles.SelectedItem is FileItem)
            {
                UpdateTrackBar((lstWavFiles.SelectedItem as FileItem).FileName, int.Parse(cbFFTSize.Text));
                if (chkAll.Checked)
                {
                    PlotWaveData((lstWavFiles.SelectedItem as FileItem).FileName, int.Parse(cbFFTSize.Text));
                } else
                {
                    PlotWaveDataOnTrackPos((lstWavFiles.SelectedItem as FileItem).FileName,
                        int.Parse(cbFFTSize.Text),
                        trackWavPcmPos.Value);
                }
            }
        }

        private void cbFFTSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstWavFiles.SelectedItem != null && lstWavFiles.SelectedItem is FileItem)
            {
                UpdateTrackBar((lstWavFiles.SelectedItem as FileItem).FileName, int.Parse(cbFFTSize.Text));
                if (chkAll.Checked)
                {
                    PlotWaveData((lstWavFiles.SelectedItem as FileItem).FileName, int.Parse(cbFFTSize.Text));
                } else
                {
                    PlotWaveDataOnTrackPos((lstWavFiles.SelectedItem as FileItem).FileName, 
                        int.Parse(cbFFTSize.Text),
                        trackWavPcmPos.Value);
                }
            }
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            trackWavPcmPos.Enabled = !chkAll.Checked;
            if (lstWavFiles.SelectedItem != null && lstWavFiles.SelectedItem is FileItem)
            {
                if (chkAll.Checked)
                {
                    PlotWaveData((lstWavFiles.SelectedItem as FileItem).FileName, int.Parse(cbFFTSize.Text));
                }
                else
                {
                    PlotWaveDataOnTrackPos((lstWavFiles.SelectedItem as FileItem).FileName,
                        int.Parse(cbFFTSize.Text),
                        trackWavPcmPos.Value);
                }
            }
        }

        private void trackWavPcmPos_Scroll(object sender, EventArgs e)
        {
            if (lstWavFiles.SelectedItem != null && lstWavFiles.SelectedItem is FileItem)
            {
                if (trackToolTip == null)
                {
                    trackToolTip = new ToolTip();
                }
                UpdateTrackBarToolTip((lstWavFiles.SelectedItem as FileItem).FileName,
                    trackWavPcmPos.Maximum,
                    trackWavPcmPos.Value);
                PlotWaveDataOnTrackPos((lstWavFiles.SelectedItem as FileItem).FileName,
                        int.Parse(cbFFTSize.Text),
                        trackWavPcmPos.Value);
            }
        }

        private void cbMaxY_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fftChart != null)
            {
                fftChart.SetYAxisScale(false, 0, int.Parse(cbMaxY.Text));
            }
        }
    }

    class FileItem
    {
        public string FileName { get; set; }
        public override string ToString()
        {
            return Path.GetFileName(FileName);
        }
    }
}
