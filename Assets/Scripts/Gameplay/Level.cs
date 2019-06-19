using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
	// Components
    [SerializeField] private RectTransform myRectTransform=null;
    public Board Board { get; private set; } // this reference ONLY changes when we undo a move, where we remake-from-scratch both board and boardView.
    public BoardView BoardView { get; private set; }
    private BoardView[,] BoardViewEchoes; // TEST for rendering flipping etc.!
    // Properties
    private bool doShowEchoes;
    public bool IsWon { get; private set; }
    public LevelAddress MyAddress { get; private set; }
    private List<BoardData> boardSnapshots = new List<BoardData>(); // note: There's always ONE value in here. These are added immediately AFTER a move.
	// References
	//private GameController gameController;
    private RectTransform rt_boardArea; // a RectTransform that ONLY informs us how the BoardView's size should be, so we can make layout changes in the editor.

    // Getters (Public)
    public int LevelIndex { get { return MyAddress.level; } }
    public PackData MyPackData { get { return GameManagers.Instance.DataManager.GetPackData(MyAddress); } }
    // Getters (Private)
    private InputController inputController { get { return InputController.Instance; } }
    private bool CanUndo() { return boardSnapshots.Count >= 2; }
    private bool IsEveryPlayerDead() { return Board!=null && Board.IsEveryPlayerDead(); }



    // ----------------------------------------------------------------
    //  Initialize / Destroy
    // ----------------------------------------------------------------
    private void Awake() {
        // Add event listeners!
        GameManagers.Instance.EventManager.BoardExecutedMoveEvent += OnBoardExecutedMove;
    }
    private void OnDestroy() {
        // Remove event listeners!
        GameManagers.Instance.EventManager.BoardExecutedMoveEvent -= OnBoardExecutedMove;
    }
    public void Initialize (GameController _gameController, Transform tf_parent, RectTransform _rt_boardArea, LevelData _levelData) {
		//this.gameController = _gameController;
		this.MyAddress = _levelData.myAddress;
        this.rt_boardArea = _rt_boardArea;

        GameUtils.ParentAndReset(this.gameObject, tf_parent);
        GameUtils.FlushRectTransform(myRectTransform); // fit me into the container 100%.
        this.transform.SetSiblingIndex(1); // hardcoded! Put me just in FRONT of the background.
		this.name = "Level " + LevelIndex;
        doShowEchoes = _levelData.doShowEchoes;
        SetZoomAmount(_levelData.startingZoom);

		// Reset!
		RemakeModelAndViewFromData (_levelData.boardData);
        // Take first snapshot.
        boardSnapshots.Add(Board.ToData());
	}

	private void RemakeModelAndViewFromData (BoardData bd) {
		// Destroy them first!
		DestroyBoardModelAndView ();
		// Make them afresh!
		Board = new Board (bd);
		BoardView = Instantiate (ResourcesHandler.Instance.BoardView).GetComponent<BoardView>();
        BoardView.Initialize (this, Board, rt_boardArea);
        // Make BoardViewEchoes!
        if (!doShowEchoes) {
            BoardViewEchoes = new BoardView[0,0];
        }
        else {
            // NOTE: This is all pretty hacked in!! VERY hardcoded values.
            if (Board.WrapH == WrapTypes.CW) {
                int cols = 2;
                int rows = 2;
                Vector2 bvSize = BoardView.Size;
                BoardView.transform.localPosition -= new Vector3(bvSize.x*0.5f,bvSize.y*0.5f);
                BoardViewEchoes = new BoardView[cols,rows];
                for (int col=0; col<cols; col++) {
                    for (int row=0; row<rows; row++) {
                        if (col==0&&row==0) { continue; } // HARDCODED Ignore the middle one. That's what my main BoardView is.
                        BoardView view = Instantiate (ResourcesHandler.Instance.BoardView).GetComponent<BoardView>();
                        view.Initialize (this, Board, rt_boardArea);
                        view.MyCanvasGroup.alpha = 0.6f;
                        view.transform.localPosition += new Vector3(col*bvSize.x, row*bvSize.y, 0);
                        view.transform.localPosition -= new Vector3(bvSize.x*0.5f,bvSize.y*0.5f); // offset to center all 4 of 'em.
                        float rot = 0;
                        if (col==0 && row==1) { rot = -90; }
                        if (col==1 && row==0) { rot =  90; }
                        if (col==1 && row==1) { rot = 180; }
                        view.transform.localEulerAngles += new Vector3(0, 0, rot);//col==0 ? -90 : 90);
                        BoardViewEchoes[col,row] = view;
                    }
                }
            }
            // NORMAL wrapping...
            else {
                int cols = Board.DoWrapH ? 3 : 1;
                int rows = Board.DoWrapV ? 3 : 1;
                Vector2 bvSize = BoardView.Size;
                BoardViewEchoes = new BoardView[cols,rows];
                for (int col=0; col<cols; col++) {
                    for (int row=0; row<rows; row++) {
                        if (col==Mathf.FloorToInt(cols*0.5f) && row==Mathf.FloorToInt(rows*0.5f)) { continue; } // HARDCODED Ignore the middle one. That's what my main BoardView is.
                        
                        BoardView view = Instantiate (ResourcesHandler.Instance.BoardView).GetComponent<BoardView>();
                        view.Initialize (this, Board, rt_boardArea);
                        //view.MyCanvasGroup.alpha = 0.6f;
                        if (cols > 1) {
                            view.transform.localPosition += new Vector3(Mathf.Ceil(col-cols*0.5f)*bvSize.x, 0, 0);
                        }
                        if (rows > 1) {
                            view.transform.localPosition += new Vector3(0, Mathf.Ceil(row-rows*0.5f)*bvSize.y, 0);
                        }
                        if (col%2==0 && Board.WrapH==WrapTypes.Flip) {
                            view.transform.localScale = new Vector3(view.transform.localScale.x, -view.transform.localScale.y, 1);
                        }
                        if (row%2==0 && Board.WrapV==WrapTypes.Flip) {
                            view.transform.localScale = new Vector3(-view.transform.localScale.x, view.transform.localScale.y, 1);
                        }
                        
                        if (Board.WrapH==WrapTypes.CW) {
                            float rot=180;
                            if (false) {}
                            if (col==1 && row==0) { rot = 90; }
                            if (col==1 && row==2) { rot = -90; }
                            if (col==2 && row==1) { rot = -90; }
                            if (col==0 && row==0) { rot = 180; }
                            view.transform.localEulerAngles += new Vector3(0, 0, rot);//col==0 ? -90 : 90);
                        }
                        //if (Board.WrapH==WrapTypes.CW) {
                        //    view.transform.localEulerAngles += new Vector3(0, 0, row==0 ? -90 : 180);
                        //}
                        BoardViewEchoes[col,row] = view;
                    }
                }
            }
        }
        Debug_ApplyEchoAlphas();
        
        UpdateIsWon();
	}
	private void DestroyBoardModelAndView () {
		// Nullify the model (there's nothing to destroy).
		Board = null;
		// Destroy view.
		if (BoardView != null) {
			Destroy(BoardView.gameObject);
			BoardView = null;
            for (int i=0; i<BoardViewEchoes.GetLength(0); i++) {
                for (int j=0; j<BoardViewEchoes.GetLength(1); j++) {
                    if (BoardViewEchoes[i,j] != null) {
                        Destroy(BoardViewEchoes[i,j].gameObject);
                    }
                }
            }
            BoardViewEchoes = null;
		}
	}
    
    
    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    private void OnBoardExecutedMove(Board _board) {
        if (_board != Board) { return; } // Not mine? Ignore.
        
        // Take a snapshot!
        boardSnapshots.Add(Board.ToData());
        UpdateIsWon();
    }
    private void UpdateIsWon() {
        IsWon = Board.AreGoalsSatisfied && (Board.NumExitSpots==0 || Board.IsAnyPlayerOnExitSpot());
        if (Board.NumExitSpots==0 && Board.NumGoalObjects==0) { IsWon = false; } // FOR TESTING. No criteria? We're never satisfied.
        GameManagers.Instance.EventManager.OnLevelSetIsWon(IsWon);
    }



    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    private void UndoMoveAttempt() {
        if (CanUndo()) {
            BoardData snapshot = boardSnapshots[boardSnapshots.Count-2]; // take the previous snapshot (the latest one is the CURRENT version of the board).
            boardSnapshots.RemoveAt(boardSnapshots.Count-1);
            RemakeModelAndViewFromData(snapshot);
        }
    }
    
    private float ZoomAmount;
    private void MultZoomAmount(float mult) {
        SetZoomAmount(ZoomAmount * mult);
    }
    private void SetZoomAmount(float val) {
        ZoomAmount = Mathf.Clamp(val, 0.05f, 1f); // keep it reeeasonable.
        this.transform.localScale = Vector3.one * ZoomAmount;
    }

    

	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void Update() {
        RegisterButtonInput();
        //// TEST
        //this.myRectTransform.anchoredPosition = -(BoardView.Temp_PlayerView.Pos-BoardView.Size*0.5f) * myRectTransform.localScale.x;
	}

	private void RegisterButtonInput() {
        // ANY key, and Player's dead? Undo.
        if (Input.anyKeyDown && IsEveryPlayerDead()) {
            UndoMoveAttempt();
            return;
        }
        // Z = Undo
        if (Input.GetKeyDown(KeyCode.Z)) {
            UndoMoveAttempt();
            return;
        }
        // Level's NOT won...!
        if (!IsWon) {
            // Arrow Keys = Move Player
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) { MovePlayerAttempt(Vector2Int.L); }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) { MovePlayerAttempt(Vector2Int.R); }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) { MovePlayerAttempt(Vector2Int.B); }
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) { MovePlayerAttempt(Vector2Int.T); }
            // SPACE = Advance time
            else if (Input.GetKeyDown(KeyCode.Space)) { MovePlayerAttempt(Vector2Int.zero); }
        }
        
        // C = Zoom OUT
        if (Input.GetKey(KeyCode.C)) { MultZoomAmount(0.95f); }
        // V = Zoom IN
        if (Input.GetKey(KeyCode.V)) { MultZoomAmount(1.05f); }
        
        
        // M = Print Beams
        if (Input.GetKeyDown(KeyCode.M)) { Board.Debug_PrintBeamSpaces(); }
        // B = Print partial Board layout
        if (Input.GetKeyDown(KeyCode.B)) { Board.Debug_PrintSomeBoardLayout(); }
        // H = Toggle echo alphas
        if (Input.GetKeyDown(KeyCode.H)) { Debug_ToggleEchoAlphas(); }
	}
    
    private void MovePlayerAttempt(Vector2Int dir) {
        //// TEST: Rotate dir to match Player's sideFacing!
        //switch (Board.player.SideFacing) {
        //    case Sides.R: dir = Vector2Int.CW(dir); break;
        //    case Sides.B: dir = Vector2Int.Opposite(dir); break;
        //    case Sides.L: dir = Vector2Int.CCW(dir); break;
        //}
        Board.MovePlayerAttempt(dir);
    }
    
    private bool debug_areEchoesLightAlpha = false;
    private void Debug_ToggleEchoAlphas() {
        debug_areEchoesLightAlpha = !debug_areEchoesLightAlpha;
        Debug_ApplyEchoAlphas();
    }
    private void Debug_ApplyEchoAlphas() {
        float alpha = debug_areEchoesLightAlpha ? 0.1f : 1f;
        for (int i=0; i<BoardViewEchoes.GetLength(0); i++) {
            for (int j=0; j<BoardViewEchoes.GetLength(1); j++) {
                if (BoardViewEchoes[i,j] != null) {
                    BoardViewEchoes[i,j].MyCanvasGroup.alpha = alpha;
                }
            }
        }
    }
    
    
    
    


}



