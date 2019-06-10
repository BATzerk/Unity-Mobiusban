//public struct BoardPos {
//	static public BoardPos undefined { get { return new BoardPos(-1,-1, -1); } }

//	public int col;
//	public int row;
//	int _sideFacing; // corresponds to Sides.cs.

//	public int sideFacing {
//		get { return _sideFacing; }
//		set {
//			_sideFacing = value;
//			if (_sideFacing<Sides.Min) { _sideFacing += Sides.Max; }
//			if (_sideFacing>=Sides.Max) { _sideFacing -= Sides.Max; }
//		}
//	}

//	public BoardPos (int col,int row) {
//		this.col = col;
//		this.row = row;
//		this._sideFacing = Sides.T;
//	}
//	public BoardPos (int col,int row, int SideFacing) {
//		this.col = col;
//		this.row = row;
//		this._sideFacing = SideFacing;
//	}
//	public BoardPos (Vector2Int posV2Int) {
////		BoardPos(posV2Int.x,posV2Int.y, 0,0);
//		this.col = posV2Int.x;
//		this.row = posV2Int.y;
//		this._sideFacing = Sides.T;
//	}

//	public Vector2Int ToVector2Int() { return new Vector2Int(col,row); }

//	public override bool Equals(object o) { return base.Equals (o); } // NOTE: Just added these to appease compiler warnings. I don't suggest their usage (because idk what they even do).
//	public override int GetHashCode() { return base.GetHashCode(); } // NOTE: Just added these to appease compiler warnings. I don't suggest their usage (because idk what they even do).

//	public static bool operator == (BoardPos b1, BoardPos b2) {
//		return b1.Equals(b2);
//	}
//	public static bool operator != (BoardPos b1, BoardPos b2) {
//		return !b1.Equals(b2);
//	}

//}
