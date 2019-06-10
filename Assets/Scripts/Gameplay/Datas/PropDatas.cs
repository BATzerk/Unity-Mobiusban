using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PropData {
}

public class BoardObjectData : PropData {
	public Vector2Int boardPos;
//	public BoardObjectData (BoardPos _boardPos) {
//		boardPos = _boardPos;
//	}
}
public class BoardOccupantData : BoardObjectData {
}
public class BoardSpaceData : BoardObjectData {
    public bool isPlayable = true;
	public BoardSpaceData (int _col,int _row) {
		boardPos.x = _col;
		boardPos.y = _row;
	}
}

//public class ObstacleData : BoardOccupantData {
//	public ObstacleData (BoardPos _boardPos) {
//		boardPos = _boardPos;
//	}
//}
public class TileData : BoardOccupantData {
	public int colorID;
	public TileData (Vector2Int _boardPos, int _colorID) {
		boardPos = _boardPos;
		colorID = _colorID;
	}
}
//public class WallData : BoardObjectData {
//	public WallData (BoardPos _boardPos) {
//		boardPos = _boardPos;
//	}
//}
