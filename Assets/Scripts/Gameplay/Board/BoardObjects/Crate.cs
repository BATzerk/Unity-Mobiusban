using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : BoardOccupant {
    // Properties
    public bool[] IsDimple { get; private set; }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public Crate (Board _boardRef, CrateData _data) {
		base.InitializeAsBoardOccupant (_boardRef, _data);
        IsDimple = GameUtils.CopyBoolArray(_data.isDimple);
    }
    
    // Serializing
	override public BoardObjectData ToData() {
        return new CrateData(BoardPos, ChirH,ChirV,SideFacing, GameUtils.CopyBoolArray(IsDimple));
	}




}
