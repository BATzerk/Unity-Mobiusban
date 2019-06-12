using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : BoardOccupant {
    // Properties
    public bool[] IsDimple { get; private set; }
    public bool DoAutoMove { get; private set; }
    public Vector2Int AutoMoveDir { get; private set; }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public Crate (Board _boardRef, CrateData _data) {
		base.InitializeAsBoardOccupant (_boardRef, _data);
        IsDimple = GameUtils.CopyBoolArray(_data.isDimple);
        DoAutoMove = _data.doAutoMove;
        AutoMoveDir = _data.autoMoveDir;
    }
    
    // Serializing
	override public BoardObjectData ToData() {
        return new CrateData(BoardPos, GameUtils.CopyBoolArray(IsDimple), DoAutoMove, AutoMoveDir);
	}


    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    override public void SetColRow(Vector2Int _colRow, Vector2Int _moveDir) {
        base.SetColRow(_colRow, _moveDir);
        AutoMoveDir = _moveDir;
    }
    public override void OnPlayerMoved() {
        base.OnPlayerMoved();
        // I auto-move, I HAVE a dir to auto-move, AND I DIDN'T just move?...
        if (DoAutoMove && AutoMoveDir!=Vector2Int.zero && PrevMoveDelta==Vector2Int.zero) {
            // We CAN move. Do!
            if (BoardUtils.MayMoveOccupant(BoardRef, ColRow, AutoMoveDir)) {
                BoardUtils.MoveOccupant(BoardRef, ColRow, AutoMoveDir);
            }
            // We CANNOT move. Zero-out AutoMoveDir.
            else {
                AutoMoveDir = Vector2Int.zero;
            }
        }
    }




}
