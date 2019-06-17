using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeamSourceView : BoardOccupantView {
	// Components
	[SerializeField] private BeamView beamView;
    [SerializeField] private Image i_body=null;
    // Properties
    public Color BeamColor { get; private set; }
    // References
    public BeamSource MyBeamSource { get; private set; }


    // ----------------------------------------------------------------
    //  Initialize / Destroy
    // ----------------------------------------------------------------
    override public void Initialize(BoardView _myBoardView, BoardObject bo) {
        MyBeamSource = bo as BeamSource;
        BeamColor = Colors.GetBeamColor (MyBeamSource.ChannelID);
		base.Initialize (_myBoardView, bo);
		beamView.Initialize (_myBoardView.tf_beamLines);
        i_body.color = BeamColor;
	}
	override protected void OnDestroy () {
		// Also destroy my BeamView because it's not parented to me (and won't be destroyed automatically)!
		Destroy (beamView.gameObject);
		beamView = null;
	}

	//override protected void ApplyFundamentalVisualProperties () {
	//	base.ApplyFundamentalVisualProperties ();
 //       // TODO: Use beamSegmentRendererOriginOffsetLoc?
	//	//beamSegmentRendererOriginOffsetLoc = myOccupant.IsMovable ? 0.32f : 0.40f; // start me closer to the center if we're movable. Not super neat (this is independent of any other diameter values).
	//}


    // ----------------------------------------------------------------
    //  Update Visuals
    // ----------------------------------------------------------------
	override public void UpdateVisualsPostMove () {
		base.UpdateVisualsPostMove ();
        
        // Update BeamView while all objs are animating.
        StartCoroutine(UpdateBeamViewForDur(0.3f));
    }
    
    private IEnumerator UpdateBeamViewForDur(float dur) {
        float timeLeft = dur;
        while (true) {
		    beamView.UpdateBeamView ();
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0) { break; }
            yield return null;
        }
	}


}
