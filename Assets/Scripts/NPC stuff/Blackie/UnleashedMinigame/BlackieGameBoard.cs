using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackieGameBoard {
    GameSpace[][] board;

    List<SourcePiece> sourceNodes;
    Queue<Piece> checkQueue;

    //constructor
    public BlackieGameBoard(){

        checkQueue = new Queue<Piece>();
    }

    #region public functions
    /* Rotates a piece at position x, y by turnAmount (1 is a 90 degree turn)
     * returns true on a sucessful rotation
     * returns false if x,y is out of bounds or if something is preventing rotation
     * **/
    public bool RotatePiece(int x, int y, int turnAmount) {
        Piece piece = GetPiece(x, y);
        if (piece == null) {
            return false;
        }

        return true;
    }
    #endregion


    #region private functions
    //returns the gamespace at the desired location, null if out of game bounds
    protected GameSpace GetSpace(int x, int y) {
        if (x >= 0 && y >= 0 && x < board.GetLength(0) && y < board.GetLength(1)) {
            return board[x][y];
        }
        return null;
    }
    //gets the piece at x, y. Null if out of bounds or if space is empty
    protected Piece GetPiece(int x, int y) {
        GameSpace space = GetSpace(x, y);
        return space == null ? null : space.piece;
    }
    //checks the board for incorrect pieces, updates visuals, and checks for victory conditions
    protected void CheckBoard() {
        
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
        protected bool isLocked; //can the player move this piece?
        protected BlackieGameBoard game;

        //input data
        protected int inputColor;
        protected int inputSide;
        //adjust the input to account for rotation  
        //int adjustedSide = rotation - side;

        //notifies this object that its receving input. 
        public virtual void ReceiveInput(int color, int side) {
            
        }
        
        //converts direction into world(board)space, then sends color input in that direction
        protected void SendInput(int color, int direction) {
            //convert direction into worldspace direction
            direction = (direction + rotation)%4;
            //set the side of the input tile from which they're receiving input from (the direction towards this tile)
            int sourceDirection = (direction + 2) % 4; 
            
            switch (direction) {
                case 0:
                    //game.GetPiece(x, y + 1)?.ReceiveInput(color, sourceDirection);
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    break;
            }
        }
        

        //rotates the piece clockwise (negative amount will result in counter-clockwise rotation). 1 is a 90 degree turn
        public void Rotate(int amount) {
            rotation += amount;
            rotation = (rotation + 4)%4; //convert to 0-3 range
        }
        
    }

    //A tile which generates "power"
    public class SourcePiece : Piece {
        int color; //the color this node supplies

        public override void ReceiveInput(int color, int side) {
            
        }

        //called when a boardcheck is initiated. Supplies power in the direction this peice is facing
        public void StartPower() {

        }
    }
    #endregion
}
