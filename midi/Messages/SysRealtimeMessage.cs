/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 05/08/2004
 */

using System;

namespace Multimedia.Midi
{
    /// <summary>
    /// Represents the method that will handle SysRealtimeEvent events from an
    /// InputDevice.
    /// </summary>
    public delegate void SysRealtimeEventHandler(object sender, SysRealtimeEventArgs e);
    
    /// <summary>
    /// Represents the various system realtime message types.
    /// </summary>
    public enum SysRealtimeType
    {
        /// <summary>
        /// Represents the clock system realtime type.
        /// </summary>
        Clock = 0xF8,

        /// <summary>
        /// Represents the tick system realtime type.
        /// </summary>
        Tick,

        /// <summary>
        /// Represents the start system realtime type.
        /// </summary>
        Start,

        /// <summary>
        /// Represents the continue system realtime type.
        /// </summary>
        Continue,

        /// <summary>
        /// Represents the stop system realtime type.
        /// </summary>
        Stop,    
    
        /// <summary>
        /// Represents the active sense system realtime type.
        /// </summary>
        ActiveSense = 0xFE,

        /// <summary>
        /// Represents the reset system realtime type.
        /// </summary>
        Reset
    }

	/// <summary>
	/// Represents Midi system realtime messages.
	/// </summary>
	/// <remarks>
	/// System realtime messages are Midi messages that are primarily concerned 
	/// with controlling and synchronizing Midi devices. 
	/// </remarks>
	public class SysRealtimeMessage : ShortMessage
	{
        #region SysRealtimeMessage Members

        #region Fields

        // The system realtime type.
        private SysRealtimeType type;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the SysRealtimeMessage class.
        /// </summary>
        /// <param name="type">
        /// The type of system realtime message.
        /// </param>
		public SysRealtimeMessage(SysRealtimeType type)
		{
            Type = type;
		}

        /// <summary>
        /// Initializes a new instance of the SysRealtimeMessage class with a
        /// system realtime message packed as an integer.
        /// </summary>
        /// <param name="message">
        /// The packed system realtime message to use for initialization.
        /// </param>
        public SysRealtimeMessage(int message)
        {
            // Enforce preconditions.
            if(!SysRealtimeMessage.IsSysRealtimeMessage(message))
                throw new ArgumentException(
                    "Message is not a system realtime message.", "message");

            // Initialize type.
            Type = (SysRealtimeType)message;
        }

        /// <summary>
        /// Initializes a new instance of the SysRealtimeMessage class with 
        /// another instance of the SysRealtimeMessage class.
        /// </summary>
        /// <param name="message">
        /// The SysRealtimeMessage instance to use for initialization.
        /// </param>
        public SysRealtimeMessage(SysRealtimeMessage message)
        {
            Type = message.Type;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tests to see if a status value belongs to a system realtime 
        /// message.
        /// </summary>
        /// <param name="status">
        /// The status value to test.
        /// </param>
        /// <returns>
        /// <b>true</b> if the status value belongs to a system realtime 
        /// message; otherwise, <b>false</b>.
        /// </returns>
        public static bool IsSysRealtimeMessage(int status)
        {
            bool result = false;

            // If the status value is in range for clock and stop type
            // system realtime type messages
            if(status >= (int)SysRealtimeType.Clock && 
                status <= (int)SysRealtimeType.Stop)
            {
                // Indicate that this is a system realtime message.
                result = true;
            }
            // Else if the status value is in range for the active sense and 
            // reset messages
            else if(status == (int)SysRealtimeType.ActiveSense ||
                status == (int)SysRealtimeType.Reset)
            {
                // Indicate that this is a system realtime message.
                result = true;
            }
            
            return result;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the system realtime type.
        /// </summary>
        public SysRealtimeType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                SetStatus((int)type);
            }
        }

        #endregion       

        #region Overrides

        /// <summary>
        /// Creates a clone of this instance of the SysRealtimeMessage class.
        /// </summary>
        /// <returns>
        /// A clone of this instance of the SysRealtimeMessage class.
        /// </returns>
        public override object Clone()
        {
            return new SysRealtimeMessage(this);
        }

        /// <summary>
        /// Accepts an IMidiMessageVisitor.
        /// </summary>
        /// <param name="visitor">
        /// The IMidiMessageVisitor to accept.
        /// </param>
        public override void Accept(IMidiMessageVisitor visitor)
        {
            visitor.Visit(this);
        }

        #endregion

        #endregion
	}

    /// <summary>
    /// Represents data for SysRealtimeEvent events.
    /// </summary>
    public class SysRealtimeEventArgs : EventArgs
    {
        #region SysRealtimeEventArgs Members

        #region Fields

        // The SysRealtimeMessage for this event.
        private SysRealtimeMessage message;

        // Time in milliseconds since the input device began recording.
        private int timeStamp;
       
        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the SysRealtimeEventArgs class with 
        /// the specified SysRealtimeMessage and time stamp.
        /// </summary>
        /// <param name="message">
        /// The SysRealtimeMessage for this event.
        /// </param>
        /// <param name="timeStamp">
        /// The time in milliseconds since the input device began recording.
        /// </param>
        public SysRealtimeEventArgs(SysRealtimeMessage message, int timeStamp)
        {
            this.message = message;
            this.timeStamp = timeStamp;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the SysRealtimeMessage for this event.
        /// </summary>
        public SysRealtimeMessage Message
        {
            get
            {
                return message;
            }
        }

        /// <summary>
        /// Gets the time in milliseconds since the input device began 
        /// recording.
        /// </summary>
        public int TimeStamp
        {
            get
            {
                return timeStamp;
            }
        }

        #endregion
        
        #endregion
    }
}
