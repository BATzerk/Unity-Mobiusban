using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoydView : BoardObjectView {
    // Components
    [SerializeField] private Image i_backing=null;
    // References
    private Voyd myVoyd;


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    override public void Initialize (BoardView _myBoardView, BoardObject bo) {
        myVoyd = bo as Voyd;
        base.Initialize (_myBoardView, bo);
    }
    
    
    
    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public override void UpdateVisualsPostMove() {
        base.UpdateVisualsPostMove();
        i_backing.color = myVoyd.IsOn ? Color.black : Color.gray;
    }


}
