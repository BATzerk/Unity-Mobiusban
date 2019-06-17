using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeamGoalView : BoardOccupantView {
	// Components
    [SerializeField] private Image i_body=null;
    [SerializeField] private Image i_goalLight=null;
	// Properties
	private Color beamColor;
	// References
    [SerializeField] private Sprite s_bodyMovable=null;
    [SerializeField] private Sprite s_bodyNotMovable=null;
	private BeamGoal myBeamGoal;

	// Getters
	//override protected Color GetPrimaryFillMovable () { return new Color (beamColor.r, beamColor.g, beamColor.b); }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
    override public void Initialize (BoardView _myBoardView, BoardObject bo) {
        myBeamGoal = bo as BeamGoal;
        beamColor = Colors.GetBeamColor (myBeamGoal.ChannelID);
        base.Initialize (_myBoardView, bo);
        
        i_body.color = beamColor;
        i_body.sprite = myBeamGoal.IsMovable ? s_bodyMovable : s_bodyNotMovable;
	}

    // ----------------------------------------------------------------
    //  Update Visuals
    // ----------------------------------------------------------------
	override public void UpdateVisualsPostMove () {
		base.UpdateVisualsPostMove ();
		UpdateGoalLight ();
	}

	public void UpdateGoalLight () {
		i_goalLight.color = myBeamGoal.IsOn ? beamColor : new Color(0.2f,0.2f,0.2f, 0.6f);
	}

}
