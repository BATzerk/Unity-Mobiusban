using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voyd : BoardOccupant {

    // Serializing
    override public BoardObjectData ToData() {
        return new VoydData (BoardPos);
    }

    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public Voyd (Board _boardRef, VoydData data) {
        base.InitializeAsBoardObject (_boardRef, data);
        // Tell my BoardSpace I'm on it!
        //MySpace.SetMyVoyd (this);
    }


}
