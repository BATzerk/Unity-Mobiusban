using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSpot : BoardObject {

    // Serializing
    override public BoardObjectData ToData() {
        return new ExitSpotData (BoardPos);
    }

	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public ExitSpot (Board _boardRef, ExitSpotData data) {
		base.InitializeAsBoardObject (_boardRef, data);
		// Tell my BoardSpace I'm on it!
		MySpace.SetMyExitSpot (this);
	}


}
