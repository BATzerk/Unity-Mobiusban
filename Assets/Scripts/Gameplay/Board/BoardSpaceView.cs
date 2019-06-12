using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardSpaceView : MonoBehaviour {
	// Components
    [SerializeField] private Image i_body=null;
	[SerializeField] private RectTransform myRectTransform=null;
    // References
    private BoardView myBoardView;

	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (BoardView _boardView, BoardSpace mySpace) {
        this.myBoardView = _boardView;
		int col = mySpace.Col;
		int row = mySpace.Row;

		// Parent me to my boooard!
		GameUtils.ParentAndReset(this.gameObject, _boardView.tf_boardSpaces);
		this.gameObject.name = "BoardSpace_" + col + "," + row;

        // Size/position me right!
        float diameter = _boardView.UnitSize * 0.95f;
		myRectTransform.anchoredPosition = new Vector2(_boardView.BoardToX(col), _boardView.BoardToY(row));
        myRectTransform.sizeDelta = new Vector2(diameter, diameter);

        // Only show body if I'm playable.
        i_body.enabled = mySpace.IsPlayable;
        
        // Add walls!
        if (mySpace.IsWall(Sides.T)) {
            AddWallImage(Sides.T);
        }
        if (mySpace.IsWall(Sides.L)) {
            AddWallImage(Sides.L);
        }
	}
    
    private void AddWallImage(int side) {
        Image img = new GameObject().AddComponent<Image>();
        img.color = new Color(0,0,0, 0.32f);
        GameUtils.ParentAndReset(img.gameObject, myBoardView.tf_walls);
        img.rectTransform.sizeDelta = new Vector2(myBoardView.UnitSize, myBoardView.UnitSize*0.1f);
        Vector2Int dir = MathUtils.GetDir(side);
        img.rectTransform.anchoredPosition = this.transform.localPosition;
        img.rectTransform.anchoredPosition += dir * myBoardView.UnitSize*0.5f;
        img.rectTransform.localEulerAngles = new Vector3(0, 0, side*90);
    }


}
