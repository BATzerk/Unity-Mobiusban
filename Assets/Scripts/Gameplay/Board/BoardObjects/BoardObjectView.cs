﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObjectView : MonoBehaviour {
	// Components
	[SerializeField] protected RectTransform myRectTransform=null;
	// Properties
	private float _rotation; // so we can use the ease-ier (waka waka) system between -180 and 180 with minimal processing effort.
	private float _scale=1;
	// References
    public BoardView MyBoardView { get; private set; }
    public BoardObject MyBoardObject { get; private set; }

    // Getters
    public Vector2 Pos {
		get { return myRectTransform.anchoredPosition; }
		set {
			myRectTransform.anchoredPosition = value;
			OnSetPos ();
		}
	}
	/** In degrees. */
	public float Rotation {
		get { return _rotation; }
		set {
			_rotation = value;
			this.transform.localEulerAngles = new Vector3 (this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, _rotation);
			OnSetRotation ();
		}
	}
	public float Scale {
		get { return _scale; }
		set {
			_scale = value;
			this.transform.localScale = Vector2.one * _scale;
			OnSetScale ();
		}
	}
    protected Vector2 GetPosFromMyObject () {
        return new Vector2 (MyBoardView.BoardToX (MyBoardObject.Col), MyBoardView.BoardToY (MyBoardObject.Row));
    }

	virtual protected void OnSetPos () { }
	virtual protected void OnSetRotation () { }
	virtual protected void OnSetScale () { }


	// ----------------------------------------------------------------
	//  Initialize / Destroy
	// ----------------------------------------------------------------
	protected void InitializeAsBoardObjectView (BoardView _myBoardView, BoardObject _myObject) {
		MyBoardView = _myBoardView;
		MyBoardObject = _myObject;

		// Parent me!
        GameUtils.ParentAndReset(this.gameObject, MyBoardView.tf_BoardObjects);
		float diameter = MyBoardView.UnitSize;
		myRectTransform.sizeDelta = new Vector2(diameter, diameter);

		// Start me in the right spot!
		Pos = GetPosFromMyObject();
		//Rotation = GetRotationFromBO(myObject);
		Scale = 1;
	}
    protected void DestroySelf() {
        Destroy(this.gameObject);
    }


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
    protected void SetPos(Vector2 _pos) { this.Pos = _pos; }
    
	/** This is called when we submit a path! */
	virtual public void UpdateVisualsPostMove() { }
    
    virtual public void OnRemovedFromPlay() {
        DestroySelf();
    }






}
