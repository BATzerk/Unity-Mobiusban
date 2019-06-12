﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardOccupantView : BoardObjectView {
	//// Properties
	//virtual protected Color GetPrimaryFill () { return new Color (1,1,1); } // Override this if we want to auto-color our body (without adding code)!
	//// Components
	//[SerializeField] private SpriteRenderer sr_body; // everyone has a primary body sprite for simplicity! Less code.
	//[SerializeField] private Sprite s_body;
    private BeamRendererCollider beamRendererCollider;

    // DEBUG
    private void OnDrawGizmos () {
        if (beamRendererCollider == null) { return; }
        Gizmos.color = Color.magenta;
        foreach (BeamRendererColliderLine line in beamRendererCollider.Debug_colliderLines) {
            Gizmos.DrawLine (line.line.start, line.line.end);
        }
    }
    
	// ----------------------------------------------------------------
	//  Initialize / Destroy
	// ----------------------------------------------------------------
	override public void Initialize (BoardView _myBoardView, BoardObject bo) {
		base.Initialize (_myBoardView, bo);
        
        // Make my beamRendererCollider!
        beamRendererCollider = new BeamRendererCollider (MyBoardView, this);

		//ApplyFundamentalVisualProperties ();
	}
    virtual protected void OnDestroy () {
        // Make sure to destroy my Collider! It's outta the BoardView now. :)
        if (beamRendererCollider != null) {
            beamRendererCollider.Destroy ();
        }
    }

	virtual protected void ApplyFundamentalVisualProperties () { // Note: We can make this overridable if we want unique visuals per object.
		//// If we DO have a bodySprite!...
		//if (sr_body != null) {
		//	// Color me impressed!
		//	sr_body.sprite = s_body;
		//	sr_body.color = GetPrimaryFill ();
		//}
	}
    
    
    // ----------------------------------------------------------------
    //  Overrides
    // ----------------------------------------------------------------
    override protected void OnSetPos () {
        base.OnSetPos ();
        if (beamRendererCollider!=null) { beamRendererCollider.UpdateLines (); }
    }
    override protected void OnSetRotation () {
        base.OnSetRotation ();
        if (beamRendererCollider!=null) { beamRendererCollider.UpdateLines (); }
    }


}
