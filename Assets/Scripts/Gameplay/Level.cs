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
    private List<BoardData> boardSnapshots = new List<BoardData>(); // note: There's always ONE value in here. These are added immediately AFTER a move.
	// References
	//private GameController gameController;
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
		//this.gameController = _gameController;
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
    //  Events
    // ----------------------------------------------------------------
    private void OnBoardExecutedMove(Board _board) {
        if (_board != Board) { return; } // Not mine? Ignore.
        
        // Take a snapshot!
        boardSnapshots.Add(Board.ToData());
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
        // Arrow Keys = Move Player
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) { Board.MovePlayerAttempt(Vector2Int.L); }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) { Board.MovePlayerAttempt(Vector2Int.R); }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) { Board.MovePlayerAttempt(Vector2Int.B); }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) { Board.MovePlayerAttempt(Vector2Int.T); }
        // Z = Undo
        else if (Input.GetKeyDown(KeyCode.Z)) {
            UndoMoveAttempt();
        }
	}
    
    
    
    


}



