using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour {
	// Constants
	private const float MAX_UNIT_SIZE = 140; // for the 3x3 lvls. So the spaces aren't ginormous.
    // Visual Properties
    public float UnitSize { get; private set; } // how big each board space is in pixels
    // Components
    [SerializeField] private DragPathView pathView=null;
    [SerializeField] private RectTransform myRectTransform=null;
    [SerializeField] private Transform tf_boardObjects=null;
    [SerializeField] private Transform tf_boardSpaces=null;
	// Objects
	private BoardSpaceView[,] spaceViews;
	private List<BoardObjectView> allObjectViews; // includes EVERY single BoardObjectView!
    // References
    public Board MyBoard { get; private set; } // this reference does NOT change during our existence! (If we undo a move, I'm destroyed completely and a new BoardView is made along with a new Board.)
    public Level MyLevel { get; private set; }

    // Getters (Private)
    private ResourcesHandler resourcesHandler { get { return ResourcesHandler.Instance; } }
    private int numCols { get { return MyBoard.NumCols; } }
    private int numRows { get { return MyBoard.NumRows; } }
    // Getters (Public)
    public List<BoardObjectView> AllObjectViews { get { return allObjectViews; } }
    public Transform tf_BoardObjects { get { return tf_boardObjects; } }
    public Transform tf_BoardSpaces { get { return tf_boardSpaces; } }
    public Vector2 Pos { get { return myRectTransform.anchoredPosition; } }
	public float BoardToX(float col) { return  (col+0.5f)*UnitSize; } // +0.5f to center.
	public float BoardToY(float row) { return -(row+0.5f)*UnitSize; } // +0.5f to center.
	public float BoardToXGlobal(float col) { return BoardToX(col) + Pos.x; }
	public float BoardToYGlobal(float row) { return BoardToY(row) + Pos.y; }
    public Vector2 BoardToPos(Vector2Int pos) { return new Vector2(BoardToX(pos.x),BoardToY(pos.y)); }
    public Vector2 BoardToPosGlobal(Vector2Int pos) { return new Vector2(BoardToXGlobal(pos.x),BoardToYGlobal(pos.y)); }
    
    /** Brute-force finds the corresponding TileView. */
    private TileView GetTileView(Vector2Int pos) {
        foreach (BoardObjectView objectView in allObjectViews) {
            BoardObject bo = objectView.MyBoardObject;
            if (bo is Tile && bo.BoardPos==pos) {
                return objectView as TileView;
            }
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

		// Make spaces!
		spaceViews = new BoardSpaceView[numCols,numRows];
		for (int i=0; i<numCols; i++) {
			for (int j=0; j<numRows; j++) {
				spaceViews[i,j] = Instantiate(resourcesHandler.BoardSpaceView).GetComponent<BoardSpaceView>();
				spaceViews[i,j].Initialize (this, MyBoard.GetSpace(i,j));
			}
		}
		// Clear out all my lists!
		allObjectViews = new List<BoardObjectView>();

		// Look right right away!
		UpdateViewsPostMove();
        
        // Add event listeners!
        GameManagers.Instance.EventManager.ClearPathEvent += OnClearPath;
        GameManagers.Instance.EventManager.AddPathSpaceEvent += OnAddPathSpace;
        GameManagers.Instance.EventManager.RemovePathSpaceEvent += OnRemovePathSpace;
        GameManagers.Instance.EventManager.SubmitPathEvent += OnSubmitPath;
	}
    private void OnDestroy() {
        // Remove event listeners!
        GameManagers.Instance.EventManager.ClearPathEvent -= OnClearPath;
        GameManagers.Instance.EventManager.AddPathSpaceEvent -= OnAddPathSpace;
        GameManagers.Instance.EventManager.RemovePathSpaceEvent -= OnRemovePathSpace;
        GameManagers.Instance.EventManager.SubmitPathEvent -= OnSubmitPath;
    }

    private void UpdatePosAndSize(RectTransform rt_availableArea) {
        // Temporarily parent me to this fella!
        transform.SetParent(rt_availableArea);
		// Determine unitSize.
        UnitSize = Mathf.Min(rt_availableArea.rect.size.x/numCols, rt_availableArea.rect.size.y/numRows);
		UnitSize = Mathf.Min(MAX_UNIT_SIZE, UnitSize); // don't let it get TOO big. (Just for the 3x3 lvls, really.)
		// Apply the board size to my rectTransform.
		Vector2 mySize = new Vector2(UnitSize*numCols, UnitSize*numRows);
		myRectTransform.sizeDelta = mySize;
		// Set my position.
        float xOffset =  (rt_availableArea.rect.width -mySize.x) * 0.5f; // nudge me so I'm centered!
        float yOffset = -(rt_availableArea.rect.height-mySize.y) * 0.5f; // nudge me so I'm centered!
        myRectTransform.anchoredPosition = new Vector2(xOffset,yOffset);
        // Now that the suit fits, sneak be back onto my Level! I'll be transformed great.
        transform.SetParent(MyLevel.transform);
	}

	private BoardObjectView AddObjectView (BoardObject sourceObject) {
		if (sourceObject is Tile) { return AddTileView (sourceObject as Tile); }
		else {
            Debug.LogError ("Trying to add BoardObjectView from BoardObject, but no clause to handle this type! " + sourceObject.GetType().ToString());
            return null;
        }
	}
    private TileView AddTileView (Tile data) {
        TileView newObj = Instantiate(resourcesHandler.TileView).GetComponent<TileView>();
        newObj.Initialize (this, data);
        allObjectViews.Add (newObj);
        return newObj;
    }



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
			BoardObjectView newView = AddObjectView(bo);
            // Start it way above the board.
            newView.Pos = new Vector2(newView.Pos.x, 500);
		}
		// Clear out the list! We've used 'em.
		MyBoard.objectsAddedThisMove.Clear();
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
    
    private void AnimateRippleForTileViewsOfColor(int colorID) {
        for (int i=0; i<allObjectViews.Count; i++) {
            TileView tileView = allObjectViews[i] as TileView;
            if (tileView != null && tileView.MyTile.ColorID==colorID) {
                tileView.AnimateRipple();
            }
        }
    }

    

    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    private void OnClearPath() {
        pathView.RemakeVisuals();
    }
    private void OnAddPathSpace(BoardSpace space) {
        pathView.RemakeVisuals();
        // Tell the TileView that's been added!
        TileView tileView = GetTileView(space.BoardPos);
        if (tileView != null) {
            tileView.AnimateRipple();
        }
        // Has this completed the circuit?? Tell all tiles of this color!
        if (MyBoard.dragPath.IsCircuitComplete) {
            AnimateRippleForTileViewsOfColor(MyBoard.dragPath.ColorID);
        }
    }
    private void OnRemovePathSpace() {
        pathView.RemakeVisuals();
    }
    private void OnSubmitPath() {
        UpdateViewsPostMove();
    }





}
