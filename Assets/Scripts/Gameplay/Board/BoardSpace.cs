using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpace {
	// Properties
	public Vector2Int ColRow { get; private set; }
	private bool isPlayable = true;
    private bool isWallL, isWallT; // walls can only be on the LEFT and TOP of spaces.
    // References
    public BoardOccupant MyOccupant { get; private set; } // occupants sit on my face. Only one Occupant occupies each space.
    public ExitSpot MyExitSpot { get; private set; }
    public List<Beam> BeamsOverMe { get; private set; } // We keep a list of Beams, not BeamSegments, because the latter refs can be remade by the parent Beams. If a Beam is over us more than once (from Mirrors and/or Portals), we'll add it to the list again.

    // Getters
    public bool IsPlayable { get { return isPlayable; } }
    public int Col { get { return ColRow.x; } }
    public int Row { get { return ColRow.y; } }
    public bool HasExitSpot { get { return MyExitSpot != null; } }
    public bool HasOccupant { get { return MyOccupant != null; } }
    public bool HasImmovableOccupant { get { return MyOccupant!=null && !MyOccupant.IsMovable; } }
    public bool IsWall(int side) {
        switch(side) {
            case Sides.L: return isWallL;
            case Sides.T: return isWallT;
            default: return false;
        }
    }
    public bool MayOccupantEverExit(Vector2Int dirOut) {
        int side = MathUtils.GetSide(dirOut);
        if (IsWall(side)) { return false; }
        return true;
    }
    public bool MayOccupantEverEnter(Vector2Int dirIn) {
        int side = MathUtils.GetSide(dirIn);
        return MayOccupantEverEnter(side);
    }
    /** Side: Relative to ME. */
    private bool MayOccupantEverEnter(int side) {
        if (!IsPlayable) { return false; }
        if (HasImmovableOccupant) { return false; }
        if (IsWall(side)) { return false; }
        return true;
    }
    public int NumBeamsOverMe () {
        return BeamsOverMe.Count;
    }
    public bool CanBeamEnter (int sideEntering) {
        if (IsWall(sideEntering)) { return false; } // Wall in the way? Return false.
        if (MyOccupant==null) { return true; } // Nobody's on me! Yes, beams are free to fly!
        return MyOccupant.CanBeamEnter(sideEntering); // I can accept a beam if there's nothing on me, OR the thing on me doesn't block beams from this side!
    }
    public bool CanBeamExit (int sideExiting) {
        if (IsWall(sideExiting)) { return false; } // Wall in the way? Return false.
        if (MyOccupant==null) { return true; } // Nobody's on me! Yes, beams are free to fly!
        return MyOccupant.CanBeamExit(sideExiting); // I can accept a beam if there's nothing on me, OR the thing on me doesn't block beams from this side!
    }
    public int GetSideBeamExits (int sideEntered) {
        if (MyOccupant==null) { return Sides.GetOpposite(sideEntered); } // No BoardOccupant? Pass straight through me; simply return the other side.
        return MyOccupant.SideBeamExits (sideEntered);
    }
    /** Returns TRUE if this Beam is over me, AND it's NOT the Beam's first (origin) space! When it comes to satisfying criteria, we don't count the Space the Beam starts at (which is where its Source is too). */
    public bool HasBeamSansSource (int channelID) {
        foreach (Beam b in BeamsOverMe) {
            if (b.ChannelID == channelID) {
                if (!b.IsSpaceMyOriginSpace (this)) {
                    return true;
                }
            }
        }
        return false;
    }
    
    
	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public BoardSpace (BoardSpaceData _data) {
        ColRow = _data.ColRow;
        isPlayable = _data.isPlayable;
        isWallL = _data.isWallL;
        isWallT = _data.isWallT;
        BeamsOverMe = new List<Beam>();
	}
	public BoardSpaceData ToData () {
        BoardSpaceData data = new BoardSpaceData(Col, Row) {
            isPlayable = isPlayable,
            isWallL = isWallL,
            isWallT = isWallT,
        };
        return data;
	}
    
    
    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public void AddWall(int side) {
        switch(side) {
            case Sides.L: isWallL = true; break;
            case Sides.T: isWallT = true; break;
            default: Debug.LogError("Whoa, we're calling AddWall for a side that's NOT Top or Left: " + side); break;
        }
    }
    
    public void SetMyExitSpot(ExitSpot bo) {
        if (MyExitSpot != null) {
            throw new UnityException ("Oops! Trying to set a Space's MyExitSpot, but that Space already has an ExitSpot! " + Col + ", " + Row);
        }
        MyExitSpot = bo;
    }
    
	public void SetMyOccupant (BoardOccupant _bo) {
		if (MyOccupant != null) {
			throw new UnityException ("Oops! Trying to set a Space's Occupant, but that Space already has an Occupant! original: " + MyOccupant.GetType() + ", new: " + _bo.GetType().ToString() + ". " + Col + ", " + Row);
		}
		MyOccupant = _bo;
	}
	public void RemoveMyOccupant (BoardOccupant _bo) {
		if (MyOccupant != _bo) {
			throw new UnityException ("Oops! We're trying to remove a " + _bo.GetType() + " from a space that doesn't own it! " + Col + " " + Row + ".");
		}
		MyOccupant = null;
	}
    
    public void AddBeam (Beam _beam) {
        BeamsOverMe.Add (_beam);
    }
    public void RemoveBeam (Beam _beam) {
        BeamsOverMe.Remove (_beam);
    }



}
