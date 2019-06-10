using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelData {
	// Properties
	public BoardData boardData;
	public LevelAddress myAddress;
    // Variable Properties
    public bool DidCompleteLevel { get; private set; }

    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public LevelData (LevelAddress myAddress, LevelDataXML ldxml) {
		// Basic properties
		this.myAddress = myAddress;
		boardData = new BoardData (ldxml);

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

