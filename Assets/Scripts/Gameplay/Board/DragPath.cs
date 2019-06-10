using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragPath {
    // Properties
    private List<BoardSpace> spaces;
    public bool IsCircuitComplete { get; private set; } // TRUE when we made a RECT with our path! When true, we can't keep adding spaces.
    public int ColorID { get; private set; }
    // References
    private Board myBoard;
    
    // Getters (Public)
    public bool HasSpaces { get { return NumSpaces > 0; } }
    public int NumSpaces { get { return spaces.Count; } }
    public BoardSpace GetSpace(int index) {
        if (index<0 || index>=NumSpaces) { return null; } // Safety check.
        return spaces[index];
    }
    public Vector2Int LastPos { get { return NumSpaces<1 ? Vector2Int.none : GetSpace(NumSpaces-1).BoardPos; } }
    public Vector2Int SecondLastPos { get { return NumSpaces<2 ? Vector2Int.none : GetSpace(NumSpaces-2).BoardPos; } }
    public bool MayAddSpaceToPath(Vector2Int boardPos) { return MayAddSpaceToPath(myBoard.GetSpace(boardPos)); }
    public bool MayAddSpaceToPath(BoardSpace space) {
        // Path is circuitComplete? Nah.
        if (IsCircuitComplete) { return false; }
        // Unplayable? Nah.
        if (space==null || !space.IsPlayable) { return false; }
        // Wrong Tile color? Nah.
        if (ColorID!=-1 && space.MyTile!=null && space.MyTile.ColorID!=ColorID) { return false; }
        // This space is NOT used? Sure!
        if (!space.IsPathOnMe) { return true; }
        // Ok, this space IS used. Return FALSE if it's at the end of the path, 'cause then we're not making a rectangle.
        return IsSpaceAtEndOfPath(space);
    }
    /** Returns TRUE if it's one of the last 3 spaces in the path. */
    private bool IsSpaceAtEndOfPath(BoardSpace space) {
        if (space == null) { return false; } // Safety check.
        if (NumSpaces > 3) {
            return spaces[spaces.Count - 1] != space
                && spaces[spaces.Count - 2] != space
                && spaces[spaces.Count - 3] != space;
        }
        // Not enough spaces for it to NOT be at the end.
        return false;
    }
    

    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public DragPath(Board _board) {
        this.myBoard = _board;
        spaces = new List<BoardSpace>();
        Clear();
    }
    
    
    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public void AddSpace(BoardSpace space) {
        // Our circuit's complete if this space is ALREADY in a path!
        IsCircuitComplete = space.IsPathOnMe;
        space.OnAddedToPath(); // tell the Space!
        spaces.Add(space);
        if (space.MyTile != null) {
            ColorID = space.MyTile.ColorID;
        }
    }
    public void RemoveSpace() {
        if (spaces.Count == 0) { return; } // Safety check.
        BoardSpace space = spaces[spaces.Count-1];
        space.OnRemovedFromPath(); // tell the Space!
        spaces.RemoveAt(spaces.Count-1);
        IsCircuitComplete = false; // This can never be true after removing a space.
    }
    public void Clear() {
        for (int i=spaces.Count-1; i>=0; --i) {
            spaces[i].OnRemovedFromPath(); // tell the Space!
        }
        spaces.Clear();
        ColorID = -1;
        IsCircuitComplete = false;
    }
    
    
}
