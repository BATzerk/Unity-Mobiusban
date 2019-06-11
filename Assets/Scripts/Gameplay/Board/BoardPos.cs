public struct BoardPos {
    public Vector2Int ColRow;
    public int ChirH; // horizontal chirality (along y-axis). -1 or 1.
    public int ChirV; // vertical chirality (along x-axis). -1 or 1.
    private int _sideFacing; // corresponds to Sides.cs.
    public int SideFacing {
        get { return _sideFacing; }
        set {
            _sideFacing = value;
            if (_sideFacing<Sides.Min) { _sideFacing += Sides.Max; }
            if (_sideFacing>=Sides.Max) { _sideFacing -= Sides.Max; }
        }
    }
    
    public BoardPos(int col,int row) {
        this.ColRow = new Vector2Int(col,row);
        ChirH = 1;
        ChirV = 1;
        _sideFacing = 0;
    }
    public BoardPos(Vector2Int colRow, int chirH,int chirV, int sideFacing) {
        this.ColRow = colRow;
        this.ChirH = chirH;
        this.ChirV = chirV;
        this._sideFacing = sideFacing;
    }
    public BoardPos(int col,int row, int chirH,int chirV, int sideFacing) {
        this.ColRow = new Vector2Int(col,row);
        this.ChirH = chirH;
        this.ChirV = chirV;
        this._sideFacing = sideFacing;
    }
    
    public override bool Equals(object o) { return base.Equals (o); } // NOTE: Just added these to appease compiler warnings. I don't suggest their usage (because idk what they even do).
    public override int GetHashCode() { return base.GetHashCode(); } // NOTE: Just added these to appease compiler warnings. I don't suggest their usage (because idk what they even do).

    public static bool operator == (BoardPos b1, BoardPos b2) {
        return b1.Equals(b2);
    }
    public static bool operator != (BoardPos b1, BoardPos b2) {
        return !b1.Equals(b2);
    }
    
    
}

//public struct BoardPos {
//    static public BoardPos undefined { get { return new BoardPos(-1,-1, -1); } }

//    public int x;
//    public int y;
//    int _sideFacing; // corresponds to Sides.cs.

//    public int sideFacing {
//        get { return _sideFacing; }
//        set {
//            _sideFacing = value;
//            if (_sideFacing<Sides.Min) { _sideFacing += Sides.Max; }
//            if (_sideFacing>=Sides.Max) { _sideFacing -= Sides.Max; }
//        }
//    }

//    public BoardPos (int x,int y) {
//        this.x = x;
//        this.y = y;
//        this._sideFacing = Sides.T;
//    }
//    public BoardPos (int x,int y, int SideFacing) {
//        this.x = x;
//        this.y = y;
//        this._sideFacing = SideFacing;
//    }
//    public BoardPos (Vector2Int posV2Int) {
////      BoardPos(posV2Int.x,posV2Int.y, 0,0);
//        this.x = posV2Int.x;
//        this.y = posV2Int.y;
//        this._sideFacing = Sides.T;
//    }

//    public Vector2Int ToVector2Int() { return new Vector2Int(x,y); }

//    public override bool Equals(object o) { return base.Equals (o); } // NOTE: Just added these to appease compiler warnings. I don't suggest their usage (because idk what they even do).
//    public override int GetHashCode() { return base.GetHashCode(); } // NOTE: Just added these to appease compiler warnings. I don't suggest their usage (because idk what they even do).

//    public static bool operator == (BoardPos b1, BoardPos b2) {
//        return b1.Equals(b2);
//    }
//    public static bool operator != (BoardPos b1, BoardPos b2) {
//        return !b1.Equals(b2);
//    }

//}
