/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 02/09/2004
 */

namespace Multimedia.Midi
{
    /// <summary>
    /// Provides an interface that allows implementing classes to perform
    /// double dispatching on IMidiMessage objects.
    /// </summary>
    public interface IMidiMessageVisitor
    {
        /// <summary>
        /// Visits channel messages.
        /// </summary>
        /// <param name="message">
        /// The ChannelMessage to visit.
        /// </param>
        void Visit(ChannelMessage message);

        /// <summary>
        /// Visits meta messages.
        /// </summary>
        /// <param name="message">
        /// The MetaMessage to visit.
        /// </param>
        void Visit(MetaMessage message);

        /// <summary>
        /// Visits system common messages.
        /// </summary>
        /// <param name="message">
        /// The SysCommonMessage to visit.
        /// </param>
        void Visit(SysCommonMessage message);

        /// <summary>
        /// Visits system exclusive messages.
        /// </summary>
        /// <param name="message">
        /// The SysExMessage to visit.
        /// </param>
        void Visit(SysExMessage message);

        /// <summary>
        /// Visits system realtime messages. 
        /// </summary>
        /// <param name="message">
        /// The system realtime message to visit.
        /// </param>
        void Visit(SysRealtimeMessage message);
    }
}
