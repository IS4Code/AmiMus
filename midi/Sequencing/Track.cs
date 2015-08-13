/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 05/08/2004
 */

using System;
using System.Collections;

namespace Multimedia.Midi
{
	/// <summary>
	/// Represents a collection of Midi events.
	/// </summary>
	public class Track : ICloneable
	{
        #region Track Members

        #region Fields

        // Midi event list.
        private ArrayList MidiEvents = new ArrayList();

        // Indicates whether or not the track is muted.
        private bool mute = false;

        // Indicates whether or not the track is soloed.
        private bool solo = false;

        // The version number for the track. Changes any time the track is 
        // modified in some way.
        private int version = 0;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the Track class.
        /// </summary>
        public Track()
        {
        }       
 
        /// <summary>
        /// Initializes a new instance of the Track class with another instance
        /// of the Track class.
        /// </summary>
        /// <param name="trk">
        /// The Track instance to use for initialization.
        /// </param>
        public Track(Track trk)
        {
            // Copy events from the existing track into this one.
            foreach(MidiEvent e in trk.MidiEvents)
            {
                this.MidiEvents.Add(e.Clone());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a Midi event to the end of the track.
        /// </summary>
        /// <param name="e">
        /// The Midi event to add to the track.
        /// </param>
        public void Add(MidiEvent e)
        {
            MidiEvents.Add(e);
            version++;
        }       
 
        /// <summary>
        /// Inserts a MidiEvent into the Track at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which <i>e</i> should be inserted. 
        /// </param>
        /// <param name="e">
        /// The MidiEvent to insert.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is less than zero or greater than or equal to 
        /// Count.
        /// </exception>
        public void Insert(int index, MidiEvent e)
        {
            // Enforce preconditions.
            if(index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index", index,
                    "Index into track out of range.");

            MidiEvents.Insert(index, e);

            // Indicate that the track has changed.
            version++;
        }

        /// <summary>
        /// Removes the first occurrance of a MidiEvent from the Track.
        /// </summary>
        /// <param name="e">
        /// The MidiEvent to remove from the Track.
        /// </param>
        public void Remove(MidiEvent e)
        {
            MidiEvents.Remove(e);

            // Indicate that the track has changed.
            version++;
        }

        /// <summary>
        /// Removes a MidiEvent at the specified index of the Track.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the MidiEvent to remove. 
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is less than zero or greater than or equal to 
        /// Count.
        /// </exception>
        public void RemoveAt(int index)
        {
            // Enforce preconditions.
            if(index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index", index,
                    "Index into track out of range.");

            MidiEvents.RemoveAt(index);

            // Indicate that the track has changed.
            version++;
        }        

        /// <summary>
        /// Slides events forwards or backwards at the specified index in the 
        /// Track.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the MidiEvent to slide. 
        /// </param>
        /// <param name="slideAmount">
        /// The amount to slide the MidiEvent.
        /// </param>
        /// <remarks>
        /// If the slide amount is a negative number, the Midi event at the
        /// specified index will be moved backwards in time; its ticks value 
        /// will be summed with the slide amount thus reducing its value. It
        /// is important that using a negative slide amount does not result in
        /// a negative tick value for the specified Midi event. If this occurs,
        /// an exception is thrown. If the slide amount is positive, the Midi 
        /// event at the specified index will be moved forwards in time; its 
        /// ticks value will be increased by the slide amount. 
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is less than zero or greater than or equal to 
        /// Count. Or if slide amount results in a ticks value less than zero.
        /// </exception>
        public void Slide(int index, int slideAmount)
        {
            // Enforce preconditions.
            if(index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index", index,
                    "Index into track out of range.");

            MidiEvent e = (MidiEvent)MidiEvents[index];

            // Enforce preconditions.
            if(e.Ticks + slideAmount < 0)
                throw new ArgumentOutOfRangeException("slideAmount", slideAmount,
                    "Slide amount out of range.");
            
            // Slide MidiEvent ticks value by the slide amount.
            e.Ticks += slideAmount;

            // Put Midi event back into track;
            MidiEvents[index] = e;

            // Indicate that the track has changed.
            version++;
        }

        /// <summary>
        /// Gets the length of the Track in ticks.
        /// </summary>
        /// <returns>
        /// The length of the track in ticks.
        /// </returns>
        public int GetLength()
        {
            int length = 0;

            // Calculate the length of the track by summing the ticks value of 
            // every Midi event in the track.
            foreach(MidiEvent e in MidiEvents)
            {
                length += e.Ticks;
            }

            return length;
        }

        /// <summary>
        /// Merges two tracks together.
        /// </summary>
        /// <param name="trackA">
        /// The first of two tracks to merge.
        /// </param>
        /// <param name="trackB">
        /// The second of two tracks to merge.
        /// </param>
        /// <returns>
        /// The merged track.
        /// </returns>
        public static Track Merge(Track trackA, Track trackB)
        {
            Track trkA = new Track(trackA);
            Track trkB = new Track(trackB);
            Track mergedTrack = new Track();
            int a = 0, b = 0; 

            //
            // The following algorithm merges two Midi tracks together. It 
            // assumes that both tracks are valid in that both end with a
            // end of track meta message.
            //

            // While neither the end of track A or track B has been reached.
            while(a < trkA.Count - 1 && b < trkB.Count - 1)
            {
                // While the end of track A has not been reached and the 
                // current Midi event in track A comes before the current Midi
                // event in track B.
                while(a < trkA.Count - 1 && trkA[a].Ticks <= trkB[b].Ticks)
                {
                    // Slide the events in track B backwards by the amount of
                    // ticks in the current event in track A. This keeps both
                    // tracks in sync.
                    trkB.Slide(b, -trkA[a].Ticks);

                    // Add the current event in track A to the merged track.
                    mergedTrack.Add(trkA[a]);

                    // Move to the next Midi event in track A.
                    a++;
                }

                // If the end of track A has not yet been reached.
                if(a < trkA.Count - 1)
                {
                    // While the end of track B has not been reached and the 
                    // current Midi event in track B comes before the current Midi
                    // event in track A.
                    while(b < trkB.Count - 1 && trkB[b].Ticks < trkA[a].Ticks)
                    {
                        // Slide the events in track A backwards by the amount of
                        // ticks in the current event in track B. This keeps both
                        // tracks in sync.
                        trkA.Slide(a, -trkB[b].Ticks);

                        // Add the current event in track B to the merged track.
                        mergedTrack.Add(trkB[b]);

                        // Move forward to the next Midi event in track B.
                        b++;
                    }
                }
            }
            
            // If the end of track A has not yet been reached.
            if(a < trkA.Count - 1)
            {
                // Add the rest of the events in track A to the merged track.
                while(a < trkA.Count)
                {
                    mergedTrack.Add(trkA[a]);
                    a++;
                }
            }
            // Else if the end of track B has not yet been reached.
            else if(b < trkB.Count - 1)
            {
                // Add the rest of the events in track B to the merged track.
                while(b < trkB.Count)
                {
                    mergedTrack.Add(trkB[b]);
                    b++;
                }
            }
            
            return mergedTrack;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the MidiEvent at the specified index.
        /// </summary>
        public MidiEvent this[int index]
        {
            get
            {
                return (MidiEvent)MidiEvents[index];
            }
            set
            {
                MidiEvents[index] = value;

                // Indicate that the track has changed.
                version++;
            }
        }

        /// <summary>
        /// Gets the number of MidiEvents in the track.
        /// </summary>
        public int Count
        {
            get
            {
                return MidiEvents.Count;
            }
        }

        /// <summary>
        /// Gets or sets the track's muted state.
        /// </summary>
        public bool Mute
        {
            get
            {
                return mute;
            }
            set
            {
                mute = value;

                // Indicate that the track's state has changed.
                version++;
            }
        }

        /// <summary>
        /// Gets or sets the track's soloed state.
        /// </summary>
        public bool Solo
        {
            get
            {
                return solo;
            }
            set
            {
                solo = value;

                // Indicate that the track's state has changed.
                version++;
            }
        }

        /// <summary>
        /// Gets the track's version value.
        /// </summary>
        internal int Version
        {
            get
            {
                return version;
            }
        }

        #endregion

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the Track.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this Track.
        /// </returns>
        public object Clone()
        {
            return new Track(this);
        }

        #endregion
    }
}
