﻿using UnityEngine;

public struct Vector2Int {
    static public readonly Vector2Int zero = new Vector2Int(0,0);
    static public readonly Vector2Int none = new Vector2Int(-1,-1);
    //static public readonly Vector2Int undefined = new Vector2Int(
	static public readonly Vector2Int L = new Vector2Int (-1, 0);
	static public readonly Vector2Int R = new Vector2Int ( 1, 0);
	static public readonly Vector2Int B = new Vector2Int ( 0,-1);
	static public readonly Vector2Int T = new Vector2Int ( 0, 1);
	static public readonly Vector2Int TL = new Vector2Int (-1, 1);
	static public readonly Vector2Int TR = new Vector2Int ( 1, 1);
	static public readonly Vector2Int BL = new Vector2Int (-1,-1);
	static public readonly Vector2Int BR = new Vector2Int ( 1,-1);

	public int x;
	public int y;
	public Vector2Int (int _x,int _y) {
		x = _x;
		y = _y;
	}
	public Vector2 ToVector2 () { return new Vector2 (x,y); }

	public override string ToString() { return x+","+y; }
	public override bool Equals(object o) { return base.Equals (o); } // NOTE: Just added these to appease compiler warnings. I don't suggest their usage (because idk what they even do).
	public override int GetHashCode() { return base.GetHashCode(); } // NOTE: Just added these to appease compiler warnings. I don't suggest their usage (because idk what they even do).
    
    
    //public static Vector2Int Opposite(Vector2Int v) { return v * -1; }
    public static Vector2Int Opposite(Vector2Int v) { return new Vector2Int(-v.x, -v.y); }
    public static Vector2Int CW(Vector2Int v) { return new Vector2Int(v.y, -v.x); }
    public static Vector2Int CCW(Vector2Int v) { return new Vector2Int(-v.y, v.x); }
    
    public static Vector2Int operator + (Vector2Int a, Vector2Int b) {
        return new Vector2Int(a.x+b.x, a.y+b.y);
    }
    public static Vector2Int operator - (Vector2Int a, Vector2Int b) {
        return new Vector2Int(a.x-b.x, a.y-b.y);
    }
    //public static Vector2Int operator * (Vector2Int v, float m) {
    //    return new Vector2Int(Mathf.RoundToInt(v.x*m), Mathf.RoundToInt(v.y*m));
    //}
    public static Vector2 operator * (Vector2Int v, float m) {
        return v.ToVector2() * m;
    }
	public static bool operator == (Vector2Int a, Vector2Int b) {
		return a.Equals(b);
	}
	public static bool operator != (Vector2Int a, Vector2Int b) {
		return !a.Equals(b);
	}
}