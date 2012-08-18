using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using AudioLib;
using Obi.Audio;
using Obi.Events.Audio.Recorder;
using urakawa.data;
using urakawa.media.data;
using urakawa.media.data.audio ;
using urakawa.media.data.audio.codec;
using urakawa.media.timing;


namespace Obi
{
    /// <summary>
    /// A recording session during which new assets are recorded. Phrases, sections and pages are created.
    /// This replaces the Record dialog.
    /// </summary>
    public class RecordingSession
    {
        private ObiPresentation mPresentation;                     // presentation to record in
        private AudioRecorder mRecorder;                        // recorder for the session

        private ManagedAudioMedia mSessionMedia;                // session asset (?)
        private int mSessionOffset;                             // offset from end of last part of the session
        private List<double> mPhraseMarks ;                     // list of phrase marks
        private List<int> mSectionMarks;                        // list of section marks (necessary?)
        private List<ManagedAudioMedia> mAudioList;             // list of assets created
        private Timer mRecordingUpdateTimer;                    // timer to send regular "recording" messages
        private Settings m_Settings;

        public event StartingPhraseHandler StartingPhrase;      // start recording a new phrase
        public event ContinuingPhraseHandler ContinuingPhrase;  // a new phrase is being recorded (time update)
        public event FinishingPhraseHandler FinishingPhrase;    // finishing a phrase
        public event FinishingPageHandler FinishingPage;        // finishing a page

        private void OnAudioRecordingFinished(object sender, AudioRecorder.AudioRecordingFinishEventArgs e)
        {
            mRecorder.AudioRecordingFinished -= OnAudioRecordingFinished;
            bool deleteAfterInsert = true;
                if (deleteAfterInsert)
                {
                    FileDataProvider dataProv = (FileDataProvider)mSessionMedia.Presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                    dataProv.InitByMovingExistingFile(e.RecordedFilePath);
                    mSessionMedia.AudioMediaData.AppendPcmData(dataProv);
                }
                else
                {
                    // TODO: progress ! (time consuming file copy)
                    mSessionMedia.AudioMediaData.AppendPcmData_RiffHeader(e.RecordedFilePath);
                }

                if (deleteAfterInsert && File.Exists(e.RecordedFilePath)) //check exist just in case file adopted by DataProviderManager
                {
                    File.Delete(e.RecordedFilePath);
                }
                m_PhDetectorBytesRecorded = 0;
        }


        /// <summary>
        /// Create a recording session for a project starting from a given node.
        /// </summary>
        /// <param name="project">The project in which we are recording.</param>
        /// <param name="recorder">The audio recorder from the project.</param>
        public RecordingSession(ObiPresentation presentation, AudioRecorder recorder, Settings settings)
        {
            mPresentation = presentation;
            mRecorder = recorder;
            
            mRecorder.RecordingDirectory =
                presentation.DataProviderManager.DataFileDirectoryFullPath;
            if (!Directory.Exists(mRecorder.RecordingDirectory)) Directory.CreateDirectory(mRecorder.RecordingDirectory);
            mSessionOffset = 0;
            mPhraseMarks = null;
            mSectionMarks = null;
            mAudioList = new List<ManagedAudioMedia>();
            mRecordingUpdateTimer = new Timer();
            mRecordingUpdateTimer.Tick += new System.EventHandler(mRecordingUpdateTimer_tick);
            mRecordingUpdateTimer.Interval = 1000;
            m_Settings = settings;
            mRecorder.PcmDataBufferAvailable += new AudioLib.AudioRecorder.PcmDataBufferAvailableHandler(DetectPhrasesOnTheFly);
        }


        /// <summary>
        /// The audio recorder used by the recording session.
        /// </summary>
        public AudioRecorder AudioRecorder { get { return mRecorder; } }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase.
        /// The phrase that was just finished receives a page number as well (auto-generated.)
        /// </summary>
        public void MarkPage()
        {
            if (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Recording)
            {
                //recorder.TimeOfAsset
                double timeOfAssetMilliseconds =
                    (double)mRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64 (mRecorder.CurrentDurationBytePosition))/
                    Time.TIME_UNIT;

            // check for illegal time input
                if (mPhraseMarks != null && mPhraseMarks.Count > 0 && mPhraseMarks[mPhraseMarks.Count - 1] >= timeOfAssetMilliseconds)
                return;

                PhraseEventArgs e = FinishedPhrase();
                if (StartingPhrase != null)
                    StartingPhrase(this, new PhraseEventArgs(mSessionMedia, mPhraseMarks.Count, 0.0));
                if (FinishingPage != null) FinishingPage(this, e);
            }
        }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase.
        /// </summary>
        public void NextPhrase()
        {
            if (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Recording)
            {
                //mRecorder.TimeOfAsset
                double timeOfAssetMilliseconds =
                    (double)mRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64 (mRecorder.CurrentDurationBytePosition)) /
                    Time.TIME_UNIT;

                // check for illegal time input
            if (mPhraseMarks != null && mPhraseMarks.Count > 0    &&    mPhraseMarks[mPhraseMarks.Count - 1] >=
                timeOfAssetMilliseconds
                )
                return;

                FinishedPhrase();
                if (StartingPhrase != null)
                    StartingPhrase(this, new PhraseEventArgs(mSessionMedia, mSessionOffset + mPhraseMarks.Count, 0.0));
            }
        }

        /// <summary>
        /// Start recording. Stop monitoring before starting recording.
        /// </summary>
        public void Record()
        {
            if (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Stopped)
            {
                mSessionOffset = mAudioList.Count;
                mPhraseMarks = new List<double>();
                mSectionMarks = new List<int>();
                AudioMediaData asset =
                    (AudioMediaData)mPresentation.MediaDataFactory.Create<WavAudioMediaData>();
                mSessionMedia = (ManagedAudioMedia)mPresentation.MediaFactory.CreateManagedAudioMedia();
                //mSessionMedia.setMediaData(asset);
                mSessionMedia.MediaData = asset;
                mRecorder.AudioRecordingFinished += OnAudioRecordingFinished;
                mRecorder.StartRecording(asset.PCMFormat.Data);
                if (StartingPhrase != null)
                    StartingPhrase(this, new PhraseEventArgs(mSessionMedia, mSessionOffset, 0.0));
                mRecordingUpdateTimer.Enabled = true;
            }
        }

        /// <summary>
        /// The list of recorded asset, in the order in which they were recorded during the session.
        /// </summary>
        public List<ManagedAudioMedia> RecordedAudio { get { return mAudioList; } }

        /// <summary>
        /// Start monitoring the audio input.
        /// This may happen at the beginning of the session,
        /// or when recording is paused.
        /// Create a new asset to "record" in (it gets discarded anyway.)
        /// </summary>
        public void StartMonitoring()
        {
            if (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Stopped)
            {
                AudioMediaData asset =
                    (AudioMediaData)mPresentation.MediaDataFactory.Create<WavAudioMediaData>();
                mRecorder.StartMonitoring(asset.PCMFormat.Data);
            }
        }

        /// <summary>
        /// Stop recording or monitoring.
        /// </summary>
        public void Stop()
        {
            bool wasRecording = mRecorder.CurrentState == AudioLib.AudioRecorder.State.Recording;

            if (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring
                || wasRecording)
            {
                if (wasRecording   &&   mPhraseMarks.Count > 0 ) FinishedPhrase();
                mRecorder.StopRecording();
                if (wasRecording)
                {
                    // Split the session asset into smaller assets starting from the end
                    // (to keep the split times correct) until the second one
                    for (int i = mPhraseMarks.Count - 2; i >= 0; --i)
                    {
                    if (mPhraseMarks[i] < mSessionMedia.Duration.AsTimeSpan.TotalMilliseconds)
                        {
                            ManagedAudioMedia split = mSessionMedia.Split(new Time(Convert.ToInt64(mPhraseMarks[i] * Time.TIME_UNIT)));
                        mAudioList.Insert ( mSessionOffset, split );
                        }
                    else
                        MessageBox.Show ( Localizer.Message ( "RecordingSession_SplitError" ) , Localizer.Message ("Caption_Warning"));
                    }
                    // The first asset is what remains of the session asset
                    mAudioList.Insert(mSessionOffset, mSessionMedia);
                }
                mRecordingUpdateTimer.Enabled = false;
            }
        }


        // Finish recording of the current phrase.
        private PhraseEventArgs FinishedPhrase()
        {
            //mRecorder.TimeOfAsset
            double timeOfAssetMilliseconds =
                (double)mRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64 (mRecorder.CurrentDurationBytePosition)) /
                Time.TIME_UNIT;

            mPhraseMarks.Add(timeOfAssetMilliseconds);
            int last = mPhraseMarks.Count - 1;
            double length = mPhraseMarks[last] - (last == 0 ? 0.0 : mPhraseMarks[last - 1]);
            length = length - (length % 100);
            PhraseEventArgs e = new PhraseEventArgs(mSessionMedia, mSessionOffset + last, length, timeOfAssetMilliseconds);
            if (FinishingPhrase != null) FinishingPhrase(this, e);
            return e;
        }

        // Send recording update
        private void mRecordingUpdateTimer_tick(object sender, EventArgs e)
        {
            //mRecorder.TimeOfAsset
            double timeOfAssetMilliseconds =
                (double)mRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64 (mRecorder.CurrentDurationBytePosition)) /
                Time.TIME_UNIT;

            double time = timeOfAssetMilliseconds - (mPhraseMarks.Count > 0 ? mPhraseMarks[mPhraseMarks.Count - 1] : 0.0);
            time = time - (time % 100);
            if (ContinuingPhrase != null)
                ContinuingPhrase(this, new PhraseEventArgs(mSessionMedia, mSessionOffset + mPhraseMarks.Count, time, timeOfAssetMilliseconds));
        }

        private byte[] m_MemStreamArray;
        private int m_PhDetectionMemStreamPosition;
        private long m_PhDetectorBytesRecorded;

        private void DetectPhrasesOnTheFly(object sender, AudioLib.AudioRecorder.PcmDataBufferAvailableEventArgs e)
        {
            ApplyPhraseDetectionOnTheFly(e);
        }

        private void ApplyPhraseDetectionOnTheFly(AudioLib.AudioRecorder.PcmDataBufferAvailableEventArgs e)
        {
            return;//letsverify with on the fly phrase detection first
            // todo : associate this function to recorder VuMeter events
            int overlapLength = 0;
            int msLentth = 0;
            if (mRecorder.RecordingPCMFormat != null)
            {
                overlapLength = Convert.ToInt32(0.250 * (mRecorder.RecordingPCMFormat.BlockAlign * mRecorder.RecordingPCMFormat.SampleRate));
                overlapLength = (Math.Abs(overlapLength / e.PcmDataBufferLength) * e.PcmDataBufferLength);
                msLentth = Convert.ToInt32(e.PcmDataBufferLength * 32);
            }

            if ((m_MemStreamArray == null || m_PhDetectionMemStreamPosition > (msLentth - e.PcmDataBufferLength)) && mRecorder.RecordingPCMFormat != null)
            {

                if (m_MemStreamArray != null)
                {
                    //m_PhDetectionMemoryStream.ReadByte();
                    long threshold = (long)m_Settings.DefaultThreshold;
                    long GapLength = (long)m_Settings.DefaultGap;
                    long before = (long)m_Settings.DefaultLeadingSilence;
                    Console.WriteLine("on the fly ph detection parameters " + threshold + " : " + GapLength);
                    AudioLib.AudioLibPCMFormat audioPCMFormat = new AudioLib.AudioLibPCMFormat(mRecorder.RecordingPCMFormat.NumberOfChannels, mRecorder.RecordingPCMFormat.SampleRate, mRecorder.RecordingPCMFormat.BitDepth);
                    List<long> timingList = AudioLib.PhraseDetection.Apply(new System.IO.MemoryStream(m_MemStreamArray),
                        audioPCMFormat,
                        threshold,
                        (long)GapLength * AudioLib.AudioLibPCMFormat.TIME_UNIT,
                        (long)before * AudioLib.AudioLibPCMFormat.TIME_UNIT);
                    if (timingList != null)
                    {
                        Console.WriteLine("timingList " + timingList.Count);
                        double overlapTime = mRecorder.RecordingPCMFormat.ConvertBytesToTime(overlapLength);
                        foreach (double d in timingList)
                        {
                            Console.WriteLine("Overlap time and list time " + (overlapTime / AudioLibPCMFormat.TIME_UNIT) + " : " + (d / AudioLibPCMFormat.TIME_UNIT));
                            if (d >= overlapTime )
                            {
                                double phraseTime = d - overlapTime;
                                double timeInSession = (mRecorder.RecordingPCMFormat.ConvertBytesToTime(m_PhDetectorBytesRecorded - msLentth) + d) / AudioLib.AudioLibPCMFormat.TIME_UNIT;
                                Console.WriteLine("phrase time: " + phraseTime + " : " + timeInSession);
                                //if (PhraseCreatedEvent != null) PhraseCreatedEvent(this, new Audio.PhraseDetectedEventArgs(timeInSession));

                                mPhraseMarks.Add(timeInSession);
                                int last = mPhraseMarks.Count - 1;
                                double length = mPhraseMarks[last] - (last == 0 ? 0.0 : mPhraseMarks[last - 1]);
                                length = length - (length % 100);
                                PhraseEventArgs eArg = new PhraseEventArgs(mSessionMedia, mSessionOffset + last, length, timeInSession);

                                if (FinishingPhrase != null) FinishingPhrase(this, eArg);
                                if (StartingPhrase != null)
                                    StartingPhrase(this, new PhraseEventArgs(mSessionMedia, mSessionOffset + mPhraseMarks.Count, 0.0));

                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("timing list is null ");
                    }
                    
                }

                byte[] overlapData = null;
                if (m_MemStreamArray != null)
                {
                    overlapData = new byte[overlapLength];
                    Array.Copy(m_MemStreamArray, m_MemStreamArray.Length - overlapLength - 1, overlapData, 0, overlapData.Length);
                    m_MemStreamArray = new byte[msLentth + overlapLength];
                    overlapData.CopyTo(m_MemStreamArray, 0);
                }
                else
                {
                    overlapLength = 0;
                    m_MemStreamArray = new byte[msLentth];
                }
                //Console.WriteLine("newMemStream length  " + m_MemStreamArray.Length);
                m_PhDetectionMemStreamPosition = overlapLength;
                m_PhDetectorBytesRecorded += msLentth;


                e.PcmDataBuffer.CopyTo(m_MemStreamArray, m_PhDetectionMemStreamPosition);
                m_PhDetectionMemStreamPosition += e.PcmDataBufferLength;
                //m_PhDetectionMemoryStream.Write(e.PcmDataBuffer, (int)m_PhDetectionMemStreamPosition, e.PcmDataBuffer.Length);
                //m_PhDetectionMemStreamPosition = m_PhDetectionMemoryStream.Position;
                Console.WriteLine("first writing of recorder buffer " + m_PhDetectionMemStreamPosition);
            }
            else if (m_MemStreamArray != null && e.PcmDataBuffer != null)
            {
                int leftOverLength = Convert.ToInt32(msLentth - m_PhDetectionMemStreamPosition);
                if (leftOverLength > e.PcmDataBuffer.Length) leftOverLength = e.PcmDataBuffer.Length;
                //Console.WriteLine("length:position:leftOver " + m_MemStreamArray.Length + " : " + m_PhDetectionMemStreamPosition + " : " + leftOverLength + " : " + e.PcmDataBuffer.Length);
                //m_PhDetectionMemoryStream.Write(e.PcmDataBuffer,0, leftOverLength);
                //m_PhDetectionMemStreamPosition = m_PhDetectionMemoryStream.Position;
                //m_MemStreamArray = new byte[msLentth];
                //m_PhDetectionMemoryStream.ToArray().CopyTo(m_MemStreamArray, 0); ;
                e.PcmDataBuffer.CopyTo(m_MemStreamArray, m_PhDetectionMemStreamPosition);
                m_PhDetectionMemStreamPosition += e.PcmDataBufferLength;

                Console.WriteLine("writing recorder buffer " + m_PhDetectionMemStreamPosition);

            }

        }

        
    }
}