using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
	// Components
    [SerializeField] private RectTransform myRectTransform=null;
    public Board Board { get; private set; } // this reference ONLY changes when we undo a move, where we remake-from-scratch both board and boardView.
    public BoardView BoardView { get; private set; }
    // Properties
    public bool IsLevelOver { get; private set; }
    public LevelAddress MyAddress { get; private set; }
	private Vector2Int touchGridPos;
    public Vector2 TouchPosBoard { get; private set; }
	// References
	private GameController gameController;
    private RectTransform rt_boardArea; // a RectTransform that ONLY informs us how the BoardView's size should be, so we can make layout changes in the editor.

    // Getters (Public)
    public int LevelIndex { get { return MyAddress.level; } }
    // Getters (Private)
    private bool IsDraggingPath() { return Board.dragPath.HasSpaces; }
	private InputController inputController { get { return InputController.Instance; } }
	private PackData myPackData { get { return GameManagers.Instance.DataManager.GetPackData(MyAddress); } }



	// ----------------------------------------------------------------
	//  Initialize / Destroy
	// ----------------------------------------------------------------
    public void Initialize (GameController _gameController, Transform tf_parent, RectTransform _rt_boardArea, LevelData _levelData) {
		this.gameController = _gameController;
		this.MyAddress = _levelData.myAddress;
        this.rt_boardArea = _rt_boardArea;

        GameUtils.ParentAndReset(this.gameObject, tf_parent);
        GameUtils.FlushRectTransform(myRectTransform); // fit me into the container 100%.
        this.transform.SetSiblingIndex(1); // hardcoded! Put me just in FRONT of the background.
		this.name = "Level " + LevelIndex;

		// Reset!
		RemakeModelAndViewFromData (_levelData.boardData);
	}

	private void RemakeModelAndViewFromData (BoardData bd) {
		// Destroy them first!
		DestroyBoardModelAndView ();
		// Make them afresh!
		Board = new Board (bd);
		BoardView = Instantiate (ResourcesHandler.Instance.BoardView).GetComponent<BoardView>();
        BoardView.Initialize (this, Board, rt_boardArea);
	}
	private void DestroyBoardModelAndView () {
		// Nullify the model (there's nothing to destroy).
		Board = null;
		// Destroy view.
		if (BoardView != null) {
			Destroy(BoardView.gameObject);
			BoardView = null;
		}
	}


    

	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void Update() {
        if (!IsLevelOver) { // Level's not over? Update stuff!
    		UpdateTouchPos();
    		RegisterTouches();
        }
	}
	private void UpdateTouchPos() {
		TouchPosBoard = Input.mousePosition/MainCanvas.Canvas.scaleFactor;
		float canvasHeight = MainCanvas.Height;
        TouchPosBoard -= BoardView.Pos;
        TouchPosBoard = new Vector2(TouchPosBoard.x, canvasHeight-TouchPosBoard.y); // convert to top-left space.
        int col = Mathf.FloorToInt(TouchPosBoard.x / BoardView.UnitSize);
        int row = Mathf.FloorToInt(TouchPosBoard.y / BoardView.UnitSize);
        // Grid-pos changed? Update it!
        if (touchGridPos.x!=col || touchGridPos.y!=row) {
            touchGridPos = new Vector2Int(col, row);
            OnTouchGridPosChanged();
        }
	}

	private void RegisterTouches() {
		if (InputController.IsTouchUp()) { OnTouchUp(); }
		else if (InputController.IsTouchDown()) { OnTouchDown(); }
	}
	private void OnTouchDown() {
        BoardSpace spaceOver = Board.GetSpace(touchGridPos);
        if (Board.MayBeginPath(spaceOver)) {
            Board.BeginDragPath(spaceOver);
        }
	}
	private void OnTouchUp() {
        if (Board.MaySubmitPath()) {
            Board.SubmitPath();
        }
        else {
            Board.ClearDragPath();
        }
	}

	private void OnTouchGridPosChanged() {
        if (IsDraggingPath()) {
            UpdatePathToTouch();
        }
	}
    
    /// Tries to get the path as close to the touch as possible.
    private void UpdatePathToTouch() {
        Vector2Int nextSpacePos;
        int safetyCount=0;
        // Loop through to keep adding/removing spaces until we A) Can't do anything, or B) We're at the touch!
        while (safetyCount++<20) {
            nextSpacePos = GetNextSpacePosFromTouchPos();
            // Is this the PREVIOUS space?? Step back!
            if (nextSpacePos == Board.dragPath.SecondLastPos) {
                Board.RemovePathSpace();
            }
            // NEW space??
            else {
                // We CAN add it! Do that!
                if (Board.dragPath.MayAddSpaceToPath(nextSpacePos)) {
                    Board.AddPathSpace(nextSpacePos);
                }
                // We CAN'T add it. Get outta the loop.
                else {
                    break;
                }
            }
            // If the nextSpacePos is AT the touch, we can get outta the loop.
            if (nextSpacePos == touchGridPos) { break; }
        }
    }
    private Vector2Int GetNextSpacePosFromTouchPos() {
        Vector2Int prevSpacePos = Board.dragPath.LastPos;
        Vector2Int dir = Vector2Int.zero;
        dir = new Vector2Int(MathUtils.Sign(touchGridPos.x-prevSpacePos.x), MathUtils.Sign(touchGridPos.y-prevSpacePos.y));
        if (dir.x!=0 && dir.y!=0) { // Trying to move diagonally? Ah-ah.
            dir.y = 0;
        }
        Vector2Int nextSpacePos = new Vector2Int(prevSpacePos.x+dir.x, prevSpacePos.y+dir.y);
        return nextSpacePos;
    }
    
    
    
    


}



