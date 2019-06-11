using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class BoardObject {
    // Properties
    public Vector2Int BoardPos { get; private set; }
    private bool isInPlay = true; // we set this to false when I'm removed from the Board!
    private int _sideFacing; // corresponds to Sides.cs.
    public Vector2Int PrevMoveDelta { get; private set; } // how far I moved the last move.
    // References
    protected Board BoardRef { get; private set; }


    // Getters
    public int SideFacing {
        get { return _sideFacing; }
        set {
            _sideFacing = value;
            if (_sideFacing<Sides.Min) { _sideFacing += Sides.Max; }
            if (_sideFacing>=Sides.Max) { _sideFacing -= Sides.Max; }
        }
    }
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
	protected void InitializeAsBoardObject (Board _boardRef, BoardObjectData data) {
		this.BoardRef = _boardRef;
		this.BoardPos = data.boardPos;
        this.SideFacing = data.sideFacing;
        
		// Automatically add me to the board!
		AddMyFootprint ();
	}
    
    
	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
    public void SetColRow(Vector2Int _pos, Vector2Int moveDir) {
        //RemoveMyFootprint();
        PrevMoveDelta = moveDir;
        BoardPos = _pos;
        //AddMyFootprint();
	}
    public void SetSideFacing(int sideFacing) {
        SideFacing = sideFacing;
    }
    public void ResetPrevMoveDelta() {
        PrevMoveDelta = Vector2Int.zero;
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
