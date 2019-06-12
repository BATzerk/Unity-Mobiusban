using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BeamRendererColliderArena))]
public class BoardView : MonoBehaviour {
	// Constants
	private const float MAX_UNIT_SIZE = 140; // for the 3x3 lvls. So the spaces aren't ginormous.
    // Visual Properties
    public float UnitSize { get; private set; } // how big each board space is in pixels
    // Components
    [SerializeField] public CanvasGroup MyCanvasGroup=null;
    [SerializeField] private RectTransform myRectTransform=null;
    [SerializeField] public Transform tf_beamLines=null;
    [SerializeField] public Transform tf_boardObjects=null;
    [SerializeField] public Transform tf_boardSpaces=null;
    [SerializeField] public Transform tf_walls=null;
    public BeamRendererColliderArena BeamRendererColliderArena { get; private set; } // Added in Initialize.
	// Objects
	private BoardSpaceView[,] spaceViews;
	private List<BoardObjectView> allObjectViews = new List<BoardObjectView>(); // includes EVERY single BoardObjectView!
    // References
    public Board MyBoard { get; private set; } // this reference does NOT change during our existence! (If we undo a move, I'm destroyed completely and a new BoardView is made along with a new Board.)
    public Level MyLevel { get; private set; }

    // Getters (Private)
    private ResourcesHandler resourcesHandler { get { return ResourcesHandler.Instance; } }
    private int NumCols { get { return MyBoard.NumCols; } }
    private int NumRows { get { return MyBoard.NumRows; } }
    public bool AreGoalsSatisfied { get { return MyBoard.AreGoalsSatisfied; } }
    // Getters (Public)
    public List<BoardObjectView> AllObjectViews { get { return allObjectViews; } }
    public Vector2 Pos { get { return myRectTransform.anchoredPosition; } }
    public Vector2 Size { get { return myRectTransform.rect.size; } }
	public float BoardToX(float col) { return (col+0.5f)*UnitSize; } // +0.5f to center.
	public float BoardToY(float row) { return (row+0.5f)*UnitSize; } // +0.5f to center.
	public float BoardToXGlobal(float col) { return BoardToX(col) + Pos.x; }
	public float BoardToYGlobal(float row) { return BoardToY(row) + Pos.y; }
    public Vector2 BoardToPos(Vector2Int pos) { return new Vector2(BoardToX(pos.x),BoardToY(pos.y)); }
    public Vector2 BoardToPosGlobal(Vector2Int pos) { return new Vector2(BoardToXGlobal(pos.x),BoardToYGlobal(pos.y)); }
    
    /** Brute-force finds the corresponding BoardOccupantView. */
    public BoardObjectView TEMP_GetObjectView(BoardOccupant bo) {
        foreach (BoardObjectView objectView in allObjectViews) {
            if (objectView.MyBoardObject == bo) { return objectView;}// as BoardOccupantView; }
        }
        return null; // oops.
    }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (Level _levelRef, Board _myBoard, RectTransform rt_availableArea) {
		this.MyLevel = _levelRef;
		this.MyBoard = _myBoard;
		GameUtils.ParentAndReset(this.gameObject, MyLevel.transform);

		// Determine unitSize and other board-specific visual stuff
        UpdatePosAndSize(rt_availableArea);

        // Add Player and Spaces!
        BeamRendererColliderArena = GetComponent<BeamRendererColliderArena>();
        BeamRendererColliderArena.Initialize(MyBoard, myRectTransform.rect);
        AddObjectView(MyBoard.player);
		spaceViews = new BoardSpaceView[NumCols,NumRows];
		for (int i=0; i<NumCols; i++) {
			for (int j=0; j<NumRows; j++) {
				spaceViews[i,j] = Instantiate(resourcesHandler.BoardSpaceView).GetComponent<BoardSpaceView>();
				spaceViews[i,j].Initialize (this, MyBoard.GetSpace(i,j));
			}
		}
		// Add all other views, and look right right away!
		UpdateViewsPostMove();
        
        // Add event listeners!
        GameManagers.Instance.EventManager.BoardExecutedMoveEvent += OnBoardExecutedMove;
	}
    private void OnDestroy() {
        // Remove event listeners!
        GameManagers.Instance.EventManager.BoardExecutedMoveEvent -= OnBoardExecutedMove;
    }

    private void UpdatePosAndSize(RectTransform rt_availableArea) {
        // Temporarily parent me to this fella!
        transform.SetParent(rt_availableArea);
		// Determine unitSize.
        UnitSize = Mathf.Min(rt_availableArea.rect.size.x/NumCols, rt_availableArea.rect.size.y/NumRows);
		UnitSize = Mathf.Min(MAX_UNIT_SIZE, UnitSize); // don't let it get TOO big. (Just for the 3x3 lvls, really.)
		// Apply the board size to my rectTransform.
		Vector2 mySize = new Vector2(UnitSize*NumCols, UnitSize*NumRows);
		myRectTransform.sizeDelta = mySize;
		// Set my position.
        //float xOffset = (rt_availableArea.rect.width -mySize.x) * 0.5f; // nudge me so I'm centered!
        //float yOffset = (rt_availableArea.rect.height-mySize.y) * 0.5f; // nudge me so I'm centered!
        //myRectTransform.anchoredPosition = new Vector2(xOffset,yOffset);
        myRectTransform.anchoredPosition = new Vector2(0,0);
        // Now that the suit fits, sneak be back onto my Level! I'll be transformed great.
        transform.SetParent(MyLevel.transform);
	}

	private void AddObjectView (BoardObject sourceObject) {
        GameObject prefab = resourcesHandler.GetBoardObjectView(sourceObject);
        if (prefab == null) { return; } // Safety check.
        BoardObjectView newView = Instantiate(prefab).GetComponent<BoardObjectView>();
        newView.Initialize (this, sourceObject);
        allObjectViews.Add (newView);
	}
    //private void AddBeamSourceView (BeamSource obj) {
    //    BeamSourceView newObj = Instantiate(resourcesHandler.BeamSourceView).GetComponent<BeamSourceView>();
    //    newObj.Initialize (this, obj);
    //    allObjectViews.Add (newObj);
    //}
    //private void AddCrateView (Crate obj) {
    //    CrateView newObj = Instantiate(resourcesHandler.CrateView).GetComponent<CrateView>();
    //    newObj.Initialize (this, obj);
    //    allObjectViews.Add (newObj);
    //}
    //private void AddCrateGoalView (CrateGoal obj) {
    //    CrateGoalView newObj = Instantiate(resourcesHandler.CrateGoalView).GetComponent<CrateGoalView>();
    //    newObj.Initialize (this, obj);
    //    allObjectViews.Add (newObj);
    //}
    //private void AddExitSpotView (ExitSpot obj) {
    //    ExitSpotView newObj = Instantiate(resourcesHandler.ExitSpotView).GetComponent<ExitSpotView>();
    //    newObj.Initialize (this, obj);
    //    allObjectViews.Add (newObj);
    //}
    //private void AddPlayerView (Player obj) {
    //    PlayerView newObj = Instantiate(resourcesHandler.PlayerView).GetComponent<PlayerView>();
    //    newObj.Initialize (this, obj);
    //    allObjectViews.Add (newObj);
    //}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void UpdateViewsPostMove() {
		RemoveOldViews();
		AddNewViews();
		for (int i=allObjectViews.Count-1; i>=0; --i) {
			allObjectViews[i].UpdateVisualsPostMove();
		}
	}
	private void AddNewViews() {
		foreach (BoardObject bo in MyBoard.objectsAddedThisMove) {
			AddObjectView(bo);
		}
	}
	private void RemoveOldViews() {
		for (int i=allObjectViews.Count-1; i>=0; --i) { // Go through backwards, as objects can be removed from the list as we go!
			if (!allObjectViews[i].MyBoardObject.IsInPlay) {
				// Destroy the object.
				allObjectViews[i].OnRemovedFromPlay();
				// Remove it from the list of views.
				allObjectViews.RemoveAt(i);
			}
		}
	}

    

    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnBoardExecutedMove(Board board) {
        if (board == MyBoard) { // It's MY Board! Update views!
            UpdateViewsPostMove();
        }
    }





}
