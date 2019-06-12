using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamView : MonoBehaviour {
	// Components
	private List<BeamSegmentView> segmentViews;
	// References
	[SerializeField] private BeamSourceView mySourceView=null;

	// Getters
	public Color BeamColor { get { return mySourceView.BeamColor; } }
	private Beam MyBeam { get { return mySourceView.MyBeamSource.Beam; } }
	public BoardView MyBoardView { get { return mySourceView.MyBoardView; } }
	private GameObject prefabGO_BeamSegmentRenderer { get { return ResourcesHandler.Instance.BeamSegmentRenderer; } }


	// ----------------------------------------------------------------
	//  Initialize / Destroy
	// ----------------------------------------------------------------
	public void Initialize (Transform parentTF) {
		// Chuck me OUT of my BeamSource and plop me onto the Beam layer!
		GameUtils.ParentAndReset(this.gameObject, parentTF);
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

		segmentViews = new List<BeamSegmentView>();
	}

	/** Instantiates a BeamSegmentView initialized by me, and returns it for further usage. */
	private BeamSegmentView GetNewSegmentView () {
		BeamSegmentView newView = Instantiate (prefabGO_BeamSegmentRenderer).GetComponent<BeamSegmentView>();
		newView.Initialize (this);
		return newView;
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void UpdateBeamView () {
		int numBeamSegments;
		// Oh, if my Beam has been destroyed, say it's got no segments; just stop rendering it completely. (There will be a small visual inconsistency, but it's almost impossible to notice.)
		if (!MyBeam.IsInPlay) {
			numBeamSegments = 0;
		}
		else {
			numBeamSegments = MyBeam.NumSegments;
		}
		// Make more segments if we need them!
		int numNewSegmentViews = numBeamSegments - segmentViews.Count;
		for (int i=0; i<numNewSegmentViews; i++) {
			BeamSegmentView newSegRend = Instantiate (prefabGO_BeamSegmentRenderer).GetComponent<BeamSegmentView>();
			newSegRend.Initialize (this);
			segmentViews.Add (newSegRend);
		}
		// Activate/deactivate the right ones!
		for (int i=0; i<numBeamSegments; i++) {
			BoardOccupant sourceOccupant = MyBeam.GetSegment(i).SourceOccupant;
			BoardObjectView sourceView = MyBoardView.TEMP_GetObjectView(sourceOccupant);
			segmentViews[i].Activate (sourceView);
		}
		for (int i=numBeamSegments; i<segmentViews.Count; i++) {
			segmentViews[i].Deactivate ();
		}
	}


}