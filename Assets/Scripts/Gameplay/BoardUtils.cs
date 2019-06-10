﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveResults { Undefined, Success, Fail }

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
        // Wrap!
        if (col < 0) {
            if (b.DoWrapH) { col += b.NumCols; }
            if (b.WrapH == WrapType.Flip) {
                row = b.NumRows-1 - row;
            }
        }
        else if (col >= b.NumCols) {
            if (b.DoWrapH) { col -= b.NumCols; }
            if (b.WrapH == WrapType.Flip) {
                row = b.NumRows-1 - row;
            }
        }
        if (row < 0) {
            if (b.DoWrapV) { row += b.NumRows; }
            if (b.WrapV == WrapType.Flip) {
                col = b.NumCols-1 - col;
            }
        }
        else if (row >= b.NumRows) {
            if (b.DoWrapV) { row -= b.NumRows; }
            if (b.WrapV == WrapType.Flip) {
                col = b.NumCols-1 - col;
            }
        }
        // Outta bounds? Return NULL.
        if (col<0 || row<0  ||  col>=b.NumCols || row>=b.NumRows) { return null; }
        // In bounds! Return space!
		return b.Spaces[col,row];
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
    private static int GetNewSideFacing(Board b, int sideFacing, Vector2Int posFrom, Vector2Int dir) {
        int col = posFrom.x + dir.x;
        int row = posFrom.y + dir.y;
        if (col < 0) {
            if (b.WrapH == WrapType.Flip) {
                return Sides.GetOpposite(sideFacing);
            }
        }
        else if (col >= b.NumCols) {
            if (b.WrapH == WrapType.Flip) {
                return Sides.GetOpposite(sideFacing);
            }
        }
        return sideFacing;
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
        BoardOccupant bo = GetOccupant(b, occPos);
        BoardSpace spaceFrom = GetSpace(b, occPos);
        BoardSpace spaceTo = GetSpace(b, occPos+dir);
        
        // Someone's null? Return Fail.
        if (bo==null || spaceTo==null) { return MoveResults.Fail; }
        // We can't EXIT this space? Return Fail.
        if (!spaceFrom.MayOccupantEverExit(spaceTo.BoardPos)) { return MoveResults.Fail; }
        // We can't ENTER this space? Return Fail.
        if (!spaceTo.MayOccupantEverEnter(occPos)) { return MoveResults.Fail; }

        // Always remove its footprint first. We're about to move it!
        bo.RemoveMyFootprint();
        
        // Next space is OCCUPIED? Ok, try to move THAT fella, and return if fail!
        if (spaceTo.HasOccupant) {
            MoveResults result = MoveOccupant(b, spaceTo.BoardPos, dir);
            if (result!=MoveResults.Success) { return result; }
        }
        
        // Okay, we're good to move our original fella! Do!
        int newSideFacing = GetNewSideFacing(b, bo.SideFacing, occPos, dir);
        bo.SetColRow(spaceTo.BoardPos);
        bo.SetSideFacing(newSideFacing);
        // Put footprint back down.
        bo.AddMyFootprint();
        
        // Return success!
        return MoveResults.Success;
    }


}