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
    /// Represents the method that will handle the event that occurs when a 
    /// system common message is received.
    /// </summary>
    public delegate void SysCommonEventHandler(object sender, SysCommonEventArgs e);

    /// <summary>
    /// Represents the various system common message types.
    /// </summary>
    public enum SysCommonType
    {
        /// <summary>
        /// Represents the MTC system common message type.
        /// </summary>
        MidiTimeCode = 0xF1,

        /// <summary>
        /// Represents the song position pointer type.
        /// </summary>
        SongPositionPointer,

        /// <summary>
        /// Represents the song select type.
        /// </summary>
        SongSelect,

        /// <summary>
        /// Represents the tune request type.
        /// </summary>
        TuneRequest = 0xF6
    }

	/// <summary>
	/// Represents Midi system common messages.
	/// </summary>
	public class SysCommonMessage : ShortMessage
	{
        #region SysCommonMessage Members

        #region Fields

        // The system common type.
        private SysCommonType type;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the SysCommonMessage class with the
        /// specified type.
        /// </summary>
        /// <param name="type">
        /// The type of system common message.
        /// </param>
		public SysCommonMessage(SysCommonType type)
		{
            Type = type;
		}

        /// <summary>
        /// Initializes a new instance of the SysCommonMessage class with the 
        /// specified type and the first data value.
        /// </summary>
        /// <param name="type">
        /// The type of system common message.
        /// </param>
        /// <param name="data1">
        /// The first data value.
        /// </param>
        public SysCommonMessage(SysCommonType type, int data1)
        {
            Type = type;
            Data1 = data1;
        }

        /// <summary>
        /// Initializes a new instance of the SysCommonMessage class with the 
        /// specified type, first data value, and second data value.
        /// </summary>
        /// <param name="type">
        /// The type of system common message.
        /// </param>
        /// <param name="data1">
        /// The first data value.
        /// </param>
        /// <param name="data2">
        /// The second data value.
        /// </param>
        public SysCommonMessage(SysCommonType type, int data1, int data2)
        {
            Type = type;
            Data1 = data1;
            Data2 = data2;
        }

        /// <summary>
        /// Initializes a new instance of the SysCommonMessage class with a
        /// system common message packed as an integer.
        /// </summary>
        /// <param name="message">
        /// The packed system common message to use for initialization.
        /// </param>
        public SysCommonMessage(int message)
        {
            // Enforce preconditions.
            if(!SysCommonMessage.IsSysCommonMessage(message))
                throw new ArgumentException(
                    "Message is not a system common message.", "message");

            //
            // Initialize properties.
            //

            Type = (SysCommonType)ShortMessage.UnpackStatus(message);
            Data1 = ShortMessage.UnpackData1(message);
            Data2 = ShortMessage.UnpackData2(message);
        }

        /// <summary>
        /// Initializes a new instance of the SysCommonMessage class with 
        /// another instance of the SysCommonMessage class.
        /// </summary>
        /// <param name="message">
        /// The SysCommonMessage instance to use for initialization.
        /// </param>
        public SysCommonMessage(SysCommonMessage message)
        {
            Type = message.Type;
            Data1 = message.Data1;
            Data2 = message.Data2;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tests to see if a status value belongs to a system common message.
        /// </summary>
        /// <param name="status">
        /// The status value to test.
        /// </param>
        /// <returns>
        /// <b>true</b> if the status value belongs to a system common message; 
        /// otherwise, <b>false</b>.
        /// </returns>
        public static bool IsSysCommonMessage(int status)
        {
            bool result = false;

            if(status >= (int)SysCommonType.MidiTimeCode && 
                status <= (int)SysCommonType.SongSelect)
            {
                result = true;
            }
            else if(status == (int)SysCommonType.TuneRequest)
            {
                result = true;
            }

            return result;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the system common type.
        /// </summary>
        public SysCommonType Type
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

        /// <summary>
        /// Gets or sets the first data value.
        /// </summary>
        public int Data1
        {
            get
            {
                return GetData1();
            }
            set
            {
                SetData1(value);
            }
        }

        /// <summary>
        /// Gets or sets the second data value.
        /// </summary>
        public int Data2
        {
            get
            {
                return GetData2();
            }
            set
            {
                SetData2(value);
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Creates a clone of this instance of the SysCommonMessage class.
        /// </summary>
        /// <returns>
        /// A clone of this instance of the SysCommonMessage class.
        /// </returns>
        public override object Clone()
        {
            return new SysCommonMessage(this);
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
    /// Represents data for SysCommonEvent events.
    /// </summary>
    public class SysCommonEventArgs : EventArgs
    {
        #region SysCommonEventArgs Members

        #region Fields

        // The SysCommonMessage for this event.
        private SysCommonMessage message;

        // Time in milliseconds since the input device began recording.
        private int timeStamp;        

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the SysCommonEventArgs class with the
        /// specified SysCommonMessage and time stamp.
        /// </summary>
        /// <param name="message">
        /// The SysCommonMessage for this event.
        /// </param>
        /// <param name="timeStamp">
        /// The time in milliseconds since the input device began recording.
        /// </param>
        public SysCommonEventArgs(SysCommonMessage message, int timeStamp)
        {
            this.message = message;
            this.timeStamp = timeStamp;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the SysCommonMessage for this event.
        /// </summary>
        public SysCommonMessage Message
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
