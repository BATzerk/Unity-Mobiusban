using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragPathView : MonoBehaviour {
    // Components
    [SerializeField] private ImageLines lines=null;
    // References
    [SerializeField] private BoardView myBoardView=null;
    
    // Getters (Private)
    private DragPath dragPath { get { return myBoardView.MyBoard.dragPath; } }
    

    // ----------------------------------------------------------------
    //  Start
    // ----------------------------------------------------------------
    private void Start() {
        float thickness = myBoardView.UnitSize * 0.18f;
        lines.SetThickness(thickness);
    }
    
    
    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public void RemakeVisuals() {
        if (dragPath.HasSpaces) {
            lines.SetNumPoints(dragPath.NumSpaces + 1);
            lines.SetColor(Colors.TileColor(dragPath.ColorID));
            for (int i=0; i<dragPath.NumSpaces; i++) {
                Vector2 pos = myBoardView.BoardToPos(dragPath.GetSpace(i).BoardPos);
                lines.SetPoint(i, pos);
            }
        }
        else {
            lines.SetNumPoints(0);
        }
    }
    
    
    // ----------------------------------------------------------------
    //  Update
    // ----------------------------------------------------------------
    private void Update() {
        UpdateLineToFinger();
    }
    private void UpdateLineToFinger() {
        if (lines.IsActive && lines.NumPoints>1) {
            Vector2 touchPosRelative = myBoardView.MyLevel.TouchPosBoard;
            touchPosRelative = new Vector2(touchPosRelative.x, -touchPosRelative.y); // flip the Y.
            lines.SetPoint(lines.NumPoints-1, touchPosRelative);
        }
    }
    
    
}
