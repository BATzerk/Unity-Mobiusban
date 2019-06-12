using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BeamRendererCollision {
	public enum Types { End, Reflect, Warp };

	private BeamRendererCollision.Types _type; // true if the beam has hit a mirror and shall keep going!
	private BoardObjectView _objectView;
	private float _exitRotation; // the angle in DEGREES the beam will go from here (doesn't matter unless _doesBeamContinue is true).
	private Vector2 _pos;

	public BeamRendererCollision.Types type { get { return _type; } set { _type = value; } }
	public BoardObjectView objectView { get { return _objectView; } set { _objectView = value; } }
	public float exitRotation { get { return _exitRotation; } set { _exitRotation = value; } }
	public Vector2 pos { get { return _pos; } set { _pos = value; } }

	public BeamRendererCollision (BoardObjectView objectView, BeamRendererCollision.Types type, float exitAngle, Vector2 pos) {
		this._objectView = objectView;
		this._type = type;
		this._exitRotation = exitAngle;
		this._pos = pos;
	}
}
