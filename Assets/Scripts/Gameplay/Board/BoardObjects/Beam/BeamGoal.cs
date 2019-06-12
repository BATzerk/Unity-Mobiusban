using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: it only partially makes sense for this class to implement IGoalObject, as it's used now.
public class BeamGoal : BoardOccupant, IGoalObject {
    // Properties
    public bool IsOn { get; private set; }
    public int ChannelID { get; private set; }


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public BeamGoal (Board _boardRef, BeamGoalData _data) {
		base.InitializeAsBoardOccupant (_boardRef, _data);
		ChannelID = _data.channelID;
	}
	override public BoardObjectData ToData() {
		return new BeamGoalData (BoardPos, ChannelID);
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void UpdateIsOn () {
		IsOn = MySpace.HasBeamSansSource (ChannelID);
	}
	override protected void UpdateCanBeamEnterAndExit() {
		SetBeamCanEnterAndExit (false); // default all can-enter-and-exits to false!
		canBeamEnter[SideFacing] = true; // Allow beams to enter me from the side I'm facing!
	}



}
