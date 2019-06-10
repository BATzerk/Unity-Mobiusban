using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpace {
	// Properties
	public Vector2Int BoardPos { get; private set; }
	private bool isPlayable = true;
    public int NumTimesInPath { get; private set; } // usually 0 or 1, but will be 2 for the space we loop back around on to make a complete circuit.
	// References
    public BoardOccupant MyOccupant { get; private set; } // occupants sit on my face. Only one Occupant occupies each space.

    // Getters
    public bool IsPlayable { get { return isPlayable; } }
    public bool IsPathOnMe { get { return NumTimesInPath > 0; } }
	public int Col { get { return BoardPos.x; } }
	public int Row { get { return BoardPos.y; } }
    /** I'm open if I'm playable and there's nothing on me. :) */
    public bool IsOpen() {
        return isPlayable && MyOccupant==null;
    }
    public Tile MyTile { get { return MyOccupant as Tile; } }
    
    
	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public BoardSpace (BoardSpaceData _data) {
        BoardPos = _data.boardPos;
        isPlayable = _data.isPlayable;
        NumTimesInPath = 0;
	}
	public BoardSpaceData SerializeAsData () {
        BoardSpaceData data = new BoardSpaceData(Col, Row) {
            isPlayable = isPlayable
        };
        return data;
	}
    
    
    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
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
    
    public void OnAddedToPath() {
        NumTimesInPath ++;
    }
    public void OnRemovedFromPath() {
        NumTimesInPath --;
    }



}
