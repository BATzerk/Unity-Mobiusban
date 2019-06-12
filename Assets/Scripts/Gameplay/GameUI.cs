using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour {
    // Components
    [SerializeField] private GameObject go_levelIsWon=null;
    [SerializeField] private TextMeshProUGUI t_packName=null;
    [SerializeField] private TextMeshProUGUI t_levelName=null;
    // References
    private Level currLevel;


    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    private void Start() {
        // Add event listeners!
        GameManagers.Instance.EventManager.LevelSetIsWonEvent += OnLevelSetIsWon;
        GameManagers.Instance.EventManager.StartLevelEvent += OnStartLevel;
    }
    private void OnDestroy() {
        // Remove event listeners!
        GameManagers.Instance.EventManager.LevelSetIsWonEvent -= OnLevelSetIsWon;
        GameManagers.Instance.EventManager.StartLevelEvent -= OnStartLevel;
    }
    
    
    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    private void OnStartLevel(Level level) {
        this.currLevel = level;
        LevelAddress addr = this.currLevel.MyAddress;
        t_levelName.text = addr.pack + "-" + addr.level;
        t_packName.text = currLevel.MyPackData==null ? "null" : currLevel.MyPackData.PackName;
    }
    private void OnLevelSetIsWon(bool isWon) {
        go_levelIsWon.SetActive(isWon);
    }


}
