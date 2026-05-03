using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NAudio.Dsp;
using NAudio.Wave;
using Timer = System.Windows.Forms.Timer;

namespace MyAudio
{
    public partial class FormMain : Form
    {
        private readonly Button btnAddAudio;
        private readonly Button btnRemoveAudio;
        private readonly Button btnPlayPause;
        private readonly Button btnStop;
        private readonly Label lblFileName;
        private readonly ListBox lstAudioFiles;

        private readonly TrackBar trkTime;
        private readonly Label lblTime;

        private readonly GroupBox grpFft;
        private readonly NumericUpDown nudFftSize;
        private readonly NumericUpDown nudFrameSize;
        private readonly NumericUpDown nudHopSize;
        private readonly ComboBox cmbWindow;
        private readonly Button btnApplyFft;

        private readonly BufferedPanel pnlWave;
        private readonly BufferedPanel pnlSpectrum;
        private readonly BufferedPanel pnlPhase;
        private readonly BufferedPanel pnlSpectrogram;

        private readonly List<AudioItem> audioItems = new();

        private double[]? fftMagnitudeDb;
        private double[]? fftPhase;
        private double[,]? spectrogramDb; // [timeFrame, freqBin]
        private bool spectrogramTruncated;
        private double spectrogramDurationSeconds;
        private Bitmap? spectrogramBitmapCache;
        private Size spectrogramBitmapCacheSize = Size.Empty;

        private WaveOutEvent? waveOut;
        private AudioFileReader? audioFileReader;
        private string? playbackFilePath;
        private bool isPlaying;

        private readonly Timer playbackTimer;
        private readonly Timer trkThrottleTimer;
        private bool isInternalTrackUpdate;
        private bool isUserDragging;
        private long lastRealtimeFftTick;

        // 新增：语谱图 Tip
        private readonly ToolTip spectrogramToolTip;
        private string lastSpectrogramTipText = string.Empty;

        public FormMain()
        {
            Text = "音频分析器";
            ClientSize = new Size(1380, 860);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;

            // 左侧：文件列表
            btnAddAudio = new Button
            {
                Name = "btnAddAudio",
                Text = "添加 WAV",
                Location = new Point(12, 12),
                Size = new Size(110, 32)
            };
            btnAddAudio.Click += BtnAddAudio_Click;

            btnRemoveAudio = new Button
            {
                Name = "btnRemoveAudio",
                Text = "移除选中",
                Location = new Point(132, 12),
                Size = new Size(110, 32)
            };
            btnRemoveAudio.Click += BtnRemoveAudio_Click;

            lstAudioFiles = new ListBox
            {
                Name = "lstAudioFiles",
                Location = new Point(12, 52),
                Size = new Size(230, 300)
            };
            lstAudioFiles.SelectedIndexChanged += LstAudioFiles_SelectedIndexChanged;

            // 左侧：FFT 设置
            grpFft = new GroupBox
            {
                Text = "FFT 设置",
                Location = new Point(12, 362),
                Size = new Size(230, 220)
            };

            var lblFftSize = new Label { Text = "FFT 长度", Location = new Point(12, 32), AutoSize = true };
            nudFftSize = new NumericUpDown
            {
                Location = new Point(100, 28),
                Size = new Size(110, 28),
                Minimum = 256,
                Maximum = 16384,
                Increment = 256,
                Value = 4096
            };

            var lblFrame = new Label { Text = "帧长", Location = new Point(12, 72), AutoSize = true };
            nudFrameSize = new NumericUpDown
            {
                Location = new Point(100, 68),
                Size = new Size(110, 28),
                Minimum = 256,
                Maximum = 4096,
                Increment = 256,
                Value = 1024
            };

            var lblHop = new Label { Text = "帧移", Location = new Point(12, 112), AutoSize = true };
            nudHopSize = new NumericUpDown
            {
                Location = new Point(100, 108),
                Size = new Size(110, 28),
                Minimum = 64,
                Maximum = 2048,
                Increment = 64,
                Value = 512
            };

            var lblWindow = new Label { Text = "窗函数", Location = new Point(12, 152), AutoSize = true };
            cmbWindow = new ComboBox
            {
                Location = new Point(100, 148),
                Size = new Size(110, 28),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbWindow.Items.AddRange(new object[] { "Hamming", "Rectangular" });
            cmbWindow.SelectedIndex = 0;

            btnApplyFft = new Button
            {
                Text = "应用参数",
                Location = new Point(12, 182),
                Size = new Size(198, 30)
            };
            btnApplyFft.Click += BtnApplyFft_Click;

            grpFft.Controls.AddRange(new Control[]
            {
                lblFftSize, nudFftSize, lblFrame, nudFrameSize, lblHop, nudHopSize, lblWindow, cmbWindow, btnApplyFft
            });

            lblFileName = new Label
            {
                Name = "lblFileName",
                Text = "文件：未选择",
                Location = new Point(265, 16),
                Size = new Size(1090, 22),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // 播放控件
            btnPlayPause = new Button
            {
                Name = "btnPlayPause",
                Text = "播放",
                Location = new Point(265, 46),
                Size = new Size(80, 30)
            };
            btnPlayPause.Click += BtnPlayPause_Click;

            btnStop = new Button
            {
                Name = "btnStop",
                Text = "停止",
                Location = new Point(355, 46),
                Size = new Size(80, 30)
            };
            btnStop.Click += BtnStop_Click;

            // 时间轴
            trkTime = new TrackBar
            {
                Name = "trkTime",
                Location = new Point(445, 42),
                Size = new Size(820, 45),
                Minimum = 0,
                Maximum = 1000,
                TickFrequency = 100,
                Value = 0
            };
            trkTime.Scroll += TrkTime_Scroll;
            trkTime.MouseDown += (_, __) => isUserDragging = true;
            trkTime.MouseUp += TrkTime_MouseUp;

            lblTime = new Label
            {
                Name = "lblTime",
                Text = "时间：0.000 s",
                Location = new Point(1275, 48),
                Size = new Size(100, 24),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // 图表区域（两行两列）
            pnlWave = CreatePanel("pnlWave", new Point(265, 110), new Size(540, 340));
            pnlSpectrum = CreatePanel("pnlSpectrum", new Point(817, 110), new Size(540, 340));
            pnlPhase = CreatePanel("pnlPhase", new Point(265, 462), new Size(540, 340));
            pnlSpectrogram = CreatePanel("pnlSpectrogram", new Point(817, 462), new Size(540, 340));

            pnlWave.Paint += PnlWave_Paint;
            pnlSpectrum.Paint += PnlSpectrum_Paint;
            pnlPhase.Paint += PnlPhase_Paint;
            pnlSpectrogram.Paint += PnlSpectrogram_Paint;

            Controls.AddRange(new Control[]
            {
                btnAddAudio, btnRemoveAudio, lstAudioFiles, grpFft,
                lblFileName, btnPlayPause, btnStop, trkTime, lblTime,
                pnlWave, pnlSpectrum, pnlPhase, pnlSpectrogram
            });

            playbackTimer = new Timer { Interval = 50 };
            playbackTimer.Tick += PlaybackTimer_Tick;

            trkThrottleTimer = new Timer { Interval = 80 }; // 时间轴拖动节流
            trkThrottleTimer.Tick += TrkThrottleTimer_Tick;

            // 初始化 ToolTip
            spectrogramToolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 80,
                ReshowDelay = 30,
                ShowAlways = true
            };

            pnlSpectrogram.MouseMove += PnlSpectrogram_MouseMove;
            pnlSpectrogram.MouseLeave += PnlSpectrogram_MouseLeave;
        }

        private static BufferedPanel CreatePanel(string name, Point location, Size size)
        {
            return new BufferedPanel
            {
                Name = name,
                Location = location,
                Size = size,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private AudioItem? CurrentAudio => lstAudioFiles.SelectedItem as AudioItem;

        private void BtnAddAudio_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "选择 WAV 文件",
                Filter = "WAV 文件 (*.wav)|*.wav",
                Multiselect = true
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            var errors = new List<string>();
            foreach (string path in ofd.FileNames)
            {
                try
                {
                    if (audioItems.Any(a => string.Equals(a.FullPath, path, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    var item = LoadWavFile(path);
                    audioItems.Add(item);
                    lstAudioFiles.Items.Add(item);
                }
                catch (Exception ex)
                {
                    errors.Add($"{Path.GetFileName(path)}: {ex.Message}");
                }
            }

            if (lstAudioFiles.SelectedIndex < 0 && lstAudioFiles.Items.Count > 0)
                lstAudioFiles.SelectedIndex = 0;

            if (errors.Count > 0)
                MessageBox.Show("以下文件加载失败：\n" + string.Join("\n", errors), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void BtnRemoveAudio_Click(object? sender, EventArgs e)
        {
            int idx = lstAudioFiles.SelectedIndex;
            if (idx < 0) return;

            var item = lstAudioFiles.Items[idx] as AudioItem;
            if (item != null && string.Equals(item.FullPath, playbackFilePath, StringComparison.OrdinalIgnoreCase))
                StopPlayback(resetPosition: false);

            lstAudioFiles.Items.RemoveAt(idx);
            audioItems.RemoveAt(idx);

            if (lstAudioFiles.Items.Count > 0)
                lstAudioFiles.SelectedIndex = Math.Min(idx, lstAudioFiles.Items.Count - 1);
            else
                ClearAnalysis();
        }

        private void LstAudioFiles_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var audio = CurrentAudio;
            if (audio == null)
            {
                ClearAnalysis();
                return;
            }

            if (!string.Equals(playbackFilePath, audio.FullPath, StringComparison.OrdinalIgnoreCase))
                StopPlayback(resetPosition: false);

            lblFileName.Text = $"文件：{audio.FileName}    采样率：{audio.SampleRate} Hz    样本数：{audio.Samples.Length}";
            int maxMs = Math.Max(1, (int)(audio.DurationSeconds * 1000.0));
            trkTime.Maximum = maxMs;
            trkTime.Value = 0;
            lblTime.Text = "时间：0.000 s";

            RecomputeAllForCurrent();
        }

        private void BtnPlayPause_Click(object? sender, EventArgs e)
        {
            var audio = CurrentAudio;
            if (audio == null)
            {
                MessageBox.Show("请先选择音频文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                if (waveOut == null || !string.Equals(playbackFilePath, audio.FullPath, StringComparison.OrdinalIgnoreCase))
                {
                    StopPlayback(resetPosition: false);

                    audioFileReader = new AudioFileReader(audio.FullPath);
                    waveOut = new WaveOutEvent();
                    waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
                    waveOut.Init(audioFileReader);
                    playbackFilePath = audio.FullPath;

                    audioFileReader.CurrentTime = TimeSpan.FromMilliseconds(trkTime.Value);
                }

                if (!isPlaying)
                {
                    waveOut!.Play();
                    isPlaying = true;
                    btnPlayPause.Text = "暂停";
                    playbackTimer.Start();
                }
                else
                {
                    waveOut!.Pause();
                    isPlaying = false;
                    btnPlayPause.Text = "播放";
                    playbackTimer.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"播放失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnStop_Click(object? sender, EventArgs e)
        {
            StopPlayback(resetPosition: true);
        }

        private void WaveOut_PlaybackStopped(object? sender, StoppedEventArgs e)
        {
            isPlaying = false;
            playbackTimer.Stop();
            btnPlayPause.Text = "播放";

            if (e.Exception != null)
            {
                MessageBox.Show($"播放异常：{e.Exception.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PlaybackTimer_Tick(object? sender, EventArgs e)
        {
            if (!isPlaying || audioFileReader == null) return;

            int ms = (int)Math.Clamp(audioFileReader.CurrentTime.TotalMilliseconds, 0, trkTime.Maximum);

            isInternalTrackUpdate = true;
            trkTime.Value = ms;
            isInternalTrackUpdate = false;
            lblTime.Text = $"时间：{ms / 1000.0:F3} s";

            long now = Environment.TickCount64;
            if (now - lastRealtimeFftTick >= 120)
            {
                lastRealtimeFftTick = now;
                ComputeFftAnalysisAtCurrentTime();
                pnlWave.Invalidate();
                pnlSpectrum.Invalidate();
                pnlPhase.Invalidate();
                pnlSpectrogram.Invalidate();
            }
        }

        private void TrkTime_MouseUp(object? sender, MouseEventArgs e)
        {
            isUserDragging = false;

            if (audioFileReader != null)
                audioFileReader.CurrentTime = TimeSpan.FromMilliseconds(trkTime.Value);

            // 拖动结束后立即做一次分析
            ComputeFftAnalysisAtCurrentTime();
            pnlWave.Invalidate();
            pnlSpectrum.Invalidate();
            pnlPhase.Invalidate();
            pnlSpectrogram.Invalidate();
        }

        private void TrkTime_Scroll(object? sender, EventArgs e)
        {
            lblTime.Text = $"时间：{trkTime.Value / 1000.0:F3} s";

            if (isInternalTrackUpdate) return;

            if (isUserDragging)
            {
                // 拖动过程节流重绘
                trkThrottleTimer.Stop();
                trkThrottleTimer.Start();
            }
            else
            {
                ComputeFftAnalysisAtCurrentTime();
                pnlWave.Invalidate();
                pnlSpectrum.Invalidate();
                pnlPhase.Invalidate();
                pnlSpectrogram.Invalidate();
            }
        }

        private void TrkThrottleTimer_Tick(object? sender, EventArgs e)
        {
            trkThrottleTimer.Stop();
            ComputeFftAnalysisAtCurrentTime();
            pnlWave.Invalidate();
            pnlSpectrum.Invalidate();
            pnlPhase.Invalidate();
            pnlSpectrogram.Invalidate();
        }

        private void BtnApplyFft_Click(object? sender, EventArgs e)
        {
            if (nudHopSize.Value > nudFrameSize.Value)
                nudHopSize.Value = nudFrameSize.Value;

            RecomputeAllForCurrent();
        }

        private void RecomputeAllForCurrent()
        {
            if (CurrentAudio == null) return;

            ComputeFftAnalysisAtCurrentTime();
            ComputeSpectrogram();

            spectrogramBitmapCache?.Dispose();
            spectrogramBitmapCache = null;
            spectrogramBitmapCacheSize = Size.Empty;

            pnlWave.Invalidate();
            pnlSpectrum.Invalidate();
            pnlPhase.Invalidate();
            pnlSpectrogram.Invalidate();
        }

        private void ClearAnalysis()
        {
            StopPlayback(resetPosition: false);

            lblFileName.Text = "文件：未选择";
            fftMagnitudeDb = null;
            fftPhase = null;
            spectrogramDb = null;
            spectrogramDurationSeconds = 0;
            spectrogramTruncated = false;
            trkTime.Value = 0;
            lblTime.Text = "时间：0.000 s";

            spectrogramBitmapCache?.Dispose();
            spectrogramBitmapCache = null;
            spectrogramBitmapCacheSize = Size.Empty;

            pnlWave.Invalidate();
            pnlSpectrum.Invalidate();
            pnlPhase.Invalidate();
            pnlSpectrogram.Invalidate();
        }

        private void StopPlayback(bool resetPosition)
        {
            playbackTimer.Stop();
            isPlaying = false;
            btnPlayPause.Text = "播放";

            if (waveOut != null)
            {
                waveOut.PlaybackStopped -= WaveOut_PlaybackStopped;
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }

            audioFileReader?.Dispose();
            audioFileReader = null;
            playbackFilePath = null;

            if (resetPosition)
            {
                trkTime.Value = 0;
                lblTime.Text = "时间：0.000 s";
                ComputeFftAnalysisAtCurrentTime();
                pnlWave.Invalidate();
                pnlSpectrum.Invalidate();
                pnlPhase.Invalidate();
            }
        }

        private static AudioItem LoadWavFile(string filePath)
        {
            using var reader = new WaveFileReader(filePath);
            int sampleRate = reader.WaveFormat.SampleRate;
            int channels = reader.WaveFormat.Channels;
            var sampleProvider = reader.ToSampleProvider();

            int block = 4096 * channels;
            float[] buffer = new float[block];
            var samples = new List<float>(Math.Max(4096, (int)Math.Max(1, reader.SampleCount)));

            while (true)
            {
                int read = sampleProvider.Read(buffer, 0, block);
                if (read <= 0) break;

                int frames = read / channels;
                for (int i = 0; i < frames; i++)
                {
                    float sum = 0f;
                    for (int ch = 0; ch < channels; ch++) sum += buffer[i * channels + ch];
                    samples.Add(sum / channels);
                }
            }

            return new AudioItem(filePath, Path.GetFileName(filePath), sampleRate, samples.ToArray());
        }

        private void ComputeFftAnalysisAtCurrentTime()
        {
            var audio = CurrentAudio;
            if (audio == null || audio.Samples.Length == 0)
            {
                fftMagnitudeDb = null;
                fftPhase = null;
                return;
            }

            int fftSize = ToPow2((int)nudFftSize.Value);
            int m = (int)Math.Log2(fftSize);
            int centerSample = (int)(trkTime.Value / 1000.0 * audio.SampleRate);

            int start = Math.Max(0, centerSample - fftSize / 2);
            if (start + fftSize > audio.Samples.Length)
                start = Math.Max(0, audio.Samples.Length - fftSize);

            var complex = new Complex[fftSize];
            string windowType = cmbWindow.SelectedItem?.ToString() ?? "Hamming";

            for (int i = 0; i < fftSize; i++)
            {
                int src = start + i;
                float s = (src < audio.Samples.Length) ? audio.Samples[src] : 0f;
                s *= (float)GetWindowValue(windowType, i, fftSize);
                complex[i].X = s;
                complex[i].Y = 0f;
            }

            FastFourierTransform.FFT(true, m, complex);

            int half = fftSize / 2;
            fftMagnitudeDb = new double[half];
            fftPhase = new double[half];

            for (int i = 0; i < half; i++)
            {
                double re = complex[i].X;
                double im = complex[i].Y;
                double mag = Math.Sqrt(re * re + im * im);
                double db = 20.0 * Math.Log10(Math.Max(mag, 1e-12));
                fftMagnitudeDb[i] = Math.Max(db, -100.0);
                fftPhase[i] = Math.Atan2(im, re);
            }
        }

        private void ComputeSpectrogram()
        {
            var audio = CurrentAudio;
            if (audio == null || audio.Samples.Length == 0 || audio.SampleRate <= 0)
            {
                spectrogramDb = null;
                spectrogramDurationSeconds = 0;
                spectrogramTruncated = false;
                return;
            }

            int frameSize = ToPow2((int)nudFrameSize.Value);
            int hopSize = Math.Max(1, (int)nudHopSize.Value);
            int fftSize = frameSize;
            int freqBins = fftSize / 2;
            int m = (int)Math.Log2(fftSize);

            int maxSamples = Math.Min(audio.Samples.Length, audio.SampleRate * 30);
            spectrogramTruncated = audio.Samples.Length > maxSamples;
            spectrogramDurationSeconds = maxSamples / (double)audio.SampleRate;

            int frameCount = (maxSamples <= frameSize)
                ? 1
                : (int)Math.Ceiling((maxSamples - frameSize) / (double)hopSize) + 1;

            spectrogramDb = new double[frameCount, freqBins];
            string windowType = cmbWindow.SelectedItem?.ToString() ?? "Hamming";

            var complex = new Complex[fftSize];
            for (int f = 0; f < frameCount; f++)
            {
                int start = f * hopSize;
                for (int n = 0; n < frameSize; n++)
                {
                    int idx = start + n;
                    float s = (idx < maxSamples) ? audio.Samples[idx] : 0f;
                    s *= (float)GetWindowValue(windowType, n, frameSize);
                    complex[n].X = s;
                    complex[n].Y = 0f;
                }

                FastFourierTransform.FFT(true, m, complex);

                for (int k = 0; k < freqBins; k++)
                {
                    double re = complex[k].X;
                    double im = complex[k].Y;
                    double mag = Math.Sqrt(re * re + im * im);
                    double db = 20.0 * Math.Log10(Math.Max(mag, 1e-12));
                    spectrogramDb[f, k] = Math.Max(db, -100.0);
                }
            }
        }

        private static int ToPow2(int v)
        {
            int p = 1;
            while (p < v) p <<= 1;
            return p;
        }

        private static double GetWindowValue(string windowType, int n, int size)
        {
            if (size <= 1) return 1.0;
            if (windowType == "Rectangular") return 1.0;
            return 0.54 - 0.46 * Math.Cos(2.0 * Math.PI * n / (size - 1)); // Hamming
        }

        private void PnlWave_Paint(object? sender, PaintEventArgs e)
        {
            DrawPanelTitle(e.Graphics, "波形图（时域）");
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle plot = new Rectangle(52, 20, pnlWave.Width - 68, pnlWave.Height - 60);
            DrawAxes(g, plot, "时间 (s)", "幅度");

            var audio = CurrentAudio;
            if (audio == null || audio.Samples.Length == 0)
            {
                DrawCenteredText(g, pnlWave.ClientRectangle, "请先添加并选择 WAV 文件");
                return;
            }

            int n = audio.Samples.Length;
            double duration = audio.DurationSeconds;
            using var pen = new Pen(Color.SteelBlue, 1f);

            // 采样点过多时，按像素列取 min/max 绘制，保证流畅
            if (n > plot.Width * 4)
            {
                for (int x = 0; x < plot.Width; x++)
                {
                    int i0 = (int)((long)x * n / plot.Width);
                    int i1 = (int)((long)(x + 1) * n / plot.Width);
                    if (i1 <= i0) i1 = i0 + 1;
                    if (i1 > n) i1 = n;

                    float min = 1f, max = -1f;
                    for (int i = i0; i < i1; i++)
                    {
                        float v = audio.Samples[i];
                        if (v < min) min = v;
                        if (v > max) max = v;
                    }

                    int yMin = plot.Top + (int)((1 - Math.Clamp(max, -1f, 1f)) * 0.5 * plot.Height);
                    int yMax = plot.Top + (int)((1 - Math.Clamp(min, -1f, 1f)) * 0.5 * plot.Height);
                    g.DrawLine(pen, plot.Left + x, yMin, plot.Left + x, yMax);
                }
            }
            else
            {
                PointF[] pts = new PointF[n];
                for (int i = 0; i < n; i++)
                {
                    float x = plot.Left + (float)i / Math.Max(1, n - 1) * plot.Width;
                    float y = plot.Top + (float)((1 - Math.Clamp(audio.Samples[i], -1f, 1f)) * 0.5) * plot.Height;
                    pts[i] = new PointF(x, y);
                }
                if (pts.Length >= 2) g.DrawLines(pen, pts);
            }

            // 时间轴指示线
            float markerX = plot.Left + (float)((trkTime.Value / 1000.0) / Math.Max(duration, 1e-9) * plot.Width);
            g.DrawLine(Pens.Red, markerX, plot.Top, markerX, plot.Bottom);

            DrawXTicks(g, plot, 0, duration, 5, "0.0");
            DrawYTicks(g, plot, -1, 1, 4, "0.0");
        }

        private void PnlSpectrum_Paint(object? sender, PaintEventArgs e)
        {
            DrawPanelTitle(e.Graphics, "频谱图（随时间轴）");
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle plot = new Rectangle(52, 20, pnlSpectrum.Width - 68, pnlSpectrum.Height - 60);
            DrawAxes(g, plot, "频率 (Hz)", "幅度(dB)");

            var audio = CurrentAudio;
            if (audio == null || fftMagnitudeDb == null || fftMagnitudeDb.Length == 0)
            {
                DrawCenteredText(g, pnlSpectrum.ClientRectangle, "无可用频谱数据");
                return;
            }

            int n = fftMagnitudeDb.Length;
            double fMax = audio.SampleRate / 2.0;
            double dbMin = -100.0;
            double dbMax = Math.Max(0.0, fftMagnitudeDb.Max());

            using var pen = new Pen(Color.OrangeRed, 1.2f);
            PointF[] pts = new PointF[n];
            for (int i = 0; i < n; i++)
            {
                double f = i * fMax / Math.Max(1, n - 1);
                double db = Math.Clamp(fftMagnitudeDb[i], dbMin, dbMax);
                float x = (float)(plot.Left + (f / fMax) * plot.Width);
                float y = (float)(plot.Top + (dbMax - db) / Math.Max(1e-9, dbMax - dbMin) * plot.Height);
                pts[i] = new PointF(x, y);
            }
            if (pts.Length >= 2) g.DrawLines(pen, pts);

            DrawXTicks(g, plot, 0, fMax, 5, "0");
            DrawYTicks(g, plot, dbMin, dbMax, 5, "0");
        }

        private void PnlPhase_Paint(object? sender, PaintEventArgs e)
        {
            DrawPanelTitle(e.Graphics, "相位图（随时间轴）");
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle plot = new Rectangle(52, 20, pnlPhase.Width - 68, pnlPhase.Height - 60);
            DrawAxes(g, plot, "频率 (Hz)", "相位(rad)");

            var audio = CurrentAudio;
            if (audio == null || fftPhase == null || fftPhase.Length == 0)
            {
                DrawCenteredText(g, pnlPhase.ClientRectangle, "无可用相位数据");
                return;
            }

            int n = fftPhase.Length;
            double fMax = audio.SampleRate / 2.0;

            using var pen = new Pen(Color.MediumPurple, 1f);
            PointF[] pts = new PointF[n];
            for (int i = 0; i < n; i++)
            {
                double f = i * fMax / Math.Max(1, n - 1);
                double p = Math.Clamp(fftPhase[i], -Math.PI, Math.PI);
                float x = (float)(plot.Left + (f / fMax) * plot.Width);
                float y = (float)(plot.Top + (Math.PI - p) / (2 * Math.PI) * plot.Height);
                pts[i] = new PointF(x, y);
            }
            if (pts.Length >= 2) g.DrawLines(pen, pts);

            DrawXTicks(g, plot, 0, fMax, 5, "0");
            DrawYTicks(g, plot, -Math.PI, Math.PI, 4, "0.00");
        }

        private void PnlSpectrogram_Paint(object? sender, PaintEventArgs e)
        {
            DrawPanelTitle(e.Graphics, "语谱图（STFT）");
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;

            Rectangle plot = GetSpectrogramPlotRect();
            Rectangle bar = new Rectangle(plot.Right + 8, plot.Top, 14, plot.Height);
            DrawAxes(g, plot, "时间 (s)", "频率 (Hz)");

            var audio = CurrentAudio;
            if (audio == null || spectrogramDb == null || spectrogramDb.GetLength(0) == 0 || spectrogramDb.GetLength(1) == 0)
            {
                DrawCenteredText(g, pnlSpectrogram.ClientRectangle, "无可用语谱图数据");
                return;
            }

            if (spectrogramBitmapCache == null || spectrogramBitmapCacheSize != plot.Size)
            {
                spectrogramBitmapCache?.Dispose();
                spectrogramBitmapCache = BuildSpectrogramBitmap(plot.Width, plot.Height);
                spectrogramBitmapCacheSize = plot.Size;
            }

            if (spectrogramBitmapCache != null)
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.DrawImage(spectrogramBitmapCache, plot);
            }

            DrawColorBar(g, bar, -100, 0);

            double maxTime = Math.Max(1e-9, spectrogramDurationSeconds);
            float markerX = plot.Left + (float)((trkTime.Value / 1000.0) / maxTime * plot.Width);
            markerX = Math.Clamp(markerX, plot.Left, plot.Right);
            g.DrawLine(Pens.Red, markerX, plot.Top, markerX, plot.Bottom);

            DrawXTicks(g, plot, 0, spectrogramDurationSeconds, 5, "0.0");
            DrawYTicks(g, plot, 0, audio.SampleRate / 2.0, 5, "0");

            if (spectrogramTruncated)
            {
                using var br = new SolidBrush(Color.FromArgb(200, Color.Black));
                using var ft = new Font("Segoe UI", 8f);
                g.DrawString("已截断显示前 30 秒", ft, br, plot.Left + 6, plot.Top + 4);
            }
        }

        private Rectangle GetSpectrogramPlotRect()
        {
            return new Rectangle(52, 20, pnlSpectrogram.Width - 92, pnlSpectrogram.Height - 60);
        }

        private Bitmap BuildSpectrogramBitmap(int width, int height)
        {
            var bmp = new Bitmap(Math.Max(1, width), Math.Max(1, height), PixelFormat.Format24bppRgb);

            if (spectrogramDb == null)
                return bmp;

            int frames = spectrogramDb.GetLength(0);
            int bins = spectrogramDb.GetLength(1);
            if (frames <= 0 || bins <= 0)
                return bmp;

            for (int x = 0; x < bmp.Width; x++)
            {
                int f = (int)((long)x * frames / bmp.Width);
                if (f >= frames) f = frames - 1;

                for (int y = 0; y < bmp.Height; y++)
                {
                    int srcY = bmp.Height - 1 - y;
                    int k = (int)((long)srcY * bins / bmp.Height);
                    if (k >= bins) k = bins - 1;

                    double db = spectrogramDb[f, k];
                    Color c = MapSpectrogramColor(db, -100, 0);
                    bmp.SetPixel(x, y, c);
                }
            }

            return bmp;
        }

        private static Color MapSpectrogramColor(double value, double min, double max)
        {
            double t = (value - min) / Math.Max(1e-9, max - min);
            t = Math.Clamp(t, 0.0, 1.0);

            // 简单 Turbo-like 渐变
            int r = (int)(255 * Math.Clamp(1.5 * t, 0, 1));
            int g = (int)(255 * Math.Clamp(1.5 * (1 - Math.Abs(t - 0.5) * 2), 0, 1));
            int b = (int)(255 * Math.Clamp(1.5 * (1 - t), 0, 1));
            return Color.FromArgb(r, g, b);
        }

        private static void DrawColorBar(Graphics g, Rectangle bar, double min, double max)
        {
            for (int y = 0; y < bar.Height; y++)
            {
                double t = 1.0 - y / Math.Max(1.0, bar.Height - 1.0);
                double v = min + (max - min) * t;
                using var p = new Pen(MapSpectrogramColor(v, min, max));
                g.DrawLine(p, bar.Left, bar.Top + y, bar.Right, bar.Top + y);
            }

            g.DrawRectangle(Pens.Black, bar);
            using var ft = new Font("Segoe UI", 7.5f);
            using var br = new SolidBrush(Color.Black);
            g.DrawString(max.ToString("0"), ft, br, bar.Right + 3, bar.Top - 6);
            g.DrawString(min.ToString("0"), ft, br, bar.Right + 3, bar.Bottom - 10);
        }

        private void PnlSpectrogram_MouseMove(object? sender, MouseEventArgs e)
        {
            var audio = CurrentAudio;
            if (audio == null || spectrogramDb == null)
                return;

            Rectangle plot = GetSpectrogramPlotRect();
            if (!plot.Contains(e.Location))
            {
                spectrogramToolTip.Hide(pnlSpectrogram);
                lastSpectrogramTipText = string.Empty;
                return;
            }

            int frames = spectrogramDb.GetLength(0);
            int bins = spectrogramDb.GetLength(1);
            if (frames <= 0 || bins <= 0) return;

            double xNorm = (e.X - plot.Left) / (double)Math.Max(1, plot.Width);
            double yNorm = 1.0 - (e.Y - plot.Top) / (double)Math.Max(1, plot.Height);

            xNorm = Math.Clamp(xNorm, 0, 1);
            yNorm = Math.Clamp(yNorm, 0, 1);

            int frame = Math.Min(frames - 1, (int)(xNorm * frames));
            int bin = Math.Min(bins - 1, (int)(yNorm * bins));

            double tSec = xNorm * spectrogramDurationSeconds;
            double freq = yNorm * (audio.SampleRate / 2.0);
            double db = spectrogramDb[frame, bin];

            string tip = $"t={tSec:F3}s, f={freq:F1}Hz, {db:F1}dB";
            if (!string.Equals(lastSpectrogramTipText, tip, StringComparison.Ordinal))
            {
                lastSpectrogramTipText = tip;
                spectrogramToolTip.Show(tip, pnlSpectrogram, e.Location.X + 15, e.Location.Y + 15, 1000);
            }
        }

        private void PnlSpectrogram_MouseLeave(object? sender, EventArgs e)
        {
            spectrogramToolTip.Hide(pnlSpectrogram);
            lastSpectrogramTipText = string.Empty;
        }

        private static void DrawPanelTitle(Graphics g, string title)
        {
            using var ft = new Font("Segoe UI", 9f, FontStyle.Bold);
            using var br = new SolidBrush(Color.Black);
            g.DrawString(title, ft, br, 8, 2);
        }

        private static void DrawAxes(Graphics g, Rectangle plot, string xLabel, string yLabel)
        {
            g.DrawRectangle(Pens.Black, plot);

            using var ft = new Font("Segoe UI", 8f);
            using var br = new SolidBrush(Color.Black);

            var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
            g.DrawString(xLabel, ft, br, plot.Left + plot.Width / 2f, plot.Bottom + 22, sfCenter);

            var state = g.Save();
            g.TranslateTransform(plot.Left - 36, plot.Top + plot.Height / 2f);
            g.RotateTransform(-90);
            g.DrawString(yLabel, ft, br, 0, 0, sfCenter);
            g.Restore(state);
        }

        private static void DrawCenteredText(Graphics g, Rectangle rect, string text)
        {
            using var ft = new Font("Segoe UI", 10f);
            using var br = new SolidBrush(Color.DimGray);
            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(text, ft, br, rect, sf);
        }

        private static void DrawXTicks(Graphics g, Rectangle plot, double min, double max, int tickCount, string fmt)
        {
            using var ft = new Font("Segoe UI", 7.5f);
            using var br = new SolidBrush(Color.Black);

            for (int i = 0; i <= tickCount; i++)
            {
                double t = i / (double)Math.Max(1, tickCount);
                double v = min + (max - min) * t;
                float x = (float)(plot.Left + t * plot.Width);

                g.DrawLine(Pens.Black, x, plot.Bottom, x, plot.Bottom + 4);
                string s = v.ToString(fmt);
                var size = g.MeasureString(s, ft);
                g.DrawString(s, ft, br, x - size.Width / 2f, plot.Bottom + 5);
            }
        }

        private static void DrawYTicks(Graphics g, Rectangle plot, double min, double max, int tickCount, string fmt)
        {
            using var ft = new Font("Segoe UI", 7.5f);
            using var br = new SolidBrush(Color.Black);

            for (int i = 0; i <= tickCount; i++)
            {
                double t = i / (double)Math.Max(1, tickCount);
                double v = max - (max - min) * t;
                float y = (float)(plot.Top + t * plot.Height);

                g.DrawLine(Pens.Black, plot.Left - 4, y, plot.Left, y);
                string s = v.ToString(fmt);
                var size = g.MeasureString(s, ft);
                g.DrawString(s, ft, br, plot.Left - size.Width - 6, y - size.Height / 2f);
            }
        }
    }

    internal sealed class AudioItem
    {
        public string FullPath { get; }
        public string FileName { get; }
        public int SampleRate { get; }
        public float[] Samples { get; }
        public double DurationSeconds => SampleRate <= 0 ? 0 : Samples.Length / (double)SampleRate;

        public AudioItem(string fullPath, string fileName, int sampleRate, float[] samples)
        {
            FullPath = fullPath;
            FileName = fileName;
            SampleRate = sampleRate;
            Samples = samples;
        }

        public override string ToString() => FileName;
    }

    internal sealed class BufferedPanel : Panel
    {
        public BufferedPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }
    }
}
