using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BlackieMiniGame : Dialog {

    public List<List<Gamepiece>> grid = new List<List<Gamepiece>>(); //grid of game pieces. First list is x values, second is y values
    Gamepiece emptyPiece = new EmptyNode(); //same object reference is used for every empty space
    public string[] puzzleFileNames;

    //reads in a puzzle set-up from a file and starts that puzzle
    public void ReadFromFile(string fileName) {
        StreamReader reader = new StreamReader("Assets/Resources/BlackieGames/" + fileName + ".txt");
        reader.Close();
    }

    //if the location x, y is within the bounds of the grid
    public bool IsInBounds(int x, int y) {
        return x >= 0 && y >= 0 && x < grid.Count && y < grid[0].Count; //&& (grid.Count == 0 ? false : y < grid[0].Count); Dont think I need this but grid[0] might cause issues if length 0
    }

    //returns the gamepiece at a specific location. Returns the empty piece if that location is not in bounds
    public Gamepiece GetAtLocation(int x, int y) {
        if (IsInBounds(x, y))
            return grid[x][y];
        else
            return emptyPiece;
    }

    //generates an adjacency array based on the location and direction
    public Gamepiece[] GenerateAdjacents(int x, int y, int direction) {
        Gamepiece[] adjacents = new Gamepiece[4];
        direction = 4 - direction;
        adjacents[(direction + 0) % 4] = GetAtLocation(x, y - 1);
        adjacents[(direction + 1) % 4] = GetAtLocation(x + 1, y);
        adjacents[(direction + 2) % 4] = GetAtLocation(x, y + 1);
        adjacents[(direction + 3) % 4] = GetAtLocation(x - 1, y);
        return adjacents;
    }

    //True if the piece can be placed at x,y on the grid. False if it cannot be. Direction must be between 0 and 3. 0 is north then it goes clockwise
    public bool CanPlace(Gamepiece p, int x, int y, int direction) {
        if (IsInBounds(x, y) && grid[x][y] == emptyPiece) {
            //adjacent squares set up so that 0 in the array is in front of the piece being placed, then clockwise
            Gamepiece[] adjacents = GenerateAdjacents(x, y, direction);
            //check if the piece can be placed there based on it's own requirements
            return p.CanPlace(adjacents);
        }
        return false;
    }

    //places p on the grid at x,y, facing the given direction. 0 is north, clockwise to 3 is west.
    public void PlacePiece(Gamepiece p, int x, int y, int direction) {
        if (CanPlace(p, x, y, direction)) {
            //add to grid and give the piece the information it needs. Then update its in world gameobject
            grid[x][y] = p;
            p.direction = direction;
            //set p.worldobject position here

            //set its power
            Gamepiece[] adjacents = GenerateAdjacents(x, y, direction);
            p.SetInitialPowerState(adjacents);
            //then if this piece becomes powered, check all adjacent unpowered pieces and update them too, chain until no more update
            if (p.IsPowered()) {
                UpdatePowerState(x, y - 1);
                UpdatePowerState(x + 1, y);
                UpdatePowerState(x, y + 1);
                UpdatePowerState(x - 1, y);
            }
        }
    }

    //used when a piece is placed to recursively update power of adjacent nodes
    public void UpdatePowerState(int x, int y) {
        Gamepiece p = GetAtLocation(x, y);
        //if the piece at location x,y is not empty or out of bounds or powered, then make its adjacency array and update its power as if it was just placed
        if (p != emptyPiece && !p.IsPowered()) {
            Gamepiece[] adjacents = GenerateAdjacents(x, y, p.direction);
            p.SetInitialPowerState(adjacents);
            //if this causes p to become powered, keep iterating
            if (p.IsPowered()){
                UpdatePowerState(x, y - 1);
                UpdatePowerState(x + 1, y);
                UpdatePowerState(x, y + 1);
                UpdatePowerState(x - 1, y);
            }
        }
    }
}
