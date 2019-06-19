using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class BoardObject {
    // Properties
    public BoardPos BoardPos { get; private set; }
    private bool isInPlay = true; // we set this to false when I'm removed from the Board!
    public Vector2Int PrevMoveDelta { get; private set; } // how far I moved the last move.
    // References
    public Board BoardRef { get; private set; }


    // Getters
	public bool IsInPlay { get { return isInPlay; } }
    public Vector2Int ColRow { get { return BoardPos.ColRow; } }
    public int Col { get { return BoardPos.ColRow.x; } }
    public int Row { get { return BoardPos.ColRow.y; } }
    public int ChirH { get { return BoardPos.ChirH; } }
    public int ChirV { get { return BoardPos.ChirV; } }
    public int SideFacing { get { return BoardPos.SideFacing; } }
    protected BoardSpace GetSpace (Vector2Int _colRow) { return BoardUtils.GetSpace (BoardRef, _colRow); }
    protected BoardSpace GetSpace (int _col,int _row) { return BoardUtils.GetSpace (BoardRef, _col,_row); }
	public BoardSpace MySpace { get { return GetSpace (Col,Row); } }
    public bool IsOrientationMatch(BoardObject other) {
        return BoardPos == other.BoardPos;
            //&& ChirH == other.ChirH
            //&& ChirV == other.ChirV
            //&& SideFacing == other.SideFacing;
    }
    
    // Serializing
    abstract public BoardObjectData ToData();
    
    
	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	protected void InitializeAsBoardObject (Board _boardRef, BoardObjectData data) {
		this.BoardRef = _boardRef;
		this.BoardPos = data.boardPos;
        
		// Automatically add me to the board!
		AddMyFootprint ();
	}
    
    
	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
    virtual public void SetColRow(Vector2Int _colRow, Vector2Int _moveDir) {
        //RemoveMyFootprint();
        PrevMoveDelta = _moveDir;
        BoardPos = new BoardPos(_colRow, ChirH,ChirV,SideFacing);
        //AddMyFootprint();
	}
    private void SetSideFacing(int _sideFacing) {
        BoardPos = new BoardPos(ColRow, ChirH,ChirV,_sideFacing);
        //BoardPos.SideFacing = sideFacing;
        OnSetSideFacing();
    }
    //private void SetChirality(int chirality) { Chirality = chirality; }
    public void ChangeSideFacing(int delta) { SetSideFacing(SideFacing+delta); }
    public void ChangeChirality(int deltaH, int deltaV) {
        BoardPos = new BoardPos(ColRow, ChirH*deltaH,ChirV*deltaV,SideFacing);
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
    virtual public void OnPlayerMoved () {}
    virtual protected void OnSetSideFacing () {}


}
