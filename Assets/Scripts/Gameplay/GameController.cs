using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameTimeController))]
public class GameController : MonoBehaviour {
	// Components
	private GameTimeController gameTimeController; // set in Awake.
    // Objects
    public Level CurrLevel { get; private set; }
    // References
    [SerializeField] private RectTransform rt_boardArea=null; // FYI: This GO stays empty; it's for layout reference. This is sized EXACTLY as big as the BoardView can be-- so make those layout changes in the editor!

    // Getters (Private)
    private LevelAddress currAddress { get { return CurrLevel.MyAddress; } }
	private DataManager dataManager { get { return GameManagers.Instance.DataManager; } }


	// ----------------------------------------------------------------
	//  Start / Destroy
	// ----------------------------------------------------------------
	private void Start () {
		// Set application values.
		Application.targetFrameRate = GameVisualProperties.TargetFrameRate;
        gameTimeController = GetComponent<GameTimeController>();

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
	public void RestartCurrLevel() { StartLevel (currAddress); }
    private void StartPrevLevel() { StartLevel(dataManager.PrevLevelAddress(currAddress)); }
    private void StartNextLevel() { StartLevel(dataManager.NextLevelAddress(currAddress)); }
    private void StartPrevPack() { StartPack(currAddress.pack - 1); }
    private void StartNextPack() { StartPack(currAddress.pack + 1); }
    private void StartPack(int packIndex) {
        int levelIndex = SaveStorage.GetInt(SaveKeys.LastPlayedLevelInPack(packIndex));
        StartLevel(new LevelAddress(packIndex, levelIndex));
    }
	public void StartLevel (LevelAddress address) {
        #if UNITY_EDITOR // In editor? Noice. Reload all levels from file so we can update during runtime!
        dataManager.ReloadLevels ();
        #endif
		StartLevel (dataManager.GetLevelData (address));
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
        SaveStorage.SetInt (SaveKeys.LastPlayedLevelInPack(currAddress.pack), currAddress.level);
        SaveStorage.SetString (SaveKeys.LastPlayedLevelAddress, currAddress.ToString());
        ExpandLevelHierarchy();
        
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
	//  Update
	// ----------------------------------------------------------------
	private void Update () {
		RegisterButtonInput ();
	}
	private void RegisterButtonInput () {
        // ESCAPE = Open MainMenu
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneHelper.OpenScene(SceneNames.MainMenu);
        }
        // SPACE = Start next level!
        if (CurrLevel.IsWon && Input.GetKeyDown(KeyCode.Space)) {
            StartNextLevel();
        }
        
		// ~~~~ DEBUG ~~~~
		// R = Reload current scene!
		if (Input.GetKeyDown(KeyCode.R)) { StartLevel(currAddress); return; }
        // ALT + BRACKET keys to change packs.
        if (InputController.IsKey_alt) {
            if (Input.GetKeyDown(KeyCode.LeftBracket)) { StartPrevPack(); return; }
            if (Input.GetKeyDown(KeyCode.RightBracket)) { StartNextPack(); return; }
        }
        // BRACKET keys to change levels.
        else {
            if (Input.GetKeyDown(KeyCode.LeftBracket)) { StartPrevLevel(); return; }
            if (Input.GetKeyDown(KeyCode.RightBracket)) { StartNextLevel(); return; }
        }
        
        // P = Toggle pause
        if (Input.GetKeyDown (KeyCode.P)) {
            gameTimeController.TogglePause();
        }
        // T = Toggle slow-mo
        else if (Input.GetKeyDown (KeyCode.T)) {
            gameTimeController.ToggleSlowMo();
        }
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
    
    private void ExpandLevelHierarchy() {
        if (!GameUtils.IsEditorWindowMaximized()) { // If we're maximized, do nothing (we don't want to open up the Hierarchy if it's not already open).
            GameUtils.SetExpandedRecursive(CurrLevel.gameObject, true); // Open up Level all the way down.
            for (int i=0; i<CurrLevel.transform.childCount; i++) { // Ok, now (messily) close all its children.
                GameUtils.SetExpandedRecursive(CurrLevel.transform.GetChild(i).gameObject, false);
            }
            GameUtils.FocusOnWindow("Game"); // focus back on Game window.
        }
    }




}






