using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesHandler : MonoBehaviour {
    // References!
    [Header ("Common")]
    [SerializeField] public GameObject ImageLine;
    [SerializeField] public GameObject ImageLinesJoint;
    
    [Header ("Mobiusban")]
    // Level, Board
    [SerializeField] public GameObject Level;
    [SerializeField] public GameObject BeamSegmentRenderer;
    [SerializeField] public GameObject BoardView;
    [SerializeField] public GameObject BoardSpaceView;
    // BoardObjects
    [SerializeField] private GameObject BeamGoalView;
    [SerializeField] private GameObject BeamSourceView;
    [SerializeField] private GameObject CrateView;
    [SerializeField] private GameObject CrateGoalView;
    [SerializeField] private GameObject ExitSpotView;
    [SerializeField] private GameObject PlayerView;
    
    // Getters
    public GameObject GetBoardObjectView(BoardObject sourceObject) {
        if (sourceObject is BeamGoal) { return BeamGoalView; }
        if (sourceObject is BeamSource) { return BeamSourceView; }
        if (sourceObject is Crate) { return CrateView; }
        if (sourceObject is CrateGoal) { return CrateGoalView; }
        if (sourceObject is ExitSpot) { return ExitSpotView; }
        if (sourceObject is Player) { return PlayerView; }
        Debug.LogError ("Trying to add BoardObjectView from BoardObject, but no clause to handle this type! " + sourceObject.GetType().ToString());
        return null;
    }
    
    
    
    // Instance
    static public ResourcesHandler Instance { get; private set; }


    // ----------------------------------------------------------------
    //  Awake
    // ----------------------------------------------------------------
    private void Awake () {
        // There can only be one (instance)!
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy (this);
        }
	}
    
}
