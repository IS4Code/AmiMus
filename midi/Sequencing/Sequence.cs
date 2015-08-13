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
	/// Represents a collection of Tracks.
	/// </summary>
	public class Sequence
	{
        #region Sequence Members

        #region Constants

        // Number of bits to shift for splitting division value.
        private const int DivisionShift = 8;

        #endregion

        #region Fields

        // The resolution of the sequence.
        private int division;

        // The collection of tracks for the sequence.
        private ArrayList tracks = new ArrayList();  

        // All of the tracks for the sequence merged into one track.
        private Track mergedTrack;

        // Indicates whether or not the sequence has been changed since the 
        // last time all of the tracks were merged.
        private bool dirty = true;

        #endregion

        #region Classes

        /// <summary>
        /// Represents information about each track.
        /// </summary>
        private class TrackInfo
        {
            // The Track.
            public Track trk;

            // The track version. Used for comparision to see if the track has 
            // changed.
            public int version;

            /// <summary>
            /// Initializes a new instance of the TrackInfo class with the 
            /// specified track.
            /// </summary>
            /// <param name="trk">
            /// The Track.
            /// </param> 
            public TrackInfo(Track trk)
            {
                this.trk = trk;
                version = trk.Version;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the Sequence class with the 
        /// specified division.
        /// </summary>
        /// <param name="division">
        /// The division value for the sequence.
        /// </param>
		public Sequence(int division)
		{
            this.division = division;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a track to the Sequence.
        /// </summary>
        /// <param name="trk">
        /// The track to add to the Sequence.
        /// </param>
        public void Add(Track trk)
        {
            tracks.Add(new TrackInfo(trk));

            // Indicate that the sequence has changed.
            dirty = true;
        }

        /// <summary>
        /// Remove the Track at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the Track to remove. 
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is less than zero or greater or equal to Count.
        /// </exception>
        public void RemoveAt(int index)
        {
            // Enforce preconditions.
            if(index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index", index,
                    "Index for removing track from sequence out of range.");

            tracks.RemoveAt(index);

            // Indicate that the sequence has changed.
            dirty = true;
        }

        /// <summary>
        /// Gets all of the Tracks in the Sequence merged into one Track.
        /// </summary>
        /// <returns>
        /// The Tracks in the Sequence merged into one Track.
        /// </returns>
        /// <remarks>
        /// Only Tracks that are not muted are merged into one Track. Also, if 
        /// any of the Tracks have been soloed, only those Tracks are merged.
        /// </remarks>
        public Track GetMergedTrack()
        {      
            // If the Sequence has been changed.
            if(IsDirty())
            {
                mergedTrack = new Track();                

                // If any of the tracks have been soloed.
                if(IsSolo())
                {
                    // Merge only the solo tracks.
                    MergeSoloTracks();
                }
                // Else none of the tracks have been soloed.
                else
                {
                    // For each track in the sequence.
                    foreach(TrackInfo info in tracks)
                    {
                        // If the track is not muted.
                        if(!info.trk.Mute)
                        {
                            // Merge track.
                            mergedTrack = Track.Merge(mergedTrack, info.trk);
                        }
                    }
                } 

                // Update versioning information.
                foreach(TrackInfo info in tracks)
                {
                    info.version = info.trk.Version;
                }

                // Indicate that the merged track has been updated.
                dirty = false;
            }

            return mergedTrack;
        }

        /// <summary>
        /// Gets the length of the Sequence in ticks.
        /// </summary>
        /// <returns>
        /// The length of the Sequence in ticks.
        /// </returns>
        /// <remarks>
        /// The length of the Sequence is represented by the longest Track in
        /// the Sequence.
        /// </remarks>
        public int GetLength()
        {
            int length = 0;

            // For each track in the Sequence.
            foreach(TrackInfo info in tracks)
            {
                // Get the length of the track.
                int trkLength = info.trk.GetLength();

                // If this is the longest track so far, update length value.
                if(length < trkLength)
                {
                    length = trkLength;
                }
            }

            return length;
        }

        /// <summary>
        /// Determines whether or not this is a Smpte sequence.
        /// </summary>
        /// <returns>
        /// <b>true</b> if this is a Smpte sequence; otherwise, <b>false</b>.
        /// </returns>
        public bool IsSmpte()
        {
            bool result = false;

            // The upper byte of the division value will be negative if this is
            // a Smpte sequence.
            if((sbyte)(division >> DivisionShift) < 0)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Determines whether or not any of the Tracks in the Sequence are 
        /// soloed.
        /// </summary>
        /// <returns>
        /// <b>true</b> if any of the Tracks are soloed; otherwise, <b>false</b>.
        /// </returns>
        private bool IsSolo()
        {
            bool solo = false;

            // Check to see if any of the tracks are soloed.
            for(int i = 0; i < Count && !solo; i++)
            {
                // If the track is soloed.
                if(this[i].Solo)
                {
                    // Indicate that the track is soloed.
                    solo = true;
                }
            }

            return solo;
        }

        /// <summary>
        /// Determines whether or not the Sequence has changed since the 
        /// last time the Tracks in the Sequence were merged.
        /// </summary>
        /// <returns>
        /// <b>true</b> if the sequence is "dirty"; otherwise, <b>false</b>.
        /// </returns>
        private bool IsDirty()
        {
            // Check to see if any of the tracks have changed.
            for(int i = 0; i < tracks.Count && !dirty; i++)
            {
                TrackInfo info = (TrackInfo)tracks[i];

                // If the track has changed.
                if(info.version != info.trk.Version)
                {
                    // Indicate that the Sequence has changed since the tracks
                    // were last merged.
                    dirty = true;
                }
            }

            return dirty;
        }

        /// <summary>
        /// Merge only Tracks that are soloed.
        /// </summary>
        private void MergeSoloTracks()
        {
            // For each track in the Sequence.
            foreach(TrackInfo info in tracks)
            {
                // If the track is soloed and not muted.
                if(info.trk.Solo && !info.trk.Mute)
                {
                    // Merge track.
                    mergedTrack = Track.Merge(mergedTrack, info.trk);
                }
            }
        }    
 
        #endregion
   
        #region Properties

        /// <summary>
        /// Gets the Track at the specified index.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is less than zero or greater or equal to Count.
        /// </exception>
        public Track this[int index]
        {
            get
            {
                // Enforce preconditions.
                if(index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", index,
                        "Index into sequence out of range.");

                return ((TrackInfo)tracks[index]).trk;
            }
        }

        /// <summary>
        /// Gets the number of Tracks in the Sequence.
        /// </summary>
        public int Count
        {
            get
            {
                return tracks.Count;
            }
        }

        /// <summary>
        /// Gets the division value for the Sequence.
        /// </summary>
        public int Division
        {
            get
            {
                return division;
            }
        }

        #endregion

        #endregion
    }
}
