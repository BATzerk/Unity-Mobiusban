﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardView))]
public class BeamRendererColliderArena : MonoBehaviour {
	// Properties
	private List<BeamRendererColliderLine> boundsLines; // just MY way-big-boundary lines. These are added to the allColliderLines list.
	// References
	private List<BeamRendererColliderLine> allColliderLines;


    // DEBUG
    private void OnDrawGizmos () {
        if (allColliderLines == null) { return; }
        Gizmos.color = Color.magenta;
        foreach (BeamRendererColliderLine line in allColliderLines) {
            Gizmos.DrawLine (line.line.start, line.line.end);
        }
    }

	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (Board b, Rect r_board) {
		allColliderLines = new List<BeamRendererColliderLine>();
        boundsLines = new List<BeamRendererColliderLine>();

        //// Populate me first with far-reaching boundaries!!
        //Rect br = new Rect (-1000,-1000, 3000,3000);
		//boundsLines.Add (new BeamRendererColliderLine (null, new Line (br.xMin,br.yMin,  br.xMax,br.yMin), BeamRendererCollision.Types.End));
		//boundsLines.Add (new BeamRendererColliderLine (null, new Line (br.xMax,br.yMin,  br.xMax,br.yMax), BeamRendererCollision.Types.End));
		//boundsLines.Add (new BeamRendererColliderLine (null, new Line (br.xMax,br.yMax,  br.xMin,br.yMax), BeamRendererCollision.Types.End));
		//boundsLines.Add (new BeamRendererColliderLine (null, new Line (br.xMin,br.yMax,  br.xMin,br.yMin), BeamRendererCollision.Types.End));
        // Add bounds on each Board side!
        Rect br = r_board;
        br.position = Vector2.zero;
        // Left
        switch (b.WrapH) {
            case WrapTypes.Parallel:
                boundsLines.Add(new BeamRendererColliderLine(LineUtils.GetLineCW(br, Sides.L), new Vector2(br.width,0), BeamRendererCollision.Types.Warp)); break;
            //case WrapTypes.Flip:TODO: This.
                //boundsLines.Add(new BeamRendererColliderLine(null, LineUtils.GetLine(br, Sides.L), BeamRendererCollision.Types.Warp)); break;
            default:
                boundsLines.Add(new BeamRendererColliderLine(null, LineUtils.GetLineCW(br, Sides.L), BeamRendererCollision.Types.End)); break;
        }
        //// Right
        //switch (b.WrapH) {
        //    case WrapTypes.Parallel:
        //        boundsLines.Add(new BeamRendererColliderLine(LineUtils.GetLine(br, Sides.R), LineUtils.GetLine(br, Sides.L), BeamRendererCollision.Types.Warp)); break;
        //    //case WrapTypes.Flip:TODO: This.
        //        //boundsLines.Add(new BeamRendererColliderLine(null, LineUtils.GetLine(br, Sides.R), BeamRendererCollision.Types.Warp)); break;
        //    default:
        //        boundsLines.Add(new BeamRendererColliderLine(null, LineUtils.GetLine(br, Sides.R), BeamRendererCollision.Types.End)); break;
        //}
        foreach (BeamRendererColliderLine boundLine in boundsLines) {
            BeamRendererColliderLine _boundLine = boundLine;
            AddLine (ref _boundLine);
        }
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void AddLine (ref BeamRendererColliderLine _colliderLine) {
		allColliderLines.Add (_colliderLine);
	}
	public void RemoveLine (ref BeamRendererColliderLine _colliderLine) {
		allColliderLines.Remove (_colliderLine);
	}


	// ----------------------------------------------------------------
	//  Getters
	// ----------------------------------------------------------------
	/** beamAngle: in DEGREES. */
	public BeamRendererCollision GetBeamRendererCollision (Vector2 beamPos, float beamAngle) {
		// Make a huge-ass line! Note: I'm doing line-line collision instead of ray-line collision because there's less code and it's easier to understand.
		Line beamLine = new Line (beamPos, beamPos + MathUtils.GetVectorFromDeg(beamAngle)*100000);

		// Get the CLOSEST line-on-line collision!
		BeamRendererCollision closestCollision = new BeamRendererCollision(null, BeamRendererCollision.Types.End, beamAngle, beamLine.end); // default this to the farthest theoretical collision possible.
		float closestCollisionDist = float.PositiveInfinity;

		Vector2 intPos;
		foreach (BeamRendererColliderLine colliderLine in allColliderLines) {
			bool isIntersection = LineUtils.GetIntersectionLineToLine (out intPos, beamLine, colliderLine.line);
			if (isIntersection) { // There IS an intersection!
				float intDist = Vector2.Distance (intPos, beamLine.start);
				if (closestCollisionDist > intDist) { // It's the closest one yet! Update the values I'm gonna return!
					closestCollisionDist = intDist;
					closestCollision.type = colliderLine.collisionType;
					closestCollision.pos = intPos;
					closestCollision.objectView = colliderLine.myObjectView;
					float lineAngle = colliderLine.line.GetAngleDeg();
					closestCollision.exitRotation = MathUtils.GetAngleReflection (beamAngle, lineAngle);
//					Debug.Log ("lineAngle: " + lineAngle + "  pos: " + intPos + "  closestCollisionDist: " + closestCollisionDist);
//					Debug.Log ("beamAngle: " + beamAngle + " lineAngle: " + lineAngle + "  exitRotation: " + closestCollision.exitRotation);
				}
			}
		}
		return closestCollision;
//		// Oh, jeez. SOMEhow we didn't intersect with anything, not even my outer bounds!! This is an error. Just return a made-up intersection at the end of the fake line we used.
//		Debug.LogWarning ("Oops! A BeamRenderer didn't have a collision with ANYthing! It should at least be hitting the Arena's bounds.");
//		return new BeamRendererCollision(false, beamAngle, beamLine.end);
	}

}