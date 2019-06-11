using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveKeys {
    // Gameplay
    public const string LastPlayedLevelAddress = "LastPlayedLevelAddress";
    public static string LastPlayedLevelInPack(int packIndex) { return "LastPlayedLevelInPack_" + packIndex; }
    public static string DidCompleteLevel(LevelAddress address) { return "DidCompleteLevel_" + address.ToString(); }
}
