using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {
    
    
    public void OnClick_Play() {
        SceneHelper.OpenScene(SceneNames.Gameplay);
    }
    
    
}
