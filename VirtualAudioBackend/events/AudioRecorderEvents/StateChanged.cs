using System;
using System.Collections;
using System.Text;	

namespace VirtualAudioBackend.events.AudioRecorderEvents
{
	public delegate void StateChangedHandler(object sender, StateChanged e);  // JQ

	/// <summary>
	/// The state of the audio recorder changed.
	/// </summary>
	public class StateChanged : AudioRecorderEvent
	{
		private static readonly StateChanged mFromNotReady= new StateChanged(AudioRecorderState.NotReady);
		private static readonly StateChanged mFromIdle = new StateChanged(AudioRecorderState.Idle);
		private static readonly StateChanged mFromListening = new StateChanged(AudioRecorderState.Listening);
		private static readonly StateChanged mFromRecording = new StateChanged(AudioRecorderState.Recording);

		private AudioRecorderState mOldState;  // the previous state of the recorder. The current state of the player is readily available.

		public StateChanged FromNotReady{ get { return mFromNotReady;; } }
		public StateChanged FromIdle { get { return mFromIdle; } }
		public StateChanged FromListening { get { return mFromListening; } }
		public StateChanged FromRecording { get { return mFromRecording; } } 
		
		public AudioRecorderState OldState
		{
			get
			{
				return mOldState;
			}
		}

		/// <summary>	
		/// Create a new StateChanged event.
		/// </summary>
		/// <param name="oldState">The state of the recorder before the change.</param>
		public StateChanged(AudioRecorderState oldState)
		{
			mOldState = oldState;
		}
	}
}
