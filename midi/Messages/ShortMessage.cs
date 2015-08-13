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
    /// The base class for all Midi short messages.
    /// </summary>
    /// <remarks>
    /// Short messages are any Midi message except for system exclusive 
    /// messages. This includes all channel, system common, and system 
    /// realtime messages.
    /// </remarks>
	public abstract class ShortMessage : IMidiMessage
	{
        #region ShortMessage Members

        #region Constants

        //
        // Bit manipulation constants.
        //

        private const int StatusMask = 16776960;
        private const int Data1Mask = 16711935;
        private const int Data2Mask = 65535;
        private const int Data1Shift = 8;
        private const int Data2Shift = 16;

        /// <summary>
        /// Maximum value allowed for any status byte.
        /// </summary>
        public const int StatusValueMax = 255;

        /// <summary>
        /// Maximum value allowed for any data byte.
        /// </summary> 
        public const int DataValueMax = 127;

        #endregion

        #region Fields

        // The short message packed as an integer.
        private int message = 0;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the ShortMessage class.
        /// </summary>
		protected ShortMessage()
		{
		}

        #endregion

        #region Methods

        /// <summary>
        /// Unpacks the status value from a packed Midi short message.
        /// </summary>
        /// <param name="message">
        /// The packed Midi short message.
        /// </param>
        /// <returns>
        /// The status value of the message.
        /// </returns>
        public static int UnpackStatus(int message)
        {
            return message & ~StatusMask;
        }

        /// <summary>
        /// Unpacks the first data value from a packed Midi short message.
        /// </summary>
        /// <param name="message">
        /// The packed Midi short message.
        /// </param>
        /// <returns>
        /// The first data value of the message.
        /// </returns>
        public static int UnpackData1(int message)
        {
            return (message & ~Data1Mask) >> Data1Shift;
        }

        /// <summary>
        /// Unpacks the second data value from a packed Midi short message.
        /// </summary>
        /// <param name="message">
        /// The packed Midi short message.
        /// </param>
        /// <returns>
        /// The second data value of the message.
        /// </returns>
        public static int UnpackData2(int message)
        {
            return (message & ~Data2Mask) >> Data2Shift;
        }

        /// <summary>
        /// Sets the status value.
        /// </summary>
        /// <param name="status">
        /// The new status value.
        /// </param>
        protected void SetStatus(int status)
        {
            // Enforce preconditions.
            if(status < 0 || status > StatusValueMax)
                throw new ArgumentOutOfRangeException("status", status,
                    "Status value out of range.");

            // Replace the current status value with the new one.
            message &= StatusMask;
            message |= status;
        }

        /// <summary>
        /// Gets the first data value.
        /// </summary>
        /// <returns>
        /// The first data value.
        /// </returns>
        protected int GetData1()
        {
            return (message & ~Data1Mask) >> Data1Shift;
        }

        /// <summary>
        /// Sets the first data value.
        /// </summary>
        /// <param name="data1">
        /// The first data value.
        /// </param>
        protected void SetData1(int data1)
        {
            // Enforce preconditions.
            if(data1 < 0 || data1 > DataValueMax)
                throw new ArgumentOutOfRangeException("data1", data1,
                    "First data byte for short message out of range.");

            // Replace current data value with the new one.
            message &= Data1Mask;
            message |= data1 << Data1Shift;
        }

        /// <summary>
        /// Gets the second data value.
        /// </summary>
        /// <returns>
        /// The second data value.
        /// </returns>
        protected int GetData2()
        {
            return (message & ~Data2Mask) >> Data2Shift;
        }

        /// <summary>
        /// Sets the second data value.
        /// </summary>
        /// <param name="data2">
        /// The second data value.
        /// </param>
        protected void SetData2(int data2)
        {
            // Enforce preconditions.
            if(data2 < 0 || data2 > DataValueMax)
                throw new ArgumentOutOfRangeException("data2", data2,
                    "Second data byte for short message out of range.");
            
            // Replace current data value with the new one.
            message &= Data2Mask;
            message |= data2 << Data2Shift;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the message as a packed integer.
        /// </summary>
        public int Message
        {
            get
            {
                return message;
            }
        }       

        #endregion

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a clone of the Midi message.
        /// </summary>
        /// <returns>
        /// A clone of the Midi message.
        /// </returns>
        public abstract object Clone();

        #endregion

        #region IMidiMessage Members
        
        /// <summary>
        /// Accepts an IMidiMessageVisitor.
        /// </summary>
        /// <param name="visitor">
        /// The IMidiMessageVisitor to accept.
        /// </param>
        public abstract void Accept(IMidiMessageVisitor visitor);

        /// <summary>
        /// Gets the Midi message's status value.
        /// </summary>
        public int Status
        {
            get
            {
                return message & ~StatusMask;
            }
        }

        #endregion
    }
}
