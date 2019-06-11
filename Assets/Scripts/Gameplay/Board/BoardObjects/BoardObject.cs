using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class BoardObject {
    // Properties
    public Vector2Int BoardPos { get; private set; }
    public int ChirH { get; private set; } // horizontal chirality (along y-axis). -1 or 1.
    public int ChirV { get; private set; } // vertical chirality (along x-axis). -1 or 1.
    private int _sideFacing; // corresponds to Sides.cs.
    private bool isInPlay = true; // we set this to false when I'm removed from the Board!
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
        this.ChirH = data.chirH;
        this.ChirV = data.chirV;
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
    private void SetSideFacing(int sideFacing) { SideFacing = sideFacing; }
    //private void SetChirality(int chirality) { Chirality = chirality; }
    public void ChangeSideFacing(int delta) { SideFacing += delta; }
    public void ChangeChirality(int deltaH, int deltaV) {
        ChirH *= deltaH;
        ChirV *= deltaV;
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
