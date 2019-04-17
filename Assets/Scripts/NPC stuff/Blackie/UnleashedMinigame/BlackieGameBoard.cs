using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
______ _            _    _      _      ___  ____       _ _____                      
| ___ \ |          | |  (_)    ( )     |  \/  (_)     (_)  __ \                     
| |_/ / | __ _  ___| | ___  ___|/ ___  | .  . |_ _ __  _| |  \/ __ _ _ __ ___   ___ 
| ___ \ |/ _` |/ __| |/ / |/ _ \ / __| | |\/| | | '_ \| | | __ / _` | '_ ` _ \ / _ \
| |_/ / | (_| | (__|   <| |  __/ \__ \ | |  | | | | | | | |_\ \ (_| | | | | | |  __/
\____/|_|\__,_|\___|_|\_\_|\___| |___/ \_|  |_/_|_| |_|_|\____/\__,_|_| |_| |_|\___|
                           _ _   _                            _   ___  ___ ___ ___            _   
  _ _  _____ __ __ __ __ _(_) |_| |_    _ __  ___ _ _ ___    /_\ / __|/ __|_ _|_ _|  __ _ _ _| |_ 
 | ' \/ _ \ V  V / \ V  V / |  _| ' \  | '  \/ _ \ '_/ -_)  / _ \\__ \ (__ | | | |  / _` | '_|  _|
 |_||_\___/\_/\_/   \_/\_/|_|\__|_||_| |_|_|_\___/_| \___| /_/ \_\___/\___|___|___| \__,_|_|  \__|
 
 */


public class BlackieGameBoard {
    //public
    public delegate void OnVictory();
    public event OnVictory OnVictoryEvent;


    //private
    GameSpace[,] board;
    List<Piece> pieceList; //full list of gamepieces
    List<Piece> sourceNodes; //list of all the source nodes
    int endNodeCount;

    //board checking
    Queue<Piece> checkQueue;
    List<Piece> errors;
    int endNodesPowered;

    //constructor
    public BlackieGameBoard() {

        checkQueue = new Queue<Piece>();
        pieceList = new List<Piece>();
        sourceNodes = new List<Piece>();
    }

    #region get/sets
    public int GetWidth() {
        return board.GetLength(0);
    }
    public int GetHeight() {
        return board.GetLength(1);
    }

    #endregion

    #region public functions
    /* Rotates a piece at position x, y by turnAmount (1 is a 90 degree turn)
     * returns true on a successful rotation
     * returns false if x,y is out of bounds or if something is preventing rotation
     * **/
    public bool RotatePiece(int x, int y, int turnAmount) {
        Piece piece = GetPiece(x, y);
        if (piece == null) {
            return false;
        }
        piece.Rotate(turnAmount);
        CheckBoard();
        return true;
    }

    /* Moves a piece at x,y 1 unit in direction (boardspace)
     * returns true on a successful movement
     * returns false if OOB or if something prevents movement
     * **/
    public bool MovePiece(int x, int y, int direction) {
        //if x,y is locked it cannot be moved
        if (GetPiece(x, y) == null || GetPiece(x, y).isLocked) {
            return false;
        }
        int targetX = x;
        int targetY = y;
        switch (direction) {
            case 0:
                targetY += 1;
                break;
            case 1:
                targetX += 1;
                break;
            case 2:
                targetY -= 1;
                break;
            case 3:
                targetX -= 1;
                break;
        }
        if (GetSpace(targetX, targetY) == null || GetPiece(targetX, targetY) != null){
            return false;
        }

        //if the peice is able to be moved, move it
        GameSpace space = GetSpace(x, y);
        GameSpace newSpace = GetSpace(targetX, targetY);
        Piece p = space.piece;
        space.piece = null;
        newSpace.piece = p;

        return true;
    }

    //check to see if the above operation is possible
    public bool CanMovePiece(int x, int y, int direction) {
        //if x,y is locked it cannot be moved
        if (GetPiece(x, y) == null || GetPiece(x, y).isLocked) {
            return false;
        }

        int targetX = x;
        int targetY = y;
        switch (direction) {
            case 0:
                targetY += 1;
                break;
            case 1:
                targetX += 1;
                break;
            case 2:
                targetY -= 1;
                break;
            case 3:
                targetX -= 1;
                break;
        }
        //to move, must be an open space within bounds
        return GetPiece(targetX, targetY) == null && GetSpace(targetX, targetY) != null;
    }

    //returns the gamespace at the desired location, null if out of game bounds
    public GameSpace GetSpace(int x, int y) {
        if (x >= 0 && y >= 0 && x < board.GetLength(0) && y < board.GetLength(1)) {
            return board[x, y];
        }
        return null;
    }

    //gets the piece at x, y. Null if out of bounds or if space is empty
    public Piece GetPiece(int x, int y) {
        GameSpace space = GetSpace(x, y);
        return space?.piece;
    }

    /* Loads a full board from a given file
     * **/
    public void LoadBoard(TextAsset textData) {
        string[] data = textData.ToString().Split('\n');
        string[] line = data[0].Split(',');

        //-----first line is the width and the height-----
        int width = int.Parse(line[0]);
        int height = int.Parse(line[1]);

        //clear lists
        board = new GameSpace[width, height];
        pieceList.Clear();
        sourceNodes.Clear();

        //loop through input file to fill grid
        for (int i = 0; i < width; i++) {
            line = data[i+1].Split('/');
            for (int j = 0; j < height; j++) {
                //leave "xxx" spaces empty
                if (line[j].Equals("xxx")) {
                    board[i, j] = null;
                    return;
                }
                //if in bounds, then fill it with gamespace
                board[i, j] = new GameSpace();
                //thats all we need if this is an empty space
                if (line[j].Equals("...")) {
                    return;
                }

                //if we get here, that means this is a real peice. Parse it and place it.
                int rotation = CharToInt(line[j][1]);
                Piece p = null;
                switch (CharToInt(line[j][0])) {
                    case 0:                                                 //empty piece
                        p = new Piece(i, j);
                        break;
                    case 1:                                                 //start node
                        p = new SourcePiece(i, j, CharToInt(line[j][3]) );
                        sourceNodes.Add(p);
                        break;
                    case 2:                                                 //end node
                        p = new EndPiece(i, j, CharToInt(line[j][3]) );
                        endNodeCount += 1;
                        break;
                    case 3:                                                 //line
                        p = new LinePiece(i, j);
                        break;
                    case 4:                                                 //elbow
                        p = new ElbowPiece(i, j);
                        break;
                    case 5:                                                 //T piece
                        p = new TPiece(i, j);
                        break;
                    case 6:                                                 //cross
                        p = new CrossPiece(i, j);
                        break;
                    case 7:                                                 //bridge
                        p = new BridgePiece(i, j);
                        break;
                }
                p.SetRotation(CharToInt(line[j][2])); //p should never be null at this point
                board[i, j].piece = p;
                pieceList.Add(p);
            }
        }
    }
    #endregion


    #region private functions
    //checks the board for incorrect pieces, updates visuals, and checks for victory conditions
    protected void CheckBoard() {
        //reset components
        errors.Clear();
        endNodesPowered = 0;
        //first reset all pieces
        foreach (Piece p in pieceList) {
            p.ResetInput();
        }
        //initial queue should be filled by powering all the source nodes
        foreach (Piece p in sourceNodes) {
            ((SourcePiece)p).StartPower();
        }

        //while the queue contrains elements, propagate power
        while (checkQueue.Count > 0) {
            Piece p = checkQueue.Dequeue();
            p.ProcessInput();
        }

        //if any pieces are preventing winning
        if (errors.Count == 0 && endNodesPowered == endNodeCount) {
            //TODO:Failure
            return;
        }
        Victory();
    }

    //wins the game
    private void Victory() {
        OnVictoryEvent?.Invoke();
    }

    //turns an single char into an int. No promise for if you pass it anything besides the 0-9 chars
    private int CharToInt(char c) {
        return c - '0';
    }
    #endregion


    #region supporting classes
    //a gamespace on the board. May contain a piece
    public class GameSpace {
        public Piece piece;
    }

    //A basic game piece. Can be placed on a gamespace, rotated, and moved
    public class Piece {
        protected int x, y; //position on the gamebaord
        protected int rotation; //which way "up" is facing (range of 0-3);
        public bool isLocked; //can the player move this piece?
        protected BlackieGameBoard game;

        //input data
        protected int inputColor;
        protected int inputSide;

        //constructor
        public Piece(int x, int y) {
            this.x = x;
            this.y = y;
        }

        #region public function
        //rotates the piece clockwise (negative amount will result in counter-clockwise rotation). 1 is a 90 degree turn
        public void Rotate(int amount) {
            rotation += amount;
            rotation = MatchRange(rotation); //convert to 0-3 range
        }
        public void SetRotation(int rotate) {
            rotation = MatchRange(rotate);
        }
        //resets input data
        public virtual void ResetInput() {
            inputColor = -1;
            inputSide = -1;
        }
        //process the input that was passed to us previously
        public virtual void ProcessInput() {

        }
        #endregion


        #region private functions
        //converts direction into world(board)space, then sends color input in that direction
        protected void SendInput(int color, int direction) {
            //convert direction into worldspace direction
            direction = (direction + rotation) % 4;
            //set the side of the input tile from which they're receiving input from (the direction towards this tile)
            int sourceDirection = (direction + 2) % 4;

            switch (direction) {
                case 0:
                    game.GetPiece(x, y + 1)?.ReceiveInput(color, sourceDirection);
                    break;
                case 1:
                    game.GetPiece(x + 1, y)?.ReceiveInput(color, sourceDirection);
                    break;
                case 2:
                    game.GetPiece(x, y - 1)?.ReceiveInput(color, sourceDirection);
                    break;
                case 3:
                    game.GetPiece(x - 1, y)?.ReceiveInput(color, sourceDirection);
                    break;
                default:
                    break;
            }
        }

        //notifies this object that its receving input. 
        private void ReceiveInput(int color, int side) {
            inputColor = color;
            inputSide = MatchRange(rotation - side); //adjust the input side to accound for rotation
            game.checkQueue.Enqueue(this);
        }

        //moves an int into the 0-3 range
        protected int MatchRange(int num) {
            return ((num % 4) + 4) % 4;
        }
        #endregion

    }

    //A tile which generates "power"
    public class SourcePiece : Piece {
        int color; //the color this tile supplies

        public SourcePiece(int x, int y, int c) : base(x, y) {
            color = c;
        }

        //called when a boardcheck is initiated. Supplies power in the direction this peice is facing
        public void StartPower() {
            SendInput(color, 0);
        }
    }

    //A tile which needs to be powered to win
    public class EndPiece : Piece {
        int goalColor;

        public EndPiece(int x, int y, int c) : base(x, y) {
            goalColor = c;
        }

        //requires power to be given of the correct color
        public override void ProcessInput() {
            if (inputSide != 0) {
                return;
            }
            if (inputColor != goalColor) {
                game.errors.Add(this);
                return;
            }

            //if correct side and color, victory counter
            game.endNodesPowered += 1;
        }
    }

    //a tile with a stright line to move power
    /*
      __________
     |   |  |   |
     |   |  |   |
     |   |  |   |
     |   |  |   |
     |___|__|___|   
     
    */
    public class LinePiece : Piece {
        int? color; //the color this tile has received

        public LinePiece(int x, int y) : base(x, y) { }

        //reset saved color info
        public override void ResetInput() {
            base.ResetInput();
            color = null;
        }

        public override void ProcessInput() {
            //ignore input from the sides
            if (inputSide == 1 || inputSide == 3) {
                return;
            }
            //if color is already assigned, this tile is a mistake
            if (color != null) {
                game.errors.Add(this);
                return;
            }
            //if we make it this far, send input to the opposite side of the tile, and set our color
            SendInput(inputColor, MatchRange(inputSide + 2));
            color = inputColor;
        }
    }

    //a tile with a bent line to move power
    /*    
      __________
     |   |  |   |
     |   |  |___|
     |   |______| 
     |          |
     |__________|   
          
    */
    public class ElbowPiece : Piece {
        int? color; //the color this tile has received

        public ElbowPiece(int x, int y) : base(x, y) { }

        //reset saved color info
        public override void ResetInput() {
            base.ResetInput();
            color = null;
        }

        public override void ProcessInput() {
            //ignore input from the wrong sides
            if (inputSide == 2 || inputSide == 3) {
                return;
            }
            //if color is already assigned, this tile is a mistake
            if (color != null) {
                game.errors.Add(this);
                return;
            }
            //if we make it this far, send input to the opposite side of the tile, and set our color
            SendInput(inputColor, (inputSide == 0 ? 1 : 0));
            color = inputColor;
        }
    }

    //a tile with a T line to move power
    /*    
      __________
     |   |  |   |
     |___|  |___|
     |__________| 
     |          |
     |__________|   
          
    */
    public class TPiece : Piece {
        int? color; //the color this tile has received

        public TPiece(int x, int y) : base(x, y) { }

        //reset saved color info
        public override void ResetInput() {
            base.ResetInput();
            color = null;
        }

        public override void ProcessInput() {
            //ignore input from the wrong sides
            if (inputSide == 2) {
                return;
            }
            //if color is already assigned, this tile is a mistake
            if (color != null) {
                game.errors.Add(this);
                return;
            }
            //if we make it this far, send input to the opposite side of the tile, and set our color
            if (inputSide == 0) {
                SendInput(inputColor, 1);
                SendInput(inputColor, 3);
            }
            if (inputSide == 1) {
                SendInput(inputColor, 0);
                SendInput(inputColor, 3);
            }
            if (inputSide == 3) {
                SendInput(inputColor, 0);
                SendInput(inputColor, 1);
            }

            color = inputColor;
        }
    }

    //a tile that powers everything adjacent to it
    /*    
      __________
     |   |  |   |
     |___|  |___|
     |____  ____| 
     |   |  |   |
     |___|__|___|   
          
    */
    public class CrossPiece : Piece {
        int? color; //the color this tile has received

        public CrossPiece(int x, int y) : base(x, y) { }

        //reset saved color info
        public override void ResetInput() {
            base.ResetInput();
            color = null;
        }

        public override void ProcessInput() {
            //if color is already assigned, this tile is a mistake
            if (color != null) {
                game.errors.Add(this);
                return;
            }
            //if we make it this far, send input to all other tiles
            SendInput(inputColor, MatchRange(inputSide + 1));
            SendInput(inputColor, MatchRange(inputSide + 2));
            SendInput(inputColor, MatchRange(inputSide + 3));
            color = inputColor;
        }
    }

    //a tile that that connects top to bottom and left to right
    /*    
      __________
     |   |__|   |
     |__/ __ \__|
     |___/  \___| 
     |   |  |   |
     |___|__|___|   
          
    */
    public class BridgePiece : Piece {
        int? verticalColor;
        int? horizontalColor;

        public BridgePiece(int x, int y) : base(x, y) { }

        //reset saved color info from both directions
        public override void ResetInput() {
            base.ResetInput();
            verticalColor = null;
            horizontalColor = null;
        }

        public override void ProcessInput() {
            
            if (inputSide == 0 || inputSide == 2) {
                //if the color in this direction is already assigned, this tile is a mistake
                if (verticalColor != null) {
                    game.errors.Add(this);
                    return;
                }
                verticalColor = inputColor;

            } else /*if inputSide is 1 or 3*/ {
                //if the color in this direction is already assigned, this tile is a mistake
                if (horizontalColor != null) {
                    game.errors.Add(this);
                    return;
                }
                horizontalColor = inputColor;
            }

            //send input out the opposite side
            SendInput(inputColor, MatchRange(inputSide + 2));
        }

    }

    #endregion
}
