using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitSpotView : BoardObjectView {
	// Components
	[SerializeField] private Image i_arrow=null;
	[SerializeField] private Image i_backing=null;


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (BoardView _myBoardView, ExitSpot _myExitSpot) {
		base.InitializeAsBoardObjectView (_myBoardView, _myExitSpot);
	}


    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
	override public void UpdateVisualsPostMove () {
		base.UpdateVisualsPostMove ();
		UpdateOpenVisuals ();
	}
    public void UpdateOpenVisuals () {
        // Color me impressed!
        i_arrow.color = MyBoardView.AreGoalsSatisfied ? new Color(0.2f,0.8f,0f, 0.6f) : new Color(0,0,0, 0.16f);
        i_backing.color = MyBoardView.AreGoalsSatisfied ? new Color(0.6f,1f,0.3f, 0.3f) : new Color(0,0,0, 0);
    }


}
