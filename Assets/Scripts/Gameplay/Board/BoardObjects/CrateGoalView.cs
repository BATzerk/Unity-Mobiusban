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
    [SerializeField] private Sprite s_bodyNoStayOn=null;
    [SerializeField] private Sprite s_bodyStayOn=null;
	private CrateGoal myCrateGoal;

	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	override public void Initialize (BoardView _myBoardView, BoardObject bo) {
        myCrateGoal = bo as CrateGoal;
        base.Initialize (_myBoardView, bo);
        
        // Rotate i_body by corner!
        i_body.transform.localEulerAngles = new Vector3(0,0,-90*myCrateGoal.Corner);
        i_body.sprite = myCrateGoal.DoStayOn ? s_bodyStayOn : s_bodyNoStayOn;
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
