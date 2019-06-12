using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Contains A) The actual line, B) How the line treats Beams (end, reflect, portal), and C) A ref to the BoardObjectView that owns it.
 * This class is LIKE a struct, but we want references to these lines, so it's not a struct. :) */
public class BeamRendererColliderLine {
	// Properties
	private BeamRendererCollision.Types _collisionType;
	private Line _line;
    private Vector2 _wrapOffset;
	// References
	private BoardObjectView _myObjectView;

	// Getters / Setters
	public BeamRendererCollision.Types collisionType { get { return _collisionType; } }
	public BoardObjectView myObjectView { get { return _myObjectView; } }
	public Line line { get { return _line; } }
	private Vector2 start {
		get { return _line.start; }
		set { _line.start = value; }
	}
    private Vector2 end {
        get { return _line.end; }
        set { _line.end = value; }
    }
    public Vector2 wrapOffset { get { return _wrapOffset; } }
	/** Conveniently sets my line based on what the unscaled/unrotated/etc. line looks like. */
	public void SetLine (Line unscaledLine, Vector2 objectPos, float objectRotation, float unitSize) {
		start = MathUtils.GetRotatedVector2Deg(unscaledLine.start, objectRotation)*unitSize + objectPos;
		end   = MathUtils.GetRotatedVector2Deg(unscaledLine.end,   objectRotation)*unitSize + objectPos;
	}

	public BeamRendererColliderLine (BoardObjectView myObjectView, BeamRendererCollision.Types collisionType) {
		this._myObjectView = myObjectView;
		this._line = new Line(); // default to an empty line for now.
		this._collisionType = collisionType;
	}
    public BeamRendererColliderLine (BoardObjectView myObjectView, Line line, BeamRendererCollision.Types collisionType) {
        this._myObjectView = myObjectView;
        this._line = line;
        this._collisionType = collisionType;
    }
    public BeamRendererColliderLine (Line line, Vector2 wrapOffset, BeamRendererCollision.Types collisionType) {
        this._line = line;
        this._wrapOffset = wrapOffset;
        this._collisionType = collisionType;
    }
}
