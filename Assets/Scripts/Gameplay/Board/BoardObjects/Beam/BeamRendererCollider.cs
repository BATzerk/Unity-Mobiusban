using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** One of these for every BoardOccupant. Contains a list of lines that'll block BeamRenderers! */
public class BeamRendererCollider {
	// Constants
	private static Vector2[] squarePoints = { // This is a list of all the points we'd use for a square with all its sides. Makes making lines easier.
		new Vector2(-0.5f, 0.5f),
		new Vector2( 0.5f, 0.5f),
		new Vector2( 0.5f,-0.5f),
		new Vector2(-0.5f,-0.5f),
		new Vector2(-0.5f, 0.5f)
	};
	// Components
	private Line[] prototypeLines; // from -0.5f (100% left/bottom side) and 0.5f (100% right/top side) in my local space. My lines list is updated from these values.
	private BeamRendererColliderLine[] colliderLines; // prototypeLines' lines, 1) scaled to board space, 2) put where my body is, and 3) rotated like my body.
	// Properties
	private float diameter;
	// References
	private BeamRendererColliderArena beamRendererColliderArena; // the arena of my board.
	private BoardObjectView myObjectView; // we need this reference so we can update our lines from its pos, rotation, and scale.

	// Getters
	private BoardObject myObject { get { return myObjectView.MyBoardObject; } }
	//public BeamRendererColliderLine[] Debug_colliderLines { get { return colliderLines; } }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public BeamRendererCollider (BoardView _myBoardView, BoardObjectView _myObjectView) {
		beamRendererColliderArena = _myBoardView.BeamRendererColliderArena;
		myObjectView = _myObjectView;

		// Shrink everyone down uniformly if they're movable.
		float unitSize = _myBoardView.UnitSize;
		diameter = unitSize * 0.98f;
		if (myObject is BoardOccupant && (myObject as BoardOccupant).IsMovable) {
			diameter = unitSize * 0.75f;
		}

		// Set my lines hardcodedly!
		//if (myObject is Mirror) { InitializeLinesForMirror (); }
		if (myObject is BeamGoal) { InitializeLinesForBeamGoalOrSource (0.5f); }
		else if (myObject is BeamSource) { InitializeLinesForBeamGoalOrSource (0.1f); }
		//else if (myObject is Bucket) { InitializeLinesForBucket (); }
		//else if (myObject is Portal) { InitializeLinesForPortal (); }
		//else if (myObject is Wall) { InitializeLinesForWall (); }
		else if (myObject is Player) { InitializeLinesForPlayer (); }
		else { InitializeLinesForDefaultSquare (); }

		// Update my actual lines right off the bat!
		UpdateLines ();
	}
	/** Removes me from the BoardView's BeamRendererColliderArena. Call this when my BoardOccupant gets destroyed too. */ // TODO: Not urgent, but inconsistent: make Walls inheret from BoardOccupant. Walls inheret BoardObjects, not Occupants, which could cause problems because of the copied code for these colliders.
	public void Destroy () {
		RemoveMyLinesFromArena ();
	}

	//private void InitializeLinesForMirror () {
	//	BeamRendererCollision.Types[] collisionTypes = new BeamRendererCollision.Types[3];
	//	collisionTypes[0] = BeamRendererCollision.Types.Reflect;
	//	collisionTypes[1] = BeamRendererCollision.Types.End;
	//	collisionTypes[2] = BeamRendererCollision.Types.End;

	//	float slashRadius = 0.53f;
	//	float capsRadius = 0.12f; // The little caps on the end of the slash. Looks like a tilted I! Mirrors actually block beams from their ends. Looks better during animations.
	//	float slashAngle = Mathf.PI*0.25f; // Looks like \.

	//	// Make 
	//	prototypeLines = new Line[3];
	//	prototypeLines[0] = new Line(0,slashRadius, 0,-slashRadius).Rotate(slashAngle);
	//	prototypeLines[1] = new Line(-capsRadius, slashRadius, capsRadius, slashRadius).Rotate(slashAngle); // top cap
	//	prototypeLines[2] = new Line(-capsRadius,-slashRadius, capsRadius,-slashRadius).Rotate(slashAngle); // bottom cap

	//	MakeColliderLinesFromPrototypeLines (collisionTypes);
	//}
	private void InitializeLinesForBeamGoalOrSource (float indentAmount) {
		Vector2[] linePoints = new Vector2[9];
		BeamRendererCollision.Types[] collisionTypes = new BeamRendererCollision.Types[8];
		int i=0;
		linePoints[i++] = new Vector2(-0.5f, 0.5f);
		linePoints[i++] = new Vector2(-0.1f, 0.5f);
		linePoints[i++] = new Vector2(-0.1f, 0.5f-indentAmount);
		linePoints[i++] = new Vector2( 0.1f, 0.5f-indentAmount);
		linePoints[i++] = new Vector2( 0.1f, 0.5f);
		linePoints[i++] = new Vector2( 0.5f, 0.5f);
		linePoints[i++] = new Vector2( 0.5f,-0.5f);
		linePoints[i++] = new Vector2(-0.5f,-0.5f);
		linePoints[i++] = new Vector2(-0.5f, 0.5f);

		MakePrototypeLinesFromContinuousPoints (linePoints);

		MakeColliderLinesFromPrototypeLines (collisionTypes);
	}
	private void InitializeLinesForBucket () {
		// These lines are a square with an open top. So right, bottom, and left sides only.
		Vector2[] linePoints = new Vector2[4];
		BeamRendererCollision.Types[] collisionTypes = new BeamRendererCollision.Types[3];
		linePoints[0] = new Vector2( 0.5f, 0.5f);
		linePoints[1] = new Vector2( 0.5f,-0.5f);
		linePoints[2] = new Vector2(-0.5f,-0.5f);
		linePoints[3] = new Vector2(-0.5f, 0.5f);
		MakePrototypeLinesFromContinuousPoints (linePoints);
		MakeColliderLinesFromPrototypeLines (collisionTypes);
	}
	//private void InitializeLinesForPortal () {
	//	// These lines are a square with an open top. So right, bottom, and left sides only.
	//	BeamRendererCollision.Types[] collisionTypes = new BeamRendererCollision.Types[4];
	//	collisionTypes[0] = BeamRendererCollision.Types.End;
	//	collisionTypes[1] = BeamRendererCollision.Types.End;
	//	collisionTypes[2] = BeamRendererCollision.Types.End;
	//	collisionTypes[3] = BeamRendererCollision.Types.Portal;

	//	prototypeLines = new Line[4];
	//	prototypeLines[0] = new Line( 0.5f, 0.5f,  0.5f,-0.5f);
	//	prototypeLines[1] = new Line( 0.5f,-0.5f, -0.5f,-0.5f);
	//	prototypeLines[2] = new Line(-0.5f,-0.5f, -0.5f, 0.5f);
	//	prototypeLines[3] = new Line(-0.48f,-0.48f, 0.48f,-0.48f); // the special inner Portal line.
	//	// Now add the special portal line!
	//	MakeColliderLinesFromPrototypeLines (collisionTypes);
	//}
	private void InitializeLinesForPlayer () {
		colliderLines = new BeamRendererColliderLine[0]; // Lines pass straight through the Playa!
	}
	private void InitializeLinesForDefaultSquare () {
		prototypeLines = new Line[4];
		BeamRendererCollision.Types[] collisionTypes = new BeamRendererCollision.Types[4];
		for (int side=0; side<4; side++) {
			prototypeLines[side] = new Line(squarePoints[side], squarePoints[side+1]);
		}
		MakeColliderLinesFromPrototypeLines (collisionTypes);
	}
	private void InitializeLinesForWall () {
		prototypeLines = new Line[1];
		BeamRendererCollision.Types[] collisionTypes = new BeamRendererCollision.Types[1];
		prototypeLines[0] = new Line(squarePoints[0], squarePoints[1]);
		MakeColliderLinesFromPrototypeLines (collisionTypes);
	}


	/** If we have one continuous shape, use this function to make making that shape easier! (Mirrors do NOT have a continuous shape; they're an I.)
	 * linePoints: In prototype line space (-0.5 to 0.5). */
	private void MakePrototypeLinesFromContinuousPoints (Vector2[] linePoints) {
		int numLines = linePoints.Length-1;
		prototypeLines = new Line[numLines];
		for (int i=0; i<prototypeLines.Length; i++) {
			prototypeLines[i] = new Line(linePoints[i], linePoints[i+1]);
		}
	}
	private void MakeColliderLinesFromPrototypeLines (BeamRendererCollision.Types[] collisionTypes) {
		colliderLines = new BeamRendererColliderLine[prototypeLines.Length];
		for (int i=0; i<colliderLines.Length; i++) {
			colliderLines[i] = new BeamRendererColliderLine(myObjectView, collisionTypes[i]);
		}
		// I've got my lines list! Add the references to the Arena! Note: If we're destroyed or removed, we'll have to remove my lines from the Arena!
		AddMyLinesToArena ();
	}

	private void AddMyLinesToArena () {
		for (int i=0; i<colliderLines.Length; i++) {
			beamRendererColliderArena.AddLine (ref colliderLines[i]);
		}
	}
	private void RemoveMyLinesFromArena () {
		for (int i=0; i<colliderLines.Length; i++) {
			beamRendererColliderArena.RemoveLine (ref colliderLines[i]);
		}
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void UpdateLines () {
		for (int i=0; i<colliderLines.Length; i++) {
			colliderLines[i].SetLine (prototypeLines[i], myObjectView.Pos, myObjectView.Rotation, diameter*1);//myObjectView.Scale);
		}
	}


}
