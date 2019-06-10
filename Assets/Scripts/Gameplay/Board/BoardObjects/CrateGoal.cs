using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateGoal : BoardObject, IGoalObject {
	// Properties
	public bool IsOn { get; private set; }
    
    // Serializing
    override public BoardObjectData ToData() {
        return new CrateGoalData (BoardPos, SideFacing);
    }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public CrateGoal (Board _boardRef, CrateGoalData data) {
		base.InitializeAsBoardObject (_boardRef, data);
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void UpdateIsOn () {
		BoardOccupant bo = MySpace.MyOccupant; // TODO: SideFacing.
		IsOn = bo!=null && !(bo is Player);
	}

}
