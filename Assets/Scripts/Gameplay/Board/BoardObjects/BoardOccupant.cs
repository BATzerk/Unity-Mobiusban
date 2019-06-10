using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
abstract public class BoardOccupant : BoardObject {

	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	protected void InitializeAsBoardOccupant (Board _boardRef, BoardOccupantData _data) {
		base.InitializeAsBoardObject (_boardRef, _data.boardPos);
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





}
