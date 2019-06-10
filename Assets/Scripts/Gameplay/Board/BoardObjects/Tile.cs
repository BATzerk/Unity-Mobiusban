using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : BoardOccupant {
    // Properties
    public int ColorID { get; private set; }
    


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public Tile (Board _boardRef, TileData _data) {
		base.InitializeAsBoardOccupant (_boardRef, _data);
		ColorID = _data.colorID;
	}
	public TileData SerializeAsData() {
		TileData data = new TileData (BoardPos, ColorID);
		return data;
	}


	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------





}
