using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager {
	// Actions and Event Variables
	public delegate void NoParamAction ();
    public delegate void BoardSpaceAction (BoardSpace space);
	public delegate void BoolAction (bool _bool);
	public delegate void StringAction (string _str);
    public delegate void IntAction (int _int);
    public delegate void StringBoolAction (string _str, bool _bool);
	public delegate void LevelAction (Level _level);

    // Common
    public event NoParamAction ScreenSizeChangedEvent;
    public void OnScreenSizeChanged () { if (ScreenSizeChangedEvent!=null) { ScreenSizeChangedEvent (); } }
    
    // Gameplay
    public event NoParamAction CompleteMoveEvent;
    public event BoolAction SetIsLevelCompletedEvent;
    public event IntAction NumMovesMadeChangedEvent;
    public event LevelAction StartLevelEvent;

	public void OnCompleteMove () { if (CompleteMoveEvent!=null) { CompleteMoveEvent (); } }
	public void OnNumMovesMadeChanged (int numMovesMade) { if (NumMovesMadeChangedEvent!=null) { NumMovesMadeChangedEvent (numMovesMade); } }
	public void OnSetIsLevelCompleted (bool isLevelComplete) { if (SetIsLevelCompletedEvent!=null) { SetIsLevelCompletedEvent (isLevelComplete); } }
	public void OnStartLevel (Level _level) { if (StartLevelEvent!=null) { StartLevelEvent(_level); } }
    
    
    public event NoParamAction ClearPathEvent;
    public event BoardSpaceAction AddPathSpaceEvent;
    public event NoParamAction RemovePathSpaceEvent;
    public event NoParamAction SubmitPathEvent;
    public void OnClearPath() { if (ClearPathEvent!=null) { ClearPathEvent(); } }
    public void OnAddPathSpace(BoardSpace space) { if (AddPathSpaceEvent!=null) { AddPathSpaceEvent(space); } }
    public void OnRemovePathSpace() { if (RemovePathSpaceEvent!=null) { RemovePathSpaceEvent(); } }
    public void OnSubmitPath() { if (SubmitPathEvent!=null) { SubmitPathEvent(); } }
    



}




