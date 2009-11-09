using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AudioLib;
using System.Xml;

namespace AudioFormatConverterUI
    {

    public partial class m_audioFormatConverterForm : Form
        {
        private XmlDocument m_SettingsDocument;

        public m_audioFormatConverterForm ()
            {
            InitializeComponent ();
            m_cb_channel.SelectedIndex = 0;
            m_cb_sampleRate.SelectedIndex = 0;

            try
                {
                LoadSettings ();
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            }

        private void m_btn_Add_Click ( object sender, EventArgs e )
            {
            OpenFileDialog addFile = new OpenFileDialog ();
            addFile.RestoreDirectory = true;
            addFile.Multiselect = true;
            //addFile.SafeFileNames = true;
            addFile.Filter = " MP3 Files(*.mp3)|*.mp3| Wave Files (*.wav)|*.wav| All Files (*.*)|*.*";

            if (addFile.ShowDialog ( this ) == DialogResult.OK)
                {
                if (addFile.FileNames.Length > 0)
                    {
                    foreach (string audioFileName in addFile.FileNames)
                        {
                        string filename = Path.GetFullPath ( audioFileName );
                        addFile.CheckFileExists = true;
                        addFile.CheckPathExists = true;
                        m_lb_addFiles.Items.Add ( filename );
                        }
                    }

                }
            }

        private void m_btn_Browse_Click ( object sender, EventArgs e )
            {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog ();
            browserDialog.ShowNewFolderButton = true;
            browserDialog.SelectedPath = m_txt_Browse.Text;

            if (browserDialog.ShowDialog ( this ) == DialogResult.OK)
                {
                m_txt_Browse.Text = browserDialog.SelectedPath;
                if (CheckIfDriveSelected ()) { return; }
                }
            }
     
        private bool CheckIfDriveSelected ()
            {
            bool flag = false;
            string[] fixedDrives = Environment.GetLogicalDrives ();
            foreach (string drive in fixedDrives)
                {
                if (m_txt_Browse.Text.Equals ( drive, StringComparison.OrdinalIgnoreCase ))
                    {
                    MessageBox.Show ( " Its a root directory , you cannot save here. Please select some other Directory. ", "Root Directory", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                    m_txt_Browse.Clear ();
                    flag = true;
                    }
                }
            return flag;
            }

        private void m_btn_cancel_Click ( object sender, EventArgs e )
            {
            Close ();
            }
        private void m_btnDelete_Click ( object sender, EventArgs e )
            {
            try
                {
                m_lb_addFiles.Items.Remove ( m_lb_addFiles.SelectedItem );
                }
            catch (Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            }
        private void m_btnReset_Click ( object sender, EventArgs e )
            {
            m_lb_addFiles.Items.Clear ();
            m_txt_Browse.Clear ();
            }

        private void m_btn_Start_Click ( object sender, EventArgs e )
            {
            IWavFormatConverter audioConverter = new WavFormatConverter ( true );
            int samplingRate = int.Parse ( m_cb_sampleRate.SelectedItem.ToString () );
            int channels = m_cb_channel.SelectedIndex + 1;
            int bitDepth = 16;
            string outputDirectory = m_txt_Browse.Text;
            string convertedFilePath = null;

            if (!Directory.Exists ( outputDirectory )) return;

            int listPositionIndex = 0;

            while (m_lb_addFiles.Items.Count > listPositionIndex && listPositionIndex < 50)
                {
                string filePath = (string)m_lb_addFiles.Items[listPositionIndex];
                //MessageBox.Show ( filePath );
                try
                    {
                    if (Path.GetExtension ( filePath ) == ".wav")
                        {
                        convertedFilePath = audioConverter.ConvertSampleRate ( filePath, outputDirectory, channels, samplingRate, bitDepth );
                        }
                    else if (Path.GetExtension ( filePath ) == ".mp3")
                        {
                        convertedFilePath = audioConverter.UnCompressMp3File ( filePath, outputDirectory, channels, samplingRate, bitDepth );
                        }
                    // rename converted file to appropriate name
                    string newFilePath = Path.Combine ( outputDirectory,
                        Path.GetFileNameWithoutExtension ( filePath ) ) + ".wav";
                    //MessageBox.Show ( newFilePath );
                    if (File.Exists ( newFilePath ))
                        {
                        if (MessageBox.Show ( "File: " + Path.GetFileName ( newFilePath ) + "  already exists. Do you want to overwrite it?", "Warning", MessageBoxButtons.YesNo ) == DialogResult.Yes)
                            {
                            File.Delete ( newFilePath );
                            File.Move ( convertedFilePath, newFilePath );
                            }
                        }
                    else
                        {
                        File.Move ( convertedFilePath, newFilePath );
                        }

                    m_lb_addFiles.Items.RemoveAt ( 0 );
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( ex.ToString () );
                    listPositionIndex++;
                    }
                }
                m_txt_Browse.Clear();
            }

        private void LoadSettings ()
            {
            string settingsFilePath = Path.Combine (
                System.AppDomain.CurrentDomain.BaseDirectory,
                "settings.xml" );
            m_SettingsDocument = CommonFunctions.CreateXmlDocument ( settingsFilePath );
            string strSamplingRate = m_SettingsDocument.GetElementsByTagName ( "samplingrate" )[0].InnerText;

            int samplingRate = int.Parse ( strSamplingRate );

            for (int i = 0; i < m_cb_sampleRate.Items.Count; i++)
                {

                if (int.Parse ( m_cb_sampleRate.Items[i].ToString () ) == samplingRate)
                    {
                    m_cb_sampleRate.SelectedIndex = i;
                    }
                }

            string strChannels = m_SettingsDocument.GetElementsByTagName ( "channels" )[0].InnerText;
            int channels = int.Parse ( strChannels );

            m_cb_channel.SelectedIndex = channels - 1;

            }

        }//m_audioFormatConverterForm Class
    }//AudioFormatConverterUI Namespace