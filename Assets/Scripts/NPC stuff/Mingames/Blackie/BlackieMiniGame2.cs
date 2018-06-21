using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BlackieMiniGame2 : Dialog2 {

    public List<List<Gamepiece>> grid; //grid of game pieces. First list is x values, second is y values
    public List<Gamepiece> goals;
    public List<Gamepiece> startNodes;
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
    public enum States { READY, RESETTING, NEUTRAL }
    public States state;
    public BlackieAI blackieRef;

    new public void Start() {
        base.Start();
        gameBounds.enabled = false;
        emptyPiece = new EmptyNode();
        state = States.READY;
    }


    //reads in a puzzle set-up from a file and starts that puzzle
    public void LoadPuzzle(int index) {
        /*
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
                switch (i) {
                    case 0:         //Standard node
                        temp = new BlackieNode(inWorld, false);
                        break;
                    case 1:         //Red prefered gate
                        temp = new ColorGate(inWorld, false);
                        break;
                    case 2:         //Blue prefered gate
                        temp = new ColorGate(inWorld, false);
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
                GameObject inWorld = Instantiate(prefabs[type],
                                        new Vector3(transform.position.x + (x - ((width - 1) / 2.0f)) * tileDis,
                                                                            transform.position.y,
                                                                            transform.position.z + y * tileDis + tileDis),
                                                                                transform.rotation);
                inWorld.transform.Rotate(0, 90.0f * d, 0);
                inWorld.transform.Rotate(Vector3.up * d * 90.0f); //rotate it to match the input direction

                Gamepiece temp;
                //place a different piece depending on which type it is
                switch (type) {
                    case 0:         //Standard node
                        temp = new BlackieNode(inWorld, true);
                        break;
                    case 1:         //Red prefered gate
                        temp = new ColorGate(inWorld, true);
                        break;
                    case 2:         //Blue prefered gate
                        temp = new ColorGate(inWorld, true);
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
        */
    }
 

    //takes in a world space location and transforms it into grid coordinates
    public Vector2Int WorldToGridSpace(Vector3 pos) {
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

    //True if the piece can be placed at x,y on the grid. False if it cannot be. Direction must be between 0 and 3. 0 is north then it goes clockwise
    public bool CanPlace(Gamepiece p, int x, int y, int direction) {
        if (IsInBounds(x, y) && grid[x][y] == emptyPiece) {
            //in the case of a BlackieNode, which can be more than 1 unit long, check to make sure the whole thing fits
            if (p is BlackieNode) {
                BlackieNode p2 = (BlackieNode)p;
                return p2.CanPlaceHere(x, y, direction);
            }
            return true;
        }
        return false;
    }

    //places p on the grid at x,y, facing the given direction. 0 is north, clockwise to 3 is west.
    public void PlacePiece(Gamepiece p, int x, int y, int direction) {
        if (CanPlace(p, x, y, direction)) {
            //add to grid and give the piece the information it needs. Then update its in world gameobject
            grid[x][y] = p;
            p.direction = direction;
            p.location = new Vector2Int(x, y);

            //in the special case of placing a long piece
            if (p is BlackieNode) {
                BlackieNode p2 = (BlackieNode)p;
                p2.PlaceHere(x, y, direction);
            }

            if (CheckPower()) {
                //valid placement, check to see if we won
                CheckVictory();
            } else {
                //caused a short. Remove that piece
                //##TODO
            }
        }
    }

    //removes the piece at location x, y and returns it if it exists. If no piece is there, reuturn null
    public Gamepiece RemovePiece(int x, int y) {
        Gamepiece removed = GetAtLocation(x, y);
        if (removed != emptyPiece && !removed.isLocked) {
            removed.SetState(Gamepiece.PowerStates.OFF);
            grid[x][y] = emptyPiece;
            //again a special removal function for blackienodes, since they take up more than one spot
            if (removed is BlackieNode) {
                BlackieNode n = (BlackieNode)removed;
                n.RemoveSelf();
            }
            //remove power from anything this was powering. This *should* never be able to cause a short or make you win...
            CheckPower();
            return removed;
        }
        Debug.Log("This error should never show. Fucking just kill me now if youre seeing this.");
        return null;
    }

    //When a piece is placed or removed, check the power of the whole thing. Returns true if this is an ok setup. False if a short has been made
    public bool CheckPower() {
        //For each start node, run check power on it. 
        //Check power function on a node tells it which nodes next to it are powered, and which power. 
        //then those nodes repeat
        bool foundShort = false;
        foreach (Gamepiece g in startNodes) {

        }
        return foundShort;
    }

    //checks to see if all goal nodes have been powered
    public void CheckVictory() {
        bool allOn = true;
        foreach (Gamepiece g in goals) {
            if (g.state == Gamepiece.PowerStates.OFF) {
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

    //Called when a short happens. Removes a piece, launches it in the air and spawns a particle effect
    IEnumerator ShortAnim(int x, int y) {
        //Spawn particle systems
        yield return new WaitForSeconds(0.5f);
        //remove peice
        gameBounds.enabled = true;
    }

    //removes the current puzzle
    public void RemovePuzzle() {
        goals.Clear(); //remove all goals so the puzzle cannot be finished again before the next starts
        gameBounds.enabled = false;
        foreach (GameObject t in tiles) {
            Destroy(t);
        }
        foreach (Gamepiece t in placeables) {
            //Destory(t.gameObject);
            t.isLocked = true;
            t.worldObject.GetComponent<WorldGamepiece>().FancyDestroy();
        }
        foreach (Gamepiece t in statics) {
            //Destory(t.gameObject);
            t.isLocked = true;
            t.worldObject.GetComponent<WorldGamepiece>().FancyDestroy();
        }
    }

    //waits 5 seconds then spawns the puzzle of the current number
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


    //############# GAMEPIECE OBJECTS ############

    //Gamepiece parent object
    public class Gamepiece {

        public enum PowerStates { RED, BLUE, GREEN, OFF };
        public PowerStates state = PowerStates.OFF;
        public bool isLocked = false; //if the player can move this object
        public Vector2Int location;
        public int direction = 0; //the direction this piece is facing. 0 is north, going clockwise to 3 being west
        public GameObject worldObject;
        public BlackieMiniGame2 gameRef;

        //initial contructor with x,y,dir
        public Gamepiece(GameObject _worldObject, bool _isLocked, int x, int y, int dir) {
            worldObject = _worldObject;
            isLocked = _isLocked;
            location = new Vector2Int(x, y);
            direction = dir;
        }
        //extra piece constructor where x,y,dir are set later on placement
        public Gamepiece(GameObject _worldObject, bool _isLocked) {
            worldObject = _worldObject;
            isLocked = _isLocked;
            location = new Vector2Int(-1, -1);
            direction = 0;
        }


        //sets the power state of this node based on adjacent ones. Usually called right after it's placed, or when the power state of adjacent nodes change
        public virtual void SetInitialPowerState(Gamepiece[] adjacents) {

        }

        //if there is anything connected to this piece
        public virtual bool IsPowered() {
            return state != PowerStates.OFF;
        }

        //sets the power state of the node. Changes its color or whatever it needs to do too
        public virtual void SetState(PowerStates newstate) {
            state = newstate;
            //if turned off, do no particles
            ParticleSystem ps = worldObject.GetComponentInChildren<ParticleSystem>();
            //check is system is null
            if (ps == null) {
                return;
            }
            if (state == PowerStates.OFF) {
                ps.Stop();
            }
            //if turned on change particles to the right color
            else {
                ps.Play();
                ParticleSystem.MainModule module = ps.main;
                if (state == PowerStates.BLUE)
                    module.startColor = new Color(0.0f, 0.0f, 1.0f);
                else if (state == PowerStates.RED)
                    module.startColor = new Color(1.0f, 0.0f, 0.0f);
                else if (state == PowerStates.GREEN)
                    module.startColor = new Color(0.0f, 1.0f, 0.0f);
            }
        }

        //returns the vector2int to add to the location of this piece to get the spot in the given direction in local space
        //So if this has a 90 degree rotation to the right, forward will return (1,0), which moves a spot 1 to the right
        //0 is forward, 1 is right, 2 is back, 3 is left
        public Vector2Int LocalDirectionVector(int dir) {
            int directionNeeded = (direction + dir)%4; //since these are both basically the number of 90 degree rotations, just add them. Angles are easy for once!
            Vector2Int toReturn; //0->0,1    1->1,0      2->0,-1     3->-1,0
            switch (directionNeeded) {
                case 0:
                    toReturn = new Vector2Int(0, 1);
                    break;
                case 1:
                    toReturn = new Vector2Int(1, 0);
                    break;
                case 2:
                    toReturn = new Vector2Int(0, -1);
                    break;
                case 3:
                    toReturn = new Vector2Int(-1, 0);
                    break;
                default:
                    toReturn = new Vector2Int();
                    break;
            }
            return toReturn;
        }
    }

    //node that has nothing that is sent with a bounds issue or an empty space. Is always unpowered. Stops me from having to check for null every time I look at adjacent nodes
    public class EmptyNode : Gamepiece {
        //Note that since each empty spot always points to the same emptynode instance, it's location/direction can't be used for anything. Just dont try it.
        public EmptyNode() : base(null, true) {
            state = PowerStates.OFF;
        }
    }

    //the standard node
    public class BlackieNode : Gamepiece {

        public int length;
        public BlackieSubnode[] subnodes;

        public BlackieNode(GameObject _inWorld, bool _locked, int x, int y, int dir, int len) : base(_inWorld, _locked, x, y, dir) {
            length = len;
            subnodes = new BlackieSubnode[length-1];
        }

        //if a piece can be placed in the specified location without hitting other pieces.
        public bool CanPlaceHere(int x, int y, int dir) {
            direction = dir;
            bool validPlace = true;
            Vector2Int testLoc = new Vector2Int(x,y);
            Vector2Int offset = LocalDirectionVector(1); //since when we place it, it extends to our right
            for (int i = 0; i < length; i++) {
                validPlace = gameRef.IsInBounds(testLoc.x, testLoc.y) && gameRef.GetAtLocation(testLoc.x, testLoc.y) == gameRef.emptyPiece;
                testLoc += offset;
            }
            return validPlace;
        }

        //place a piece at a specific location, and make sure to fill along its length
        public void PlaceHere(int x, int y, int dir) {
            gameRef.grid[x][y] = this;
            location = new Vector2Int(x, y);
            direction = dir;
            Vector2Int newLoc = new Vector2Int(x, y);
            Vector2Int offset = LocalDirectionVector(1); //since when we place it, it extends to our right
            for (int i = 1; i < length; i++) { //start at 1 since this piece has already been asssigned to this location
                BlackieSubnode temp = new BlackieSubnode(newLoc, this);
                gameRef.grid[newLoc.x][newLoc.y] = temp;
                subnodes[i - 1] = temp;
                newLoc += offset;
            }
        }

        public void RemoveSelf() {
            Vector2Int offset = LocalDirectionVector(1); //since when we place it, it extends to our right
            for (int i = 0; i < length; i++) { //clear them all from the grid
                gameRef.grid[location.x][location.y] = gameRef.emptyPiece;
                location += offset;
            }
            //subnodes still technically holds references to these pieces until theyre overwritten the next time this is placed
        }

        //there should only be at most one powered node next to this one when this is called. Set it to the same power state as that one, or to off if there isnt another powered one
        public override void SetInitialPowerState(Gamepiece[] adjacents) {
            //if theres an adjacent powered node, we want to match that
            //state = PowerStates.Off;
            SetState(PowerStates.OFF);
            foreach (Gamepiece g in adjacents) {
                if (g.IsPowered())
                    SetState(g.state);
            }
        }

        //a connection piece for blackienodes. Spans between the start and the end.
        public class BlackieSubnode : Gamepiece {
            public BlackieNode parentNode;
            public BlackieSubnode(Vector2Int loc, BlackieNode parent) : base(null, true) {
                location = loc;
                parentNode = parent;
            }
        }
    }

    //takes two input. If they're the same, it outputs that color. If they're different then it outputs the third color
    public class ColorGate : Gamepiece {

        public ColorGate(GameObject _inWorld, bool _locked, int x, int y, int dir) : base(_inWorld, _locked, x, y, dir) {
            
        }

    }

    //Connects power from the left/right side and power from the forward/back sides
    public class BridgeNode : Gamepiece{
        public BridgeNode(GameObject _inWorld, bool _locked, int x, int y, int dir) : base (_inWorld, _locked, x, y, dir) {

        }
    }

    //source of power
    public class SourceNode : Gamepiece {
        //Source nodes have no direction
        public SourceNode(GameObject _inWorld, PowerStates _color, int x, int y) : base (_inWorld, true, x, y, 0) {
            state = _color;
        }

        //Cannot change the state of a source node
        public override void SetState(PowerStates newstate) {
        }

    }

    //goal node. Supply these with power of the right color to win
    public class GoalNode : Gamepiece {
        //goal nodes have no direction
        public GoalNode(GameObject _inWorld, PowerStates _color, int x, int y) : base (_inWorld, true, x, y, 0) {
            powerColor = _color;
            state = PowerStates.OFF;
        }
        //the desired color for it to be
        public PowerStates powerColor;

        //Goals will never supply power. Always return false here
        public override bool IsPowered() {
            return false;
        }

        //If there is an adjacent thing powered with the right color, then this powers
        public override void SetInitialPowerState(Gamepiece[] adjacents) {
            //if theres an adjacent powered node, we want to match that
            state = PowerStates.OFF;
            foreach (Gamepiece g in adjacents) {
                if (g.IsPowered() && g.state == powerColor)
                    SetState(g.state);
            }
        }
    }
}
