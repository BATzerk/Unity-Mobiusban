using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamSource : BoardOccupant {
    // Components
    public Beam Beam { get; private set; } // beams are their own class. They have enough going on.
    // Properties
    public int ChannelID { get; private set; }


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public BeamSource (Board _boardRef, BeamSourceData _data) {
		Beam = new Beam(this);
		ChannelID = _data.channelID;
		base.InitializeAsBoardOccupant (_boardRef, _data);
	}
	override public BoardObjectData ToData() {
        return new BeamSourceData(BoardPos, ChannelID) {
            isMovable = IsMovable
        };
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	override public void AddMyFootprint () {
		base.AddMyFootprint ();
		Beam.AddMyFootprint ();
	}
	override public void RemoveMyFootprint () {
		base.RemoveMyFootprint ();
		Beam.RemoveMyFootprint ();
	}

	//override public void RotateMe (int rotationDir) {
	//	base.RotateMe (rotationDir);
	//	// Just remove me and re-add me.
	//	RemoveMyFootprint ();
	//	AddMyFootprint ();
	//}

	//override protected void UpdateCanBeamEnterAndExit () {
	//	SetBeamCanEnterAndExit (false); // default all can-enter-and-exits to false!
	//	canBeamExit[SideFacing] = true; // Allow beams to exit me from the side I'm facing (so my beam can escape)!
	//}



}
