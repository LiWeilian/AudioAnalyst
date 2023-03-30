using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFTWithNAudio
{
    public partial class FormSpectrogram : Form
    {
        private static FormSpectrogram _instance;
        public static FormSpectrogram Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FormSpectrogram();
                }

                return _instance;
            }
        }

        public Image Image
        {
            get
            {
                return pic.Image;
            }
            set
            {
                pic.Image = value;
            }
        }

        private ImageScale _imageScale;
        public ImageScale ImageScale
        {
            get
            {
                return _imageScale;
            }
            set
            {
                _imageScale = value;
                this.Text = $"频谱图    {_imageScale.ToString()}";
            }
        }

        private FormSpectrogram()
        {
            InitializeComponent();
        }

        ToolTip toolTip;

        private void DisplayTips(int x, int y)
        {
            if (ImageScale != null)
            {
                double seconds = ImageScale.X * x / pic.Width;
                double frequency = ImageScale.Y * (pic.Height - y) / pic.Height;

                if (toolTip == null)
                {
                    toolTip = new ToolTip();
                }

                string s = (seconds - Math.Truncate(seconds)).ToString("0.###");
                if (s.StartsWith("0."))
                {
                    s = s.Substring(2);
                }

                string toolTipText = $"时间：{Utils.ConvertIntToTimeFormat((int)Math.Truncate(seconds))}.{s}\r\n频率：{frequency:###} Hz";
                try
                {
                    toolTip.SetToolTip(pic, toolTipText);
                }
                catch (Exception)
                {

                }
            }
        }

        private void FormSpectrogram_FormClosed(object sender, FormClosedEventArgs e)
        {
            _instance = null;
        }

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            DisplayTips(e.X, e.Y);
        }
    }

    public class ImageScale
    {
        public double X { get; set; }
        public double Y { get; set; }

        public override string ToString()
        {
            return $"时长：{X}s    频率范围：0 ~ {Y}Hz";
        }
    }
}
