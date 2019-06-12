using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PropData {
}

public class BoardObjectData : PropData {
    public BoardPos boardPos;
}
public class BoardOccupantData : BoardObjectData {
    public bool isMovable = true;
}
public class BoardSpaceData {
    public Vector2Int ColRow;
    public bool isPlayable = true;
    public bool isWallL=false, isWallT=false;
	public BoardSpaceData (int _col,int _row) {
		this.ColRow.x = _col;
		this.ColRow.y = _row;
	}
}

public class CrateData : BoardOccupantData {
    public bool[] isDimple;
    public CrateData (BoardPos boardPos, bool[] isDimple) {
        this.boardPos = boardPos;
        this.isDimple = isDimple;
        //this.isMovable = isMovable;
    }
}
public class CrateGoalData : BoardObjectData {
    public bool doStayOn;
    public bool isOn;
    public int corner;
    public CrateGoalData(BoardPos boardPos, int corner, bool doStayOn, bool isOn) {
        this.boardPos = boardPos;
        this.corner = corner;
        this.doStayOn = doStayOn;
        this.isOn = isOn;
    }
}
public class ExitSpotData : BoardObjectData {
    public ExitSpotData(BoardPos boardPos) {
        this.boardPos = boardPos;
    }
}
public class PlayerData : BoardOccupantData {
    public PlayerData (BoardPos boardPos) {
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
