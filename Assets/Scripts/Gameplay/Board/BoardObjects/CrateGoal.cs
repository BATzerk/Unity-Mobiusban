using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateGoal : BoardObject, IGoalObject {
	// Properties
    public bool IsOn { get; private set; }
    public bool DoStayOn { get; private set; }
    public int Corner { get; private set; } // TL, TR, BR, BL.
    
    // Getters
    // TEMP In here.
    private static Matrix2x2 MatrixFromCorner(int corner) {
        switch (corner) {
            case Corners.TL: return Matrix2x2.TL;
            case Corners.TR: return Matrix2x2.TR;
            case Corners.BR: return Matrix2x2.BR;
            case Corners.BL: return Matrix2x2.BL;
            default: return Matrix2x2.zero;
        }
    }
    private static int CornerFromMatrix(Matrix2x2 mat) {
        if (mat.m00 == 1) { return Corners.TL; }
        if (mat.m10 == 1) { return Corners.TR; }
        if (mat.m11 == 1) { return Corners.BR; }
        if (mat.m01 == 1) { return Corners.BL; }
        return Corners.undefined;
    }
    
    // Serializing
    override public BoardObjectData ToData() {
        return new CrateGoalData (BoardPos, Corner, DoStayOn, IsOn);
    }
	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public CrateGoal (Board _boardRef, CrateGoalData data) {
		base.InitializeAsBoardObject (_boardRef, data);
        Corner = data.corner;
        DoStayOn = data.doStayOn;
        IsOn = data.isOn;
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void UpdateIsOn () {
        if (DoStayOn) { // I STAY on? It's an OR condition!
            IsOn |= GetIsSatisfied();
        }
        else { // I DON'T stay on. Only on if it's true now.
            IsOn = GetIsSatisfied();
        }
    }
    private bool GetIsSatisfied() {
		Crate crate = MySpace.MyOccupant as Crate;
        if (crate == null) { return false; } // No Crate on me at all.
        
        // KINDA SLOPPY! just getting to work for now!
        // TODO: Clean this up when we know what our needs are.
        Matrix2x2 mat = MatrixFromCorner(Corner);
        if (crate.ChirH == -1) {
            mat = mat.HorzFlipped();
        }
        if (crate.ChirV == -1) {
            mat = mat.VertFlipped();
        }
        //mat.Rotate(crate.SideFacing - SideFacing);
        int cornerRel = CornerFromMatrix(mat); // my corner, relative to the Crate.
        cornerRel += SideFacing - crate.SideFacing;
        if (cornerRel < 0) { cornerRel += Corners.NumCorners; }
        if (cornerRel >= Corners.NumCorners) { cornerRel -= Corners.NumCorners; }
        
		return crate.IsDimple[cornerRel];
	}

}
