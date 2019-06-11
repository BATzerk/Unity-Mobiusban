using UnityEngine;

public struct LevelAddress {
	// Statics
	static public readonly LevelAddress undefined = new LevelAddress(-1, -1);
	static public readonly LevelAddress zero = new LevelAddress(0, 0);
	// Properties
	public int pack;
	public int level;

	public LevelAddress (int pack, int level) {
		this.pack = pack;
		this.level = level;
	}

	//public bool IsTutorial { get { return mode == GameModes.TutorialIndex; } }
    //public LevelAddress NextPack { get { return new LevelAddress(pack+1, level); } }
    //public LevelAddress PrevPack { get { return new LevelAddress(pack-1, level); } }
    public LevelAddress NextLevel { get { return new LevelAddress(pack, level+1); } }
    public LevelAddress PrevLevel { get { return new LevelAddress(pack, level-1); } }

	public override string ToString() { return pack + "," + level; }
	static public LevelAddress FromString(string str) {
		string[] array = str.Split(',');
		if (array.Length >= 2) {
			return new LevelAddress (int.Parse(array[0]), int.Parse(array[1]));
		}
		return LevelAddress.undefined; // Hmm.
	}

	public override bool Equals(object o) { return base.Equals (o); } // NOTE: Just added these to appease compiler warnings. I don't suggest their usage (because idk what they even do).
	public override int GetHashCode() { return base.GetHashCode(); } // NOTE: Just added these to appease compiler warnings. I don't suggest their usage (because idk what they even do).

	public static bool operator == (LevelAddress a, LevelAddress b) {
		return a.Equals(b);
	}
	public static bool operator != (LevelAddress a, LevelAddress b) {
		return !a.Equals(b);
	}
}