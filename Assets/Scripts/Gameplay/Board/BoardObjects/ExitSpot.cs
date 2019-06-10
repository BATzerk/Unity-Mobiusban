using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSpot : BoardObject {

	//// Getters
	//private int GetSideToFaceFromPos (Board _boardRef, int _col,int _row) {
	//	if (_col == 0) { return 3; } // left!
	//	if (_col >= _boardRef.NumCols-1) { return 1; } // right!
	//	if (_row == 0) { return 0; } // up!
	//	if (_row >= _boardRef.NumRows-1) { return 2; } // down!
	//	return 0; // default to face up.
	//}
    // Serializing
    override public BoardObjectData ToData() {
        return new ExitSpotData (BoardPos, SideFacing);
    }

	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public ExitSpot (Board _boardRef, ExitSpotData data) {
		// Override what side I'm facing, based on what wall I'm against!!
		//data.sideFacing = GetSideToFaceFromPos (_boardRef, data.boardPos.x,data.y);
		base.InitializeAsBoardObject (_boardRef, data);
		// Tell my BoardSpace I'm on it!
		//MySpace.SetMyExitSpot (this);//TODO: This
	}


}
