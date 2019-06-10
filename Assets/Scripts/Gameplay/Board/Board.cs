using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board {
    // Properties
    public int NumCols { get; private set; }
    public int NumRows { get; private set; }
    // Objects
    public Player player;
	public BoardSpace[,] spaces;
    // Reference Lists
    public List<BoardObject> allObjects; // includes EVERY BoardObject in every other list!
    public List<BoardObject> objectsAddedThisMove;

	// Getters
    public BoardSpace GetSpace(Vector2Int pos) { return GetSpace(pos.x, pos.y); }
    public BoardSpace GetSpace(int col,int row) { return BoardUtils.GetSpace(this, col,row); }
    public BoardOccupant GetOccupant(int col,int row) { return BoardUtils.GetOccupant(this, col,row); }
    //public Crate GetTile(Vector2Int pos) { return GetTile(pos.x,pos.y); }
    //public Crate GetTile(int col,int row) { return BoardUtils.GetTile(this, col,row); }
	public BoardSpace[,] Spaces { get { return spaces; } }

    // Serializing
	public Board Clone () {
		BoardData data = ToData();
		return new Board(data);
	}
	public BoardData ToData() {
		BoardData bd = new BoardData(NumCols,NumRows);
        bd.playerData = player.ToData() as PlayerData;
		foreach (BoardObject obj in allObjects) { bd.allObjectDatas.Add(obj.ToData()); }
		for (int col=0; col<NumCols; col++) {
			for (int row=0; row<NumRows; row++) {
				bd.spaceDatas[col,row] = GetSpace(col,row).ToData();
			}
		}
		return bd;
	}

    

	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public Board (BoardData bd) {
		NumCols = bd.numCols;
		NumRows = bd.numRows;
        
        // Empty out lists.
        allObjects = new List<BoardObject>();
        objectsAddedThisMove = new List<BoardObject>();

		// Add all gameplay objects!
		MakeBoardSpaces (bd);
		AddPropsFromBoardData (bd);
	}

	private void MakeBoardSpaces (BoardData bd) {
		spaces = new BoardSpace[NumCols,NumRows];
		for (int i=0; i<NumCols; i++) {
			for (int j=0; j<NumRows; j++) {
				spaces[i,j] = new BoardSpace (bd.spaceDatas[i,j]);
			}
		}
	}
	private void AddPropsFromBoardData (BoardData bd) {
        player = new Player(this, bd.playerData);
		foreach (BoardObjectData objData in bd.allObjectDatas) {
            System.Type type = objData.GetType();
            if (type == typeof(CrateData)) {
                AddCrate (objData as CrateData);
            }
        }
	}
    
    private Crate AddCrate (CrateData data) {
        Crate prop = new Crate (this, data);
        allObjects.Add (prop);
        objectsAddedThisMove.Add(prop);
        return prop;
    }


    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    //private void ClearTile(Crate tile) {
    //    if (tile == null) { return; } // Safety check.
    //    tile.RemoveFromPlay();
    //}


	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
    public void OnObjectRemovedFromPlay (BoardObject bo) {
        // Remove it from its lists!
        allObjects.Remove (bo);
        //if (bo is Crate) { tiles.Remove (bo as Crate); }
        //else { Debug.LogError ("Trying to RemoveFromPlay an Object of type " + bo.GetType() + ", but our OnObjectRemovedFromPlay function doesn't recognize this type!"); }
    }


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
    public void MovePlayerAttempt(Vector2Int dir) {
        if (BoardUtils.MayMoveOccupant(this, player.BoardPos, dir)) {
            // Clear out the list NOW.
            objectsAddedThisMove.Clear();
            // Move it!
            BoardUtils.MoveOccupant(this, player.BoardPos, dir);
            
            // Dispatch event!
            GameManagers.Instance.EventManager.OnBoardExecutedMove(this);
        }
    }
    
    
    
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //  Debug
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void Debug_PrintBoardLayout() {
        string str = "";
        for (int row=0; row<NumRows; row++) {
            for (int col=0; col<NumCols; col++) {
                str += Debug_GetPrintoutSpaceChar(col,row);
            }
            str += "\n";
        }
        Debug.Log("Board Layout:\n" + str);
    }
    private string Debug_GetPrintoutSpaceChar(int col,int row) {
        BoardOccupant occupant = GetOccupant(col,row);
        if (occupant == null) { return "."; }
        if (occupant is Player) { return "@"; }
        if (occupant is Crate) { return "o"; }
        return "?";
    }





}

