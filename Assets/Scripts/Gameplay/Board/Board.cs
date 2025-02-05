﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Board {
    // Properties
    public int NumCols { get; private set; }
    public int NumRows { get; private set; }
    public WrapTypes WrapH { get; private set; }
    public WrapTypes WrapV { get; private set; }
    public bool AreGoalsSatisfied { get; private set; }
    public int NumExitSpots { get; private set; }
    // Objects
    public BoardSpace[,] spaces;
    public List<Player> players;
    public List<BoardObject> allObjects; // includes every object EXCEPT Player!
    // Reference Lists
    public List<IGoalObject> goalObjects; // contains JUST the objects that have winning criteria.
    public List<BoardObject> objectsAddedThisMove;
    private List<BeamSource> beamSources=new List<BeamSource>();

	// Getters
    public bool DoWrapH { get { return WrapH==WrapTypes.Parallel || WrapH==WrapTypes.Flip; } }
    public bool DoWrapV { get { return WrapV==WrapTypes.Parallel || WrapV==WrapTypes.Flip; } }
    public int NumGoalObjects { get { return goalObjects.Count; } }
    public BoardSpace GetSpace(Vector2Int pos) { return GetSpace(pos.x, pos.y); }
    public BoardSpace GetSpace(int col,int row) { return BoardUtils.GetSpace(this, col,row); }
    public BoardOccupant GetOccupant(int col,int row) { return BoardUtils.GetOccupant(this, col,row); }
    public BoardOccupant GetOccupant(Vector2Int pos) { return GetOccupant(pos.x, pos.y); }
	public BoardSpace[,] Spaces { get { return spaces; } }
    public bool IsAnyPlayerOnExitSpot () {
        for (int i=0; i<players.Count; i++) {
            if (players[i].MySpace.HasExitSpot && players[i].MySpace.MyExitSpot.IsOrientationMatch(players[i])) {
                return true;
            }
        }
        return false;
    }
    public bool IsEveryPlayerDead() {
        for (int i=0; i<players.Count; i++) { if (!players[i].IsDead) { return false; } }
        return true;
    }
    //private bool CheckAreGoalsSatisfied () {
    //    if (goalObjects.Count == 0) { return true; } // If there's NO criteria, then sure, we're satisfied! For levels that're just about getting to the exit.
    //    for (int i=0; i<goalObjects.Count; i++) {
    //        if (!goalObjects[i].IsOn) { return false; } // return false if any of these guys aren't on.
    //    }
    //    return true; // Looks like we're soooo satisfied!!
    //}

    // Serializing
	public Board Clone() {
		BoardData data = ToData();
		return new Board(data);
	}
	public BoardData ToData() {
		BoardData bd = new BoardData(NumCols,NumRows);
        bd.wrapH = WrapH;
        bd.wrapV = WrapV;
        //bd.playerData = player.ToData() as PlayerData;
        foreach (Player p in players) { bd.playerDatas.Add(p.ToData() as PlayerData); }
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
        players = new List<Player>();
        allObjects = new List<BoardObject>();
        goalObjects = new List<IGoalObject>();
        objectsAddedThisMove = new List<BoardObject>();

		// Add all gameplay objects!
		MakeBoardSpaces (bd);
		AddPropsFromBoardData (bd);
        AreGoalsSatisfied = GetAreGoalsSatisfied(); // know from the get-go.
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
        //player = new Player(this, bd.playerData);
		foreach (BoardObjectData objData in bd.allObjectDatas) {
            System.Type type = objData.GetType();
            if (type == typeof(BeamGoalData)) {
                AddBeamGoal (objData as BeamGoalData);
            }
            else if (type == typeof(BeamSourceData)) {
                AddBeamSource (objData as BeamSourceData);
            }
            else if (type == typeof(CrateData)) {
                AddCrate (objData as CrateData);
            }
            else if (type == typeof(CrateGoalData)) {
                AddCrateGoal (objData as CrateGoalData);
            }
            else if (type == typeof(ExitSpotData)) {
                AddExitSpot (objData as ExitSpotData);
            }
            else if (type == typeof(PlayerData)) {
                AddPlayer (objData as PlayerData);
            }
            else if (type == typeof(VoydData)) {
                AddVoyd (objData as VoydData);
            }
            else {
                Debug.LogError("PropData not recognized to add to Board: " + type);
            }
        }
	}
    
    private void AddBeamGoal (BeamGoalData data) {
        BeamGoal prop = new BeamGoal (this, data);
        allObjects.Add (prop);
        objectsAddedThisMove.Add(prop);
        goalObjects.Add (prop);
    }
    private void AddBeamSource (BeamSourceData data) {
        BeamSource prop = new BeamSource (this, data);
        allObjects.Add (prop);
        objectsAddedThisMove.Add(prop);
        beamSources.Add(prop);
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
        NumExitSpots ++;
    }
    private void AddPlayer (PlayerData data) {
        Player prop = new Player (this, data);
        allObjects.Add (prop);
        players.Add(prop);
        objectsAddedThisMove.Add(prop);
    }
    private void AddVoyd (VoydData data) {
        Voyd prop = new Voyd (this, data);
        allObjects.Add (prop);
        objectsAddedThisMove.Add(prop);
    }


	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
    public void OnObjectRemovedFromPlay (BoardObject bo) {
        // Remove it from its lists!
        allObjects.Remove (bo);
        if (bo is ExitSpot) { NumExitSpots --; }
        else { Debug.LogError ("Trying to RemoveFromPlay an Object of type " + bo.GetType() + ", but our OnObjectRemovedFromPlay function doesn't recognize this type!"); }
    }


	// ----------------------------------------------------------------
	//  Makin' Moves
	// ----------------------------------------------------------------
    private bool MayExecuteMove(Vector2Int dir) {
        if (IsEveryPlayerDead()) { return false; }
        //if (AreGoalsSatisfied) { return false; } // We can't execute after we've won.
        return BoardUtils.MayMovePlayers(this, players, dir); // Ok, now just check if it's legal!
    }
    private bool GetAreGoalsSatisfied() {
        if (IsEveryPlayerDead()) { return false; } // Players are all kaput? Nah, we need at least one alive.
        if (goalObjects.Count == 0) { return true; } // If there's NO criteria, then sure, we're satisfied! For levels that're just about getting to the exit.
        for (int i=0; i<goalObjects.Count; i++) {
            if (!goalObjects[i].IsOn) { return false; } // return false if any of these guys aren't on.
        }
        return true; // Looks like we're soooo satisfied!!
    }
    
    
    public void MovePlayerAttempt(Vector2Int dir) {
        if (MayExecuteMove(dir)) {
            // Clear out the list NOW.
            objectsAddedThisMove.Clear();
            // Reset PrevMoveDelta.
            ResetOccupantsPrevMoveDelta();
            // Move Player!
            BoardUtils.MovePlayers(this, players, dir);
            // Tell all other Occupants!
            for (int i=0; i<allObjects.Count; i++) { allObjects[i].OnPlayerMoved(); }
            // Call OnMoveComplete!
            OnMoveComplete();
        }
    }
    
    private void OnMoveComplete () {
        // Just redundantly remake all the beams.
        RemakeAllBeams();
        // Check if the player's in a toaster oven!
        UpdatePlayersIsDead();
        // Update Goals!
        foreach (IGoalObject igo in goalObjects) { igo.UpdateIsOn (); }
        AreGoalsSatisfied = GetAreGoalsSatisfied();
        // Dispatch event!
        GameManagers.Instance.EventManager.OnBoardExecutedMove(this);
    }


    private void ResetOccupantsPrevMoveDelta() {
        //player.ResetPrevMoveDelta();
        for (int i=0; i<allObjects.Count; i++) { allObjects[i].ResetPrevMoveDelta(); }
    }
    private void RemakeAllBeams () {
        foreach (BeamSource bs in beamSources) { bs.Beam.RemakeBeam (); }
    }
    private void UpdatePlayersIsDead() {
        foreach (Player p in players) {
            if (p.IsDead) { continue; } // Already dead? Skip 'em.
            if (p.MySpace.IsLethalBeamOverMe()) {
                p.Die();
            }
        }
    }
    //private bool UpdateAreGoalsSatisfied() {
    //    // Update all GoalObjects' isOn; if any aren't, I'm not satisfied!
    //    bool areSatisfied = true;
    //    for (int i=0; i<goalObjects.Count; i++) {
    //        areSatisfied &= goalObjects[i].IsOn;
    //    }
    //    AreGoalsSatisfied = areSatisfied;
    //}
    
    public void ToggleVoydsIsOn() {
        // Togga da vayds.
        for (int i=0; i<allObjects.Count; i++) {
            if (allObjects[i] is Voyd) {
                (allObjects[i] as Voyd).ToggleIsOn();
            }
        }
        // Merve der plurer(s).
        MovePlayerAttempt(Vector2Int.zero);
    }
    
    
    
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //  Debug
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void Debug_PrintSomeBoardLayout() {
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
    public void Debug_PrintBeamSpaces() {
        string str = "";
        for (int row=0; row<NumRows; row++) {
            for (int col=0; col<NumCols; col++) {
                BoardSpace space = GetSpace(col,row);
                if (space.BeamsOverMe.Count > 0) {
                    str += space.BeamsOverMe[0].ChannelID;
                }
                else {
                    str += ".";
                }
            }
            str += "\n";
        }
        Debug.Log("Beams:\n" + str);
    }





}

