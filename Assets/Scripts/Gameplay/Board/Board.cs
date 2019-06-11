using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Board {
    // Properties
    public int NumCols { get; private set; }
    public int NumRows { get; private set; }
    public WrapTypes WrapH { get; private set; }
    public WrapTypes WrapV { get; private set; }
    public bool AreGoalsSatisfied { get; private set; }
    // Objects
    public BoardSpace[,] spaces;
    public Player player;
    public List<BoardObject> allObjects; // includes every object EXCEPT Player!
    // Reference Lists
    public List<IGoalObject> goalObjects; // contains JUST the objects that have winning criteria.
    public List<BoardObject> objectsAddedThisMove;

	// Getters
    public bool DoWrapH { get { return WrapH != WrapTypes.None; } }
    public bool DoWrapV { get { return WrapV != WrapTypes.None; } }
    public BoardSpace GetSpace(Vector2Int pos) { return GetSpace(pos.x, pos.y); }
    public BoardSpace GetSpace(int col,int row) { return BoardUtils.GetSpace(this, col,row); }
    public BoardOccupant GetOccupant(int col,int row) { return BoardUtils.GetOccupant(this, col,row); }
	public BoardSpace[,] Spaces { get { return spaces; } }
    public bool IsPlayerOnExitSpot () {
        return player.MySpace.HasExitSpot;
    }
    private bool CheckAreGoalsSatisfied () {
        if (goalObjects.Count == 0) { return true; } // If there's NO criteria, then sure, we're satisfied! For levels that're just about getting to the exit.
        for (int i=0; i<goalObjects.Count; i++) {
            if (!goalObjects[i].IsOn) { return false; } // return false if any of these guys aren't on.
        }
        return true; // Looks like we're soooo satisfied!!
    }

    // Serializing
	public Board Clone () {
		BoardData data = ToData();
		return new Board(data);
	}
	public BoardData ToData() {
		BoardData bd = new BoardData(NumCols,NumRows);
        bd.wrapH = WrapH;
        bd.wrapV = WrapV;
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
        WrapH = bd.wrapH;
        WrapV = bd.wrapV;
        
        // Empty out lists.
        allObjects = new List<BoardObject>();
        goalObjects = new List<IGoalObject>();
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
            else if (type == typeof(CrateGoalData)) {
                AddCrateGoal (objData as CrateGoalData);
            }
            else if (type == typeof(ExitSpotData)) {
                AddExitSpot (objData as ExitSpotData);
            }
            else {
                Debug.LogError("PropData not recognized to add to Board: " + type);
            }
        }
	}
    
    private void AddCrate (CrateData data) {
        Crate prop = new Crate (this, data);
        allObjects.Add (prop);
        objectsAddedThisMove.Add(prop);
    }
    private void AddCrateGoal (CrateGoalData data) {
        CrateGoal prop = new CrateGoal (this, data);
        allObjects.Add (prop);
        objectsAddedThisMove.Add(prop);
        goalObjects.Add (prop);
    }
    private void AddExitSpot (ExitSpotData data) {
        ExitSpot prop = new ExitSpot (this, data);
        allObjects.Add (prop);
        objectsAddedThisMove.Add(prop);
    }
    //private void AddPlayer (PlayerData data) {
    //    Player prop = new Player (this, data);
    //    allObjects.Add (prop);
    //    objectsAddedThisMove.Add(prop);
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
            // Reset PrevMoveDelta.
            for (int i=0; i<allObjects.Count; i++) { allObjects[i].ResetPrevMoveDelta(); }
            // Move it!
            BoardUtils.MoveOccupant(this, player.BoardPos, dir);
            // Update goals!
            UpdateAreGoalsSatisfied();
            
            // Dispatch event!
            GameManagers.Instance.EventManager.OnBoardExecutedMove(this);
        }
    }
    
    public void UpdateAreGoalsSatisfied() {
        // Update all GoalObjects' isOn; if any aren't, I'm not satisfied!
        bool areSatisfied = true;
        for (int i=0; i<goalObjects.Count; i++) {
            goalObjects[i].UpdateIsOn();
            areSatisfied &= goalObjects[i].IsOn;
        }
        AreGoalsSatisfied = areSatisfied;
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

