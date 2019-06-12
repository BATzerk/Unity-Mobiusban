using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveResults { Undefined, Success, Fail }
public enum WrapTypes {
    Undefined,
    
    None,
    Parallel,
    Flip,
    CW, // clockwise TODO: This DOESN'T apply separately to H/V; it applies to BOTH! Clarify this up in BoardData and stuff.
}

public static class BoardUtils {
    // ----------------------------------------------------------------
    //  Wrap Types
    // ----------------------------------------------------------------
    static public int WrapTypeToInt(WrapTypes wt) {
        switch (wt) {
            case WrapTypes.None: return 0;
            case WrapTypes.Parallel: return 1;
            case WrapTypes.Flip: return 2;
            case WrapTypes.CW: return 3;
            default: return -1; // Hmm.
        }
    }
    static public WrapTypes IntToWrapType(int wt) {
        switch (wt) {
            case 0: return WrapTypes.None;
            case 1: return WrapTypes.Parallel;
            case 2: return WrapTypes.Flip;
            case 3: return WrapTypes.CW;
            default: return WrapTypes.Undefined; // Hmm.
        }
    }
    
    public static TranslationInfo GetTranslationInfo(Board b, Vector2Int from, Vector2Int dir) {
        Vector2Int to = from + dir;
        Vector2Int dirOut = dir;
        Vector2Int dirIn = Vector2Int.Opposite(dirOut);
        int chirH = 1;
        int chirV = 1;
        int sideDelta = 0;
        // Wrap!
        if (to.x < 0) {
            if (b.DoWrapH) { to.x += b.NumCols; }
            if (b.WrapH == WrapTypes.Flip) {
                to.y = b.NumRows-1 - to.y;
                chirV *= -1;
            }
        }
        else if (to.x >= b.NumCols) {
            if (b.DoWrapH) { to.x -= b.NumCols; }
            if (b.WrapH == WrapTypes.Flip) {
                to.y = b.NumRows-1 - to.y;
                chirV *= -1;
            }
            if (b.WrapH == WrapTypes.CW) {
                to = new Vector2Int(to.y, b.NumRows-1);
                sideDelta ++;
                dirOut = Vector2Int.CW(dir);
            }
        }
        if (to.y < 0) {
            if (b.DoWrapV) { to.y += b.NumRows; }
            if (b.WrapV == WrapTypes.Flip) {
                to.x = b.NumCols-1 - to.x;
                chirH *= -1;
            }
        }
        else if (to.y >= b.NumRows) {
            if (b.DoWrapV) { to.y -= b.NumRows; }
            if (b.WrapV == WrapTypes.Flip) {
                to.x = b.NumCols-1 - to.x;
                chirH *= -1;
            }
            if (b.WrapH == WrapTypes.CW) {
                to = new Vector2Int(b.NumCols-1, to.x);
                sideDelta --;
                dirOut = Vector2Int.CCW(dir);
            }
        }
        return new TranslationInfo {
            from = from,
            to = to,
            dirOut = dirOut,
            dirIn = dirIn,
            sideDelta = sideDelta,
            chirDeltaH = chirH,
            chirDeltaV = chirV,
        };
    }
    //private static int GetNewSideFacing(Board b, int sideFacing, Vector2Int posFrom, Vector2Int dir) {
    //    int col = posFrom.x + dir.x;
    //    int row = posFrom.y + dir.y;
    //    if (col < 0) {
    //        if (b.WrapH == WrapTypes.Flip) {
    //            return Sides.GetOpposite(sideFacing);
    //        }
    //    }
    //    else if (col >= b.NumCols) {
    //        if (b.WrapH == WrapTypes.Flip) {
    //            return Sides.GetOpposite(sideFacing);
    //        }
    //    }
    //    return sideFacing;
    //}
    
    
    
	// ----------------------------------------------------------------
	//  Basic Getters
	// ----------------------------------------------------------------
	public static bool IsSpaceEven (int col,int row) {
		bool isColEven = col%2 == 0;
		if (row%2 == 0) { // If it's an EVEN row, return if it's an even col!
			return isColEven;
		}
		return !isColEven; // If it's an ODD row, return if it's NOT an even col!
	}

    public static BoardSpace GetSpace(Board b, Vector2Int pos) { return GetSpace(b, pos.x,pos.y); }
    public static BoardSpace GetSpace(Board b, int col,int row) {
        if (col<0 || row<0  ||  col>=b.NumCols || row>=b.NumRows) { return null; } // Outta bounds? Return NULL.
		return b.Spaces[col,row]; // In bounds! Return Space!
	}
    public static BoardOccupant GetOccupant(Board b, Vector2Int pos) { return GetOccupant(b, pos.x,pos.y); }
	public static BoardOccupant GetOccupant(Board b, int col,int row) {
		BoardSpace space = GetSpace (b, col,row);
		if (space==null) { return null; }
		return space.MyOccupant;
	}
	public static bool IsSpacePlayable(Board b, int col,int row) {
		BoardSpace bs = GetSpace (b, col,row);
		return bs!=null && bs.IsPlayable;
	}
    public static bool CanBeamEnterSpace (BoardSpace bs, int sideEntering) {
        return bs!=null && bs.CanBeamEnter(sideEntering);
    }
    public static bool CanBeamExitSpace (BoardSpace bs, int sideExiting) {
        return bs!=null && bs.CanBeamExit(sideExiting);
    }
    
    
    // ----------------------------------------------------------------
    //  Moving Occupants
    // ----------------------------------------------------------------
    public static bool MayMoveOccupant(Board b, Vector2Int occPos, Vector2Int dir) {
        if (b==null) { return false; } // Safety check.
        // Clone the Board!
        Board boardClone = b.Clone();
        // Move the occupant, and return the result!
        return MoveOccupant(boardClone, occPos, dir) == MoveResults.Success;
    }
    
    public static MoveResults MoveOccupant(Board b, Vector2Int occPos, Vector2Int dir) {
        // No dir?? Do nothing; return success!
        if (dir == Vector2Int.zero) { return MoveResults.Success; }
        
        TranslationInfo ti = GetTranslationInfo(b, occPos, dir);
        BoardOccupant bo = GetOccupant(b, occPos);
        BoardSpace spaceFrom = GetSpace(b, ti.from);
        BoardSpace spaceTo = GetSpace(b, ti.to);
        
        // Someone's null? Return Fail.
        if (bo==null || spaceTo==null) { return MoveResults.Fail; }
        // We can't EXIT this space? Return Fail.
        if (!spaceFrom.MayOccupantEverExit(ti.dirOut)) { return MoveResults.Fail; }
        // We can't ENTER this space? Return Fail.
        if (!spaceTo.MayOccupantEverEnter(ti.dirIn)) { return MoveResults.Fail; }

        // Always remove its footprint first. We're about to move it!
        bo.RemoveMyFootprint();
        
        // Next space is OCCUPIED? Ok, try to move THAT fella, and return if fail!
        if (spaceTo.HasOccupant) {
            MoveResults result = MoveOccupant(b, ti.to, ti.dirOut);
            if (result!=MoveResults.Success) { return result; }
        }
        
        // Okay, we're good to move our original fella! Do!
        //int newSideFacing = GetNewSideFacing(b, bo.SideFacing, occPos, dir);
        bo.SetColRow(ti.to, ti.dirOut);
        bo.ChangeSideFacing(ti.sideDelta);
        bo.ChangeChirality(ti.chirDeltaH, ti.chirDeltaV);
        // Put footprint back down.
        bo.AddMyFootprint();
        
        // Return success!
        return MoveResults.Success;
    }


}
public class TranslationInfo {
    public Vector2Int from, to;
    public Vector2Int dirOut, dirIn;
    public int chirDeltaH, chirDeltaV;
    public int sideDelta;
    //public TranslationInfo(
}