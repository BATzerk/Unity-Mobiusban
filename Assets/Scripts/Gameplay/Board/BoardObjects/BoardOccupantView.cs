using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardOccupantView : BoardObjectView {
//TODO: Clean this all up. Is this class used or what?
	// Properties
	virtual protected Color GetPrimaryFill () { return new Color (1,1,1); } // Override this if we want to auto-color our body (without adding code)!
	//// Components
	//[SerializeField] private SpriteRenderer sr_body; // everyone has a primary body sprite for simplicity! Less code.
	// References
	protected BoardOccupant myOccupant; // a direct reference to my model. Doesn't change.
	//[SerializeField] private Sprite s_body;

	// ----------------------------------------------------------------
	//  Initialize / Destroy
	// ----------------------------------------------------------------
	protected void InitializeAsBoardOccupantView (BoardView _myBoardView, BoardOccupant _myOccupant) {
		base.InitializeAsBoardObjectView (_myBoardView, _myOccupant);
		myOccupant = _myOccupant;

		ApplyFundamentalVisualProperties ();
	}

	virtual protected void ApplyFundamentalVisualProperties () { // Note: We can make this overridable if we want unique visuals per object.
		//// If we DO have a bodySprite!...
		//if (sr_body != null) {
		//	// Color me impressed!
		//	sr_body.sprite = s_body;
		//	sr_body.color = GetPrimaryFill ();
		//}
	}


}
