using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardUtils {
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
		if (col<0 || row<0  ||  col>=b.NumCols || row>=b.NumRows) return null;
		return b.Spaces[col,row];
	}
    public static BoardOccupant GetOccupant(Board b, Vector2Int pos) { return GetOccupant(b, pos.x,pos.y); }
	public static BoardOccupant GetOccupant(Board b, int col,int row) {
		BoardSpace space = GetSpace (b, col,row);
		if (space==null) { return null; }
		return space.MyOccupant;
	}
	//public static Crate GetTile(Board b, int col,int row) {
	//	return GetOccupant(b, col,row) as Crate;
	//}

	//public static bool IsSpaceOpen(Board b, int col,int row) {
	//	BoardSpace bs = GetSpace (b, col,row);
	//	return bs!=null && bs.IsOpen();
	//}
	public static bool IsSpacePlayable(Board b, int col,int row) {
		BoardSpace bs = GetSpace (b, col,row);
		return bs!=null && bs.IsPlayable;
	}
    
    
    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    //static public void ApplyGravity(Board b) {
    //    for (int row=b.NumRows-1; row>0; --row) {
    //        for (int col=0; col<b.NumCols; col++) {
    //            if (IsSpaceOpen(b, col,row)) {
    //                PullTileAboveIntoSpace(b, col,row);
    //            }
    //        }
    //    }
    //}
    //static public void PullTileAboveIntoSpace(Board b, int colTo,int rowTo) {
    //    // Look up until we find a Tile to pull down into this space. Then move it to this space!
    //    for (int row=rowTo-1; row>=0; --row) {
    //        Crate tile = GetTile(b, colTo,row);
    //        if (tile != null) {
    //            tile.SetColRow(colTo,rowTo);
    //            break;
    //        }
    //    }
    //}
    public static bool MayMoveOccupant(Board b, Vector2Int occPos, Vector2Int dir) {
        if (b==null) { return false; } // Safety check.
        // Clone the Board!
        Board boardClone = b.Clone();
        // Move the occupant, and return the result!
        return MoveOccupant(boardClone, occPos, dir) == MoveResults.Success;
    }
    
    public static MoveResults MoveOccupant(Board b, Vector2Int occPos, Vector2Int dir) {
        BoardOccupant bo = GetOccupant(b, occPos);
        BoardSpace spaceTo = GetSpace(b, occPos+dir);
        
        // Someone's null? Return Fail.
        if (bo==null || spaceTo==null) {
            return MoveResults.Fail;
        }
        // We may NOT move into this space? Return Fail.
        if (!spaceTo.MayOccupantEverEnter(occPos)) {
            return MoveResults.Fail;
        }

        // Always remove its footprint first. We're about to move it!
        bo.RemoveMyFootprint();
        
        // Next space is OCCUPIED? Ok, try to move THAT fella, and return if fail!
        if (spaceTo.HasOccupant) {
            MoveResults result = MoveOccupant(b, spaceTo.BoardPos, dir);
            if (result!=MoveResults.Success) { return result; }
        }
        
        // Okay, we're clear to move our original fella! Do!
        bo.SetColRow(spaceTo.BoardPos);
        // Put footprint back down.
        bo.AddMyFootprint();
        
        // Return success!
        return MoveResults.Success;
    }


}
public enum MoveResults { Undefined, Success, Fail }