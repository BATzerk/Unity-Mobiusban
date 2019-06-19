using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voyd : BoardOccupant {
    // Properties
    public bool IsOn; // if FALSE, then I can be pushed!
    public bool DoTogFromUsage; // if TRUE, then I'll toggle IsOn when I'm A) Passed-over, and B) Moved.

    // Serializing
    override public BoardObjectData ToData() {
        return new VoydData (BoardPos, IsOn, DoTogFromUsage);
    }

    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public Voyd (Board _boardRef, VoydData data) {
        base.InitializeAsBoardOccupant (_boardRef, data);
        this.IsOn = data.isOn;
        this.DoTogFromUsage = data.doTogFromUsage;
        // Tell my BoardSpace I'm on it!
        //MySpace.SetMyVoyd (this);
    }
    
    

    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public void ToggleIsOn() {
        IsOn = !IsOn;
    }
    public void OnPassedThrough() {
        if (DoTogFromUsage) {
            IsOn = false;
        }
    }
    public override void SetColRow(Vector2Int _colRow, Vector2Int _moveDir) {
        base.SetColRow(_colRow, _moveDir);
        if (DoTogFromUsage) {
            if (_moveDir != Vector2Int.zero) {
                IsOn = true;
            }
        }
    }


}
