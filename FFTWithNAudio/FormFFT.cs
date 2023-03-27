using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        }

        private void updateFFT()
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
            if (dataFft == null)
            {
                dataFft = new double[fftPoints / 2];
                dataFft_Total = new double[fftPoints / 2];
            }
            for (int i = 0; i < fftPoints / 2; i++)
            {
                double fftLeft = Math.Abs(fftFull[i].X + fftFull[i].Y);
                double fftRight = Math.Abs(fftFull[fftPoints - i - 1].X + fftFull[fftPoints - i - 1].Y);
                dataFft[i] = fftLeft + fftRight;

                dataFft_Total[i] = Math.Max(dataFft_Total[i], dataFft[i]);
            }
        }

        private WaveInEvent wvin;
        double[] dataFft;
        double[] dataFft_Total;
        short[] dataPcm;

        #region FFT Chart with ZGraph
        private ucFFTChartPanel fftChart;
        private void InitializeFFTChartControl()
        {
            fftChart = new ucFFTChartPanel();
            fftChart.Parent = gbFrequency;
            fftChart.Dock = DockStyle.Fill;
        }
        #endregion
    }
}
