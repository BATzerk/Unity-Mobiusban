using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour {
    // Components
    [SerializeField] private Image i_fullOverlay=null; // for circuit-complete feedback!
    [SerializeField] private TextMeshProUGUI t_levelName=null;
    // References
    private Level currLevel;
    
    // Getters (Private)
    private DragPath dragPath { get { return currLevel.Board.dragPath; } }


    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    private void Start() {
        // Add event listeners!
        GameManagers.Instance.EventManager.StartLevelEvent += OnStartLevel;
        GameManagers.Instance.EventManager.ClearPathEvent += OnClearPath;
        GameManagers.Instance.EventManager.AddPathSpaceEvent += OnAddPathSpace;
        GameManagers.Instance.EventManager.RemovePathSpaceEvent += OnRemovePathSpace;
    }
    private void OnDestroy() {
        // Remove event listeners!
        GameManagers.Instance.EventManager.StartLevelEvent -= OnStartLevel;
        GameManagers.Instance.EventManager.ClearPathEvent -= OnClearPath;
        GameManagers.Instance.EventManager.AddPathSpaceEvent -= OnAddPathSpace;
        GameManagers.Instance.EventManager.RemovePathSpaceEvent -= OnRemovePathSpace;
    }
    
    
    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    private void OnStartLevel(Level level) {
        this.currLevel = level;
        t_levelName.text = "LV " + (currLevel.MyAddress.level+1);
    }
    private void OnAddPathSpace(BoardSpace space) {
        UpdateFullOverlay();
    }
    private void OnClearPath() {
        UpdateFullOverlay();
    }
    private void OnRemovePathSpace() {
        UpdateFullOverlay();
    }
    private void UpdateFullOverlay() {
        i_fullOverlay.enabled = dragPath.IsCircuitComplete;
        GameUtils.SetUIGraphicColor(i_fullOverlay, Colors.TileColor(dragPath.ColorID), 0.3f);
    }


}
