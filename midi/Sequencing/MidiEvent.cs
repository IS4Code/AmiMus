/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 04/08/2004
 */

using System;

namespace Multimedia.Midi
{
    /// <summary>
    /// Represents the method for handling Midi events.
    /// </summary>
    public delegate void MidiEventHandler(object sender, MidiEventArgs e);

	/// <summary>
	/// Represents a time-stamped event in which a Midi message occurs.
	/// </summary>
	public struct MidiEvent : ICloneable
	{
        #region MidiEvent Members

        #region Fields

        // The Midi message for the event.
        private IMidiMessage message;

        // The delta tick value for the event.
        private int ticks;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the Midi event struct with the 
        /// specified Midi message and the number of ticks for this event.
        /// </summary>
        /// <param name="message">
        /// The Midi message for the event.
        /// </param>
        /// <param name="ticks">
        /// The delta tick value for the event.
        /// </param>
		public MidiEvent(IMidiMessage message, int ticks)
		{
            this.message = message;
            this.ticks = ticks;
		}  

        #endregion

        #region Properties

        /// <summary>
        /// Gets or set the Midi message for the Midi event.
        /// </summary>
        public IMidiMessage Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }

        /// <summary>
        /// Gets or sets the ticks for the Midi event.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the ticks value is set to a negative number.
        /// </exception>
        public int Ticks
        {
            get
            {
                return ticks;
            }
            set
            {
                // Enforce preconditions.
                if(value < 0)
                    throw new ArgumentOutOfRangeException("Ticks", value,
                        "Ticks value out of range.");

                // Initialize ticks.
                ticks = value;
            }
        }

        #endregion

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the Midi event.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this Midi event.
        /// </returns>
        public object Clone()
        {
            IMidiMessage msg = null;

            if(Message != null)
            {
                msg = (IMidiMessage)Message.Clone();
            }

            return new MidiEvent(msg, Ticks);
        }

        #endregion
    }

    /// <summary>
    /// Provides data for Midi events.
    /// </summary>
    public class MidiEventArgs : EventArgs
    {
        #region MidiEventArgs Members

        #region Fields

        // The Midi event.
        private MidiEvent evt;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the MidiEventArgs class with the
        /// specified Midi event.
        /// </summary>
        /// <param name="e">
        /// The Midi event for this event.
        /// </param>
        public MidiEventArgs(MidiEvent e)
        {
            evt = e;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Midi event for this event.
        /// </summary>
        public MidiEvent MidiEvent
        {
            get
            {
                return evt;
            }
        }

        #endregion

        #endregion
    }
}
