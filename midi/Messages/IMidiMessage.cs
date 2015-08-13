/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 02/10/2004
 */

using System;

namespace Multimedia.Midi
{
    /// <summary>
    /// Represents the basic functionality required for all Midi messages.
    /// </summary>
    public interface IMidiMessage : ICloneable
    {  
        /// <summary>
        /// Accepts an IMidiMessageVisitor.
        /// </summary>
        /// <param name="visitor">
        /// The IMidiMessageVisitor to accept.
        /// </param>
        void Accept(IMidiMessageVisitor visitor);

        /// <summary>
        /// Gets the Midi message's status value.
        /// </summary>
        int Status
        {
            get;
        }
    }
}
