using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PropData {
}

public class BoardObjectData : PropData {
	public Vector2Int boardPos;
}
public class BoardOccupantData : BoardObjectData {
    public bool isMovable = true;
}
public class BoardSpaceData : BoardObjectData {
    public bool isPlayable = true;
	public BoardSpaceData (int _col,int _row) {
		this.boardPos.x = _col;
		this.boardPos.y = _row;
	}
}

public class CrateData : BoardOccupantData {
    public CrateData (Vector2Int boardPos, bool isMovable) {
        this.boardPos = boardPos;
        this.isMovable = isMovable;
    }
}
public class PlayerData : BoardOccupantData {
    public PlayerData (Vector2Int boardPos) {
        this.boardPos = boardPos;
    }
}
public class WallData : BoardObjectData {
    public int sideFacing { get; private set; }// = -1;
	public WallData (Vector2Int boardPos, int sideFacing) {
		this.boardPos = boardPos;
        this.sideFacing = sideFacing;
	}
}
