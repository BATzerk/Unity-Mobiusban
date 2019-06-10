using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board {
    // Properties
    public int NumCols { get; private set; }
    public int NumRows { get; private set; }
    public int NumColors { get; private set; }
    // Objects
    public DragPath dragPath;
	public BoardSpace[,] spaces;
	public List<Tile> tiles;
    // Reference Lists
    public List<BoardObject> allObjects; // includes EVERY BoardObject in every other list!
    public List<BoardObject> objectsAddedThisMove;

	// Getters
    public BoardSpace GetSpace(Vector2Int pos) { return GetSpace(pos.x, pos.y); }
    public BoardSpace GetSpace(int col,int row) { return BoardUtils.GetSpace(this, col,row); }
    public BoardOccupant GetOccupant(int col,int row) { return BoardUtils.GetOccupant(this, col,row); }
    public Tile GetTile(Vector2Int pos) { return GetTile(pos.x,pos.y); }
    public Tile GetTile(int col,int row) { return BoardUtils.GetTile(this, col,row); }
	public BoardSpace[,] Spaces { get { return spaces; } }
    private int RandTileColorID() {
        return Random.Range(0, NumColors);
    }
    public bool MayBeginPath(BoardSpace firstSpace) {
        return firstSpace != null
            && firstSpace.IsPlayable;
    }
    public bool MaySubmitPath() {
        return dragPath.NumSpaces >= GameProperties.MinPathLength;
    }

    // Serializing
	public Board Clone () {
		BoardData data = ToData();
		return new Board(data);
	}
	public BoardData ToData() {
		BoardData bd = new BoardData(NumCols,NumRows);
        bd.numColors = NumColors;
		foreach (Tile p in tiles) { bd.tileDatas.Add (p.SerializeAsData()); }
		for (int col=0; col<NumCols; col++) {
			for (int row=0; row<NumRows; row++) {
				bd.spaceDatas[col,row] = GetSpace(col,row).SerializeAsData();
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
        NumColors = bd.numColors;
        
        // Empty out lists.
        dragPath = new DragPath(this);
        allObjects = new List<BoardObject>();
        tiles = new List<Tile>();
        objectsAddedThisMove = new List<BoardObject>();

		// Add all gameplay objects!
		MakeBoardSpaces (bd);
		AddPropsFromBoardData (bd);
        
        // Populate Board!
        AddTilesInOpenSpaces();
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
		foreach (TileData data in bd.tileDatas) { AddTile (data); }
	}


    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
	private Tile AddTile (TileData data) {
		Tile prop = new Tile (this, data);
		tiles.Add (prop);
		allObjects.Add (prop);
        objectsAddedThisMove.Add(prop);
		return prop;
	}
    private void AddTilesInOpenSpaces() {
        for (int col=0; col<NumCols; col++) {
            for (int row=0; row<NumRows; row++) {
                BoardSpace space = GetSpace(col,row);
                if (space.IsOpen()) {
                    AddRandomTile(col,row);
                }
            }
        }
    }
    private void AddRandomTile(int col,int row) {
        int colorID = RandTileColorID();
        TileData tileData = new TileData(new Vector2Int(col,row), colorID);
        AddTile(tileData);
    }
    
    private void ClearTile(Tile tile) {
        if (tile == null) { return; } // Safety check.
        tile.RemoveFromPlay();
    }


	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
    public void OnObjectRemovedFromPlay (BoardObject bo) {
        // Remove it from its lists!
        allObjects.Remove (bo);
        if (bo is Tile) { tiles.Remove (bo as Tile); }
        else { Debug.LogError ("Trying to RemoveFromPlay an Object of type " + bo.GetType() + ", but our OnObjectRemovedFromPlay function doesn't recognize this type!"); }
    }


	// ----------------------------------------------------------------
	//  Path Doers
	// ----------------------------------------------------------------
    public void BeginDragPath(BoardSpace firstSpace) {
        ClearDragPath(); // Clear just in case.
        AddPathSpace(firstSpace);
    }
    public void ClearDragPath() {
        if (dragPath.NumSpaces == 0) { return; } // No path? Do nothin'.
        dragPath.Clear();
        GameManagers.Instance.EventManager.OnClearPath();
    }
    public void AddPathSpace(Vector2Int pos) { AddPathSpace(GetSpace(pos)); }
    public void AddPathSpace(BoardSpace space) {
        dragPath.AddSpace(space);
        GameManagers.Instance.EventManager.OnAddPathSpace(space);
    }
    public void RemovePathSpace() {
        dragPath.RemoveSpace();
        GameManagers.Instance.EventManager.OnRemovePathSpace();
    }
    
    public void SubmitPath() {
        // Remove all Tiles in the path!
        for (int i=0; i<dragPath.NumSpaces; i++) {
            ClearTile(dragPath.GetSpace(i).MyTile);
        }
        // Was this a circuit-complete path? Clear all Tiles of this color!
        if (dragPath.IsCircuitComplete) {
            ClearAllTilesOfColorID(dragPath.ColorID);
        }
        // Clear out the path!
        ClearDragPath();
        // Move everyone down!
        BoardUtils.ApplyGravity(this);
        // Add new tiles!
        AddTilesInOpenSpaces();
        // Dispatch event!
        GameManagers.Instance.EventManager.OnSubmitPath();
    }
    
    private void ClearAllTilesOfColorID(int colorID) {
        for (int i=tiles.Count-1; i>=0; --i) {
            if (tiles[i].ColorID == colorID) {
                ClearTile(tiles[i]);
            }
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
        Tile tile = GetTile(col,row);
        if (tile == null) { return "."; }
        return tile.ColorID.ToString();
    }





}

