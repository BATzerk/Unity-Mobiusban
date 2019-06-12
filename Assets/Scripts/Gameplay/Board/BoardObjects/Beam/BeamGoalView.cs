using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeamGoalView : BoardOccupantView {
	// Components
	[SerializeField] private Image i_goalLight=null;
	// Properties
	private Color beamColor;
	// References
	private BeamGoal myBeamGoal;

	// Getters
	//override protected Color GetPrimaryFillMovable () { return new Color (beamColor.r, beamColor.g, beamColor.b); }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (BoardView _boardView, BeamGoal _data) {
		myBeamGoal = _data;
		beamColor = Color.yellow;//TODO: Dis. Colors.GetBeamColor (_boardView.WorldIndex, _data.ChannelID);
		base.InitializeAsBoardOccupantView (_boardView, _data);
	}

	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	override public void UpdateVisualsPostMove () {
		base.UpdateVisualsPostMove ();
		UpdateGoalLight ();
	}

	public void UpdateGoalLight () {
		// Update sr_goalLight!
		i_goalLight.color = myBeamGoal.IsOn ? beamColor : new Color(0.2f,0.2f,0.2f, 0.6f);
	}

}
