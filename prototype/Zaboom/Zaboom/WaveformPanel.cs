using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

using urakawa.properties.channel;
using urakawa.media.data;
using urakawa.media.data.audio;

namespace Zaboom
{
    public partial class WaveformPanel : UserControl
    {
        private Project project;
        private urakawa.core.TreeNode node;

        public static readonly int DEFAULT_PIXELS_PER_SECOND = 24;

        private static readonly Pen MONOPEN = new Pen(Color.FromArgb(192, 0, 0, 255), 1);
        private static readonly Pen STEREOPEN1 = new Pen(Color.FromArgb(128, 0, 0, 255), 1);
        private static readonly Pen STEREOPEN2 = new Pen(Color.FromArgb(128, 255, 0, 0), 1);

        private int pixelsPerSecond;
        private double samplesPerPixel;

        public WaveformPanel()
        {
            InitializeComponent();
        }

        public WaveformPanel(Project project, urakawa.core.TreeNode node, int pps)
        {
            InitializeComponent();
            this.project = project;
            this.node = node;
            PixelsPerSecond = pps;
        }

        public int PixelsPerSecond
        {
            get { return pixelsPerSecond; }
            set
            {
                pixelsPerSecond = value;
                Width = Convert.ToInt32(Math.Round(AudioData.getAudioDuration().getTimeDeltaAsMillisecondFloat() /
                    1000.0 * value));
                samplesPerPixel = Math.Ceiling((double)AudioData.getPCMFormat().getByteRate() /
                    AudioData.getPCMFormat().getBlockAlign() / value);
                Invalidate();
            }
        }

        private AudioMediaData AudioData
        {
            get
            {
                ChannelsProperty channelsProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
                Channel audioChannel = project.GetSingleChannelByName(Project.AUDIO_CHANNEL_NAME);
                ManagedAudioMedia media = (ManagedAudioMedia)channelsProp.getMedia(audioChannel);
                return media.getMediaData();
            }
        }

        // TODO: stereo
        // TODO: read a chunk of bytes
        // TODO: cache y values
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            byte[] shbuf = new byte[2];
            int sample = 0;
            int samples = AudioData.getPCMLength() / AudioData.getPCMFormat().getBlockAlign();
            int x = 0;
            System.IO.BinaryReader br = new System.IO.BinaryReader(AudioData.getAudioData());
            while (sample < samples)
            {
                short min = short.MaxValue;
                short max = short.MinValue;
                for (int s = 0; sample < samples && s < samplesPerPixel; ++s, ++sample)
                {
                    br.Read(shbuf, 0, 2);
                    short sh = BitConverter.ToInt16(shbuf, 0);
                    if (sh < min) min = sh;
                    if (sh > max) max = sh;
                }
                int ymin = Height - ((min - short.MinValue) * Height) / ushort.MaxValue;
                int ymax = Height - ((max - short.MinValue) * Height) / ushort.MaxValue;
                if (ymin == ymax) ++ymax;
                e.Graphics.DrawLine(MONOPEN, x, ymin, x, ymax);
                ++x;
            }
            br.Close();
        }
    }
}
