using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : BoardOccupant {


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public Crate (Board _boardRef, CrateData _data) {
		base.InitializeAsBoardOccupant (_boardRef, _data);
	}
	override public BoardObjectData ToData() {
		CrateData data = new CrateData (BoardPos, IsMovable);
		return data;
	}




}
