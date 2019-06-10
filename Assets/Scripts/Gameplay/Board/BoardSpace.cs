using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpace {
	// Properties
	public Vector2Int BoardPos { get; private set; }
	private bool isPlayable = true;
    private bool[] isWall; // index is side.
	// References
    public BoardOccupant MyOccupant { get; private set; } // occupants sit on my face. Only one Occupant occupies each space.

    // Getters
    public bool IsPlayable { get { return isPlayable; } }
	public int Col { get { return BoardPos.x; } }
	public int Row { get { return BoardPos.y; } }
    ///** I'm open if I'm playable and there's nothing on me. :) */
    //public bool IsOpen() {
    //    return isPlayable && MyOccupant==null;
    //}
    public bool HasOccupant { get { return MyOccupant!=null; } }
    public bool HasImmovableOccupant { get { return MyOccupant!=null && !MyOccupant.IsMovable; } }
    private bool IsWall(int side) { return isWall[side]; }
    /** Side: Relative to ME. */
    public bool MayOccupantEverEnter(Vector2Int posFrom) {
        Vector2Int dir = posFrom - BoardPos;
        int side = MathUtils.GetSide(dir);
        return MayOccupantEverEnter(side);
    }
    private bool MayOccupantEverEnter(int side) {
        if (!IsPlayable) { return false; }
        if (HasImmovableOccupant) { return false; }
        if (IsWall(side)) { return false; }
        return true;
    }
    
    
	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public BoardSpace (BoardSpaceData _data) {
        BoardPos = _data.boardPos;
        isPlayable = _data.isPlayable;
        isWall = new bool[4];
	}
	public BoardSpaceData ToData () {
        BoardSpaceData data = new BoardSpaceData(Col, Row) {
            isPlayable = isPlayable
        };
        return data;
	}
    
    
    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public void AddWall(int side) {
        isWall[side] = true;
    }
	public void SetMyOccupant (BoardOccupant _bo) {
		if (MyOccupant != null) {
			throw new UnityException ("Oops! Trying to set a Space's Occupant, but that Space already has an Occupant! original: " + MyOccupant.GetType() + ", new: " + _bo.GetType().ToString() + ". " + Col + ", " + Row);
		}
		MyOccupant = _bo;
	}
	public void RemoveMyOccupant (BoardOccupant _bo) {
		if (MyOccupant != _bo) {
			throw new UnityException ("Oops! We're trying to remove a " + _bo.GetType() + " from a space that doesn't own it! " + Col + " " + Row + ".");
		}
		MyOccupant = null;
	}



}
