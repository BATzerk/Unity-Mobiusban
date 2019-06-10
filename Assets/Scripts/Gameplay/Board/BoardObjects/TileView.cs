using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileView : BoardObjectView {
	// Components
	[SerializeField] private Image i_body=null; // main dot.
    [SerializeField] private Image i_ripple=null; // border around main dot. Animated when we're added to the path.
	// References
	public Tile MyTile { get; private set; }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (BoardView _myBoardView, Tile _tile) {
		MyTile = _tile;
		base.InitializeAsBoardObjectView (_myBoardView, MyTile);
        
        i_body.color = Colors.TileColor(MyTile.ColorID);
        i_ripple.color = Colors.TileColor(MyTile.ColorID);
	}
    

    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
	override public void UpdateVisualsPostMove() {
		base.UpdateVisualsPostMove();
        
        // Animate into new pos!
        LeanTween.cancel(this.gameObject);
        Vector2 newPos = GetPosFromMyObject();
        LeanTween.value(this.gameObject,SetPos, Pos,newPos, 0.32f).setEaseOutBounce().setDelay(0.22f);
	}
    public void AnimateRipple() {
        // Animate i_highlight!
        LeanTween.cancel(i_ripple.gameObject);
        i_ripple.transform.localScale = Vector3.one;
        GameUtils.SetUIGraphicAlpha(i_ripple, 0.8f);
        LeanTween.alpha(i_ripple.rectTransform, 0, 0.7f);
        LeanTween.scale(i_ripple.gameObject, Vector3.one*4f, 0.7f);
    }
    override public void OnRemovedFromPlay() {
        // Animate out!
        LeanTween.cancel(this.gameObject);
        LeanTween.cancel(i_body.rectTransform);
        //LeanTween.alpha(i_body.rectTransform, 0, 0.15f).setEaseInBack();
        LeanTween.scale(i_body.gameObject, Vector3.zero, 0.15f).setEaseInBack().setOnComplete(DestroySelf);
    }



}

