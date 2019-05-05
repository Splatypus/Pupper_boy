using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BlackieGameGenerator : EditorWindow
{
    public static BlackieGameGenerator instance;
    public static int currentBoardPieceX;
    public static int currentBoardPieceY;

    [SerializeField]
    string xSize = "5";
    [SerializeField]
    string ySize = "5";

    int maxBoardSize = 10;

    TextAsset puzzleGame;
    [SerializeField]
    string currentPuzzleName = "Blackie Puzzle";
    [SerializeField]
    string tempPuzzleName = "";

    string defaultTextAssetPath = "BlackieGames/New Version/";
    string defaultTex2DPath = "BlackieGames/Pieces_Tex2D/";

    string lockedPiecePath = "Locked";
    string emptyPiecePath = "Empty";
    string startPiecePath = "Start";
    string endPiecePath = "End";
    string linePiecePath = "Line";
    string lPiecePath = "L";
    string tPiecePath = "T";
    string crossPiecePath = "Cross";
    string bridgePiecePath = "Bridge";

    public string[] allPuzzles;
    public int allPuzzlesIndex;

    Texture2D emptyPiece;
    Texture2D startPiece;
    Texture2D endPiece;
    Texture2D linePiece;
    Texture2D lPiece;
    Texture2D tPiece;
    Texture2D crossPiece;
    Texture2D bridgePiece;

    [SerializeField]
    BoardPiece[,] gameBoard;

    public enum BoardPieceType
    {
        EMPTY_PIECE,
        START_NODE,
        END_NODE,
        LINE_PIECE,   // (top and bottom)
        L_PIECE,      // (top and right)
        T_PIECE,      // (top, left, right)
        CROSS_PIECE,  // (all sides)
        BRIDGE_PIECE, // (top-bottom, left-right)
        LOCKED,

        _END
    }

    public struct BoardPiece
    {
        public BoardPieceType myType;
        public int myRotation;

        public BoardPiece(BoardPieceType type = BoardPieceType.LOCKED, int rotation = 0)
        {
            myType = type;
            myRotation = rotation;
        }
    }

    // Add menu named "My Window" to the Window menu
    [MenuItem("Tools/Blackie Game Generator #&g")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        BlackieGameGenerator window = (BlackieGameGenerator)EditorWindow.GetWindow(typeof(BlackieGameGenerator), true, "Blackie Game Generator");
        window.Show();
    }

    //---------------------------------------------------------------------------
    void LoadPuzzle(string puzzleName)
    {
        gameBoard = new BoardPiece[maxBoardSize, maxBoardSize];

        if (!File.Exists(Application.dataPath + "/Resources/" + defaultTextAssetPath + puzzleName + ".txt"))
        {

            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    gameBoard[i, j] = new BoardPiece(BoardPieceType.LOCKED, 0);
                }
            }

            currentPuzzleName = "";
            tempPuzzleName = "";
        }
        else
        {
            puzzleGame = Resources.Load(defaultTextAssetPath + puzzleName) as TextAsset;

            string[] data = puzzleGame.ToString().Split('\n');
            string[] line = data[0].Split(',');

            //-----first line is the width and the height-----
            xSize = line[0];
            ySize = line[1];

            //loop through input file to fill grid
            for (int j = 0; j < int.Parse(ySize); j++)
            {
                line = data[j + 1].Split('/');

                for (int i = 0; i < int.Parse(ySize); i++)
                {
                    try
                    {
                        gameBoard[i, j] = StringToBoardPiece(line[i]);
                    }
                    catch
                    {

                    }
                }
            }
        }

        currentPuzzleName = puzzleName;
    }

    //---------------------------------------------------------------------------
    void SavePuzzle(string puzzleName)
    {
        StreamWriter writer = new StreamWriter(Application.dataPath + "/Resources/" + defaultTextAssetPath + puzzleName + ".txt");

        //Write To File
        writer.WriteLine(xSize + "," + ySize);

        for (int j = 0; j < gameBoard.GetLength(1); j++)
        {
            string newRow = "";
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                if (gameBoard[i, j].myType == BoardPieceType.LOCKED)
                    newRow += "xxx/";
                else
                {
                    int type = (int)gameBoard[i, j].myType;
                    int rot = (int)gameBoard[i, j].myRotation;
                    newRow += type.ToString() + rot.ToString() + "0" + "/";
                }
            }

            newRow = newRow.Substring(0, newRow.Length - 1);
            writer.WriteLine(newRow);
        }

        writer.Close();
    }

    //---------------------------------------------------------------------------
    void CreatePuzzle(string puzzleName)
    {
        if (File.Exists(Application.dataPath + "/Resources/" + defaultTextAssetPath + puzzleName + ".txt"))
        { }
        else
        {
            SavePuzzle(puzzleName);
        }
    }

    //---------------------------------------------------------------------------
    void Refresh()
    {
        AssetDatabase.Refresh();
        RefreshPopupStringArray();
    }

    //---------------------------------------------------------------------------
    BoardPiece StringToBoardPiece(string input)
    {
        string firstChar;
        int rotation = 0;

        try
        {
            firstChar = input.ToCharArray()[0].ToString();
        }
        catch
        {
            firstChar = "x";
        }

        if (firstChar == "x")
        {
            return new BoardPiece(BoardPieceType.LOCKED, 0);
        }

        if (input.ToCharArray().Length > 1)
        {
            try
            {
                rotation = int.Parse(input.ToCharArray()[1].ToString());
            }
            catch
            {
                rotation = 0;
            }
        }

        switch (firstChar)
        {
            default:
                return new BoardPiece(BoardPieceType.LOCKED, 0);

            case "0":
                return new BoardPiece(BoardPieceType.EMPTY_PIECE, rotation);

            case "1":
                return new BoardPiece(BoardPieceType.START_NODE, rotation);

            case "2":
                return new BoardPiece(BoardPieceType.END_NODE, rotation);

            case "3":
                return new BoardPiece(BoardPieceType.LINE_PIECE, rotation);

            case "4":
                return new BoardPiece(BoardPieceType.L_PIECE, rotation);

            case "5":
                return new BoardPiece(BoardPieceType.T_PIECE, rotation);

            case "6":
                return new BoardPiece(BoardPieceType.CROSS_PIECE, rotation);

            case "7":
                return new BoardPiece(BoardPieceType.BRIDGE_PIECE, rotation);
        }
    }

    //---------------------------------------------------------------------------
    Texture2D LoadPieceTex2D(BoardPiece piece)
    {
        string assetName = "";
        switch (piece.myType)
        {
            default:
                assetName = "Empty";
                break;

            case BoardPieceType.LOCKED:
                assetName = lockedPiecePath;
                break;

            case BoardPieceType.EMPTY_PIECE:
                assetName = emptyPiecePath;
                break;

            case BoardPieceType.START_NODE:
                assetName = startPiecePath;
                break;

            case BoardPieceType.END_NODE:
                assetName = endPiecePath;
                break;

            case BoardPieceType.LINE_PIECE:
                assetName = linePiecePath;
                break;

            case BoardPieceType.L_PIECE:
                assetName = lPiecePath;
                break;

            case BoardPieceType.T_PIECE:
                assetName = tPiecePath;
                break;

            case BoardPieceType.CROSS_PIECE:
                assetName = crossPiecePath;
                break;

            case BoardPieceType.BRIDGE_PIECE:
                assetName = bridgePiecePath;
                break;
        }

        assetName += "_" + piece.myRotation;
        return Resources.Load(defaultTex2DPath + assetName) as Texture2D;
    }

    //---------------------------------------------------------------------------
    BoardPiece[,] ResizeArray(BoardPiece[,] original, int cols, int rows)
    {
        var newArray = new BoardPiece[cols, rows];

        for (int i = 0; i < newArray.GetLength(0); i++)
        {
            for (int j = 0; j < newArray.GetLength(1); j++)
            {
                newArray[i, j] = new BoardPiece(BoardPieceType.LOCKED, 0);
            }
        }

        int minCols = Mathf.Min(cols, original.GetLength(0));
        int minRows = Mathf.Min(rows, original.GetLength(1));
        for (int i = 0; i < minCols; i++)
            for (int j = 0; j < minRows; j++)
                newArray[i, j] = original[i, j];
        return newArray;
    }

    // ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
    /// ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
    // ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----


    //---------------------------------------------------------------------------
    static void RefreshPopupStringArray()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/" + instance.defaultTextAssetPath);
        FileInfo[] files = dir.GetFiles("*.txt");
        instance.allPuzzles = new string[files.Length];

        int index = 0;
        while (index > files.Length)
        {
            instance.allPuzzles[index] = files[index].FullName;
            Debug.Log(files[index].Name);

            index++;
        }
    }



    #region Popup Menu Items
    //---------------------------------------------------------------------------
    [MenuItem("Popups/Select_Piece/Locked")]
    static void ChangeSquareToLocked()
    {
        instance.gameBoard[currentBoardPieceX, currentBoardPieceY] = new BoardPiece(BoardPieceType.LOCKED, 0);
    }

    //---------------------------------------------------------------------------
    [MenuItem("Popups/Select_Piece/Empty")]
    static void ChangeSquareToEmpty()
    {
        instance.gameBoard[currentBoardPieceX, currentBoardPieceY] = new BoardPiece(BoardPieceType.EMPTY_PIECE, 0);
    }

    //---------------------------------------------------------------------------
    [MenuItem("Popups/Select_Piece/Start")]
    static void ChangeSquareToStart()
    {
        instance.gameBoard[currentBoardPieceX, currentBoardPieceY] = new BoardPiece(BoardPieceType.START_NODE, 0);
    }

    //---------------------------------------------------------------------------
    [MenuItem("Popups/Select_Piece/End")]
    static void ChangeSquareToEnd()
    {
        instance.gameBoard[currentBoardPieceX, currentBoardPieceY] = new BoardPiece(BoardPieceType.END_NODE, 0);
    }

    //---------------------------------------------------------------------------
    [MenuItem("Popups/Select_Piece/Line")]
    static void ChangeSquareToLine()
    {
        instance.gameBoard[currentBoardPieceX, currentBoardPieceY] = new BoardPiece(BoardPieceType.LINE_PIECE, 0);
    }

    //---------------------------------------------------------------------------
    [MenuItem("Popups/Select_Piece/L")]
    static void ChangeSquareToL()
    {
        instance.gameBoard[currentBoardPieceX, currentBoardPieceY] = new BoardPiece(BoardPieceType.L_PIECE, 0);
    }

    //---------------------------------------------------------------------------
    [MenuItem("Popups/Select_Piece/T")]
    static void ChangeSquareToT()
    {
        instance.gameBoard[currentBoardPieceX, currentBoardPieceY] = new BoardPiece(BoardPieceType.T_PIECE, 0);
    }

    //---------------------------------------------------------------------------
    [MenuItem("Popups/Select_Piece/Cross")]
    static void ChangeSquareToCross()
    {
        instance.gameBoard[currentBoardPieceX, currentBoardPieceY] = new BoardPiece(BoardPieceType.CROSS_PIECE, 0);
    }

    //---------------------------------------------------------------------------
    [MenuItem("Popups/Select_Piece/Bridge")]
    static void ChangeSquareToBridge()
    {
        instance.gameBoard[currentBoardPieceX, currentBoardPieceY] = new BoardPiece(BoardPieceType.BRIDGE_PIECE, 0);
    }
    #endregion

    //---------------------------------------------------------------------------
    void OnGUI()
    {
        if (!instance)
            instance = this;

        //Refresh All Puzzles
        if (allPuzzles == null || allPuzzles.Length == 0)
        {
            Refresh();
        }

        GUILayout.BeginVertical();//-----

        GUILayout.Space(10);
        GUILayout.BeginHorizontal(); //-----

        tempPuzzleName = EditorGUILayout.TextField("Puzzle Name", tempPuzzleName);

        //allPuzzlesIndex = EditorGUILayout.Popup(allPuzzlesIndex, allPuzzles);

        //Load \ Save \ Create Btn
        if (tempPuzzleName == currentPuzzleName)
        {
            if (File.Exists(Application.dataPath + "/Resources/" + defaultTextAssetPath + currentPuzzleName + ".txt"))
            {
                if (GUILayout.Button("Refresh / Save"))
                {
                    SavePuzzle(currentPuzzleName);
                    Refresh();
                }
            }
            else
            {
                if (GUILayout.Button("Create"))
                {
                    CreatePuzzle(currentPuzzleName);
                    Refresh();
                }

            }
        }
        else
        {
            if (File.Exists(Application.dataPath + "/Resources/" + defaultTextAssetPath + tempPuzzleName + ".txt"))
            {
                if (GUILayout.Button("Load"))
                {
                    LoadPuzzle(tempPuzzleName);
                }
            }
            else
            {
                if (GUILayout.Button("Create"))
                {
                    CreatePuzzle(currentPuzzleName);
                    Refresh();
                }

            }
        }

        GUILayout.EndHorizontal(); //-----

        GUILayout.Space(10);
        GUILayout.BeginVertical(); //-----

        GUILayout.Label("Size: ", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal(); //-----

        xSize = EditorGUILayout.DelayedTextField("X Size", xSize);
        ySize = EditorGUILayout.DelayedTextField("Y Size", ySize);

        GUILayout.EndHorizontal(); //-----

        GUILayout.EndVertical(); //-----

        GUILayout.BeginVertical();//-----

        int xIntSize;
        int yIntSize;

        int.TryParse(xSize, out xIntSize);
        int.TryParse(ySize, out yIntSize);

        xIntSize = Mathf.Clamp(xIntSize, 2, maxBoardSize);
        yIntSize = Mathf.Clamp(yIntSize, 2, maxBoardSize);

        xSize = xIntSize.ToString();
        ySize = yIntSize.ToString();


        //Resize If Needed
        if (gameBoard != null)
        {
            if (gameBoard.GetLength(0) < xIntSize || gameBoard.GetLength(1) < yIntSize || gameBoard.GetLength(0) > xIntSize || gameBoard.GetLength(0) > yIntSize)
            {
                gameBoard = ResizeArray(gameBoard, yIntSize, xIntSize);
            }
        }

        if (allPuzzles == null || allPuzzles.Length == 0)
            Refresh();

        //Draw Grid
        if (gameBoard != null)
        {
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    Rect pieceLocation = new Rect(15 + 80 * i, 90 + j * 80, 75, 75);
                    Texture2D pieceImg = LoadPieceTex2D(gameBoard[i, j]);

                    GUI.DrawTexture(pieceLocation, pieceImg, ScaleMode.ScaleToFit, true);

                    if (GUI.Button(pieceLocation, "", new GUIStyle()))
                    {
                        currentBoardPieceX = i;
                        currentBoardPieceY = j;
                        Vector2 mousePos = Event.current.mousePosition;

                        if (Event.current.button == 0)
                        {
                            EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), "Popups/Select_Piece/", null);
                        }
                        else if (Event.current.button == 1)
                        {
                            gameBoard[i, j].myRotation++;
                            if (gameBoard[i, j].myRotation > 3)
                                gameBoard[i, j].myRotation = 0;
                        }
                    }

                }
            }
        }
        else
        {
            LoadPuzzle(tempPuzzleName);
            //gameBoard = new BoardPiece[xIntSize, yIntSize];
        }

        GUILayout.EndVertical();//-----

        GUILayout.EndVertical();//-----

        //if (File.Exists(Application.dataPath + "/Resources/" + defaultTextAssetPath + currentPuzzleName + ".txt"))
        //    SavePuzzle(currentPuzzleName);
    }
}
