using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BeamRendererCollision {
	public enum Types {
        //Undefined,
        End,
        //Reflect,
        WarpParallel,
        WarpFlipH,
        WarpFlipV,
    };
    // Properties
    public BeamRendererColliderLine collLine;
    public Vector2 pos;
    public float angleIn;
    public Types collType; // determined by collLine.

    // Constructor
	public BeamRendererCollision (BeamRendererColliderLine collLine, Vector2 pos, float angleIn) {
        this.collLine = collLine;
        this.collType = collLine==null ? Types.End : collLine.collType;
        
		this.pos = pos;
        this.angleIn = angleIn;
    }
    public void SetCollLine(BeamRendererColliderLine collLine) {
        this.collLine = collLine;
        this.collType = collLine==null ? Types.End : collLine.collType;
	}
}
