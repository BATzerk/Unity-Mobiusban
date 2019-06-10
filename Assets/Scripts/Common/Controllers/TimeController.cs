using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour {
    // Properties
    static public float FrameTimeScale { get; private set; }
    static public float FrameTimeScaleUnscaled { get; private set; }


    // ----------------------------------------------------------------
    //  Update
    // ----------------------------------------------------------------
    private void Update () {
		FrameTimeScale = GameVisualProperties.TargetFrameRate * Time.deltaTime;
		FrameTimeScaleUnscaled = GameVisualProperties.TargetFrameRate * Time.unscaledDeltaTime;
	}



}
