using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Dsp;
using NAudio.Wave;
using ZedGraph;
using static System.Net.WebRequestMethods;

namespace FFTWithNAudio
{
    public partial class FormFFT : Form
    {
        public FormFFT()
        {
            //Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            cbFFTSize.SelectedIndex = 4;
            cbMaxPowerY.SelectedIndex = 7;

            this.InitializeFFTPowerChartControl();
            this.InitializeFFTPhaseChartControl();
        }

        double[] dataFftPower;
        double[] dataFftPhase;
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
            if (dataFftPower == null)
            {
                dataFftPower = new double[fftPoints / 2];
            }
            if (dataFftPhase == null)
            {
                dataFftPhase = new double[fftPoints / 2];
            }
            for (int i = 0; i < fftPoints / 2; i++)
            {
                //功率
                double fftLeft = Math.Abs(fftFull[i].X + fftFull[i].Y);
                double fftRight = Math.Abs(fftFull[fftPoints - i - 1].X + fftFull[fftPoints - i - 1].Y);
                dataFft[i] = fftLeft + fftRight;
                dataFftPower[i] = Math.Max(dataFftPower[i], dataFft[i]);

                //相位
                double phaseLeft = Math.Atan2(fftFull[i].Y, fftFull[i].X);
                double phaseRight = Math.Atan2(fftFull[fftPoints - i - 1].Y, fftFull[fftPoints - i - 1].X);
                dataFftPhase[i] = (phaseLeft + phaseRight) / 2 * 360.0 / Math.PI;
            }

            return (fftPoints, dataFft);
        }

        #region FFT Chart with ZGraph
        private ucFFTChartPanel fftPowerChart;
        private void InitializeFFTPowerChartControl()
        {
            fftPowerChart = new ucFFTChartPanel();
            fftPowerChart.Parent = gbPower;
            fftPowerChart.Dock = DockStyle.Fill;
            fftPowerChart.ChartZoomEvent += ChartControl_ChartZoomEvent;
            fftPowerChart.ChartMouseMoveEvent += ChartControl_ChartMouseMoveEvent;
            fftPowerChart.SetTitle("频率 - 幅度");
            fftPowerChart.SetYAxisScale(false, 0, int.Parse(cbMaxPowerY.Text));
        }

        private void PlotPower(double[] xpoints, double[] dataFft)
        {
            fftPowerChart.ClearData();
            fftPowerChart.InitFrequencyList(xpoints.ToList(), 0);
            fftPowerChart.AddDisplayData("幅度", "当前", dataFft.ToList());
        }


        private ucFFTChartPanel fftPhaseChart;
        private void InitializeFFTPhaseChartControl()
        {
            fftPhaseChart = new ucFFTChartPanel();
            fftPhaseChart.Parent = gbPhase;
            fftPhaseChart.Dock = DockStyle.Fill;
            fftPhaseChart.ChartZoomEvent += ChartControl_ChartZoomEvent;
            fftPhaseChart.ChartMouseMoveEvent += ChartControl_ChartMouseMoveEvent;
            fftPhaseChart.SetTitle("频率 - 相位");
            fftPhaseChart.SetYAxisScale(false, -400, 400);
        }

        private void PlotPhase(double[] xpoints, double[] dataFft)
        {
            fftPhaseChart.ClearData();
            fftPhaseChart.InitFrequencyList(xpoints.ToList(), 0);
            fftPhaseChart.AddDisplayData("相位", "当前", dataFft.ToList());
        }

        private void ClearPhase()
        {
            fftPhaseChart.ClearData();
        }
        #endregion

        string gbPowerTitle = "频率 - 幅度";
        string gbPhaseTitle = "频率 - 相位";
        private void PlotWaveData(string wavFile, int fftSize) 
        {
            dataFftPower = null;
            dataFftPhase = null;

            gbPower.Text = $"{gbPowerTitle} - 计算中...";
            //gbPhase.Text = $"{gbPhaseTitle} - 计算中...";
            Application.DoEvents();

            WaveFileReader wavReader = new WaveFileReader(wavFile);

            double fftIntval = (double)wavReader.WaveFormat.SampleRate / (double)fftSize;

            double[] XPoints = new double[fftSize / 2];
            for (int i = 0; i < fftSize / 2; i++)
            {
                XPoints[i] = fftIntval * i;
            }

            byte[] allBytes = System.IO.File.ReadAllBytes(wavFile);
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
                this.CalcFFT(dataPcm);

                offSet += fftSize;
            }

            if (fftSize > 512)
            {
                int sampleRate = fftSize / 512;
                double[] XPoints2 = new double[256];
                double[] dataFftPower2 = new double[256];
                double[] dataFftPhase2 = new double[256];
                for (int i = 0; i < 256; i++)
                {
                    XPoints2[i] = XPoints[i * sampleRate];
                    dataFftPower2[i] = dataFftPower[i * sampleRate];
                    dataFftPhase2[i] = dataFftPhase[i * sampleRate];
                }
                PlotPower(XPoints2, dataFftPower2);
            } else
            {
                double[] XPoints2 = new double[XPoints.Length];
                double[] dataFftPower2 = new double[XPoints.Length];
                double[] dataFftPhase2 = new double[XPoints.Length];
                for (int i = 0; i < XPoints2.Length; i++)
                {
                    XPoints2[i] = XPoints[i];
                    dataFftPower2[i] = dataFftPower[i];
                    dataFftPhase2[i] = dataFftPhase[i];
                }

                PlotPower(XPoints2, dataFftPower2);
            }

            ClearPhase();

            gbPower.Text = $"{gbPowerTitle}";
            //gbPhase.Text = $"{gbPhaseTitle}";
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

        bool scrolling = false;
        private void PlotWaveDataOnTrackPos(string wavFile, int fftSize, int pos)
        {
            try
            {
                dataFftPower = null;
                dataFftPhase = null;

                gbPower.Text = $"{gbPowerTitle} - 计算中...";
                gbPhase.Text = $"{gbPhaseTitle} - 计算中...";
                Application.DoEvents();

                WaveFileReader wavReader = new WaveFileReader(wavFile);

                double fftIntval = (double)wavReader.WaveFormat.SampleRate / (double)fftSize;
                double[] XPoints = new double[fftSize / 2];
                for (int i = 0; i < fftSize / 2; i++)
                {
                    XPoints[i] = fftIntval * i;
                }

                byte[] allBytes = System.IO.File.ReadAllBytes(wavFile);
                byte[] pcmBytes = allBytes.Skip(44).ToArray();


                short[] dataPcm = new short[fftSize];
                byte[] currentPcmBytes = pcmBytes.Skip(pos * fftSize * 2).Take(fftSize * 2).ToArray();
                for (int i = 0; i < fftSize; i++)
                {
                    dataPcm[i] = BitConverter.ToInt16(currentPcmBytes.Skip(i * 2).Take(2).ToArray(), 0);
                }
                this.CalcFFT(dataPcm);

                if (fftSize > 512)
                {
                    int sampleRate = fftSize / 512;
                    double[] XPoints2 = new double[256];
                    double[] dataFftPower2 = new double[256];
                    double[] dataFftPhase2 = new double[256];
                    for (int i = 0; i < 256; i++)
                    {
                        XPoints2[i] = XPoints[i * sampleRate];
                        dataFftPower2[i] = dataFftPower[i * sampleRate];
                        dataFftPhase2[i] = dataFftPhase[i * sampleRate];
                    }
                    PlotPower(XPoints2, dataFftPower2);
                    PlotPhase(XPoints2, dataFftPhase2);
                }
                else
                {
                    double[] XPoints2 = new double[XPoints.Length];
                    double[] dataFftPower2 = new double[XPoints.Length];
                    double[] dataFftPhase2 = new double[XPoints.Length];
                    for (int i = 0; i < XPoints2.Length; i++)
                    {
                        XPoints2[i] = XPoints[i];
                        dataFftPower2[i] = dataFftPower[i];
                        dataFftPhase2[i] = dataFftPhase[i];
                    }

                    PlotPower(XPoints2, dataFftPower2);
                    PlotPhase(XPoints2, dataFftPhase2);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                gbPower.Text = $"{gbPowerTitle}";
                gbPhase.Text = $"{gbPhaseTitle}";
                Application.DoEvents();
            }
        }

        private ToolTip trackToolTip;
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

            string toolTipText = $"{Utils.ConvertIntToTimeFormat((int)Math.Truncate(currentSecond))}.{s}";
            try
            {
                trackToolTip.SetToolTip(trackWavPcmPos, toolTipText);
            }
            catch (Exception)
            {
                trackToolTip = null;
            }
        }

        private void CreateSpectrogram(string wavFile, int fftSize)
        {
            dataFftPower = null;
            dataFftPhase = null;

            WaveFileReader wavReader = new WaveFileReader(wavFile);

            double fftIntval = (double)wavReader.WaveFormat.SampleRate / (double)fftSize;

            double[] XPoints = new double[fftSize / 2];
            for (int i = 0; i < fftSize / 2; i++)
            {
                XPoints[i] = fftIntval * i;
            }

            byte[] allBytes = System.IO.File.ReadAllBytes(wavFile);
            byte[] pcmBytes = allBytes.Skip(44).ToArray();
            List<double[]> spectrogramData = new List<double[]>();
            int offSet = 0;
            while (offSet + fftSize <= pcmBytes.Length / 2)
            {
                byte[] currentPcmBytes = pcmBytes.Skip(offSet * 2).Take(fftSize * 2).ToArray();
                short[] dataPcm = new short[fftSize];
                for (int i = 0; i < fftSize; i++)
                {
                    dataPcm[i] = BitConverter.ToInt16(currentPcmBytes.Skip(i * 2).Take(2).ToArray(), 0);
                }
                (int points, double[] data) = this.CalcFFT(dataPcm);
                //double[] data = new double[dataFftPower.Length];
                //for (int i = 0;i < dataFftPower.Length;i++)
                //{
                //    data[i] = dataFftPower[i];
                //}
                spectrogramData.Add(data);

                offSet += fftSize;
            }

            //绘图
            double max = 2000;
            double blueFilter = 10;
            double blueFilterRatio = (100 - blueFilter) / 10;
            double greenFilter = 50;
            double coloramp = (double)(trackColoramp.Maximum + 1 - trackColoramp.Value) / trackColoramp.Maximum;
            var bitmap = new Bitmap((int)pcmBytes.Length / 2 / fftSize, fftSize /2);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    double value = spectrogramData[x][fftSize / 2 - y - 1];
                    value = value > max ? max : value;
                    value = value / max;
                    value = Math.Pow(value, coloramp);

                    /*
                    //黄->红->蓝
                    value *= 100;
                    int red = value < 50 ? 255 : (int)(255 - (value - 50) * 5.1);
                    int green = value < 50 ? (int)(value * 5.1) : (int)(255 - (value - 50) * 5.1);
                    int blue = value < 50 ? 0 : (int)((value - 50) * 5.1);
                    Color color = Color.FromArgb(red, green, blue);
                    */

                    //蓝->红->黄
                    value *= 100;
                    int blue = value < blueFilter ? 255 : (int)(255 - (value - blueFilter) * 25.5 / blueFilterRatio);
                    int red = (int)(value / 2 * 5.1);
                    //int green = value < greenFilter ? 0 : (int)((value - greenFilter) * 5.1);
                    int green = value < greenFilter ? 0 : (int)((value - greenFilter) / (100 - greenFilter) * 255);
                    Color color = Color.FromArgb(red, green, blue);

                    /*
                    //蓝->红
                    var red = (int)(255 * value);
                    var blue = (int)(255 * (1 - value));
                    var color = Color.FromArgb(red, 0, blue);
                    */
                    // 绘制像素
                    bitmap.SetPixel(x, y, color);
                }
            }

            bitmap.Save($".\\temp\\{DateTime.Now.ToString("yyyyMMddHHmmss")}.png", ImageFormat.Png);
            FormSpectrogram.Instance.Image = bitmap;
            FormSpectrogram.Instance.ImageScale = new ImageScale
            {
                X = wavReader.Length / 2 / wavReader.WaveFormat.SampleRate,
                Y = wavReader.WaveFormat.SampleRate / 2
            };
            FormSpectrogram.Instance.Show();
            FormSpectrogram.Instance.BringToFront();
        }

        #region 滤波
        private MyWaveProvider CreateFrequencyFilter(string wavFileName,
            float lowPassFreq = 1500, float lowPassQ = 1,
            float highPassFreq = 700, float highPassQ = 1)
        {
            WaveFileReader reader = new WaveFileReader(wavFileName);
            WaveChannel32 wavChannel = new WaveChannel32(reader);
            MyWaveProvider myWaveProvider = new MyWaveProvider(wavChannel.ToSampleProvider(), 
                reader.WaveFormat.SampleRate,
                lowPassFreq, lowPassQ, highPassFreq, highPassQ);

            return myWaveProvider;

            ////播放
            //WaveOut wavOut = new WaveOut();
            //wavOut.Init(myWaveProvider);
            //wavOut.Play();

            ////保存
            //string outputFilePath = $".\\temp\\testfilter_{DateTime.Now.ToString("yyyyMMddHHmmss")}.wav";
            //WaveFileWriter wavFileWriter = new WaveFileWriter(outputFilePath, wavOut.OutputWaveFormat);
            //wavOut.Play();
        }

        private void CreateWaveFile(string filename, IWaveProvider sourceProvider)
        {
            using (var writer = new WaveFileWriter(filename, sourceProvider.WaveFormat))
            {
                var buffer = new byte[sourceProvider.WaveFormat.AverageBytesPerSecond * 4];
                while (true)
                {
                    int bytesRead = sourceProvider.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        // end of source provider
                        break;
                    }
                    // Write will throw exception if WAV file becomes too large
                    writer.Write(buffer, 0, bytesRead);
                }
            }
        }

        WaveOut wavOut = null;

        private void Play()
        {
            if (lstWavFiles.SelectedItem != null && lstWavFiles.SelectedItem is FileItem)
            {
                if (wavOut == null)
                {
                    wavOut = new WaveOut();
                    float q;
                    if (!float.TryParse(txtQValue.Text, out q) || q <= 0)
                    {
                        q = 1.0f;
                    }
                    string wavFileName = (lstWavFiles.SelectedItem as FileItem).FileName;
                    MyWaveProvider provider = CreateFrequencyFilter(wavFileName,
                        tbLowPass.Value * 50, q,
                        tbHighPass.Value * 50, q);

                    //MyWaveProvider provider = CreateFrequencyFilter(wavFileName,
                    //    tbHighPass.Value * 50, q,
                    //    tbLowPass.Value * 50, q);
                    wavOut.Init(provider);
                }
                wavOut.Play();
            }
                
        }

        private void Pause()
        {
            if (wavOut != null)
            {
                wavOut.Pause();
            }
        }

        private void Stop() 
        { 
            if (wavOut != null)
            {
                wavOut.Stop();
                wavOut = null;
            }
        }

        private void SaveFiltered()
        {
            if (lstWavFiles.SelectedItem != null && lstWavFiles.SelectedItem is FileItem)
            {
                float q;
                if (!float.TryParse(txtQValue.Text, out q) || q <= 0)
                {
                    q = 1.0f;
                }
                float lowPassFreq = tbLowPass.Value * 50;
                float highPassFreq = tbHighPass.Value * 50;

                string wavFileName = (lstWavFiles.SelectedItem as FileItem).FileName;

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "波形音频文件(*.wav)|*.wav";
                sfd.DefaultExt = ".wav";
                sfd.FileName = $"{Path.GetDirectoryName(wavFileName)}\\{Path.GetFileNameWithoutExtension(wavFileName)}.{highPassFreq}Hz-{lowPassFreq}Hz.{DateTime.Now.ToString("yyyyMMddHHmmss")}.wav";
                sfd.OverwritePrompt = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        btnSaveFilteredWaveFile.Enabled = false;
                        Application.DoEvents();
                        int times = (int)nudTimes.Value;
                        string sourceFile = wavFileName;
                        for (int i = 0; i < times; i++)
                        {
                            string filteredFile = $".\\temp\\temp.filtered.{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.wav";
                            if (i == times - 1)
                            {
                                filteredFile = sfd.FileName;
                            }
                            this.SaveFilteredFile(sourceFile, filteredFile,
                                lowPassFreq, q,
                                highPassFreq, q);

                            Application.DoEvents();
                            sourceFile = filteredFile;
                        }

                        Application.DoEvents();
                        this.AddWaveFileToList(new string[] { sfd.FileName });
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        btnSaveFilteredWaveFile.Enabled = true;
                    }
                }                
            }
        }

        private void SaveFilteredFile(string sourceFile, string filteredFile, 
            float lowPassFreq, float lowPassQ,
            float highPassFreq, float highPassQ)
        {
            WaveFileReader reader = new WaveFileReader(sourceFile);
            MyWaveProvider provider = CreateFrequencyFilter(sourceFile,
                lowPassFreq, lowPassQ,
                highPassFreq, highPassQ);

            //获取返回滤波数据后，每4字节->float->int16->bytes->写入wav文件
            float[] buffer = new float[reader.SampleCount * 2];
            int readCount = provider.Read(buffer, 0, buffer.Length);

            Int16[] sampleInt16 = new Int16[reader.SampleCount];
            byte[] sampleBytes = new byte[reader.SampleCount * 2];
            for (int i = 0; i < reader.SampleCount; i++)
            {
                //32768f，16位采样数据专用
                sampleInt16[i] = (Int16)(buffer[i * 2] * 32768f);
                byte[] bytes = BitConverter.GetBytes(sampleInt16[i]);
                sampleBytes[i * 2] = bytes[0];
                sampleBytes[i * 2 + 1] = bytes[1];
            }

            FileStream fs = new FileStream(filteredFile, FileMode.Create, FileAccess.Write);
            WaveFileWriter writer = new WaveFileWriter(fs, reader.WaveFormat);
            writer.Write(sampleBytes, 0, sampleBytes.Length);
            writer.Flush();
            writer.Close();
            fs.Close();
        }
        #endregion

        private bool listingFile = false;
        private void AddWaveFileToList(string[] fileNames)
        {
            try
            {
                listingFile = true;
                for (int i = 0; i < fileNames.Length; i++)
                {
                    lstWavFiles.Items.Add(new FileItem { FileName = fileNames[i] });
                }
            }
            finally
            {
                listingFile = false;
            }
            lstWavFiles.SelectedIndex = lstWavFiles.Items.Count - 1;
        }

        private void btnOpenWavFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Wave音频文件(*.wav)|*.wav",
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.AddWaveFileToList(openFileDialog.FileNames);
            }
        }

        private void lstWavFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!listingFile && lstWavFiles.SelectedItem != null && lstWavFiles.SelectedItem is FileItem)
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

                if (wavOut != null)
                {
                    this.Stop();
                    wavOut = null;
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

        private async void trackWavPcmPos_Scroll(object sender, EventArgs e)
        {
            if (lstWavFiles.SelectedItem != null && lstWavFiles.SelectedItem is FileItem)
            {
                if (trackToolTip == null)
                {
                    trackToolTip = new ToolTip();
                    //trackToolTip.IsBalloon = true;
                    //trackToolTip.UseFading = false;
                }
                if (!scrolling)
                {
                    try
                    {
                        scrolling = true;
                        UpdateTrackBarToolTip((lstWavFiles.SelectedItem as FileItem).FileName,
                            trackWavPcmPos.Maximum,
                            trackWavPcmPos.Value);

                        PlotWaveDataOnTrackPos((lstWavFiles.SelectedItem as FileItem).FileName,
                            int.Parse(cbFFTSize.Text),
                            trackWavPcmPos.Value);
                    }
                    finally
                    {
                        scrolling = false;
                    }
                }
            }
        }

        private void cbMaxPowerY_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fftPowerChart != null)
            {
                fftPowerChart.SetYAxisScale(false, 0, int.Parse(cbMaxPowerY.Text));
            }
        }

        private void panel2_SizeChanged(object sender, EventArgs e)
        {
            gbPower.Height = ((sender as Panel).Height - panel4.Height) / 2;
        }

        private void ChartControl_ChartZoomEvent(object sender, ChartZoomEventArgs e)
        {
            if (sender != this.fftPowerChart)
            {
                this.fftPowerChart.HandleOnZoomEvent(e.XMin, e.XMax);
            }
            if (sender != this.fftPhaseChart)
            {
                this.fftPhaseChart.HandleOnZoomEvent(e.XMin, e.XMax);
            }
        }

        private void ChartControl_ChartMouseMoveEvent(object sender, ChartMouseMoveEventArgs e)
        {
            if (sender != this.fftPowerChart)
            {
                this.fftPowerChart.HandleOnMouseMove(e.XIndex, e.X, e.Y);
            }
            if (sender != this.fftPhaseChart)
            {
                this.fftPhaseChart.HandleOnMouseMove(e.XIndex, e.X, e.Y);
            }
        }

        private void btnCreateSpectrogram_Click(object sender, EventArgs e)
        {
            if (lstWavFiles.SelectedItem != null && lstWavFiles.SelectedItem is FileItem)
            {
                try
                {
                    btnCreateSpectrogram.Text = "正在生成...";
                    btnCreateSpectrogram.Enabled = false;
                    Application.DoEvents();

                    CreateSpectrogram((lstWavFiles.SelectedItem as FileItem).FileName, int.Parse(cbFFTSize.Text));
                }
                catch (Exception)
                {

                }
                finally
                {
                    btnCreateSpectrogram.Text = "生成";
                    btnCreateSpectrogram.Enabled = true;
                }
            }
        }

        private void btnRemoveWavFile_Click(object sender, EventArgs e)
        {
            if (lstWavFiles.SelectedItem != null)
            {
                int selectedIndex = lstWavFiles.SelectedIndex;
                lstWavFiles.Items.Remove(lstWavFiles.SelectedItem);
                if (lstWavFiles.Items.Count > 0)
                {
                    if (selectedIndex == 0)
                    {
                        lstWavFiles.SelectedIndex = 0;
                    } else
                    {
                        lstWavFiles.SelectedIndex = selectedIndex - 1;
                    }
                } else
                {
                    fftPowerChart.ClearData();
                    fftPhaseChart.ClearData();
                }
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            this.Play();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            this.Pause();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.Stop();
        }

        private void tbHighPass_ValueChanged(object sender, EventArgs e)
        {
            if (tbHighPass.Value >= tbLowPass.Value)
            {
                tbLowPass.Value = tbHighPass.Value;
            }

            lblHighPassFreq.Text = $"最低频率：{tbHighPass.Value * 50} Hz";
        }

        private void tbLowPass_ValueChanged(object sender, EventArgs e)
        {
            if (tbLowPass.Value <= tbHighPass.Value)
            {
                tbHighPass.Value = tbLowPass.Value;
            }
            lblLowPassFreq.Text = $"最高频率：{tbLowPass.Value * 50} Hz";
        }

        private void btnSaveFilteredWaveFile_Click(object sender, EventArgs e)
        {
            this.SaveFiltered();
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

    enum FftCalcType
    {
        Power,
        Phase
    }

    class MyWaveProvider : ISampleProvider
    {
        private ISampleProvider sourceProvider;
        private float lowPassFreq, highPassFreq;
        private float lowPassQ, highPassQ;
        private int sampleRate;
        private int channels;
        private MyBiQuadFilter[] lowPassFilters;
        private MyBiQuadFilter[] highPassFilters;

        private MyBiQuadFilter[] bandPassSkirts;
        private MyBiQuadFilter[] bandPassPeaks;

        public MyWaveProvider(ISampleProvider sourceProvider, int sampleRate, 
            float lowPassFreq = 1500, float lowPassQ = 1,
            float highPassFreq = 700, float highPassQ = 1)
        {
            this.sourceProvider = sourceProvider;
            this.sampleRate = sampleRate;
            this.lowPassFreq = lowPassFreq;
            this.lowPassQ = lowPassQ;
            this.highPassFreq = highPassFreq;
            this.highPassQ = highPassQ;

            channels = sourceProvider.WaveFormat.Channels;
            lowPassFilters = new MyBiQuadFilter[channels];
            highPassFilters = new MyBiQuadFilter[channels];
            bandPassSkirts = new MyBiQuadFilter[channels];
            bandPassPeaks = new MyBiQuadFilter[channels];
            CreateFilters();            
        }

        private void CreateFilters()
        {
            for (int n = 0; n < channels; n++)
                if (lowPassFilters[n] == null)
                    lowPassFilters[n] = MyBiQuadFilter.LowPassFilter(sampleRate, lowPassFreq, lowPassQ);
                else
                    lowPassFilters[n].SetLowPassFilter(sampleRate, lowPassFreq, lowPassQ);

            for (int n = 0; n < channels; n++)
                if (highPassFilters[n] == null)
                    highPassFilters[n] = MyBiQuadFilter.HighPassFilter(sampleRate, highPassFreq, highPassQ);
                else
                    highPassFilters[n].SetHighPassFilter(sampleRate, highPassFreq, highPassQ);

            for (int i = 0; i < channels; i++)
            {
                bandPassSkirts[i] = MyBiQuadFilter.BandPassFilterConstantSkirtGain(sampleRate, (lowPassFreq + highPassFreq)/2.0f, lowPassQ);
            }

            for (int i = 0; i < channels; i++)
            {
                bandPassPeaks[i] = MyBiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, (lowPassFreq + highPassFreq) / 2.0f, lowPassQ);
            }

            /*
            //Shelf
            for (int n = 0; n < channels; n++)
                if (lowPassFilters[n] == null)
                    lowPassFilters[n] = BiQuadFilter.LowShelf(sampleRate, lowPassFreq, 0.5f, lowPassQ);
                else
                    lowPassFilters[n].SetLowPassFilter(sampleRate, lowPassFreq, lowPassQ);

            for (int n = 0; n < channels; n++)
                if (highPassFilters[n] == null)
                    highPassFilters[n] = BiQuadFilter.HighShelf(sampleRate, highPassFreq, 0.5f, highPassQ);
                else
                    highPassFilters[n].SetHighPassFilter(sampleRate, highPassFreq, highPassQ);
            */
        }

        public WaveFormat WaveFormat { get { return sourceProvider.WaveFormat; } }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = sourceProvider.Read(buffer, offset, count);
            
            for (int i = 0; i < samplesRead; i++)
                buffer[offset + i] = lowPassFilters[(i % channels)].Transform(buffer[offset + i]);
            for (int i = 0; i < samplesRead; i++)
                buffer[offset + i] = highPassFilters[(i % channels)].Transform(buffer[offset + i]);
            
            /*
            for (int i = 0; i < samplesRead; i++)
            {
                buffer[offset + i] = bandPassSkirts[(i % channels)].Transform(buffer[offset + i]);
            }
            */
            /*
            for (int i = 0; i < samplesRead; i++)
            {
                buffer[offset + i] = bandPassPeaks[(i % channels)].Transform(buffer[offset + i]);
            }
            */
            return samplesRead;
        }
    }
}
