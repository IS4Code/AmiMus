/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 05/07/2004
 */

using System;
using System.Text;

namespace Multimedia.Midi
{
    /// <summary>
    /// Represents the method that will handle the event that occurs when a
    /// system exclusive message is received.
    /// </summary>
    public delegate void SysExEventHandler(object sender, SysExEventArgs e);

    /// <summary>
    /// Represents various system exclusive message types.
    /// </summary>
    public enum SysExType
    {
        /// <summary>
        /// Represents the start of system exclusive message type.
        /// </summary>
        Start = 0xF0,

        /// <summary>
        /// Represents either the continuation or the escape system 
        /// exclusive message type.
        /// </summary>
        Special = 0xF7
    }

	/// <summary>
	/// Represents Midi system exclusive messages.
	/// </summary>
    public class SysExMessage : IMidiMessage
    {
        #region SysExMessage Members

        #region Fields

        // The system exclusive type.
        private SysExType type;

        // The system exclusive message data.
        private StringBuilder message;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the SysExMessage class with the
        /// specified system exclusive type and data.
        /// </summary>
        /// <param name="type">
        /// The type of system exclusive message.
        /// </param>
        /// <param name="data">
        /// The system exclusive data.
        /// </param>
        public SysExMessage(SysExType type, byte[] data)
        {
            this.type = type;

            // If this is a regular system exclusive message.
            if(this.type == SysExType.Start)
            {
                // Create storage for message data which includes an extra byte
                // for the status value.
                message = new StringBuilder(data.Length + 1);
                message.Length = message.Capacity;

                // Store status value.
                message[0] = (char)this.type;
            }
            // Else this is a continuation message or an escaped message.
            else
            {
                message = new StringBuilder(data.Length);
            }

            // Copy data into message.
            for(int i = 0; i < data.Length; i++)
            {
                this[i] = data[i];
            }
        }

        /// <summary>
        /// Initializes a new instance of the SysExMessage class with 
        /// another instance of the SysExMessage class.
        /// </summary>
        /// <param name="message">
        /// The SysExMessage instance to use for initialization.
        /// </param>
        public SysExMessage(SysExMessage message)
        {
            type = message.type;

            this.message = new StringBuilder(message.Message);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tests to see if a status value belongs to a system exclusive 
        /// message.
        /// </summary>
        /// <param name="status">
        /// The status value to test.
        /// </param>
        /// <returns>
        /// <b>true</b> if the status value belongs to a system exclusive 
        /// message; otherwise, <b>false</b>.
        /// </returns>
        public static bool IsSysExMessage(int status)
        {
            bool result = false;

            if(status == (int)SysExType.Start ||
                status == (int)SysExType.Special)
            {
                result = true;
            }

            return result;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <remarks>
        /// Indexing this class allows access to the system exclusive data. 
        /// This is any element other than the status value, which cannot 
        /// be changed.
        /// </remarks>
        public byte this[int index]
        {
            get
            {
                // Enforce preconditions.
                if(index < 0 || index >= Length)
                    throw new ArgumentOutOfRangeException("index", index,
                        "Index into system exclusive message out of range.");

                byte value;

                // If this is a regular system exclusive message.
                if(Type == SysExType.Start)
                {
                    // Offset by one so that the status byte isn't 
                    // included.
                    value = (byte)message[index + 1];
                }
                // Else this is a continuation or escaped system exclusive 
                // message.
                else
                {
                    // No offset necessary.
                    value = (byte)message[index];
                }

                return value;
            }            
            set
            {
                // Enforce preconditions.
                if(index < 0 || index >= Length)
                    throw new ArgumentOutOfRangeException("index", index,
                        "Index into system exclusive message out of range.");

                // If this is a regular system exclusive message.
                if(Type == SysExType.Start)
                {
                    // Offset by one so that the status byte isn't 
                    // overwritten.
                    message[index + 1] = (char)value;
                }
                // Else this is a continuation or escaped system exclusive 
                // message.
                else
                {
                    // Assign value at the specified index.
                    message[index] = (char)value;
                }
            }
        }

        /// <summary>
        /// Gets the length of the system exclusive data.
        /// </summary>
        public int Length
        {
            get
            {
                int length;

                // If this is a regular system exclusive message
                if(Type == SysExType.Start)
                {
                    // Do not include status value in data length.
                    length = message.Length - 1;
                }
                // Else this is a continuation or escaped system exclusive 
                // message.
                else
                {
                    // Data length is the length of the message.
                    length = message.Length;
                }

                return length;
            }
        }

        /// <summary>
        /// Gets the system exclusive data.
        /// </summary>
        public string Message
        {
            get
            {
                return message.ToString();
            }
        }        

        /// <summary>
        /// Gets the system exclusive type.
        /// </summary>
        public SysExType Type
        {
            get
            {
                return type;
            }
        }

        #endregion

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a clone of this instance of the SysExMessage class.
        /// </summary>
        /// <returns>
        /// A clone of this instance of the SysExMessage class.
        /// </returns>
        public object Clone()
        {
            return new SysExMessage(this);
        }

        #endregion

        #region IMidiMessage Members
        
        /// <summary>
        /// Accepts an IMidiMessageVisitor.
        /// </summary>
        /// <param name="visitor">
        /// The IMidiMessageVisitor to accept.
        /// </param>
        public void Accept(IMidiMessageVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Gets the status value.
        /// </summary>
        public int Status
        {
            get
            {
                return (int)Type;
            }
        }

        #endregion
    }

    /// <summary>
    /// Provides data for the <b>SysExReceived</b> event.
    /// </summary>
    public class SysExEventArgs : EventArgs
    {
        #region SysExEventArgs Memebers

        #region Fields

        // The system exclusive message for this event.
        private SysExMessage message;

        // The time in milliseconds since the input device began recording.
        private int timeStamp;    
    
        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the SysExEventArgs class with the 
        /// specified system exclusive message and the time stamp.
        /// </summary>
        /// <param name="message">
        /// The system exclusive message for this event.
        /// </param>
        /// <param name="timeStamp">
        /// The time in milliseconds since the input device began recording.
        /// </param>
        public SysExEventArgs(SysExMessage message, int timeStamp)
        {
            this.message = message;
            this.timeStamp = timeStamp;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the system exclusive message for this event.
        /// </summary>
        public SysExMessage Message
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
