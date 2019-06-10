using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlipTypes { Horizontal, Vertical }

public class BoardData {
	// Constants
	private readonly char[] LINE_BREAKS_CHARS = { ',' }; // our board layouts are comma-separated (because XML's don't encode line breaks).
	// Properties
    public int numCols,numRows;
    public WrapType wrapH,wrapV;
    // BoardObjects
    public PlayerData playerData;// { get; private set; }
    public BoardSpaceData[,] spaceDatas { get; private set; }
    public List<BoardObjectData> allObjectDatas;
    private BoardOccupantData[,] occupantsInBoard; // this is SOLELY so we can go easily back and modify properties of an occupant we've already announced.
    
    // Getters
	private string[] GetLevelStringArrayFromLayoutString (string layout) {
		List<string> stringList = new List<string>(layout.Split (LINE_BREAKS_CHARS, System.StringSplitOptions.None));
		// Remove the last element, which will be just empty space (because of how we format the layout in the XML).
		stringList.RemoveAt (stringList.Count-1);
		// Cut the excess white space.
		for (int i=0; i<stringList.Count; i++) {
			stringList[i] = TextUtils.RemoveWhitespace (stringList[i]);
		}
		return stringList.ToArray();
	}
    private bool IsInBounds(int col,int row) {
        return col>=0 && col<numCols  &&  row>=0 && row<numRows;
    }
	


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
	public BoardData (LevelDataXML ldxml) {
        wrapH = Board.IntToWrapType(ldxml.wrapH);
        wrapV = Board.IntToWrapType(ldxml.wrapV);
        
		// Layout!
		string[] levelStringArray = GetLevelStringArrayFromLayoutString (ldxml.layout);
		if (levelStringArray.Length == 0) { levelStringArray = new string[]{"."}; } // Safety catch.

		int numLayoutLayers = 1; // will increment for every "", we find.
		for (int i=0; i<levelStringArray.Length; i++) {
			if (levelStringArray[i].Length == 0) { // We found a break that denotes another layer of layout!
				numLayoutLayers ++;
			}
		}

		// Set numCols and numRows!
		if (levelStringArray.Length == 0) {
			Debug.LogError ("Uhh! levelStringArray is empty!");
		}
		numCols = levelStringArray[0].Length;
		numRows = (int)((levelStringArray.Length-numLayoutLayers+1)/numLayoutLayers);

		// Add all gameplay objects!
		MakeEmptyBoardSpaces ();
		MakeEmptyLists ();
        
		occupantsInBoard = new BoardOccupantData[numCols,numRows];

		for (int layer=0; layer<numLayoutLayers; layer++) {
			for (int i=0; i<numCols; i++) {
				for (int j=0; j<numRows; j++) {
					int stringArrayIndex = j + layer*(numRows+1);
					if (stringArrayIndex>=levelStringArray.Length || i>=levelStringArray[stringArrayIndex].Length) {
						Debug.LogError ("Whoops! Mismatch in layout in a board layout XML. " + stringArrayIndex + ", " + i);
						continue;
					}
                    char spaceChar = (char) levelStringArray[stringArrayIndex][i];
                    //char spaceChar = (char) levelStringArray[j][i];
                    int col = i;
                    int row = numRows-1 - j;
                    switch (spaceChar) {
                    // BoardSpace properties!
                    case '~': GetSpaceData (col,row).isPlayable = false; break;
					// Player!
					case '@': SetPlayerData(col,row); break;
                    // Crates!
                    case 'o': AddCrateData (col,row, true); break;
                    case 'O': AddCrateData (col,row, false); break;
					// Walls!
					case '_': SetIsWallT (col,row-1); break; // note: because the underscore looks lower, consider it in the next row (so layout text file looks more intuitive).
					case '|': SetIsWallL (col,row); break;
					}
				}
			}
		}
        
        // Safety check.
        if (playerData == null) { SetPlayerData(0,0); }

		// We can empty out those lists now.
		occupantsInBoard = null;
	}

	/** Initializes a totally empty BoardData. */
	public BoardData (int _numCols,int _numRows) {
		numCols = _numCols;
		numRows = _numRows;
		MakeEmptyBoardSpaces ();
		MakeEmptyLists ();
	}

	private void MakeEmptyLists () {
        playerData = null;
		allObjectDatas = new List<BoardObjectData>();
		occupantsInBoard = new BoardOccupantData[numCols,numRows];
	}
	private void MakeEmptyBoardSpaces () {
		spaceDatas = new BoardSpaceData[numCols,numRows];
		for (int i=0; i<numCols; i++) {
			for (int j=0; j<numRows; j++) {
				spaceDatas[i,j] = new BoardSpaceData (i,j);
			}
		}
	}

	private BoardSpaceData GetSpaceData (int col,int row) { return spaceDatas[col,row]; }
	private BoardOccupantData GetOccupantInBoard (int col,int row) { return occupantsInBoard[col,row]; }
	private void SetOccupantInBoard (BoardOccupantData data) { occupantsInBoard[data.boardPos.x,data.boardPos.y] = data; }

    
    void SetPlayerData(int col,int row) {
        if (playerData != null) { Debug.LogError("Whoa! Two players defined in Level XML layout."); return; } // Safety check.
        playerData = new PlayerData(new Vector2Int(col,row));
        //allObjectDatas.Add (playerData);
        SetOccupantInBoard (playerData);
    }
	void AddCrateData (int col,int row, bool isMovable) {
		CrateData newData = new CrateData (new Vector2Int(col,row), isMovable);
		allObjectDatas.Add (newData);
        SetOccupantInBoard (newData);
	}
    void SetIsWallL(int col,int row) {
        if (!IsInBounds(col,row)) { return; } // Safety check.
        spaceDatas[col,row].isWallL = true;
    }
    void SetIsWallT(int col,int row) {
        if (!IsInBounds(col,row)) { return; } // Safety check.
        spaceDatas[col,row].isWallT = true;
    }



}
    /*
    static private BoardPos GetRotatedBoardPos (BoardPos _boardPos, int rotOffset, int _numCols,int _numRows) {
        if (rotOffset < 0) { rotOffset += 4; } // keep it in bounds between 1-3.
        // Simple check.
        if (rotOffset==0) { return _boardPos; }

        BoardPos newBoardPos = _boardPos;
        int sin = (int)Mathf.Sin(rotOffset*Mathf.PI*0.5f);
        int cos = (int)Mathf.Cos(rotOffset*Mathf.PI*0.5f);

        int fullColOffset=0;
        int fullRowOffset=0;
        switch(rotOffset) {
        case 1:
            fullColOffset = _numCols-1; break;
        case 2:
            fullColOffset = _numCols-1;
            fullRowOffset = _numRows-1; break;
        case 3:
            fullRowOffset = _numRows-1; break;
        default:
            Debug.LogError ("Passed in an invalid value into GetRotatedBoardPos: " + rotOffset +". Only 1, 2, or 3 are allowed."); break;
        }
        // 0,0 -> numCols,0
        // numCols,0 -> numCols,numRows
        // numCols,numRows -> 0,numRows
        // 0,numRows -> 0,0

        // 0,1 -> 1,numRows
        //      {{1,0},{0,1},{-1,0},{0,-1}}

        // col,row!
        newBoardPos.col = fullColOffset + _boardPos.col*cos - _boardPos.row*sin;
        newBoardPos.row = fullRowOffset + _boardPos.col*sin + _boardPos.row*cos;
        // sideFacing!
        newBoardPos.sideFacing += rotOffset*2; // DOUBLE the rotOffset for this. rotOffset is cardinal directions, but sideFacing is 8-dir.
        return newBoardPos;
    }
    static private BoardPos GetFlippedBoardPos (BoardPos _boardPos, FlipTypes flipType, int numCols,int numRows) {
        BoardPos newPos = _boardPos;
        switch (flipType) {
            case FlipTypes.Horizontal:
                newPos.col = numCols-1 - newPos.col;
                newPos.sideFacing = Sides.GetHorzFlipped(newPos.sideFacing);
                break;
            case FlipTypes.Vertical:
                newPos.row = numRows-1 - _boardPos.row;
                newPos.sideFacing = Sides.GetVertFlipped(newPos.sideFacing);
                break;
        }
        return newPos;
    }

    public void RandomlyRotateOrFlip(bool doPreserveDimensions=true) {
        int rand;
        if (numCols==numRows || !doPreserveDimensions) { // If we're a square, OR we don't care about our dimensions...!
            rand = Random.Range(0, 5); // 0-7 inclusive. There are 8 unique ways to rotate/flip a board.
        }
        else {
            rand = Random.Range(0, 3); // 0-3 inclusive. There are only 4 unique ways to rotate/flip a board *without* changing its dimensions.
        }
        switch (rand) {
            case 0: break; // Do nothin'. ;)
            case 1: FlipHorizontal(); break;
            case 2: FlipVertical(); break;
            case 3: Rotate180(); break;
            case 4: RotateCW(); break;
            case 5: RotateCCW(); break;
            case 6: FlipHorizontal(); RotateCW(); break;
            case 7: FlipHorizontal(); RotateCCW(); break;
        }
    }
    public void FlipHorizontal() { Flip(FlipTypes.Horizontal); }
    public void FlipVertical() { Flip(FlipTypes.Vertical); }
    public void RotateCW () { Rotate (1); }
    public void Rotate180 () { Rotate (2); }
    public void RotateCCW () { Rotate (3); }
    private void Rotate (int rotOffset) {
        int pnumCols = numCols;
        int pnumRows = numRows;
        // Update my # of cols/rows!
        if (rotOffset%2==1) {
            numCols = pnumRows;
            numRows = pnumCols;
        }
        // Remake grid spaces!
        BoardSpaceData[,] newSpaces = new BoardSpaceData[numCols,numRows];
        for (int col=0; col<numCols; col++) {
            for (int row=0; row<numRows; row++) {
                BoardPos oldSpacePos = GetRotatedBoardPos (new BoardPos(col,row), -rotOffset, pnumCols,pnumRows); // -rotOffset because we're starting with the *new* col/row and looking for the old one.
                newSpaces[col,row] = GetSpaceData(oldSpacePos.col, oldSpacePos.row); // set the new guy to EXACTLY the old guy!
                newSpaces[col,row].boardPos = new BoardPos(col,row); // Update its col/row, of course (that hasn't been done yet)!
            }
        }
        spaceDatas = newSpaces;

        // Update BoardPos of all BoardObjects!
        foreach (BoardObjectData data in allObjectDatas) {
            data.boardPos = GetRotatedBoardPos (data.boardPos, rotOffset, numCols,numRows);
        }
    }
    private void Flip (FlipTypes flipType) {
        // Remake grid spaces!
        BoardSpaceData[,] newSpaces = new BoardSpaceData[numCols,numRows];
        for (int col=0; col<numCols; col++) {
            for (int row=0; row<numRows; row++) {
                Vector2Int oldSpacePos = GetFlippedBoardPos (new Vector2Int(col,row), flipType, numCols,numRows);
                newSpaces[col,row] = GetSpaceData(oldSpacePos.x, oldSpacePos.y); // set the new guy to EXACTLY the old guy!
                newSpaces[col,row].boardPos = new Vector2Int(col,row); // Update its col/row, of course (that hasn't been done yet)!
            }
        }
        spaceDatas = newSpaces;

        // Update BoardPos of all BoardObjects!
        foreach (BoardObjectData data in allObjectDatas) {
            data.boardPos = GetFlippedBoardPos (data.boardPos, flipType, numCols,numRows);
        }
    }
    */