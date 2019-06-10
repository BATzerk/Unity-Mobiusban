using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardSpaceView : MonoBehaviour {
	// Components
    [SerializeField] private Image i_body=null;
	[SerializeField] private RectTransform myRectTransform=null;

	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (BoardView _boardView, BoardSpace mySpace) {
		int col = mySpace.Col;
		int row = mySpace.Row;

		// Parent me to my boooard!
		GameUtils.ParentAndReset(this.gameObject, _boardView.tf_BoardSpaces);
		this.gameObject.name = "BoardSpace_" + col + "," + row;

        // Size/position me right!
        float diameter = _boardView.UnitSize * 0.95f;
		myRectTransform.anchoredPosition = new Vector2(_boardView.BoardToX(col), _boardView.BoardToY(row));
        myRectTransform.sizeDelta = new Vector2(diameter, diameter);

        // Only show body if I'm playable.
        i_body.enabled = mySpace.IsPlayable;
	}


	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	//public void Update() {
 //       if (mySpace==null) { return; } // Safety check.

	//	UpdateHighlight();
	//}
	//private void UpdateHighlight() {
		//if (highlightAlpha == 0) {
		//	i_highlight.enabled = false;
		//}
		//// YES highlight!
		//else {
		//	i_highlight.enabled = true;
		//	if (!mySpace.IsPathOnMe) {
		//		highlightAlpha += (0-highlightAlpha) * 0.3f;
		//		if (Mathf.Approximately(highlightAlpha, 0)) { highlightAlpha = 0; } // Almost there? Get it, hunty!
		//	}
		//	ApplyHighlightAlpha();
		//}
	//}

}
