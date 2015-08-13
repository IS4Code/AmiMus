/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 06/18/2004
 */

using System;

namespace Multimedia.Midi
{
    /// <summary>
    /// Represents the method that will handle the event that occurs when a 
    /// channel message is received.
    /// </summary>
    public delegate void ChannelMessageEventHandler(object sender, ChannelMessageEventArgs e);

    /// <summary>
    /// Represents the various channel message types.
    /// </summary>
    public enum ChannelCommand 
    {
        /// <summary>
        /// Represents the note-off command type.
        /// </summary>
        NoteOff = 0x80,

        /// <summary>
        /// Represents the note-on command type.
        /// </summary>
        NoteOn = 0x90,

        /// <summary>
        /// Represents the poly pressure (aftertouch) command type.
        /// </summary>
        PolyPressure = 0xA0,

        /// <summary>
        /// Represents the controller command type.
        /// </summary>
        Controller = 0xB0,  
  
        /// <summary>
        /// Represents the program change command type.
        /// </summary>
        ProgramChange = 0xC0,

        /// <summary>
        /// Represents the channel pressure (aftertouch) command 
        /// type.
        /// </summary>
        ChannelPressure = 0xD0,   
     
        /// <summary>
        /// Represents the pitch wheel command type.
        /// </summary>
        PitchWheel = 0xE0
    }

	/// <summary>
	/// Represents Midi channel messages.
	/// </summary>
	public class ChannelMessage : ShortMessage
	{
        #region ChannelMessage Members

        #region Constants

        //
        // Bit manipulation constants.
        //

        private const int ChannelMask = 16777200;
        private const int CommandMask = 16776975;

        /// <summary>
        /// Maximum value allowed for Midi channel.
        /// </summary> 
        public const int ChannelValueMax = 15;

        #endregion

        #region Fields

        // Midi Channel.
        private int channel;

        // Command.
        private ChannelCommand command;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the ChannelMessage class with the 
        /// specified command value.
        /// </summary>
        /// <param name="command">
        /// The Midi command represented by this message.
        /// </param>
        public ChannelMessage(ChannelCommand command)
        {
            Command = command;
            MidiChannel = 0;
        }

        /// <summary>
        /// Initializes a new instance of the ChannelMessage class with the 
        /// specified command value and Midi channel.
        /// </summary>
        /// <param name="command">
        /// The Midi command represented by the message.
        /// </param>
        /// <param name="channel">
        /// The Midi channel associated with the message.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the Midi channel is set to a value less than zero or 
        /// greater than ChannelValueMax.
        /// </exception>
		public ChannelMessage(ChannelCommand command, int channel)
		{
            Command = command;
            MidiChannel = channel;
		}

        /// <summary>
        /// Initializes a new instance of the ChannelMessage class with the
        /// specified command value, Midi channel, and first data value.
        /// </summary>
        /// <param name="command">
        /// The Midi command represented by the message.
        /// </param>
        /// <param name="channel">
        /// The Midi channel associated with the message.
        /// </param>
        /// <param name="data1">
        /// The first data value.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the Midi channel or the first data value is out of range.
        /// </exception>
        public ChannelMessage(ChannelCommand command, int channel, 
            int data1)
        {
            Command = command;
            MidiChannel = channel;
            Data1 = data1;
        }        

        /// <summary>
        /// Initializes a new instance of the ChannelMessage class with the
        /// specified command value, Midi channel, first data value, and 
        /// second data value.
        /// </summary>
        /// <param name="command">
        /// The Midi command represented by the message.
        /// </param>
        /// <param name="channel">
        /// The Midi channel associated with the message.
        /// </param>
        /// <param name="data1">
        /// The first data value.
        /// </param>
        /// <param name="data2">
        /// The second data value.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the Midi channel, first data value, or second data value 
        /// is out of range.
        /// </exception>
        public ChannelMessage(ChannelCommand command, int channel, 
            int data1, int data2)
        {
            Command = command;
            MidiChannel = channel;
            Data1 = data1;
            Data2 = data2;
        }

        /// <summary>
        /// Initializes a new instance of the ChannelMessage class with a 
        /// channel message packed into an integer.
        /// </summary>
        /// <param name="message">
        /// The packed channel message to use for initialization.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if the message does not represent a channel message.
        /// </exception>
        public ChannelMessage(int message)
        {
            // Get status byte.
            int status = UnpackStatus(message);

            // Enforce preconditions.
            if(!ChannelMessage.IsChannelMessage(status))
                throw new ArgumentException("Message is not a channel message.",
                    "message");            

            //
            // Initialize properties.
            //

            Command = (ChannelCommand)(message & ~CommandMask);
            MidiChannel = message & ~ChannelMask;
            SetData1(ShortMessage.UnpackData1(message));
            SetData2(ShortMessage.UnpackData2(message));
        }

        /// <summary>
        /// Initializes a new instance of the ChannelMessage class with 
        /// another instance of the ChannelMessage class.
        /// </summary>
        /// <param name="message">
        /// The ChannelMessage instance to use for initialization.
        /// </param>
        public ChannelMessage(ChannelMessage message)
        {
            Command = message.Command;
            MidiChannel = message.MidiChannel;
            Data1 = message.Data1;
            Data2 = message.Data2;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tests to see if a status value belongs to a channel message.
        /// </summary>
        /// <param name="status">
        /// The message to test.
        /// </param>
        /// <returns>
        /// <b>true</b> if the status value belongs to a channel message;
        /// otherwise, <b>false</b>.
        /// </returns>
        public static bool IsChannelMessage(int status)
        {
            bool result = false;

            // If the status value is in range for channel messages
            if(status >= (int)ChannelCommand.NoteOff &&
                status <= (int)ChannelCommand.PitchWheel + ChannelValueMax)
            {
                // Indicate that the status value belongs to a channel message.
                result = true;
            }

            return result;
        }        

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Midi command type.
        /// </summary>
        public ChannelCommand Command
        {
            get
            {
                return command;
            }
            set
            {
                command = value;

                SetStatus(MidiChannel | (int)command);
            }
        }

        /// <summary>
        /// Gets or sets the Midi channel.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <b>Midichannel</b> is set to a value less than zero or greater than 
        /// ChannelValueMax.
        /// </exception>
        public int MidiChannel
        {
            get
            {
                return channel;
            }
            set
            {
                // Enforce preconditions.
                if(value < 0 || value > ChannelValueMax)
                    throw new ArgumentOutOfRangeException("MidiChannel",
                        value, "MIDI channel out of range.");
                
                // Assign new Midi channel.
                channel = value;
                SetStatus(channel | (int)command);
            }
        }

        /// <summary>
        /// Gets or sets the first data value.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <b>Data1</b> is set to a value less than zero or greater than 
        /// DataValueMax.
        /// </exception>
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
        /// <exception cref="ArgumentOutOfRangeException">
        /// <b>Data2</b> is set to a value less than zero or greater than 
        /// DataValueMax.
        /// </exception>
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
        /// Creates a clone of this instance of the ChannelMessage.
        /// </summary>
        /// <returns>
        /// A clone of this instance of the ChannelMessage.
        /// </returns>
        public override object Clone()
        {
            return new ChannelMessage(this);
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
    /// Provides data for ChannelMessageReceived events.
    /// </summary>
    public class ChannelMessageEventArgs : EventArgs
    {
        #region ChannelMessageEventArgs Members

        #region Fields

        // The ChannelMessage for this event.
        private ChannelMessage message;

        // Time in milliseconds since the input device began recording.
        private int timeStamp;    
    
        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the ChannelMessageEventArgs class with the 
        /// specified ChannelMessage and time stamp.
        /// </summary>
        /// <param name="message">
        /// The ChannelMessage for this event.
        /// </param>
        /// <param name="timeStamp">
        /// The time in milliseconds since the input device began recording.
        /// </param>
        public ChannelMessageEventArgs(ChannelMessage message, int timeStamp)
        {
            this.message = message;
            this.timeStamp = timeStamp;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ChannelMessage for this event.
        /// </summary>
        public ChannelMessage Message
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
