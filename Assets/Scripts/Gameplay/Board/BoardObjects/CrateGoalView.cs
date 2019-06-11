using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrateGoalView : BoardObjectView {
	// Constants
	static public readonly Color color_on = new ColorHSB(60/255f,255/255f,200/255f, 1).ToColor();
	static public readonly Color color_off = new Color(0.9f,0.9f,0.9f, 0.84f);
	// Components
	[SerializeField] private Image i_body=null;
	// References
	private CrateGoal myCrateGoal;

	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (BoardView _myBoardView, CrateGoal _myCrateGoal) {
		base.InitializeAsBoardObjectView (_myBoardView, _myCrateGoal);
		myCrateGoal = _myCrateGoal;
	}

	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	override public void UpdateVisualsPostMove () {
		base.UpdateVisualsPostMove ();
		UpdateBodyColor ();
	}
	private void UpdateBodyColor () {
		i_body.color = myCrateGoal.IsOn ? color_on : color_off;
	}
}
