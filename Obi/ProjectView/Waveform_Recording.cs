using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using AudioLib;

namespace Obi.ProjectView
{
    public partial class Waveform_Recording : UserControl
    {
        private ContentView m_ContentView = null;
        private ProjectView m_ProjectView = null;
        private float m_ZoomFactor;
        private AudioLib.VuMeter m_VUMeter;
        private System.Drawing.Graphics g;
        private Pen br1 = new Pen(SystemColors.Highlight);
        private Pen br_Channel2 = new Pen(SystemColors.Highlight);
        private Pen br2 = new Pen(SystemColors.ControlText);
        private EmptyNode m_ExistingPhrase = null;
        private int m_Counter = 0;
        private int m_CounterForInterval = 0;
        private int m_LocalCount = 0;

        public Waveform_Recording()
        {
            InitializeComponent();
            m_ZoomFactor = 1.0f;
            this.Height = Convert.ToInt32(104 * m_ZoomFactor);
            g = this.CreateGraphics();
            if (m_ProjectView != null && m_ProjectView.TransportBar.RecordingPhrase != null)
                m_ExistingPhrase = m_ProjectView.TransportBar.RecordingPhrase;
        }

        public ContentView contentView
        {
            get { return m_ContentView; }
            set { m_ContentView = value; }
        }


        public ProjectView projectView
        {
            get { return m_ProjectView; }
            set 
            { 
                m_ProjectView = value;
                if(m_ProjectView != null)
                m_ProjectView.TransportBar.Recorder.PcmDataBufferAvailable += new AudioRecorder.PcmDataBufferAvailableHandler(OnPcmDataBufferAvailable_Recorder);
            }
        }

        public AudioLib.VuMeter VUMeter
        {
            get { return m_VUMeter; }
            set { m_VUMeter = value; }
        }

        public float zoomFactor
        {
            get { return m_ZoomFactor; }
            set
            {
                m_ZoomFactor = value;
                ZoomWaveform();
            }
        }

        public void ZoomWaveform()
        {
            if (m_ContentView != null)
                this.Size = new Size(m_ContentView.Width, Convert.ToInt32(104 * m_ZoomFactor));
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            Pen pen = new Pen(Color.Black);
            int m_X = m_ContentView.Width / 2 - 15;
            int x_Loc = m_X + (-1 * Location.X);
            string text = "";
            Font myFont = new Font("Microsoft Sans Serif", 7);
            Pen newPen = new Pen(SystemColors.Control);

            if (m_VUMeter == null || m_ContentView == null) return;
            g = this.CreateGraphics();
            //Console.WriteLine("height waveform " + (Height - (int)Math.Round(((min - short.MinValue) * Height) / (float)ushort.MaxValue)) + " : " + (Height - (int)Math.Round(((max - short.MinValue) * Height) / (float)ushort.MaxValue)));
            g.DrawLine(br1, new Point(x_Loc, Height - (int)Math.Round(((minChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)),
                    new Point(x_Loc, Height - (int)Math.Round(((maxChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)));
            
            if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels > 1)
            {
                g.DrawLine(br_Channel2, new Point(x_Loc, Height - (int)Math.Round(((minChannel2 - short.MinValue) * Height) / (float)ushort.MaxValue)),
                       new Point(x_Loc, Height - (int)Math.Round(((maxChannel2 - short.MinValue) * Height) / (float)ushort.MaxValue)));
            }
            
            g.DrawLine(br2, 0, Height / 2, m_ContentView.Width, Height / 2);
            Location = new Point(Location.X - 1, Location.Y);
            this.Width = this.Width + 50;
            g.DrawLine(br2, 0, Height / 2, Width, Height / 2);

            if (m_ProjectView.TransportBar.CurrentState != TransportBar.State.Monitoring && m_ExistingPhrase != m_ProjectView.TransportBar.RecordingPhrase)
             {
                 if (m_ProjectView.TransportBar.RecordingPhrase.Role_ == EmptyNode.Role.Page)
                     text = "Page";
                 else if (m_ProjectView.TransportBar.RecordingPhrase.Role_ == EmptyNode.Role.Plain)
                     text = "Phrase";
                 g.DrawLine(pen, x_Loc, 0, x_Loc, Height);
                 g.DrawString(text, myFont, Brushes.Black, x_Loc, 0);
                 m_ExistingPhrase = m_ProjectView.TransportBar.RecordingPhrase;
             }
             m_Counter++;
             if (m_Counter == 10)
             {
                 g.DrawLine(newPen, x_Loc, 0, x_Loc, Height);
                 m_Counter = 0;
                 m_CounterForInterval++;
             }
             
             if (m_CounterForInterval % 10 == 0 && m_LocalCount != m_CounterForInterval)
             {
                 text = m_CounterForInterval.ToString();
                 g.DrawString(text, myFont, Brushes.Gray, x_Loc, 0);
                 m_LocalCount = m_CounterForInterval;
             }
         }

        private void Waveform_Recording_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
                timer1.Start();
            else 
            {
                Location = new Point(0, Location.Y);
                timer1.Stop();
            }
        }

        private short[] m_Amp = new short[2];
        private int m_AmpValue = 0;
        short minChannel1;
        short maxChannel1;
        short minChannel2;
        short maxChannel2;

        public void OnPcmDataBufferAvailable_Recorder(object sender, AudioRecorder.PcmDataBufferAvailableEventArgs e)
        {
            if (e.PcmDataBuffer != null && e.PcmDataBuffer.Length > 1)
            {
                m_Amp[0] = e.PcmDataBuffer[0];
                //m_Amp[1] = e.PcmDataBuffer[1];

                minChannel1 = short.MaxValue;
                            maxChannel1 = short.MinValue;
int channels = m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels ;
int channel = 0;
                int frameSize = m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.BlockAlign ;
                short [] samples = new short[e.PcmDataBufferLength];
                Buffer.BlockCopy(e.PcmDataBuffer,0,samples,0,e.PcmDataBufferLength ) ;

            //for (int i = 0 ; i < (int)Math.Ceiling(e.PcmDataBufferLength/ (float)frameSize); i += frameSize)
                for (int i = channel; i < (int)Math.Ceiling(e.PcmDataBufferLength/ (float)frameSize); i += channels)
            {
                
                                if (samples [i] < minChannel1) minChannel1 = samples [i];
                                if (samples[i] > maxChannel1) maxChannel1 = samples [i];
            }

            if (channels > 1)
            {
                minChannel1 = short.MaxValue;
                maxChannel1 = short.MinValue;
                channel = 1;
                for (int i = channel; i < (int)Math.Ceiling(e.PcmDataBufferLength / (float)frameSize); i += channels)
                {

                    if (samples[i] < minChannel2) minChannel2 = samples[i];
                    if (samples[i] > maxChannel2) maxChannel2 = samples[i];
                }
            }
                m_AmpValue =  m_Amp[0] + (int) (m_Amp[1] *  Math.Pow(8, m_Amp.Length));
            }
        }

    }
}