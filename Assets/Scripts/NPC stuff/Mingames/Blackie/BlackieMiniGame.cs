using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BlackieMiniGame : Dialog2 {
/*
    public List<List<Gamepiece>> grid; //grid of game pieces. First list is x values, second is y values
    public List<Gamepiece> goals;
    public List<Gamepiece> placeables;
    public List<Gamepiece> statics;
    Gamepiece emptyPiece; //same object reference is used for every empty space
    List<GameObject> tiles; //list of tiles so they can be cleaned up when puzzle is finished
    public BoxCollider gameBounds;

    public TextAsset[] puzzleFiles;
    public GameObject[] prefabs;
    public GameObject connectionPrefab;
    public GameObject[] gridTiles;
    public float tileDis;

    public int puzzleNumber = 0;
    public enum States { READY, RESETTING, NEUTRAL}
    public States state;
    public BlackieAI blackieRef;



    new public void Start()
    {
        base.Start();
        gameBounds.enabled = false;
        emptyPiece = new EmptyNode();
        state = States.READY;
    }

    //reads in a puzzle set-up from a file and starts that puzzle
    public void LoadPuzzle(int index) {
        string[] data = puzzleFiles[index].ToString().Split('\n');
        string[] line = data[0].Split(',');

        //-----first line is the width and the height-----
        int width = int.Parse(line[0]);
        int height = int.Parse(line[1]);

        //set bounding box to contain puzzle
        gameBounds.enabled = true;
        gameBounds.center = new Vector3(transform.position.x, 2.0f, transform.position.z + ((height + 1) / 2.0f) * tileDis);
        gameBounds.size = new Vector3((width - 1) * tileDis, 4.0f, (height - 1) * tileDis);

        //Make a grid of the given dimensions and fill with empty pieces - also instantiate the base grid
        grid = new List<List<Gamepiece>>(width);
        tiles = new List<GameObject>(width * height);
        goals = new List<Gamepiece>();
        statics = new List<Gamepiece>();
        placeables = new List<Gamepiece>();
        for (int i = 0; i < width; i++) {
            grid.Add(new List<Gamepiece>(height));
            for (int j = 0; j < height; j++) {
                //place corners or sides if along the corners or sides. Otherwise place centers
                Vector3 placeLocation = new Vector3(transform.position.x + (i - ((width - 1) / 2.0f)) * tileDis, transform.position.y - 0.5f, transform.position.z + j * tileDis + tileDis);
                placeLocation = GridToWorldSpace(new Vector2Int(i, j));
                placeLocation.y = 0.0f;
                grid[i].Add(emptyPiece);

                //place grid parts with edges along the edge and corners in the corner
                GameObject tile = null;
                if (i == 0) {
                    if (j == 0) {
                        //close left
                        tile = Instantiate(gridTiles[2], placeLocation, transform.rotation);
                        tile.transform.Rotate(new Vector3(0, -90.0f, 0));
                    } else if (j == height - 1) {
                        //far left
                        tile = Instantiate(gridTiles[2], placeLocation, transform.rotation);
                    } else {
                        //left edge
                        tile = Instantiate(gridTiles[1], placeLocation, transform.rotation);
                    }
                } else if (i == width - 1) {
                    if (j == 0) {
                        //close right
                        tile = Instantiate(gridTiles[2], placeLocation, transform.rotation);
                        tile.transform.Rotate(new Vector3(0, 180.0f, 0));
                    } else if (j == height - 1) {
                        //far right
                        tile = Instantiate(gridTiles[2], placeLocation, transform.rotation);
                        tile.transform.Rotate(new Vector3(0, 90.0f, 0));
                    } else {
                        //right edge
                        tile = Instantiate(gridTiles[1], placeLocation, transform.rotation);
                        tile.transform.Rotate(new Vector3(0, 180.0f, 0));
                    }
                } else if (j == 0) {
                    //close edge
                    tile = Instantiate(gridTiles[1], placeLocation, transform.rotation);
                    tile.transform.Rotate(new Vector3(0, -90.0f, 0));
                } else if (j == height - 1) {
                    //far edge
                    tile = Instantiate(gridTiles[1], placeLocation, transform.rotation);
                    tile.transform.Rotate(new Vector3(0, 90.0f, 0));
                } else {
                    //middle section
                    tile = Instantiate(gridTiles[0], placeLocation, transform.rotation);
                }
                tiles.Add(tile);
            }
        }

        //-----second line is number of each available piece you are given-----
        line = data[1].Split(',');
        for (int i = 0; i < line.Length; i++) {
            int count = int.Parse(line[i]);
            //instantiate that many of i
            for (int j = 0; j < count; j++) {
                GameObject inWorld = Instantiate(prefabs[i], new Vector3(transform.position.x - (placeables.Count + 1) * tileDis, transform.position.y, transform.position.z), transform.rotation);
                Gamepiece temp;
                switch (i)
                {
                    case 0:         //Standard node
                        temp = new BlackieNode(inWorld, false);
                        break;
                    case 1:         //Red prefered gate
                        temp = new ColorGate(inWorld, false, Gamepiece.PowerStates.Red);
                        break;
                    case 2:         //Blue prefered gate
                        temp = new ColorGate(inWorld, false, Gamepiece.PowerStates.Blue);
                        break;
                    case 3:         //Inverter
                        temp = new Inverter(inWorld, false);
                        break;
                    default:
                        temp = emptyPiece;
                        break;
                }
                WorldGamepiece inWorldScript = inWorld.GetComponent<WorldGamepiece>();
                inWorldScript.Start();
                inWorldScript.boardPiece = temp;
                placeables.Add(temp);
            }
        }
        //-----the remaining lines are which pieces are defaulted to which locations-----
        for (int i = 2; i < data.Length; i++) {
            line = data[i].Split(',');
            if (line.Length == 4) {
                //parse string
                int x = int.Parse(line[0]);
                int y = int.Parse(line[1]);
                int d = int.Parse(line[2]);
                int type = int.Parse(line[3]);
                //make a new object of the given type
                GameObject inWorld = Instantiate(prefabs[type], GridToWorldSpace(new Vector2Int(x, y)), transform.rotation);
                inWorld.transform.Rotate(0, 90.0f * d, 0);
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
                        goals.Add(temp);
                        break;
                    case 7:         //blue goal
                        temp = new GoalNode(inWorld, Gamepiece.PowerStates.Blue);
                        goals.Add(temp);
                        break;
                    default:
                        temp = emptyPiece;
                        break;
                }
                statics.Add(temp);
                WorldGamepiece inWorldScript = inWorld.GetComponent<WorldGamepiece>();
                inWorldScript.Start();
                inWorldScript.boardPiece = temp;
                inWorldScript.OnPlace();
            }
        }

    }

    //takes in a world space location and transforms it into grid coordinates
    public Vector2Int WorldToGridSpace(Vector3 pos) {
        pos -= transform.position;
        pos.x /= tileDis;
        pos.z /= tileDis;
        pos.x += (grid.Count - 1) / 2.0f;
        pos.z -= 1;
        pos.x += 0.5f;
        pos.z += 0.5f;
        Vector2Int gridSpace = new Vector2Int((int)pos.x, (int)pos.z);
        return gridSpace;

        Vector3 inLocalSpace = gameObject.transform.InverseTransformPoint(pos);
        inLocalSpace /= tileDis;
        //adjust to the shift in the 0,0 point away from the box
        inLocalSpace.x += (grid.Count - 1) / tileDis;
        inLocalSpace.z -= 1;
        //add 0.5f for rounding
        inLocalSpace.x += 0.5f;
        inLocalSpace.z += 0.5f;
        Vector2Int gridSpace = new Vector2Int((int)inLocalSpace.x, (int)inLocalSpace.z);
        return gridSpace;
        
    }

    //takes a position in the grid and finds the worldspace position of that point
    public Vector3 GridToWorldSpace(Vector2Int pos) {
        Vector3 worldSpace = new Vector3(pos.x, transform.position.y, pos.y);
        worldSpace *= tileDis;
        worldSpace.z += 1.0f;
        worldSpace.x -= (grid.Count - 1) * tileDis;
        worldSpace = gameObject.transform.TransformPoint(worldSpace);
        return worldSpace;
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
            CheckVictory();
        }
    }

    //removes the piece at location x, y and returns it if it exists. If no piece is there, reuturn null
    public Gamepiece RemovePiece(int x, int y) {
        Gamepiece removed = GetAtLocation(x, y);
        if (removed != emptyPiece && !removed.isLocked) {
            removed.SetState(Gamepiece.PowerStates.Off);
            grid[x][y] = emptyPiece;
            //remove power from anything this was powering.
            //check along anything this was connected to and is powered. If we find a power source, repower everything connected to it. If no souce, then leave dead. 
            RemovePower(x, y - 1);
            RemovePower(x + 1, y);
            RemovePower(x, y + 1);
            RemovePower(x - 1, y);
            CheckVictory(); //can removing a piece ever make you win? Probably not but whatever lets be safe here
            return removed;
        }
        Debug.Log("This error should never show. Fucking just kill me now if youre seeing this.");
        return null;
    }

    //recursively remove power
    public void RemovePower(int x, int y) {
        Gamepiece p = GetAtLocation(x, y);
        if (p != emptyPiece && p.IsPowered()) {
            //if the node can be turned off, turn it off
            p.SetState(Gamepiece.PowerStates.Off);
            //if p is still powered after being shut off, then it is supplying power. Update the power states of all the shit we just turned off
            //Note that because of color gates, we can't just return a bool here to see if things are powered way back at whichever node was removed. It all needs to be recalculated
            if (p.IsPowered()) {
                UpdatePowerState(x, y - 1);
                UpdatePowerState(x + 1, y);
                UpdatePowerState(x, y + 1);
                UpdatePowerState(x - 1, y);
            }
            else {
                RemovePower(x, y - 1);
                RemovePower(x + 1, y);
                RemovePower(x, y + 1);
                RemovePower(x - 1, y);
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
            if (p.IsPowered()) {
                UpdatePowerState(x, y - 1);
                UpdatePowerState(x + 1, y);
                UpdatePowerState(x, y + 1);
                UpdatePowerState(x - 1, y);
            }
        }
    }

    //checks to see if all goal nodes have been powered
    public void CheckVictory() {
        bool allOn = true;
        foreach (Gamepiece g in goals) {
            if (g.state == Gamepiece.PowerStates.Off) {
                allOn = false;
            }
        }
        //if we turned on all the goal nodes, win the game!
        if (allOn && goals.Count > 0) {
            RemovePuzzle();
            blackieRef.FinishedGame();
            progressionNum = 1;
        }
    }

    //starts machine so it can make puzzles
    public void SetUpMachine() {
        if (state == States.READY) {
            progressionNum = 1;
            state = States.NEUTRAL;
        }
    }

    //removes the current puzzle
    public void RemovePuzzle() {
        goals.Clear(); //remove all goals so the puzzle cannot be finished again before the next starts
        gameBounds.enabled = false;
        foreach (GameObject t in tiles)
        {
            Destroy(t);
        }
        foreach (Gamepiece t in placeables)
        {
            //Destory(t.gameObject);
            t.isLocked = true;
            t.worldObject.GetComponent<WorldGamepiece>().FancyDestroy();
        }
        foreach (Gamepiece t in statics)
        {
            //Destory(t.gameObject);
            t.isLocked = true;
            t.worldObject.GetComponent<WorldGamepiece>().FancyDestroy();
        }
    }

    IEnumerator DelayedNextLevel() {
        yield return new WaitForSeconds(5.0f);
        state = States.NEUTRAL;
        if (puzzleNumber < puzzleFiles.Length)
            LoadPuzzle(puzzleNumber);
    }

    //for callling from node editor. Reloads the current puzzle
    public void Reload() {
        if (state != States.RESETTING) {
            state = States.RESETTING;
            RemovePuzzle();
            StartCoroutine(DelayedNextLevel());
        }
    }
    //called from node editor. Loads puzzle of current number
    public void LoadNext() {
        LoadPuzzle(puzzleNumber);
    }
    */
}
