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
    /// Represents the various types.
    /// </summary>
    public enum MetaType
    {
        /// <summary>
        /// Represents sequencer number type.
        /// </summary>
        SequenceNumber,

        /// <summary>
        /// Represents the text type.
        /// </summary>
        Text,

        /// <summary>
        /// Represents the copyright type.
        /// </summary>
        Copyright,

        /// <summary>
        /// Represents the track name type.
        /// </summary>
        TrackName,

        /// <summary>
        /// Represents the instrument name type.
        /// </summary>
        InstrumentName,

        /// <summary>
        /// Represents the lyric type.
        /// </summary>
        Lyric,

        /// <summary>
        /// Represents the marker type.
        /// </summary>
        Marker,

        /// <summary>
        /// Represents the cue point type.
        /// </summary>
        CuePoint,

        /// <summary>
        /// Represents the program name type.
        /// </summary>
        ProgramName,

        /// <summary>
        /// Represents the device name type.
        /// </summary>
        DeviceName,

        /// <summary>
        /// Represents then end of track type.
        /// </summary>
        EndOfTrack = 0x2F,

        /// <summary>
        /// Represents the tempo type.
        /// </summary>
        Tempo = 0x51,

        /// <summary>
        /// Represents the Smpte offset type.
        /// </summary>
        SmpteOffset = 0x54,

        /// <summary>
        /// Represents the time signature type.
        /// </summary>
        TimeSignature = 0x58,

        /// <summary>
        /// Represents the key signature type.
        /// </summary>
        KeySignature,

        /// <summary>
        /// Represents the proprietary event type.
        /// </summary>
        ProprietaryEvent = 0x7F
    }

	/// <summary>
	/// Represents Midi meta messages.
	/// </summary>
	/// <remarks>
	/// Meta messages are Midi messages that are stored in Midi files. These
	/// messages are not sent or received via Midi but are read and 
	/// interpretted from Midi files. They provide information that describes 
	/// a Midi file's properties. For example, tempo changes are implemented
	/// using meta messages.
	/// </remarks>
	public class MetaMessage : IMidiMessage
	{
        #region MetaMessage Members

        #region Constants

        /// <summary>
        /// Maximum value allowed for any data value.
        /// </summary> 
        private const int DataValueMax = 127;

        /// <summary>
        /// The amount to shift data bytes when packing and unpacking them.
        /// </summary>
        private const int Shift = 8;

        #endregion

        #region Fields

        // The meta message type.
        private MetaType type;

        // The meta message data.
        private byte[] data;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the MetaMessage class with the
        /// specified type and length.
        /// </summary>
        /// <param name="type">
        /// The type of meta message.
        /// </param>
        /// <param name="length">
        /// The length of the meta message data.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The length of the meta message is not valid for the type of meta
        /// message.
        /// </exception>
        /// <remarks>
        /// Each meta message has type and length properties. For certain 
        /// types, the length of the message data must be a specific value. For
        /// example, tempo messages must have a data length of exactly three. 
        /// Some meta message types can have any data length. Text messages are
        /// an example of a meta message that can have a variable data length.
        /// When a meta message is created, the length of the data is checked
        /// to make sure that it is valid for the specified type. If it is not,
        /// an exception is thrown. 
        /// </remarks>
        public MetaMessage(MetaType type, int length)
        {
            // Enforce preconditions.
            if(!ValidateDataLength(type, length))
                throw new ArgumentOutOfRangeException("length", length,
                    "Length out of range for meta message.");

            this.type = type;

            // Create storage for meta message data.
            this.data = new byte[length];
        }

        /// <summary>
        /// Initializes a new instance of the MetaMessage class.
        /// </summary>
        /// <param name="type">
        /// The type of meta message.
        /// </param>
        /// <param name="data">
        /// The meta message data.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The length of the meta message is not valid for the type of meta
        /// message.
        /// </exception>
        /// <remarks>
        /// Each meta message has type and length properties. For certain 
        /// types, the length of the message data must be a specific value. For
        /// example, tempo messages must have a data length of exactly three. 
        /// Some meta message types can have any data length. Text messages are
        /// an example of a meta message that can have a variable data length.
        /// When a meta message is created, the length of the data is checked
        /// to make sure that it is valid for the specified type. If it is not,
        /// an exception is thrown. 
        /// </remarks>
		public MetaMessage(MetaType type, byte[] data)
		{
            // Enforce preconditions.
            if(!ValidateDataLength(type, data.Length))
                throw new ArgumentOutOfRangeException("data", data.Length,
                    "Length of data out of range for meta message.");
            
            this.type = type;
            
            // Create storage for meta message data.
            this.data = new byte[data.Length];

            // Copy data into storage.
            data.CopyTo(this.data, 0);
        }

        /// <summary>
        /// Initializes a new instance of the MetaMessage class with 
        /// another instance of the MetaMessage class.
        /// </summary>
        /// <param name="message">
        /// The MetaMessage instance to use for initialization.
        /// </param>
        public MetaMessage(MetaMessage message)
        {
            type = message.type;
            data = new byte[message.data.Length];
            message.data.CopyTo(data, 0);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tests to see if a status value belongs to a meta message.
        /// </summary>
        /// <param name="status">
        /// The status value to test.
        /// </param>
        /// <returns>
        /// <b>true</b> if the status value belongs to a meta message; 
        /// otherwise, <b>false</b>.
        /// </returns>
        public static bool IsMetaMessage(int status)
        {
            bool result = false;

            if(status == 0xFF)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Packs the data from a tempo meta message into an integer.
        /// </summary>
        /// <param name="message">
        /// The tempo meta message.
        /// </param>
        /// <returns>
        /// Returns the tempo packed into an integer.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the message is not a tempo meta message.
        /// </exception>
        public static int PackTempo(MetaMessage message)
        {
            // Enforce preconditions.
            if(message.Type != MetaType.Tempo)
                throw new ArgumentException(
                    "Message is not a tempo meta message", "message");

            int tempo = 0;

            if(BitConverter.IsLittleEndian)
            {
                int d = message.data.Length - 1;

                for(int i = 0; i < message.data.Length; i++)
                {
                    tempo |= message.data[d] << (Shift * i);
                    d--;
                }
            }
            else
            {                    
                for(int i = 0; i < message.data.Length; i++)
                {
                    tempo |= message.data[i] << (Shift * i);
                }                    
            }

            return tempo;
        }

        /// <summary>
        /// Validates data length.
        /// </summary>
        /// <param name="type">
        /// The type of meta message.
        /// </param>
        /// <param name="length">
        /// The length of the meta message data.
        /// </param>
        /// <returns>
        /// <b>true</b> if the data length is valid for this type of meta
        /// message; otherwise, <b>false</b>.
        /// </returns>
        private bool ValidateDataLength(MetaType type, int length)
        {
            const int TempoLength = 3;
            const int SmpteOffsetLength = 5;
            const int TimeSigLength = 4;
            const int KeySigLength = 2;

            bool valid = true;

            // Determine which type of meta message this is and check to make
            // sure that the data length value is valid.
            switch(type)
            {
                case MetaType.SequenceNumber:
                    if(length != 0 || length != 2)
                    {
                        valid = false;
                    }
                    break;

                case MetaType.EndOfTrack:
                    if(length != 0)
                    {
                        valid = false;
                    }
                    break;

                case MetaType.Tempo:
                    if(length != TempoLength)
                    {
                        valid = false;
                    }
                    break;

                case MetaType.SmpteOffset:
                    if(length != SmpteOffsetLength)
                    {
                        valid = false;
                    }
                    break;

                case MetaType.TimeSignature:
                    if(length != TimeSigLength)
                    {
                        valid = false;
                    }
                    break;

                case MetaType.KeySignature:
                    if(length != KeySigLength)
                    {
                        valid = false;
                    }
                    break;

                default:
                    // Assumes one of the variable length meta message.
                    break;
            }

            return valid;            
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        public byte this[int index]
        {
            get
            {
                // Enforce preconditions.
                if(index < 0 || index >= Length)
                    throw new ArgumentOutOfRangeException("index", index,
                        "Index into meta message out of range.");                

                return data[index];
            }
            set
            {
                // Enforce preconditions.
                if(index < 0 || index >= Length)
                    throw new ArgumentOutOfRangeException("index", index,
                        "Index into meta message out of range.");
                else if(value > DataValueMax)
                    throw new ArgumentOutOfRangeException("value", value,
                        "Data value for meta message out of range."); 

                // Assign value at the specified index.
                data[index] = value;
            }                
        }

        /// <summary>
        /// Gets the length of the meta message.
        /// </summary>
        public int Length
        {
            get
            { 
                return data.Length;
            }
        }
        
        /// <summary>
        /// Gets the type of meta message.
        /// </summary>
        public MetaType Type
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
        /// Creates a clone of this instance of the MetaMessage class.
        /// </summary>
        /// <returns>
        /// A clone of this instance of the MetaMessage class.
        /// </returns>
        public object Clone()
        {
            return new MetaMessage(this);
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
                // All meta messages have the same status byte (0xFF).
                return 0xFF;
            }
        }

        #endregion
    }
}
