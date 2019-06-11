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
    public bool IsWon { get; private set; }
    public LevelAddress MyAddress { get; private set; }
    private List<BoardData> boardSnapshots = new List<BoardData>(); // note: There's always ONE value in here. These are added immediately AFTER a move.
	// References
	private GameController gameController;
    private RectTransform rt_boardArea; // a RectTransform that ONLY informs us how the BoardView's size should be, so we can make layout changes in the editor.

    // Getters (Public)
    public int LevelIndex { get { return MyAddress.level; } }
    // Getters (Private)
	private InputController inputController { get { return InputController.Instance; } }
	private PackData myPackData { get { return GameManagers.Instance.DataManager.GetPackData(MyAddress); } }
    private bool CanUndo() { return boardSnapshots.Count >= 2; }



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
		this.gameController = _gameController;
		this.MyAddress = _levelData.myAddress;
        this.rt_boardArea = _rt_boardArea;

        GameUtils.ParentAndReset(this.gameObject, tf_parent);
        GameUtils.FlushRectTransform(myRectTransform); // fit me into the container 100%.
        this.transform.SetSiblingIndex(1); // hardcoded! Put me just in FRONT of the background.
		this.name = "Level " + LevelIndex;

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
        // NOTE: This is all pretty hacked in!! VERY hardcoded values.
        int cols = Board.DoWrapH ? 3 : 1;
        int rows = Board.DoWrapV ? 3 : 1;
        Vector2 bvSize = BoardView.Size;
        BoardViewEchoes = new BoardView[cols,rows];
        for (int col=0; col<cols; col++) {
            for (int row=0; row<rows; row++) {
                if (col==Mathf.FloorToInt(cols*0.5f) && row==Mathf.FloorToInt(rows*0.5f)) { continue; } // HARDCODED Ignore the middle one. That's what my main BoardView is.
                //if (col==2 && row==2) { continue; } // HARDCODED Ignore the middle one. That's what my main BoardView is.
                
                
                BoardView view = Instantiate (ResourcesHandler.Instance.BoardView).GetComponent<BoardView>();
                view.Initialize (this, Board, rt_boardArea);
                view.MyCanvasGroup.alpha = 0.6f;
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
                
                if (col%2==0 && Board.WrapH==WrapTypes.CW) {
                    view.transform.localEulerAngles += new Vector3(0, 0, col==0 ? -90 : 90);
                }
                if (row%2==0 && Board.WrapV==WrapTypes.CW) {
                    view.transform.localEulerAngles += new Vector3(0, 0, row==0 ? -90 : 90);
                }
                BoardViewEchoes[col,row] = view;
            }
        }
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
        IsWon = Board.AreGoalsSatisfied && Board.IsPlayerOnExitSpot();
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

    

	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void Update() {
        RegisterButtonInput();
	}

	private void RegisterButtonInput() {
        // Z = Undo
        if (Input.GetKeyDown(KeyCode.Z)) {
            UndoMoveAttempt();
        }
        // Level's NOT won...!
        if (!IsWon) {
            // Arrow Keys = Move Player
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) { Board.MovePlayerAttempt(Vector2Int.L); }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) { Board.MovePlayerAttempt(Vector2Int.R); }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) { Board.MovePlayerAttempt(Vector2Int.B); }
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) { Board.MovePlayerAttempt(Vector2Int.T); }
        }
	}
    
    
    
    


}



