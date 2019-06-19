using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelData {
	// Properties
	public BoardData boardData;
	public LevelAddress myAddress;
    public bool doShowEchoes { get; private set; }
    public float startingZoom { get; private set; }
    // Variable Properties
    public bool DidCompleteLevel { get; private set; }

    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public LevelData (LevelAddress myAddress, LevelDataXML ldxml) {
		// Basic properties
		this.myAddress = myAddress;
		boardData = new BoardData (ldxml);
        doShowEchoes = ldxml.doShowEchoes;
        startingZoom = ldxml.zoom;

		// LOAD up stats!
		DidCompleteLevel = SaveStorage.GetBool (SaveKeys.DidCompleteLevel(myAddress));
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void SetDidCompleteLevel (bool _didCompleteLevel) {
		if (DidCompleteLevel != _didCompleteLevel) {
			DidCompleteLevel = _didCompleteLevel;
			SaveStorage.SetBool (SaveKeys.DidCompleteLevel(myAddress), DidCompleteLevel);
		}
	}

}

