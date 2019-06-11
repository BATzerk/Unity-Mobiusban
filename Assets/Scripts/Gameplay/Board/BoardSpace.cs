using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpace {
	// Properties
	public Vector2Int BoardPos { get; private set; }
	private bool isPlayable = true;
    private bool isWallL, isWallT; // walls can only be on the LEFT and TOP of spaces.
	// References
    public BoardOccupant MyOccupant { get; private set; } // occupants sit on my face. Only one Occupant occupies each space.
    private ExitSpot MyExitSpot;

    // Getters
    public bool IsPlayable { get { return isPlayable; } }
    public int Col { get { return BoardPos.x; } }
    public int Row { get { return BoardPos.y; } }
    public bool HasExitSpot { get { return MyExitSpot != null; } }
    public bool HasOccupant { get { return MyOccupant != null; } }
    public bool HasImmovableOccupant { get { return MyOccupant!=null && !MyOccupant.IsMovable; } }
    public bool IsWall(int side) {
        switch(side) {
            case Sides.L: return isWallL;
            case Sides.T: return isWallT;
            default: return false;
        }
    }
    public bool MayOccupantEverExit(Vector2Int dirOut) {
        int side = MathUtils.GetSide(dirOut);
        if (IsWall(side)) { return false; }
        return true;
    }
    public bool MayOccupantEverEnter(Vector2Int dirIn) {
        int side = MathUtils.GetSide(dirIn);
        return MayOccupantEverEnter(side);
    }
    /** Side: Relative to ME. */
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
        isWallL = _data.isWallL;
        isWallT = _data.isWallT;
	}
	public BoardSpaceData ToData () {
        BoardSpaceData data = new BoardSpaceData(Col, Row) {
            isPlayable = isPlayable,
            isWallL = isWallL,
            isWallT = isWallT,
        };
        return data;
	}
    
    
    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public void AddWall(int side) {
        switch(side) {
            case Sides.L: isWallL = true; break;
            case Sides.T: isWallT = true; break;
            default: Debug.LogError("Whoa, we're calling AddWall for a side that's NOT Top or Left: " + side); break;
        }
    }
    public void SetMyExitSpot(ExitSpot bo) {
        if (MyExitSpot != null) {
            throw new UnityException ("Oops! Trying to set a Space's MyExitSpot, but that Space already has an ExitSpot! " + Col + ", " + Row);
        }
        MyExitSpot = bo;
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
