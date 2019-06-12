using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrateView : BoardObjectView {
    // Components
    [SerializeField] private Image i_body=null;
    [SerializeField] private Image i_autoMoveDir=null;
    //private List<
 //   // Properties
 //   [SerializeField] private Color c_movable=Color.white;
 //   [SerializeField] private Color c_unmovable=Color.white;
	// References
    [SerializeField] private Sprite s_dimple=null;
    //[SerializeField] private Sprite s_movable=null;
    //[SerializeField] private Sprite s_unmovable=null;
    public Crate MyCrate { get; private set; }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (BoardView _myBoardView, Crate myObj) {
		MyCrate = myObj;
		base.InitializeAsBoardObjectView (_myBoardView, MyCrate);
        
        // Auto-move visuals!
        if (MyCrate.DoAutoMove) {
            i_body.color = new ColorHSB(0.5f, 0.3f, 1f).ToColor();
        }
        else {
            Destroy(i_autoMoveDir.gameObject);
            i_autoMoveDir = null;
        }
        
        // Add dimple images!
        for (int corner=0; corner<Corners.NumCorners; corner++) {
            if (myObj.IsDimple[corner]) {
                Image newImg = new GameObject().AddComponent<Image>();
                GameUtils.ParentAndReset(newImg.gameObject, rt_contents);
                GameUtils.FlushRectTransform(newImg.rectTransform);
                newImg.name = "Dimple" + corner;
                newImg.sprite = s_dimple;
                newImg.transform.localEulerAngles = new Vector3(0,0,-90*corner);
            }
        }
	}


    public override void UpdateVisualsPostMove() {
        base.UpdateVisualsPostMove();
        
        if (MyCrate.DoAutoMove) {
            i_autoMoveDir.enabled = MyCrate.AutoMoveDir!=Vector2Int.zero;
            
            float dirRot = MathUtils.GetSide(MyCrate.AutoMoveDir) * -90;
            i_autoMoveDir.transform.localEulerAngles = new Vector3(0, 0, dirRot);
        }
    }



}

