using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSpotView : BoardObjectView {
	// Components
	[SerializeField] private SpriteRenderer sr_arrow;
	[SerializeField] private SpriteRenderer sr_backing;


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (BoardView _myBoardView, ExitSpot _myExitSpot) {
		base.InitializeAsBoardObjectView (_myBoardView, _myExitSpot);
	}

	public void UpdateOpenVisuals () {
		// Color me impressed!
		sr_arrow.color = MyBoardView.AreGoalsSatisfied ? new Color(0.2f,0.8f,0f, 0.8f) : new Color(0,0,0, 0.16f);
		sr_backing.color = MyBoardView.AreGoalsSatisfied ? new Color(0.6f,1f,0.3f, 0.8f) : new Color(0,0,0, 0);
	}

	override public void UpdateVisualsPostMove () {
		base.UpdateVisualsPostMove ();
		UpdateOpenVisuals ();
	}

}
