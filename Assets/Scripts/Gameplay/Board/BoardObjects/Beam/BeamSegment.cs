using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** A segment is a continuous line. Beams only have multiple segments if they split in a Portal. */
public class BeamSegment {
	// References
    private List<BoardSpace> spacesOver;
    private Beam MyBeam;
    public BoardOccupant SourceOccupant { get; private set; } // the Occupant I come out of! Usually a BeamSource, but could also be a Portal.

    public BoardSpace GetFirstSpace() { return spacesOver[0]; }
    public Vector2Int LastColRow { get { return spacesOver[spacesOver.Count-1].ColRow; } }
	//public int LastCol { get { return spacesOver[spacesOver.Count-1].Col; } }
	//public int LastRow { get { return spacesOver[spacesOver.Count-1].Row; } }

	public BeamSegment (Beam _myBeam, BoardOccupant _sourceOccupant) {
		MyBeam = _myBeam;
		SourceOccupant = _sourceOccupant;
		spacesOver = new List<BoardSpace>();
		AddSpace (SourceOccupant.MySpace); // start with the space this segment originates from.
	}

	public void AddSpace (BoardSpace _space) {
		spacesOver.Add (_space);
		_space.AddBeam (MyBeam);
	}
	public void RemoveFromBoard () {
		foreach (BoardSpace bs in spacesOver) {
			bs.RemoveBeam (MyBeam);
		}
	}

}
