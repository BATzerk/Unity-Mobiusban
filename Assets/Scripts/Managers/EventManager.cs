using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager {
	// Actions and Event Variables
	public delegate void NoParamAction ();
    public delegate void BoardAction (Board board);
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
    public event BoardAction BoardExecutedMoveEvent;
    public event BoolAction LevelSetIsWonEvent;
    public event IntAction NumMovesMadeChangedEvent;
    public event LevelAction StartLevelEvent;

    public void OnStartLevel (Level _level) { if (StartLevelEvent!=null) { StartLevelEvent(_level); } }
    public void OnLevelSetIsWon (bool isWon) { if (LevelSetIsWonEvent!=null) { LevelSetIsWonEvent (isWon); } }
	public void OnBoardExecutedMove (Board board) { if (BoardExecutedMoveEvent!=null) { BoardExecutedMoveEvent (board); } }
	public void OnNumMovesMadeChanged (int numMovesMade) { if (NumMovesMadeChangedEvent!=null) { NumMovesMadeChangedEvent (numMovesMade); } }
    
    
    



}




