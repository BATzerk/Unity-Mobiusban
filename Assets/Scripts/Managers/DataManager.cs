using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class DataManager {
	// Properties
	private PackData[] packDatas;


	// ----------------------------------------------------------------
	//  Getters
	// ----------------------------------------------------------------
    public PackData GetPackData (LevelAddress address) {
        return GetPackData (address.pack);
    }
    public PackData GetPackData (int pack) {
        if (pack<0 || pack>=packDatas.Length) { return null; } // Safety check.
        return packDatas[pack];
    }
    public LevelData GetLevelData(int pack, int level) {
        PackData packData = GetPackData(pack);
        if (packData == null) { return null; } // Safety check.
        return packData.GetLevelData(level);
    }
	public LevelData GetLevelData (LevelAddress address) {
		return GetLevelData (address.pack, address.level);
	}
    
    public bool DidCompleteLevel (int packIndex, int levelIndex) {
        LevelData levelData = GetLevelData (packIndex, levelIndex);
        if (levelData == null) { return false; } // Safety check.
        return levelData.DidCompleteLevel;
    }

	public LevelAddress GetLastPlayedLevelAddress() {
		// Save data? Use it!
		if (SaveStorage.HasKey (SaveKeys.LastPlayedLevelAddress)) { 
			return LevelAddress.FromString (SaveStorage.GetString (SaveKeys.LastPlayedLevelAddress));
		}
		// No save data. Default to the first level, I guess.
		else {
			return new LevelAddress(0, 0);
		}
	}
    public LevelAddress PrevLevelAddress(LevelAddress addr) {
        if (addr==LevelAddress.zero) { return addr; } // Safety check; can't go before 0,0.
        addr.level --;
        if (addr.level < 0) { // Wrap back to previous pack.
            addr.pack --;
            addr.level = GetPackData(addr.pack).NumLevels-1;
        }
        return addr;
    }
    public LevelAddress NextLevelAddress(LevelAddress addr) {
        PackData pack = GetPackData(addr.pack);
        if (pack == null) { return LevelAddress.undefined; } // Safety check.
        addr.level ++;
        if (addr.level >= pack.NumLevels) { // Wrap over to next pack.
            addr.pack ++;
            addr.level = 0;
        }
        return addr;
    }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public DataManager() {
        ReloadLevels ();
	}

	public void ReloadLevels () {
        // Read Levels.xml file.
        PackCollectionDataXML collectionXML;
        TextAsset levelsXML = Resources.Load<TextAsset>("Levels/Levels");
        if (levelsXML != null) {
            XmlSerializer serializer = new XmlSerializer(typeof(PackCollectionDataXML));
            StringReader reader = new StringReader(levelsXML.text);
            collectionXML = serializer.Deserialize(reader) as PackCollectionDataXML;
        }
        else {
            Debug.LogError("Can't find Levels.xml file!");
            collectionXML = new PackCollectionDataXML();
        }

        // Make those PackDatas from the XML!
        int numPacks = collectionXML.packDataXMLs.Count;
        packDatas = new PackData[numPacks];
        for (int i=0; i<numPacks; i++) {
            LevelAddress packAddress = new LevelAddress(i, -1);
            packDatas[i] = new PackData (packAddress, collectionXML.packDataXMLs[i]);
        }
    }


    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    //public void OnCompleteLevel (LevelAddress levelAddress) {
    //  PackData packData = GetPackData (levelAddress);
    //  packData.OnCompleteLevel (levelAddress);
    //}


    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
	public void ClearAllSaveData() {
		// NOOK IT
		SaveStorage.DeleteAll ();
		ReloadLevels ();
		Debug.Log ("All SaveStorage CLEARED!");
	}




}


