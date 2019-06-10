using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerView : BoardObjectView {
	// References
	public Player MyPlayer { get; private set; }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (BoardView _myBoardView, Player _player) {
		MyPlayer = _player;
		base.InitializeAsBoardObjectView (_myBoardView, _player);
	}



}

