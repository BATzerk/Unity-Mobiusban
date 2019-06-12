using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamSegmentView : MonoBehaviour {
	// Components
	[SerializeField] private LineRenderer lineRenderer;
	[SerializeField] private ParticleSystem ps_endSparks;
	private List<BeamSegmentView> myChildren; // every segment has a list of children! For recursion with Portals.
	// Properties
	private int numPoints; // how many points in our list that are actually used. (We have a bigger bucket than we need so we don't have to remake the bucket!)
	private Vector2[] points; // the actual line data points, in board space.
	// References
	private BeamView myBeamView; // the BeamView I originate from!
	private BoardOccupantView sourceOccupantView; // NOTE: Because we can be recycled, this can change (when I'm recycled).

	// Getters
	public Color BeamColor { get { return myBeamView.BeamColor; } }
	private BoardView MyBoardView { get { return myBeamView.MyBoardView; } }
	private BeamRendererColliderArena colliderArena { get { return MyBoardView.BeamRendererColliderArena; } }
	//	public bool DoEndInPortal {
	//		
	//	}

	// DEBUG
	private void OnDrawGizmos () {
		Gizmos.color = Color.yellow;
		for (int i=0; i<numPoints-1; i++) {
			Gizmos.DrawLine (points[i], points[i+1]);
		}
	}

	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (BeamView _myBeamView) {
		myBeamView = _myBeamView;

		// Parent me on my BeamSourceRenderer.
		gameObject.transform.SetParent (myBeamView.transform);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localEulerAngles = Vector3.zero;

		points = new Vector2[20]; // this is pleeenty of points to render lots of lines.
		myChildren = new List<BeamSegmentView>();

		// Apply base visual properties.
		UpdateColorThickness ();
		lineRenderer.sortingOrder = 30; // renders over most BoardObjects.
	}

	public void Activate (BoardOccupantView _sourceOccupantView) {
		this.gameObject.SetActive (true);
		sourceOccupantView = _sourceOccupantView;
		UpdateSegmentView ();
	}
	public void Deactivate () {
		this.gameObject.SetActive (false);
		sourceOccupantView = null;
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	private void UpdateColorThickness () {
		float beamLineThickness = MyBoardView.UnitSize * 0.12f;
		lineRenderer.startWidth = lineRenderer.endWidth = beamLineThickness;

		lineRenderer.startColor = new Color (BeamColor.r,BeamColor.g,BeamColor.b, BeamColor.a*0.5f);
		lineRenderer.endColor = new Color (BeamColor.r,BeamColor.g,BeamColor.b, BeamColor.a*0.4f); // fade it out slightly towards the end
		GameUtils.SetParticleSystemColor (ps_endSparks, BeamColor);
	}


	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	/** Updates my line points AND my childrens' points, as well as adds/removes my BeamSegmentView children. */
	public void UpdateSegmentView () {
		// Update ME first.
		UpdatePoints ();
		UpdateLineRenderer ();
		// Update my children!
		for (int i=0; i<myChildren.Count; i++) {
			myChildren[i].UpdateSegmentView ();
		}
	}

	private void UpdatePoints () {
		numPoints = 1;
		float bodyRotation = sourceOccupantView.Rotation;
		float lineRotation = -bodyRotation;
		//float originOffsetLoc = sourceOccupantView.BeamSegmentRendererOriginOffsetLoc;
		Vector2 originOffset = Vector2.zero;//TODO: This. MathUtils.GetRotatedVector2Deg(new Vector2(0,MyBoardView.UnitSize*sourceOccupantView.Scale*originOffsetLoc), bodyRotation); // we visually draw an EXTRA point partially INSIDE of me. See next comment.
//		Vector2 secondPosOffset = GameMathUtils.GetRotatedVector2Deg(new Vector2(0,boardRef.UnitSize*0.5f), bodyRotation); // start our collision stuff from HERE, just outside of me so I don't intersect with my insides.
		points[0] = sourceOccupantView.Pos + originOffset;
//		points[1] = mySource.Body.Pos + secondPosOffset;
		AddPointRecursively (lineRotation);
	}
	/** lineRotation: Which way is the line pointing right now? */
	private void AddPointRecursively (float lineRotation) {
		Vector2 offset = MathUtils.GetVectorFromDeg(lineRotation)*5f; // actually OFFSET it a little bit FORWARD! So I can reflect off a mirror without being unable to escape it.
		Vector2 currentPos = points[numPoints-1] + offset;
		BeamRendererCollision collision = colliderArena.GetBeamRendererCollision (currentPos, lineRotation);
		// Add this point, wherever it is!
		points[numPoints] = collision.pos;
		numPoints ++;
		// If we're somehow out of points, we can stop here. :)
		if (numPoints >= points.Length) { return; }
		// How does this collision end??
		switch (collision.type) {
			case BeamRendererCollision.Types.End: // We've hit a regular wall? Stop adding points!
				return;
			case BeamRendererCollision.Types.Reflect: // Oh, a reflection off a Mirror? Add another point!
				AddPointRecursively (collision.exitRotation);
				return;
//			case BeamRendererCollision.Types.Portal: // Oooh, a Portal? Add new segments!!
//				AddSegmentsToOtherPortalViews (collision.objectView as PortalView);
//				return;
		}
	}



	private void UpdateLineRenderer () {
		lineRenderer.positionCount = numPoints;
		for (int i=0; i<numPoints; i++) {
			lineRenderer.SetPosition (i, points[i]);
		}

		// And finally, position the ps_endSparks!
		ps_endSparks.transform.localPosition = points[numPoints-1];
		float endSparksRotation = -LineUtils.GetAngle_Degrees (lineRenderer.GetPosition(numPoints-1), lineRenderer.GetPosition(numPoints-2));
		ps_endSparks.transform.localEulerAngles = new Vector3 (ps_endSparks.transform.localEulerAngles.x, ps_endSparks.transform.localEulerAngles.y, endSparksRotation);
	}


}
