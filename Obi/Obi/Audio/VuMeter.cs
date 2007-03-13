using System;
using System.Threading;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace Obi.Audio
{
	public class VuMeter
	{
		// public event Events.Audio.VuMeter.LevelTooLowHandler LevelTooLow;
		public event Events.Audio.VuMeter.PeakOverloadHandler PeakOverload;
		public event Events.Audio.VuMeter.ResetHandler ResetEvent;
		public event Events.Audio.VuMeter.UpdateFormsHandler UpdateForms;

		/// <summary>
		/// Create the VU meter object.
		/// </summary>
        public VuMeter()
		{
			SetSampleCount(m_SampleTimeLength);
			m_SampleArrayPosition = 0;
		}

        /// <summary>
        /// Set the VU meter to listen to the audio player and recorder.
        /// </summary>
        public void SetEventHandlers()
        {
            AudioPlayer.Instance.UpdateVuMeter +=
                new Events.Audio.Player.UpdateVuMeterHandler(CatchUpdateVuMeterEvent);
            AudioRecorder.Instance.UpdateVuMeterFromRecorder +=
                new Events.Audio.Recorder.UpdateVuMeterHandler(CatchUpdateVuMeterEvent);
        }

		//Member variable used in properties
		private int m_Channels =2 ;
		private double m_ScaleFactor = 2 ;
		private double m_SampleTimeLength = 400 ;
		internal bool m_bOverload = false ;
        private int m_UpperThreshold = (int)((double)int.MaxValue * 0.8);
		private int m_LowerThreshold = (int)((double)int.MaxValue * 0.3);
		private int [] arPeakOverloadValue = new int [2] ;
		private bool [] arPeakOverloadFlag = new bool [2] ;
		
		public int Channels
		{
			get
			{
				return m_Channels ;
			}
			set
			{
				m_Channels = value ;
			}
		}

		public object Stream
		{
			set
			{
			}
		}
		
		public UserControl VisualControl
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public UserControl TextControl
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public int [] PeakValue
		{
			get
			{
				return arPeakOverloadValue ;
			}
		}

		public bool[] Overloaded
		{
			get
			{
				return arPeakOverloadFlag ;
			}
		}

		// properties added by app team india
		public int UpperThreshold 
		{
			get
			{
				return m_UpperThreshold ;
			}
			set
			{
				m_UpperThreshold = value ;
			}
		}

		public int LowerThreshold
		{
			get
			{
				return m_LowerThreshold ;
			}
			set
			{
				m_LowerThreshold = value ;
			}
		}

		public double ScaleFactor
		{
			get
			{
				return m_ScaleFactor ;
			}
			set
			{
				m_ScaleFactor = value ;
			}
		}


		public double SampleTimeLength
		{
			get
			{
				return m_SampleTimeLength ;
			}
			set
			{
				SetSampleCount (value) ;
			}
		}


		public void Reset()
		{
			arPeakOverloadValue [0] = 0 ;
			arPeakOverloadValue [1] = 0 ;
			arPeakOverloadFlag[0] = arPeakOverloadFlag [1] = false ;
			for (int i = 0 ; i< m_SampleCount ; i++)
			{
				SampleArrayLeft [i] = 0;
				SampleArrayRight [i] = 0;
			}
			m_SampleArrayPosition  = 0 ;
			m_MeanValueLeft = 0 ;
			m_MeanValueRight = 0 ;
			ResetEvent(this, new Events.Audio.VuMeter.ResetEventArgs());
			// ob_Reset.TriggerReset  (this , ob_Reset) ;
		}

		// calculate and sets the count of samples to be read for computing RMS or mean value of amplitude
		void SetSampleCount ( double TimeLengthArg ) 
		{
			m_SampleTimeLength  = TimeLengthArg ;
			m_SampleCount  = Convert.ToInt32 (TimeLengthArg / 50 ); // 50 is byte reading interval 
			SampleArrayLeft  = new int [ 2 * m_SampleCount] ;
			SampleArrayRight  = new int [2 * m_SampleCount] ;
		}

		// member variables for internal processing
		private byte [] m_arUpdatedVM  ;
		private int m_UpdateVMArrayLength ;
		private int m_FrameSize ;

		// for position of graph which is centre of graph in Y and centre of left edges of two graph for x
		private int m_GraphPositionX = 170 ;
		private int m_GraphPositionY = 300 ;
			

		private AudioPlayer ob_AudioPlayer ;
		private AudioRecorder  ob_AudioRecorder ;
		private int  m_SampleCount    ;
		internal int m_MeanValueLeft ;
		internal int m_MeanValueRight  ;
		private  int [] SampleArrayLeft ;
		private int [] SampleArrayRight ;
		private int m_SampleArrayPosition = 0;

		private bool boolPlayer =false ;

		// Handles VuMeter event from player
		public void CatchUpdateVuMeterEvent ( object sender , Events.Audio.Player.UpdateVuMeterEventArgs Update) 
		{
			boolPlayer = true ;
			ob_AudioPlayer = sender as AudioPlayer;
			m_FrameSize = ob_AudioPlayer.m_FrameSize ;
			m_Channels = ob_AudioPlayer.m_Channels ;
            lock (this)
            {
    			m_UpdateVMArrayLength = ob_AudioPlayer.m_UpdateVMArrayLength ;
                m_arUpdatedVM = new byte[m_UpdateVMArrayLength];
                Array.Copy(ob_AudioPlayer.arUpdateVM, m_arUpdatedVM, m_UpdateVMArrayLength);

                AnimationComputation();
            }
            //Thread UpdateVMForm = new Thread(new ThreadStart (AnimationComputation  ));
            //UpdateVMForm.IsBackground = true ;
            //UpdateVMForm.Start()  ;
		}

		// handles update event from audio recorder
		public void CatchUpdateVuMeterEvent (object sender , Events.Audio.Recorder.UpdateVuMeterEventArgs UpdateVuMeter)
		{
			AudioRecorder Recorder = sender as AudioRecorder ;
			ob_AudioRecorder = Recorder ;
			m_FrameSize = Recorder.m_FrameSize ;
			m_Channels = Recorder.m_Channels ;

            lock (this)
            {
                m_UpdateVMArrayLength = Recorder.m_UpdateVMArrayLength;
                m_arUpdatedVM = new byte[m_UpdateVMArrayLength];
                Array.Copy(Recorder.arUpdateVM, m_arUpdatedVM, m_UpdateVMArrayLength);

                AnimationComputation();
            }
            //Thread UpdateVMForm = new Thread(new ThreadStart (AnimationComputation  ));
            //UpdateVMForm.IsBackground = true;
            //UpdateVMForm.Start()  ;


		}

		int m_PeakValueLeft = 0;
		int m_PeakValueRight = 0;
		void AnimationComputation ()
		{
			//Thread.Sleep (25) ;
			// finds the origin i.e upper left corner of graph display rectangle
			//origin is computed from centre position of graph
			int OriginX = m_GraphPositionX -  Convert.ToInt32  ( ( m_ScaleFactor * 60 ));
			int OriginY =m_GraphPositionY- Convert.ToInt32 ( 125 * m_ScaleFactor) ;

			// create an local array and fill the amplitude value of both channels from function
			int [] AmpArray = new int [2] ;
			Array.Copy ( AmplitudeValue() , AmpArray , 2) ;

			// feed the amplitude in sampple array for computing mean value
				
			SampleArrayLeft [m_SampleArrayPosition ] = AmpArray [0] ;
			SampleArrayRight [m_SampleArrayPosition ] = AmpArray [1] ;


			m_SampleArrayPosition++ ;
			if (m_SampleArrayPosition >= m_SampleCount )
			{
				m_SampleArrayPosition = 0 ;

            }

			// Find Mean Values of Left and Right Channels 
			m_MeanValueLeft = m_MeanValueRight = 0 ;
			for (int i = 0 ; i < m_SampleCount ; i++ )
			{
				m_MeanValueLeft = m_MeanValueLeft + SampleArrayLeft [i] ;
				m_MeanValueRight = m_MeanValueRight + SampleArrayRight [i] ;
			}
			m_MeanValueLeft = m_MeanValueLeft / m_SampleCount ;
			m_MeanValueRight = m_MeanValueRight / m_SampleCount ;

            // update peak values if it is greater than previous value
			if (m_PeakValueLeft < m_MeanValueLeft)  
				arPeakOverloadValue[0] = m_PeakValueLeft = m_MeanValueLeft ;

			if (m_PeakValueRight < m_MeanValueRight )  
				arPeakOverloadValue[1] =  m_PeakValueRight = m_MeanValueRight ;


			// Check for Peak Overload  and fire event if overloaded
			if ( m_MeanValueLeft > m_UpperThreshold )
			{
				arPeakOverloadFlag [0] = true ;
				Events.Audio.VuMeter.PeakOverloadEventArgs e;
				if (boolPlayer)
				{
					e = new Events.Audio.VuMeter.PeakOverloadEventArgs(1,
						ob_AudioPlayer.GetCurrentBytePosition(), ob_AudioPlayer.GetCurrentTimePosition());
				}
				else
				{
                    e = new Events.Audio.VuMeter.PeakOverloadEventArgs(1, 0, 0);
				}
				PeakOverload(this, e);
			}
			else
			{
				arPeakOverloadFlag[0] = false ;
			}

			if ( m_MeanValueRight > m_UpperThreshold)
			{
				m_bOverload = true ;

				arPeakOverloadFlag [1] = true ;
				Events.Audio.VuMeter.PeakOverloadEventArgs e;
				if (boolPlayer)
				{
					e = new Events.Audio.VuMeter.PeakOverloadEventArgs(2,
						ob_AudioPlayer.GetCurrentBytePosition() , ob_AudioPlayer.GetCurrentTimePosition());
				}
				else
				{
					e = new Events.Audio.VuMeter.PeakOverloadEventArgs(2,0 , 0) ;
				}
				PeakOverload(this, e);				
			}
			else
			{
				arPeakOverloadFlag[1] = false;
			}

			// compute the cordinates of graph and animation
			DisplayGraph () ;

			int ThresholdFactor  = 12500 / ( m_UpperThreshold - m_LowerThreshold) ;
			int DisplayAmpLeft = (m_MeanValueLeft * ThresholdFactor )/100 ;
			int DisplayAmpRight = (m_MeanValueRight  * ThresholdFactor )/100 ;
			int Offset = 65 - ( (m_LowerThreshold * ThresholdFactor) / 100 ) ;
			DisplayAmpLeft = DisplayAmpLeft + Offset ;
			DisplayAmpRight = DisplayAmpRight + Offset ;

			Graph.EraserLeft = OriginY + Convert.ToInt32 (m_ScaleFactor * ( 254 - DisplayAmpLeft) ) ;
			Graph.EraserRight= OriginY + Convert.ToInt32 (m_ScaleFactor * ( 254 - DisplayAmpRight)) ;

			//Thread.Sleep (25) ;
			// Update ccurrent graph cordinates to VuMeter display
			UpdateForms(this, new Events.Audio.VuMeter.UpdateFormsEventArgs());
		}

		// calculates the amplitude of both channels from input taken from DirectX buffers
		int [] AmplitudeValue()
        {
            #region Old algorithm for computing amplitude
            /*
            long s0 =0 ;
			long s1 = 0 ;
			int Sum0 = 0 ;
			int Sum1 = 0 ;
			for (int i = 0 ; i< m_UpdateVMArrayLength  ; i=i+m_FrameSize)
			{
				if (m_FrameSize == 1)
					Sum0 = m_arUpdatedVM[0+i] ;
				else if (m_FrameSize == 2)
				{
					if (m_Channels == 2)
					{
						Sum0 = (m_arUpdatedVM [0+i]  ) ;
						Sum1 = m_arUpdatedVM [1+i] ;
					
					}
					else if (m_Channels == 1)
					{
						Sum0 = m_arUpdatedVM[1+i] * 256 ;
						Sum0 = Sum0 + m_arUpdatedVM[0 + i] ;
						Sum0 = (Sum0 * 256 ) / 65792 ;
					}
					// / end of if (FrameSize == 2)
				}
				else if (m_FrameSize == 4 && m_Channels == 2 )
				{
					Sum0 = m_arUpdatedVM[1+i] * 256 ;
					Sum0 = Sum0 + m_arUpdatedVM [0+i] ;
					Sum0 = (Sum0 * 256 ) /65792  ;

					Sum1 = m_arUpdatedVM[3+i] * 256 ;
					Sum1 = Sum1 + m_arUpdatedVM [2+i] ;
					Sum1 = (Sum1 * 256 ) / 65792  ;

				}
				s0 = s0 + Sum0 ;
				s1 = s1 + Sum1 ;
			}
			int Divisor = m_UpdateVMArrayLength  / m_FrameSize ;
			Sum0 = Convert.ToInt32 (s0 / Divisor ) ;
			Sum1 = Convert.ToInt32 (s1 / Divisor ) ;
            */
            #endregion

            #region New algorith for computing current volume based on amplitude
            /*
            int iLeftMax = int.MinValue;
            int iLeftMin = int.MaxValue;
            int iRightMax = int.MinValue;
            int iRightMin = int.MaxValue;
            int iCurrLeft = 0;
            int iCurrRight = 0;

            for (int i = 0; i < m_UpdateVMArrayLength; i = i + m_FrameSize)
            {
                if (m_FrameSize == 1)
                    iCurrLeft = m_arUpdatedVM[0 + i];
                else if (m_FrameSize == 2)
                {
                    if (m_Channels == 2)
                    {
                        iCurrLeft = (m_arUpdatedVM[0 + i]);
                        iCurrRight = m_arUpdatedVM[1 + i];

                    }
                    else if (m_Channels == 1)
                    {
                        iCurrLeft = m_arUpdatedVM[i] * byte.MaxValue + m_arUpdatedVM[i + 1];
                    }
                    // / end of if (FrameSize == 2)
                }
                else if (m_FrameSize == 4 && m_Channels == 2)
                {
                    iCurrLeft = m_arUpdatedVM[i + 0];// *byte.MaxValue + m_arUpdatedVM[i + 1];
                    iCurrRight = m_arUpdatedVM[i + 2];// *byte.MaxValue + m_arUpdatedVM[i + 3];

                }
                iLeftMin = (iCurrLeft < iLeftMin) ? iCurrLeft : iLeftMin;
                iLeftMax = (iCurrLeft > iLeftMax) ? iCurrLeft : iLeftMax;
                iRightMin = (iCurrRight < iRightMin) ? iCurrRight : iRightMin;
                iRightMax = (iCurrRight > iRightMax) ? iCurrRight : iRightMax;
            }

            if (m_FrameSize / m_Channels > 1)
            {
                iLeftMin /= byte.MaxValue;
                iRightMin /= byte.MaxValue;
                iLeftMax /= byte.MaxValue;
                iRightMax /= byte.MaxValue;
            }
            */
            #endregion 

            #region Calc. based on diff from half max val.
            /*
            System.Collections.Generic.List<int> rightValues = new System.Collections.Generic.List<int>();
            System.Collections.Generic.List<int> leftValues = new System.Collections.Generic.List<int>();

            for (int i = 0; i < m_UpdateVMArrayLength; i = i + m_FrameSize)
            {
                if (m_FrameSize == 1)
                    iCurrLeft = m_arUpdatedVM[0 + i];
                else if (m_FrameSize == 2)
                {
                    if (m_Channels == 2)
                    {
                        iCurrLeft = (m_arUpdatedVM[0 + i]);
                        iCurrRight = m_arUpdatedVM[1 + i];

                    }
                    else if (m_Channels == 1)
                    {
                        iCurrLeft = m_arUpdatedVM[i];// *byte.MaxValue + m_arUpdatedVM[i + 1];
                    }
                    // / end of if (FrameSize == 2)
                }
                else if (m_FrameSize == 4 && m_Channels == 2)
                {
                    iCurrLeft = m_arUpdatedVM[i + 0];// *byte.MaxValue + m_arUpdatedVM[i + 1];
                    iCurrRight = m_arUpdatedVM[i + 2];// *byte.MaxValue + m_arUpdatedVM[i + 3];

                }
                iCurrLeft = Math.Abs(iCurrLeft - (byte.MaxValue / 2));
                iCurrRight = Math.Abs(iCurrRight - (byte.MaxValue / 2));

                leftValues.Add(iCurrLeft);
                rightValues.Add(iCurrRight);

            }
            long leftSum = 0;
            long rightSum = 0;
            for (int i = 0; i < leftValues.Count; i++)
            {
                leftSum += leftValues[i];
                rightSum += rightValues[i];
            }
            leftSum = leftSum / leftValues.Count;
            rightSum = rightSum / leftValues.Count;

             */
            #endregion

            #region Cal. based on BitConverter.ToInt16(m_arUpdatedVM);

            int leftVal = 0;
            int rightVal = 0;

            switch (m_FrameSize)
            { 
                case 1:
                    //each byte is a simple sample

                    sbyte curVal = 0;
                    sbyte castVal = 0;
                    for (int i = 0; i < m_arUpdatedVM.Length; i++)
                    {
                        castVal = (sbyte)m_arUpdatedVM[i];
//                        castVal = Math.Abs(castVal);
                        if (castVal > curVal)
                            curVal = castVal;
                    }
                    leftVal = (int) curVal * (int.MaxValue / sbyte.MaxValue);
                    break;

                case 2:
                    switch (m_Channels)
                    { 
                        case 1:
                            short sCurVal = 0;
                            short sCastVal = 0;
                            for (int i = 0; i < m_arUpdatedVM.Length; i += 2)
                            {
                                sCastVal = BitConverter.ToInt16(m_arUpdatedVM, i);
//                                sCastVal = Math.Abs(sCastVal);
                                if (sCurVal < sCastVal)
                                    sCurVal = sCastVal;
                            }
                            leftVal = (int)sCurVal *(int.MaxValue/short.MaxValue);

                            break;
                        case 2:
                            sbyte curValLeft = 0;
                            sbyte castValLeft = 0;
                            sbyte curValRight = 0;
                            sbyte castValRight = 0;
                            for (int i = 0; i < m_arUpdatedVM.Length; i+=2)
                            {
                                castValLeft = (sbyte)m_arUpdatedVM[i];
//                                castValLeft = Math.Abs(castValLeft);
                                if (castValLeft > curValLeft)
                                    curValLeft = castValLeft;

                                castValRight = (sbyte)m_arUpdatedVM[i+1];
//                                castValRight = Math.Abs(castValRight);
                                if (castValRight > curValRight)
                                    curValRight = castValRight;
                            }
                            leftVal = (int)curValLeft *(int.MaxValue/sbyte.MaxValue);
                            rightVal = (int)curValRight * (int.MaxValue / sbyte.MaxValue);
                            break;
                        default:
                            break;
                    }
                    break;

                case 4:
                    short sCurValLeft = 0;
                    short sCastValLeft = 0;
                    short sCurValRight = 0;
                    short sCastValRight = 0;
                    for (int i = 0; i < m_arUpdatedVM.Length; i += 2)
                    {
                        sCastValLeft = BitConverter.ToInt16(m_arUpdatedVM, i);
//                        sCastValLeft = Math.Abs(sCastValLeft);
                        if (sCurValLeft < sCastValLeft)
                            sCurValLeft = sCastValLeft;
                        sCastValRight = BitConverter.ToInt16(m_arUpdatedVM, i);
//                        sCastValRight = Math.Abs(sCastValRight);
                        if (sCurValRight < sCastValRight)
                            sCurValRight = sCastValRight;
                    }
                    leftVal = (int)sCurValLeft * (int.MaxValue/short.MaxValue);
                    rightVal = (int)sCurValRight * (int.MaxValue / short.MaxValue);
                    break;

                default:
                    break;
            }
            
            #endregion


            int [] arSum= new int [2] ;
//            arSum[0] = Sum0;
//            arSum[1] = Sum1;
//            arSum[0] = iLeftMax - iLeftMin;
//            arSum[1] = iRightMax - iRightMin;

//            arSum[0] = (int)leftSum;
//            arSum[1] = (int)rightSum;

            arSum[0] = leftVal;
            arSum[1] = rightVal;

            System.Diagnostics.Debug.WriteLine(arSum[0]);

			return arSum ;
		}

		// calculates the values of graph cordinates with respect to position and scale factor
		public GraphPts Graph = new GraphPts() ;
		public void DisplayGraph	 ()
		{
			int OriginX = m_GraphPositionX -  Convert.ToInt32  ( ( m_ScaleFactor * 60 ));
			int OriginY =m_GraphPositionY- Convert.ToInt32 ( 125 * m_ScaleFactor) ;

			// Position of BackGround color of graph 
			Graph.BackGroundTop = OriginY - (Convert.ToInt32 (50 * m_ScaleFactor) ) ;
			Graph.BackGroundBottom = OriginY + (Convert.ToInt32 ( 304 * m_ScaleFactor)) ;
			Graph.BackGroundX= OriginX - (Convert.ToInt32(50 * m_ScaleFactor)) ;
			Graph.BackGroundWidth = Convert.ToInt32 ( 220 * m_ScaleFactor) ;

			// Position of tri colored vertical lines

			Graph.LineWidth = Convert.ToInt32 (30 * m_ScaleFactor) ;

			Graph.HighTop = OriginY ;
			Graph.HighBottom = OriginY + Convert.ToInt32 (m_ScaleFactor * 60) ;

			Graph.NormalTop = OriginY + Convert.ToInt32 (m_ScaleFactor * 67) ;
			Graph.NormalBottom = OriginY + Convert.ToInt32 (m_ScaleFactor * 187 ) ;

			Graph.LowTop  = OriginY + Convert.ToInt32 (m_ScaleFactor * 194) ;
			Graph.LowBottom = OriginY + Convert.ToInt32 (m_ScaleFactor * 254) ;

			Graph.LeftGraphX = OriginX ;
			Graph.RightGraphX = OriginX +Convert.ToInt32 (m_ScaleFactor * 60) ;

			Graph.PeakOverloadLightX = 240 ;
			Graph.PeakOverloadLightY = 30 ;
		}
		/*
				internal class Threshold
				{
			public int UpperThreshold ;
					public int LowerThreshold ;
				}
		*/
		public struct GraphPts
		{

			public int HighTop ;
			public int HighBottom ;

			public int NormalTop ;
			public int NormalBottom ;

			public int LowTop ;
			public int LowBottom ;

			public int LineWidth ;

			public int ScaleFactor ;
			public int LeftGraphX ;
			public int RightGraphX ;

			public int BackGroundWidth ;
			public int BackGroundTop ;
			public int BackGroundBottom ;
			public int BackGroundX ;

			public int EraserLeft ;
			public int EraserRight ;

			public int PeakOverloadLightX ;
			public int PeakOverloadLightY ;
		}
		// end of class
	}

}
