using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PropData {
}

public class BoardObjectData : PropData {
    public int chirH=1;
    public int chirV=1;
    public int sideFacing;
    public Vector2Int boardPos;
}
public class BoardOccupantData : BoardObjectData {
    public bool isMovable = true;
}
public class BoardSpaceData : BoardObjectData {
    public bool isPlayable = true;
    public bool isWallL=false, isWallT=false;
	public BoardSpaceData (int _col,int _row) {
		this.boardPos.x = _col;
		this.boardPos.y = _row;
	}
}

public class CrateData : BoardOccupantData {
    public bool[] isDimple;
    public CrateData (Vector2Int boardPos, int chirH,int chirV, int sideFacing, bool[] isDimple) {
        this.boardPos = boardPos;
        this.chirH = chirH;
        this.chirV = chirV;
        this.sideFacing = sideFacing;
        this.isDimple = isDimple;
        //this.isMovable = isMovable;
    }
}
public class CrateGoalData : BoardObjectData {
    public int corner;
    public CrateGoalData(Vector2Int boardPos, int corner) {
        this.boardPos = boardPos;
        this.corner = corner;
    }
}
public class ExitSpotData : BoardObjectData {
    public ExitSpotData(Vector2Int boardPos, int sideFacing) {
        this.boardPos = boardPos;
        this.sideFacing = sideFacing;
    }
}
public class PlayerData : BoardOccupantData {
    public PlayerData (Vector2Int boardPos) {
        this.boardPos = boardPos;
    }
}
//public class WallData : BoardObjectData {
//    public int sideFacing { get; private set; }// = -1;
//	public WallData (Vector2Int boardPos, int sideFacing) {
//		this.boardPos = boardPos;
//        this.sideFacing = sideFacing;
//	}
//}
