using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BlackieMiniGame : Dialog {

    public List<List<Gamepiece>> grid; //grid of game pieces. First list is x values, second is y values
    Gamepiece emptyPiece; //same object reference is used for every empty space
    public TextAsset[] puzzleFiles;
    public GameObject[] prefabs;
    public GameObject[] gridTiles;
    public float tileDis;

    new public void Start()
    {
        base.Start();
        emptyPiece = new EmptyNode();
        LoadPuzzle(0);
    }

    //reads in a puzzle set-up from a file and starts that puzzle
    public void LoadPuzzle(int index) {
        string[] data = puzzleFiles[index].ToString().Split('\n');
        string[] line = data[0].Split(',');
        //-----first line is the width and the height-----
        int width = int.Parse(line[0]);
        int height = int.Parse(line[1]);
        //Make a grid of the given dimensions and fill with empty pieces - also instantiate the base grid
        grid = new List<List<Gamepiece>>(width);
        for (int i = 0; i < width; i++) {
            grid.Add(new List<Gamepiece>(height));
            for (int j = 0; j < height; j++) {
                grid[i].Add(emptyPiece);
                Instantiate(gridTiles[0], new Vector3(transform.position.x + (i - ((width-1)/2.0f)) * tileDis, transform.position.y - 0.5f, transform.position.z + j * tileDis + tileDis), transform.rotation);
            }
        }
        //-----second line is number of each available piece you are given-----

        
        //-----the remaining lines are which pieces are defaulted to which locations-----
        for (int i = 1; i < data.Length; i++) {
            line = data[i].Split(',');
            if (line.Length == 4) {
                //parse string
                int x = int.Parse(line[0]);
                int y = int.Parse(line[1]);
                int d = int.Parse(line[2]);
                int type = int.Parse(line[3]);
                //make a new object of the given type
                GameObject inWorld = Instantiate(prefabs[type], new Vector3(transform.position.x + x - (width / 2.0f), transform.position.y, transform.position.z + y), transform.rotation);
                inWorld.transform.Rotate(Vector3.up * d * 90.0f); //rotate it to match the input direction
                Gamepiece temp;
                //place a different piece depending on which type it is
                switch (type) {
                    case 0:         //Standard node
                        temp = new BlackieNode(inWorld, true);
                        break;
                    case 1:         //Red prefered gate
                        temp = new ColorGate(inWorld, true, Gamepiece.PowerStates.Red);
                        break;
                    case 2:         //Blue prefered gate
                        temp = new ColorGate(inWorld, true, Gamepiece.PowerStates.Blue);
                        break;
                    case 3:         //Inverter
                        temp = new Inverter(inWorld, true);
                        break;
                    case 4:         //red source
                        temp = new SourceNode(inWorld, Gamepiece.PowerStates.Red);
                        break;
                    case 5:         //blue source
                        temp = new SourceNode(inWorld, Gamepiece.PowerStates.Blue);
                        break;
                    case 6:         //red goal
                        temp = new GoalNode(inWorld, Gamepiece.PowerStates.Red);
                        break;
                    case 7:         //blue goal
                        temp = new GoalNode(inWorld, Gamepiece.PowerStates.Blue);
                        break;
                    default:
                        temp = emptyPiece;
                        break;
                }
                PlacePiece(temp, x, y, d);
            }
        }

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
