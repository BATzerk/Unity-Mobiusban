using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrateView : BoardObjectView {
    // Components
    [SerializeField] private Image i_body=null;
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
        
        // TEMP. TODO: Make Obstacle class. All crates should be movable.
        //i_body.color = MyCrate.IsMovable ? c_movable : c_unmovable;
        //i_body.sprite = MyCrate.IsMovable ? s_movable : s_unmovable;
        
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



}

