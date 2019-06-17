using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObjectView : MonoBehaviour {
    // Overridables
    virtual public float BeamOriginOffsetLoc { get { return 0.38f; } }
	// Components
    [SerializeField] protected RectTransform rt_contents;
    protected RectTransform myRectTransform { get; private set; } // Set in Awake.
	// Properties
	private float _rotation;
	private Vector2 _scale;
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
			rt_contents.localEulerAngles = new Vector3 (rt_contents.localEulerAngles.x, rt_contents.localEulerAngles.y, _rotation);
			OnSetRotation ();
		}
	}
	public Vector2 Scale {
		get { return _scale; }
		set {
			_scale = value;
			myRectTransform.localScale = _scale;
			OnSetScale ();
		}
	}
    protected Vector2 GetPosFromMyObject () {
        return MyBoardView.BoardToPos (MyBoardObject.ColRow);
    }
    private float GetRotationFromMyObject () {
        float returnValue = -90 * MyBoardObject.SideFacing;
        if (returnValue<-180) returnValue += 360;
        if (returnValue> 180) returnValue -= 360;
        if (Mathf.Abs (returnValue-Rotation) > 180) {
            if (Rotation<returnValue) { Rotation += 360; }
            else { Rotation -= 360; }
        }
        return returnValue;
    }
    private Vector2 GetScaleFromMyObject () {
        return new Vector2(MyBoardObject.ChirH, MyBoardObject.ChirV);
    }

	virtual protected void OnSetPos () { }
	virtual protected void OnSetRotation () { }
	virtual protected void OnSetScale () { }


    // ----------------------------------------------------------------
    //  Initialize / Destroy
    // ----------------------------------------------------------------
    private void Awake() {
        myRectTransform = GetComponent<RectTransform>();
    }
    virtual public void Initialize(BoardView _myBoardView, BoardObject bo) {
		MyBoardView = _myBoardView;
		MyBoardObject = bo;

		// Parent me!
        GameUtils.ParentAndReset(this.gameObject, MyBoardView.tf_boardObjects);
		float diameter = MyBoardView.UnitSize;
		myRectTransform.sizeDelta = new Vector2(diameter, diameter);

		// Start me in the right spot!
		Pos = GetPosFromMyObject();
		Rotation = GetRotationFromMyObject();
        Scale = GetScaleFromMyObject();
	}
    protected void DestroySelf() {
        Destroy(this.gameObject);
    }


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
    protected void SetPos(Vector2 _pos) { this.Pos = _pos; }
    
	virtual public void UpdateVisualsPostMove() {
        // Snap to correct rotation/scale (no animating for now, no need).
        Rotation = GetRotationFromMyObject();
        Scale = GetScaleFromMyObject();
        // Animate from old to new pos!
        Vector2 posFrom = MyBoardView.BoardToPos(MyBoardObject.ColRow - MyBoardObject.PrevMoveDelta);
        Vector2 posTo = GetPosFromMyObject();
        LeanTween.cancel(this.gameObject);
        SetPos(posFrom); // start there now.
        LeanTween.value(this.gameObject,SetPos, posFrom,posTo, 0.18f).setEaseOutQuint();
    }
    
    virtual public void OnRemovedFromPlay() {
        DestroySelf();
    }






}
