using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamSegmentView : MonoBehaviour {
	// Components
	[SerializeField] private List<ImageLine> imgLines=new List<ImageLine>(); // NOTE: These lines AREN'T connected. No reflections for now.
	[SerializeField] private ParticleSystem ps_endSparks=null;
	// Properties
	private int numLines; // how many lines in our list that are actually used. (We have a bigger bucket than we need so we don't have to remake the bucket!)
	private Line[] lines; // the actual line datas, in board space.
    private float beamLineThickness;
    private Color beamLineColor;
	// References
	private BeamView myBeamView; // the BeamView I originate from!
	private BoardObjectView sourceObjView; // NOTE: Because we can be recycled, this can change (when I'm recycled).

	// Getters
	public Color BeamColor { get { return myBeamView.BeamColor; } }
	private BoardView MyBoardView { get { return myBeamView.MyBoardView; } }
	private BeamRendererColliderArena colliderArena { get { return MyBoardView.BeamRendererColliderArena; } }

	// DEBUG
	private void OnDrawGizmos () {
        if (lines == null) { return; } // Safety check.
		Gizmos.color = Color.yellow;
		for (int i=0; i<lines.Length-1; i++) {
			Gizmos.DrawLine (lines[i].start, lines[i].end);
		}
	}

	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (BeamView _myBeamView) {
		myBeamView = _myBeamView;

		// Parent me on my BeamSourceRenderer.
        GameUtils.ParentAndReset(this.gameObject, myBeamView.transform);
        
        beamLineThickness = MyBoardView.UnitSize * 0.12f;
        beamLineColor = new Color (BeamColor.r,BeamColor.g,BeamColor.b, BeamColor.a*0.5f);
        GameUtils.SetParticleSystemColor (ps_endSparks, BeamColor);

		lines = new Line[20]; // this is pleeenty of points to render lots of lines.

		//// Apply base visual properties.
		//UpdateColorThickness ();
	}

	public void Activate (BoardObjectView _view) {
        sourceObjView = _view;
        //Activate(_view.Pos, _view.Rotation);
    //}
    //private void Activate(Vector2 _pos, float _rot) {
        this.gameObject.SetActive (true);
        //sourcePos = _pos;
        //sourceRot = _rot;
        
		UpdateSegmentView ();
	}
	public void Deactivate () {
		this.gameObject.SetActive (false);
		sourceObjView = null;
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	private void ApplyColorThickness () {
      //  for (int i=0; i<imgLines.Count; i++) {
    		//imgLines[i].SetThickness(beamLineThickness);
    		//imgLines[i].SetColor(beamLineColor);
        //}
		//lineRenderer.endColor = new Color (BeamColor.r,BeamColor.g,BeamColor.b, BeamColor.a*0.4f); // fade it out slightly towards the end
	}


	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	/** Updates the collision logic, and then updates the sprites. */
	public void UpdateSegmentView () {
		UpdateLines ();
		UpdateLineRenderers ();
	}

	private void UpdateLines () {
		numLines = 0;
        //float sourceRot = Mathf.Round(sourceObjView.Rotation); // Note: Round the source rotation so we don't get subtle weird angle drifting.
        Vector2 sourcePos = sourceObjView.Pos;
        float sourceRot = sourceObjView.Rotation;
        // HACK TEMP
        if (sourceObjView.Scale.x < 0) {
            sourceRot = sourceRot + 180;
        }
        if (sourceObjView.Scale.y < 0) {
            sourceRot = -sourceRot + 180;
        }
        
		float originOffsetLoc = sourceObjView.BeamOriginOffsetLoc;
		Vector2 originOffset = MathUtils.GetRotatedVector2Deg(new Vector2(0,MyBoardView.UnitSize*originOffsetLoc), sourceRot);
		AddLineRecursively (sourcePos+originOffset, sourceRot);
	}
	/** lineRotation: Which way is the line pointing right now? */
	private void AddLineRecursively (Vector2 _pos, float _rot) {
		Vector2 offset = MathUtils.GetVectorFromDeg(_rot)*1f; // actually OFFSET it a little bit FORWARD! So I can reflect off a mirror without being unable to escape it.
		BeamRendererCollision coll = colliderArena.GetBeamRendererCollision (_pos+offset, _rot);
		// Add this line!
		lines[numLines] = new Line(_pos, coll.pos);
		numLines ++;
		// Safety check: If we're somehow out of lines, we can stop here. :)
		if (numLines >= lines.Length) { return; }
		// How does this collision end??
		switch (coll.collType) {
			case BeamRendererCollision.Types.End: // We've hit a regular wall? Stop adding!
				return;
            //case BeamRendererCollision.Types.Reflect: // Oh, a reflection off a Mirror? Add another point!
                //AddPointRecursively (collision.exitRotation);
                //return;
            case BeamRendererCollision.Types.WarpFlipH: // Warp? Add another line!
            case BeamRendererCollision.Types.WarpFlipV:
                AddWarpLineRecursively(coll);
                return;
		}
	}
    /// Gets the necessary info for adding a new line via a warp, adds it, and keeps looking.
    private void AddWarpLineRecursively(BeamRendererCollision coll) {
        float angleIn = coll.angleIn;
        Vector2 posIn = coll.pos;
        float loc = LineUtils.PosToLoc(coll.collLine.line, posIn);
        // Translate pos/rot in to pos/rot out.
        float angleOut;
        switch (coll.collLine.collType) {
            case BeamRendererCollision.Types.WarpFlipH:
                angleOut = MathUtils.FlipDegHorz(angleIn);
                break;
            case BeamRendererCollision.Types.WarpFlipV:
                angleOut = MathUtils.FlipDegVert(angleIn);
                break;
            default:
                angleOut = angleIn;
                break;
        }

        Line lineOut = coll.collLine.lineOut;
        Vector2 posOut = lineOut.LocToPos(loc);
        
        AddLineRecursively(posOut, angleOut);
    }



	private void UpdateLineRenderers () {
        // Update ImageLines.
		//lineRenderer.SetNumPoints(numPoints);
		//for (int i=0; i<numPoints; i++) {
		//	lineRenderer.SetPoint (i, points[i]);
		//}
        // Add missing ImageLines!
        for (int i=imgLines.Count; i<numLines; i++) {
            ImageLine newObj = Instantiate(ResourcesHandler.Instance.ImageLine).GetComponent<ImageLine>();
            newObj.Initialize(this.transform);
            newObj.SetAnchors(Vector2.zero, Vector2.zero); // bottom-left anchor.
            newObj.SetColor(beamLineColor);
            newObj.SetThickness(beamLineThickness);
            imgLines.Add(newObj);
        }
        // Update all ImageLines!
        for (int i=0; i<imgLines.Count; i++) {
            bool isVisible = i < numLines;
            imgLines[i].IsVisible = isVisible;
            imgLines[i].SetStartAndEndPos(lines[i]);
        }

		// Position the ps_endSparks!
        Line endLine = lines[numLines-1];
		ps_endSparks.transform.localPosition = endLine.end;
		float endSparksRot = endLine.GetAngleDeg();// -LineUtils.GetAngle_Degrees (lineRenderer.GetPoint(numPoints-1), lineRenderer.GetPoint(numPoints-2));
		ps_endSparks.transform.localEulerAngles = new Vector3 (ps_endSparks.transform.localEulerAngles.x, ps_endSparks.transform.localEulerAngles.y, endSparksRot);
	}


}
