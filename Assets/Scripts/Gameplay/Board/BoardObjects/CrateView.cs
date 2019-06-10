using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrateView : BoardObjectView {
	// References
	public Crate MyCrate { get; private set; }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (BoardView _myBoardView, Crate _tile) {
		MyCrate = _tile;
		base.InitializeAsBoardObjectView (_myBoardView, MyCrate);
        
        //i_body.color = Colors.TileColor(MyCrate.ColorID);
	}



}

