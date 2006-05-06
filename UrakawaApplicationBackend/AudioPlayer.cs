using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using Microsoft.DirectX.AudioVideoPlayback;
using Microsoft.DirectX.DirectInput ;

namespace UrakawaApplicationBackend
{
	public class AudioPlayer : IAudioPlayer
	{
	
		// declare member variables
		IAudioMediaAsset m_Asset ;
		SecondaryBuffer SoundBuffer;
		Microsoft.DirectX.DirectSound.Device 			 SndDevice= null ;
		BufferDescription BufferDesc = null ;
		FileStream fs ;		int m_BufferCheck ;
		int m_SizeBuffer ;
		int m_RefreshLength ;
		long m_lLength;
		long m_lPlayed ;
		Thread  RefreshThread ;
		enum CurrentState {stopped, playing, paused} ;
		
		byte [] 	ByteBuffer ;
		bool m_PlayFile ;
		byte [] RefreshArray;
		long m_lArrayPosition;
		bool m_FastPlay ;
		int m_Step ;
		int m_FrameSize ;
		int m_SamplingRate ;
		CurrentState state ;

		//static counter to implement singleton
		private static int m_ConstructorCounter =0;

// constructor 
		public 		AudioPlayer ()
		{
			if (m_ConstructorCounter == 0)
			{
				state = CurrentState.stopped ;
				m_PlayFile = true ;
				m_FastPlay = false ;
				m_Step = 10 ;
			}
			else
			{
				throw new Exception("This class is Singleton");
				
			}
		}

		public int State
		{
			get
			{
				return Convert.ToInt32 (state) ;
			}
		}
		

		public Microsoft.DirectX.DirectSound.Device 			   OutputDevice
		{
			get
			{
				return SndDevice ;
			}
			set
			{
				SndDevice = value ;
			}
		}

		public IAudioMediaAsset CurrentAsset
		{
			get
			{
				return m_Asset ;
			}
		}

		public bool FastPlay
		{
		get
		{
return m_FastPlay ;
	}
			set
			{
m_FastPlay = value ;
			}
	}

		public int CompFactor
		{
			get
			{
return m_Step ;
			}
			set
			{
m_Step = value ;
			}
		}

		public long CurrentBytePosition
		{
			get
			{
				return GetCurrentBytePosition();
			}
			set
			{
				SetCurrentBytePosition (value) ;
			}
		}

		public double CurrentTimePosition
		{
			get
			{
				return GetCurrentTimePosition() ;
			}
			set
			{
				SetCurrentTimePosition (value) ;
			}
		}

		public ArrayList GetOutputDevices()
		{
			CollectOutputDevices() ;
			ArrayList OutputDevices = new ArrayList ();

for(int i = 0; i < devList.Count; i++) 
{
	OutputDevices.Add (devList[i].Description);
	
}
			return OutputDevices ;
		}

		public void SetDevice (Control FormHandle, int Index)
		{
Microsoft.DirectX.DirectSound.Device dSound = new  Microsoft.DirectX.DirectSound.Device ( devList [Index].DriverGuid);
			dSound.SetCooperativeLevel(FormHandle, CooperativeLevel.Priority);
			SndDevice  = dSound ;
		}

						DevicesCollection devList ;
		DevicesCollection  CollectOutputDevices()
		{
			devList = new DevicesCollection();
return devList  ;
			}

// Functions


		public void Play(IAudioMediaAsset asset )
		{
			m_Asset = asset ;

InitPlay(0, 0) ;
		}

		void InitPlay(long lStartPosition, long lEndPosition)
		{
			CalculationFunctions calc = new CalculationFunctions() ;
// Check if anything is playing or input length is out of bound
			if (state.Equals  (CurrentState.stopped))
			{
				// Adjust the start and end position according to frame size
				lStartPosition = calc.AdaptToFrame(lStartPosition, m_Asset.FrameSize) ;
				lEndPosition = calc.AdaptToFrame(lEndPosition, m_Asset.FrameSize) ;
m_SamplingRate = m_Asset.SampleRate ;
				// creates file stream from file
				fs = new FileStream (m_Asset .Path, FileMode.Open,  								FileAccess.Read) ;

				// lEndPosition = 0 means that file is played to end
				if (lEndPosition != 0)
				{
					m_lLength = lEndPosition ;
				}
				else
				{
					m_lLength = m_Asset .SizeInBytes - 44 ;
				}

				// set the file pointer position ahead header of file
				fs.Position= lStartPosition + 44 ;

				WaveFormat newFormat = new WaveFormat () ;				
				BufferDesc = new BufferDescription();

				// retrieve format from file
				m_FrameSize = m_Asset.FrameSize ;
				newFormat.AverageBytesPerSecond = m_Asset .SampleRate * m_Asset .FrameSize ;
				newFormat.BitsPerSample = Convert.ToInt16(m_Asset .BitDepth) ;
				newFormat.BlockAlign = Convert.ToInt16(m_Asset .FrameSize );
				newFormat.Channels  = Convert.ToInt16(m_Asset .Channels );

				newFormat.FormatTag = WaveFormatTag.Pcm ;

				newFormat.SamplesPerSecond = m_Asset .SampleRate ;

				// loads  format to buffer description
				BufferDesc.Format = newFormat ;

				// calculate size of buffer so as to contain 1 second of audio
				m_SizeBuffer = m_Asset .SampleRate *  m_Asset .FrameSize ;
				m_RefreshLength = (m_Asset .SampleRate / 2 ) * m_Asset .FrameSize ;

				// sets the calculated size of buffer
				BufferDesc.BufferBytes = m_SizeBuffer ;

				// initialising secondary buffer
				SoundBuffer= new SecondaryBuffer(BufferDesc, SndDevice);

				// check for fast play
				int reduction = 0 ;
				if (m_FastPlay == false)
				{
					SoundBuffer.Write (0 , fs , m_SizeBuffer, 0) ;
				}
				else
				{
					// for fast play buffer is filled in parts with new part overlapping previous part
					for (int i = 0 ; i< m_SizeBuffer ; i = i + (m_Step * m_FrameSize))
					{
						SoundBuffer.Write (i , fs , m_Step * m_FrameSize, 0) ;
						i = i - (2*m_FrameSize) ;
						// compute the difference in bytes skipped
						reduction = reduction + (2*m_FrameSize) ;
					}

				}
				// Adds the length (count) of file played into a variable
				m_lPlayed = m_SizeBuffer + reduction;

				m_PlayFile = true ;
				
state = CurrentState.playing ;
				// starts playing
				SoundBuffer.Play(0, BufferPlayFlags.Looping);
				m_BufferCheck = 1 ;

				//initialise and start thread for refreshing buffer
				RefreshThread = new Thread(new ThreadStart (RefreshBuffer));
				RefreshThread.Start() ;

// end of playing check 
			}
			// end of function
		}


		
		void RefreshBuffer ()
		{
// variable to count byte difference in compressed and non compressed data of audio file
int reduction = 0 ;
			while (m_lPlayed < m_lLength)
			{//1
reduction = 0 ;
				Thread.Sleep (50) ;
				// check if play cursor is in second half , then refresh first half else second
				if ((m_BufferCheck% 2) == 1 &&  SoundBuffer.PlayPosition > m_RefreshLength) 
				{//2
					// Checks if file is played or byteBuffer is played
					if (m_PlayFile == true)
					{//3
						// check for normal or fast play
						if (m_FastPlay == false)
						{//4
							SoundBuffer.Write (0 , fs , m_RefreshLength, 0) ;
						}//-4
						else
						{//4
							for (int i = 0 ; i< m_RefreshLength; i=i+(m_Step*m_FrameSize))
							{
								SoundBuffer.Write ( i, fs , m_Step*m_FrameSize, 0) ;
i = i-(2*m_FrameSize );
								reduction = reduction + (2* m_FrameSize);
							}//-4

						}//-3
					}//-2
					else
					{//2
for (int i = 0 ; i< m_RefreshLength ; i++)
RefreshArray[i] = ByteBuffer [m_lArrayPosition + i] ;

SoundBuffer.Write (0 , RefreshArray,  0) ;
m_lArrayPosition = m_lArrayPosition + m_RefreshLength ;
					}//-2
					m_lPlayed = m_lPlayed + m_RefreshLength+ reduction;

					m_BufferCheck++ ;
				}//-1
				else if ((m_BufferCheck % 2 == 0) &&  SoundBuffer.PlayPosition < m_RefreshLength)
				{//1
					if (m_PlayFile == true)
					{//2
						if (m_FastPlay == false)
						{//3
							SoundBuffer.Write (m_RefreshLength, fs , m_RefreshLength, 0)  ;						}//-3						else						{//3							for (int i = 0 ; i< m_RefreshLength; i=i+(m_Step*m_FrameSize))
							{//4
								SoundBuffer.Write ( i+ m_RefreshLength, fs , m_Step*m_FrameSize, 0) ;
								i = i-(2*m_FrameSize) ;
								reduction = reduction + (2* m_FrameSize);
															}						//-4
							
							// end of FastPlay check
						}//-3					}//-2					else					{//2						for (int i = 0 ; i< m_RefreshLength ; i++)
							RefreshArray[i] = ByteBuffer [m_lArrayPosition + i] ;						SoundBuffer.Write (m_RefreshLength , RefreshArray,  0) ;
						m_lArrayPosition = m_lArrayPosition + m_RefreshLength ;						// end of file check					}//-2
					m_lPlayed = m_lPlayed + m_RefreshLength+ reduction;

					m_BufferCheck++ ;

// end of even/ odd part of buffer;
				}//-1
				
					
// end of while
			}
// calculate time to stop according to remaining data
			int time ;
			long lRemaining = (m_lPlayed - m_lLength);
			double dTemp ;
			
				dTemp= ((m_RefreshLength + m_RefreshLength - lRemaining			)*1000)/m_RefreshLength ;
time = Convert.ToInt32(dTemp * 0.48 );

//if (m_FastPlay == true)
//time = (time-250) * (1- (2/m_Step));

			Thread.Sleep (time) ;
			SoundBuffer.Stop () ;
state = CurrentState.stopped ;
				
			// RefreshBuffer ends
		}
		
		public void Play(IAudioMediaAsset  asset, double timeFrom)
		{

			m_Asset = asset ;
CalculationFunctions calc = new CalculationFunctions () ;
long lPosition = calc.ConvertTimeToByte (timeFrom, m_Asset .SampleRate, m_Asset .FrameSize) ;
			lPosition = calc.AdaptToFrame(lPosition, m_Asset .FrameSize) ;

			if(lPosition>0   && lPosition < m_Asset.LengthByte)
			{
				InitPlay ( lPosition, 0 );
			}
			else
			{
MessageBox.Show ("Parameters out of range") ;
			}
		}

// contains the end position  to play to be used in starting playing  after seeking
long lByteTo = 0 ;		
		public void Play(IAudioMediaAsset asset , double timeFrom, double timeTo)
		{
			m_Asset = asset ;
			CalculationFunctions calc = new CalculationFunctions () ;

			long lStartPosition = calc.ConvertTimeToByte (timeFrom, m_Asset .SampleRate, m_Asset .FrameSize) ;
			lStartPosition = calc.AdaptToFrame(lStartPosition, m_Asset .FrameSize) ;

			long lEndPosition = calc.ConvertTimeToByte (timeTo , m_Asset.SampleRate, m_Asset.FrameSize) ;
			lByteTo = lEndPosition ;

// check for valid arguments
if (lStartPosition>0 && lStartPosition < lEndPosition && lEndPosition <= m_Asset.LengthByte)
																						   {
																							   InitPlay ( lStartPosition, lEndPosition );
																						   }
			else
			{
				MessageBox.Show("Arguments out of range") ;
			}
		}

		public void Play(byte [] byteArray)
		{
			
CalculationFunctions calc = new CalculationFunctions () ;
			
			//declare   array variable of size 4 as the max chunk in header is 4 bytes long
			int [] Ar = new int[4] ;

			// retrieve the format of audio from header of byte stream
			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;
Ar[0] = Convert.ToInt32(byteArray [22]);
			Ar[1] = Convert.ToInt32(byteArray [23]);
long lTemp = calc.ConvertToDecimal (Ar) ;
		short shChannels = Convert.ToInt16 (lTemp) ;


			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;
			Ar[0] = Convert.ToInt32(byteArray [24]);
			Ar[1] = Convert.ToInt32(byteArray [25]);
			lTemp = calc.ConvertToDecimal (Ar) ;
			int iSamplingRate = Convert.ToInt32 (lTemp) ;
			m_SamplingRate = iSamplingRate ;


			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;
			Ar[0] = Convert.ToInt32(byteArray [32]);
			Ar[1] = Convert.ToInt32(byteArray [33]);
			lTemp = calc.ConvertToDecimal (Ar) ;
			short shFrameSize = Convert.ToInt16 (lTemp) ;
m_FrameSize = Convert.ToInt32 (shFrameSize) ;


			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;
			Ar[0] = Convert.ToInt32(byteArray [34]);
			Ar[1] = Convert.ToInt32(byteArray [35]);
			lTemp = calc.ConvertToDecimal (Ar) ;
			short shBitDepth = Convert.ToInt16 (lTemp) ;

// get the maximum length to play
m_lLength = byteArray.LongLength ;
			
// set the read position ahead of header
			m_lLength = m_lLength - 44 ;

			// fill the byteBuffer to be played  from byte array
ByteBuffer = new byte [m_lLength] ;
			for (long i = 0 ; i< m_lLength ; i++)
ByteBuffer[i] = byteArray [i+44] ;

// sets the wave format of secondary buffer
			WaveFormat newFormat = new WaveFormat () ;				
			BufferDesc = new BufferDescription();


			newFormat.AverageBytesPerSecond = iSamplingRate * shFrameSize ;
			newFormat.BitsPerSample = shBitDepth ;
			newFormat.BlockAlign = shFrameSize ;
			newFormat.Channels  = shChannels ;

			newFormat.FormatTag = WaveFormatTag.Pcm ;

			newFormat.SamplesPerSecond = iSamplingRate ;

			// gets the length to be played
m_lLength = calc.AdaptToFrame(m_lLength , Convert.ToInt32(shFrameSize)) ;

			BufferDesc.Format = newFormat ;

// gets the size of buffer to be created
			m_SizeBuffer = iSamplingRate *   shFrameSize;
			m_RefreshLength = (iSamplingRate / 2 ) * shFrameSize;


			BufferDesc.BufferBytes = m_SizeBuffer ;


				// initialise the buffer
SoundBuffer= new SecondaryBuffer(BufferDesc, SndDevice);

			//creates temporary byte array to fill secondary buffer
RefreshArray = new byte[m_SizeBuffer] ;
			for (int i = 0 ; i< m_SizeBuffer ; i++)
				RefreshArray[i] = ByteBuffer [i];

// set the current position to be filled next time
m_lArrayPosition = m_SizeBuffer ;

SoundBuffer.Write (0 , RefreshArray, 0) ;
m_lPlayed = m_SizeBuffer ;
			RefreshArray = new byte [m_RefreshLength] ;

m_PlayFile = false ;
state = CurrentState.playing ;
			SoundBuffer.Play(0, BufferPlayFlags.Looping);
			m_BufferCheck= 1 ;
			RefreshThread = new Thread(new ThreadStart (RefreshBuffer));
			RefreshThread.Start() ;

			// end of play byteBuffer
		}



		public void Pause()
		{
			if (state.Equals(CurrentState.playing))
			{
				SoundBuffer.Stop () ;
				
				state = CurrentState.paused ;
			}
		}

		public void Resume()
		{
			if (state.Equals (CurrentState.paused))
			{
				
				state = CurrentState.playing ;
				SoundBuffer.Play(0, BufferPlayFlags.Looping);
			}			
		}

		public void Stop()
		{
			if (SoundBuffer.Status.Looping )			
			{
				SoundBuffer.Stop () ;

state = CurrentState.stopped ;
				RefreshThread.Abort () ;
			
				if (m_PlayFile == true)
					fs.Close();
				else
					m_lArrayPosition = 0 ;
			}

		}

		long GetCurrentBytePosition()
		{
int PlayPosition  = SoundBuffer.PlayPosition;
			
			long lCurrentPosition ;
			if (PlayPosition < m_RefreshLength)
			{ 
				lCurrentPosition = m_lPlayed - ( 2 * m_RefreshLength) + PlayPosition ;
			}
			else
			{
lCurrentPosition = m_lPlayed - (3 * m_RefreshLength) + PlayPosition ;
			}
return lCurrentPosition ;
		}

		double GetCurrentTimePosition()
		{
			CalculationFunctions calc = new CalculationFunctions () ;
			long lPos = GetCurrentBytePosition ();
			double dTime = calc.ConvertByteToTime (lPos , m_SamplingRate, m_FrameSize) ;
			return dTime ;
		}		

		void SetCurrentBytePosition (long localPosition) 
		{
			if (SoundBuffer.Status.Looping)
			{
				if ( m_PlayFile== true)
				{

					Stop();
					InitPlay(localPosition , lByteTo);
				}
				
			}
			else if (!SoundBuffer.Status.Looping)
			{
				if (m_PlayFile == true)
				{
					fs.Position = localPosition ;
				}
			}
			else if(state.Equals (CurrentState.paused) )
			{
				Stop();
				InitPlay(localPosition , lByteTo);
				SoundBuffer.Stop () ;
			}
		}


		void SetCurrentTimePosition (double localPosition) 
		{
CalculationFunctions calc = new CalculationFunctions() ;
			long lTemp = calc.ConvertTimeToByte (localPosition, m_SamplingRate, m_FrameSize);
SetCurrentBytePosition(lTemp) ;
		}

		public void  test ()
		{
if (state.Equals(CurrentState.stopped))
	MessageBox.Show("Stopped") ;
		}
// End Class
	}
	// End NameSpace
}
