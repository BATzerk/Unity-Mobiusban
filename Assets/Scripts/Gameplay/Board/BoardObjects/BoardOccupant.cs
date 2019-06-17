using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
abstract public class BoardOccupant : BoardObject {
    // Properties
    public bool IsMovable { get; private set; }
    protected bool[] canBeamEnter = {false,false,false,false}; // one for each side. Default all to false. My extensions will say otherwise.
    protected bool[] canBeamExit = {false,false,false,false}; // one for each side. Default all to false. My extensions will say otherwise.
    
    public bool CanBeamEnter (int sideEntered) {
        if (ChirH<0) { // hacky. flip sideEntered for chirality.
            if (sideEntered==Sides.L) { sideEntered = Sides.R; }
            else if (sideEntered==Sides.R) { sideEntered = Sides.L; }
        }
        if (ChirV<0) { // hacky. flip sideEntered for chirality.
            if (sideEntered==Sides.B) { sideEntered = Sides.T; }
            else if (sideEntered==Sides.T) { sideEntered = Sides.B; }
        }
        return canBeamEnter[sideEntered];
    }
    public bool CanBeamExit (int sideExited) { return canBeamExit[sideExited]; }
    virtual public int SideBeamExits (int sideEntered) {
        return Sides.GetOpposite(sideEntered);
    }

	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	protected void InitializeAsBoardOccupant (Board _boardRef, BoardOccupantData _data) {
		base.InitializeAsBoardObject (_boardRef, _data);
        IsMovable = _data.isMovable;
        UpdateCanBeamEnterAndExit();
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	override public void AddMyFootprint () {
		MySpace.SetMyOccupant (this);
	}
	override public void RemoveMyFootprint () {
		MySpace.RemoveMyOccupant (this);
	}

    /** Extensions of this class will call this if they totally don't obstruct beams OR other Occupants. */
    virtual protected void UpdateCanBeamEnterAndExit () { }
    protected void SetBeamCanEnterAndExit (bool _canEnterAndExit) {
        for (int i=0;i<4;i++) { canBeamEnter[i]=canBeamExit[i]=_canEnterAndExit; }
    }
    override protected void OnSetSideFacing () {
        base.OnSetSideFacing ();
        UpdateCanBeamEnterAndExit ();
    }


}
