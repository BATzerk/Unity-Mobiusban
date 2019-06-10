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

	public static BoardSpace GetSpace(Board b, int col, int row) {
		if (col<0 || row<0  ||  col>=b.NumCols || row>=b.NumRows) return null;
		return b.Spaces[col,row];
	}
	public static BoardOccupant GetOccupant(Board b, int col,int row) {
		BoardSpace space = GetSpace (b, col,row);
		if (space==null) { return null; }
		return space.MyOccupant;
	}
	public static Tile GetTile(Board b, int col,int row) {
		return GetOccupant(b, col,row) as Tile;
	}

	public static bool IsSpaceOpen(Board b, int col,int row) {
		BoardSpace bs = GetSpace (b, col,row);
		return bs!=null && bs.IsOpen();
	}
	public static bool IsSpacePlayable(Board b, int col,int row) {
		BoardSpace bs = GetSpace (b, col,row);
		return bs!=null && bs.IsPlayable;
	}
    
    
    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    static public void ApplyGravity(Board b) {
        for (int row=b.NumRows-1; row>0; --row) {
            for (int col=0; col<b.NumCols; col++) {
                if (IsSpaceOpen(b, col,row)) {
                    PullTileAboveIntoSpace(b, col,row);
                }
            }
        }
    }
    static public void PullTileAboveIntoSpace(Board b, int colTo,int rowTo) {
        // Look up until we find a Tile to pull down into this space. Then move it to this space!
        for (int row=rowTo-1; row>=0; --row) {
            Tile tile = GetTile(b, colTo,row);
            if (tile != null) {
                tile.SetColRow(colTo,rowTo);
                break;
            }
        }
    }


}
