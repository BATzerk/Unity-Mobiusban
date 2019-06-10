using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class BoardObject {
    // Properties
    public Vector2Int BoardPos { get; private set; }
    private bool isInPlay = true; // we set this to false when I'm removed from the Board!
	// References
	protected Board BoardRef { get; private set; }

	// Getters
	public bool IsInPlay { get { return isInPlay; } }
    public int Col { get { return BoardPos.x; } }
	public int Row { get { return BoardPos.y; } }
	protected BoardSpace GetSpace (int _col,int _row) { return BoardUtils.GetSpace (BoardRef, _col,_row); }
	public BoardSpace MySpace { get { return GetSpace (Col,Row); } }
    
    // Serializing
    abstract public BoardObjectData ToData();
    
    
	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	protected void InitializeAsBoardObject (Board _boardRef, Vector2Int _boardPos) {
		BoardRef = _boardRef;
		BoardPos = _boardPos;
        
		// Automatically add me to the board!
		AddMyFootprint ();
	}
    
    
	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
    public void SetColRow(Vector2Int _pos) {
        //RemoveMyFootprint();
		BoardPos = _pos;//new Vector2Int(_col,_row);
        //AddMyFootprint();
	}

	/** This removes me from the Board completely and permanently. */
	public void RemoveFromPlay () {
		// Gemme outta here!
		isInPlay = false;
		RemoveMyFootprint();
		// Tell my boardRef I'm toast!
		BoardRef.OnObjectRemovedFromPlay (this);
	}

	// Override these!
	virtual public void AddMyFootprint () {}
	virtual public void RemoveMyFootprint () {}


}
