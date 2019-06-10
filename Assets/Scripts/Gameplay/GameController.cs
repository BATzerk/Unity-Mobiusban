using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour {
	// Properties
	private bool isPaused = false;
    // Objects
    public Level CurrLevel { get; private set; }
    // References
    [SerializeField] private RectTransform rt_boardArea=null; // FYI: This GO stays empty; it's for layout reference. This is sized EXACTLY as big as the BoardView can be-- so make those layout changes in the editor!

    // Getters (Private)
    private LevelAddress currLevelAddress { get { return CurrLevel.MyAddress; } }
	private DataManager dataManager { get { return GameManagers.Instance.DataManager; } }


	// ----------------------------------------------------------------
	//  Start / Destroy
	// ----------------------------------------------------------------
	private void Start () {
		// Set application values.
		Application.targetFrameRate = GameVisualProperties.TargetFrameRate;
        UpdateTimeScale();

		// In the editor? Reload our levels file!
		#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh();
		#endif

		// Start at the level we've most recently played!
		StartLevel(dataManager.GetLastPlayedLevelAddress());
	}


	// ----------------------------------------------------------------
	//  Doers - Loading Level
	// ----------------------------------------------------------------
	public void RestartCurrLevel () { StartLevel (currLevelAddress); }
	public void StartPrevLevel () {
		LevelData data = dataManager.GetLevelData (currLevelAddress.PreviousLevel);
		if (data != null) { StartLevel (data); }
	}
	public void StartNextLevel () {
		LevelData data = dataManager.GetLevelData (currLevelAddress.NextLevel);
		if (data != null) { StartLevel (data); }
		//else { OnCompleteLastLevelInPack(); } // If we've reached the end of this world...
	}
	public void StartLevel (LevelAddress address) {
        #if UNITY_EDITOR // In editor? Noice. Reload all levels from file so we can update during runtime!
        dataManager.ReloadLevels ();
        #endif
		LevelData ld = dataManager.GetLevelData (address);
		if (ld == null) { Debug.LogError ("Requested LevelData doesn't exist! address: " + address.ToString()); } // Useful feedback for dev.
		StartLevel (ld);
	}
	private void StartLevel (LevelData ld) {
		if (ld == null) { // Safety check.
			Debug.LogError ("Can't load the requested level! Can't find its LevelData.");
			if (CurrLevel == null) { // If there's no currentLevel, yikes! Default us to something.
				ld = dataManager.GetLevelData (0,0);
			}
			else { return; } // If there IS a currentLevel, then don't leave it.
		}
        
		// Reset some values
		DestroyCurrentLevel ();

		// Instantiate the Level from the provided LevelData!
		CurrLevel = Instantiate (ResourcesHandler.Instance.Level).GetComponent<Level>();
        CurrLevel.Initialize (this, MainCanvas.Canvas.transform, rt_boardArea, ld);
		SaveStorage.SetString (SaveKeys.LastPlayedLevelAddress, currLevelAddress.ToString());
        
		// Dispatch event!
		GameManagers.Instance.EventManager.OnStartLevel (CurrLevel);
	}

	private void DestroyCurrentLevel () {
		if (CurrLevel != null) {
			Destroy(CurrLevel.gameObject);
			CurrLevel = null;
		}
	}



	// ----------------------------------------------------------------
	//  Doers - Gameplay
	// ----------------------------------------------------------------
	private void TogglePause () {
		isPaused = !isPaused;
		UpdateTimeScale ();
	}
	private void UpdateTimeScale () {
		if (isPaused) { Time.timeScale = 0; }
		else { Time.timeScale = 1; }
	}



	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void Update () {
		RegisterButtonInput ();
	}
	private void RegisterButtonInput () {
		// ~~~~ DEBUG ~~~~
		// R = Reload current scene!
		if (Input.GetKeyDown(KeyCode.R)) { SceneHelper.ReloadScene(); return; }
		if (CurrLevel != null) {
			// LEFT/RIGHT arrow keys to change levels.
			if (Input.GetKeyDown(KeyCode.LeftArrow)) { StartPrevLevel(); return; }
			if (Input.GetKeyDown(KeyCode.RightArrow)) { StartNextLevel(); return; }
		}
		//// P = Toggle pause
		//else if (Input.GetKeyDown (KeyCode.P)) {
		//	TogglePause ();
		//}
	}



    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Debug
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#if UNITY_EDITOR
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded() {
        if (UnityEditor.EditorApplication.isPlaying) {
            SceneHelper.ReloadScene();
        }
    }
#endif




}






