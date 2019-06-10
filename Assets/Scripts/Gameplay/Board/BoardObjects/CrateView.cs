using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrateView : BoardObjectView {
    // Components
    [SerializeField] private Image i_body=null;
    // Properties
    [SerializeField] private Color c_movable=Color.white;
    [SerializeField] private Color c_unmovable=Color.white;
	// References
    [SerializeField] private Sprite s_movable=null;
    [SerializeField] private Sprite s_unmovable=null;
    public Crate MyCrate { get; private set; }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public void Initialize (BoardView _myBoardView, Crate myObj) {
		MyCrate = myObj;
		base.InitializeAsBoardObjectView (_myBoardView, MyCrate);
        
        i_body.color = MyCrate.IsMovable ? c_movable : c_unmovable;
        i_body.sprite = MyCrate.IsMovable ? s_movable : s_unmovable;
	}



}

