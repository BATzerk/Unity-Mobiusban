using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam {
	// References
	private BeamSource mySource;
	private List<BeamSegment> segments; // Recreated every time I'm moved. Note that Portals can create tree-like/fractal segment structures. This is just a linear list that doesn't care about that; it's just all the segments.

	// Getters
	public bool IsInPlay { get { return mySource.IsInPlay; } }
	public int ChannelID { get { return mySource.ChannelID; } }
	public int NumSegments { get { return segments.Count; } }
	public BeamSegment GetSegment (int index) { return segments[index]; }

	private Board BoardRef { get { return mySource.BoardRef; } }
    private BoardSpace GetSpace (Vector2Int _colRow) { return BoardUtils.GetSpace (BoardRef, _colRow); }
    private BoardSpace GetSpace (int _col,int _row) { return BoardUtils.GetSpace (BoardRef, _col,_row); }
	private BoardOccupant GetOccupant (int _col,int _row) { return BoardUtils.GetOccupant (BoardRef, _col,_row); }

	/** Returns TRUE if the provided Space is ALSO the Space where this Beam originates.
	This check exists so we can discount the origin Space as satisfying FloorBeamGoals. */
	public bool IsSpaceMyOriginSpace (BoardSpace _space) {
		return GetSegment(0).GetFirstSpace() == _space;
	}


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public Beam (BeamSource _mySource) {
		mySource = _mySource;
	}

	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void AddMyFootprint () {
		// segments!
		segments = new List<BeamSegment> ();
		AddSegmentRecursively (mySource);//mySource.Col,mySource.Row, mySource.SideFacing);
	}
	public void RemoveMyFootprint () {
		if (segments == null) { return; } // Best to check JIC.
		// Remove all my segments from the board!
		foreach (BeamSegment bs in segments) {
			bs.RemoveFromBoard ();
		}
		segments = null; // go ahead and just null it out
	}
	private void AddSegmentRecursively (BoardOccupant _sourceOccupant) {//int _col,int _row, int _sideExiting) {
		// Create empty segment to populate, and add it to our list.
		BeamSegment newSegment = new BeamSegment(this, _sourceOccupant);
		segments.Add (newSegment);
		int exitSide = _sourceOccupant.SideFacing; // assume that we ALWAYS come OUT of any Occupant's SideFacing.
		AddSegmentSpaceRecursively (newSegment, exitSide); // populate the segment (and maybe make more segments)!
	}

	private void AddSegmentSpaceRecursively (BeamSegment _segment, int exitSide) {
		// Do we have too many segments? Okay, stop; we're probably caught in an infinite-Beam Portal mirroring situation.
		if (NumSegments > 10) { return; }
		// Are we not allowed to EXIT this space? Then stop here.
		if (!BoardUtils.CanBeamExitSpace (GetSpace(_segment.LastColRow), exitSide)) { return; }
		// What space will we add it to?
        TranslationInfo ti = BoardUtils.GetTranslationInfo(mySource.BoardRef, _segment.LastColRow, exitSide);
		BoardSpace spaceToAddBeam = BoardUtils.GetSpace (BoardRef, ti.to);
		// If we can't ENTER the next space, then stop. :)
		int nextSideIn = MathUtils.GetSide(ti.dirIn);
		if (!BoardUtils.CanBeamEnterSpace (spaceToAddBeam, nextSideIn)) { return; }
		// Otherwise, add this space to the segment!
		_segment.AddSpace (spaceToAddBeam);
		// How is the beam exiting??
		int endSideExiting = spaceToAddBeam.GetSideBeamExits (nextSideIn); // keep updaing endSideExiting (until we hit the end).
		// Otherwise, keep going! Add again!
		AddSegmentSpaceRecursively (_segment, endSideExiting);
	}

	// This function is a bit overkill for what we need, but it's easier to brute-force remake the whole thing. It's not that much more expensive.
	public void RemakeBeam () {
		RemoveMyFootprint ();
		AddMyFootprint ();
	}

}

        //// Hey, are we ENTERING a Portal?! Then add ANOTHER entire Segment starting at all the other Portals in this Channel!
        //Portal portalWithExitInNextSpace = BoardUtils.PortalWithWarpSideInSpace (BoardRef, _prevCol,_prevRow, sideExiting);
        //if (portalWithExitInNextSpace != null) {
        //  Portal[] otherPortals = portalWithExitInNextSpace.GetOtherPortalsInChannel ();
        //  foreach (Portal p in otherPortals) {
        //      AddSegmentRecursively (p); // future todo: Orient this differently in case Portals have fewer than 3 sides.
        //  }
        //}