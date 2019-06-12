using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BoardOccupant {
    // Properties
    public bool IsDead { get; private set; }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public Player (Board _boardRef, PlayerData _data) {
		base.InitializeAsBoardOccupant (_boardRef, _data);
        IsDead = _data.isDead;
	}
    // Serializing
    override public BoardObjectData ToData() {
		PlayerData data = new PlayerData(BoardPos, IsDead);
		return data;
	}
    // Overrides
    override protected void UpdateCanBeamEnterAndExit () {
        SetBeamCanEnterAndExit (true); // player does NOT block beams at ALL!
    }
    
    
    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public void Die() {
        IsDead = true;
    }



}
