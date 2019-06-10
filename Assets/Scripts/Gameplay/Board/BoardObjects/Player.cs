using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BoardOccupant {


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public Player (Board _boardRef, PlayerData _data) {
		base.InitializeAsBoardOccupant (_boardRef, _data);
	}
    override public BoardObjectData ToData() {
		PlayerData data = new PlayerData(BoardPos);
		return data;
	}




}
