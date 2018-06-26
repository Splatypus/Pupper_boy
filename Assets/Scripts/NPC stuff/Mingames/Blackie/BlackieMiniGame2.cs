using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BlackieMiniGame2 : Dialog2 {

    public List<List<Gamepiece>> grid; //grid of game pieces. First list is x values, second is y values
    int width, height;
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
    public enum States { READY, RESETTING, GAMEGOING }
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
        state = States.GAMEGOING;
        string[] data = puzzleFiles[index].ToString().Split('\n');
        string[] line = data[0].Split(',');

        //-----first line is the width and the height-----
        width = int.Parse(line[0]);
        height = int.Parse(line[1]);

        //set bounding box to contain puzzle
        gameBounds.enabled = true;
        gameBounds.center = new Vector3(transform.position.x, 2.0f, transform.position.z + ((height + 1) / 2.0f) * tileDis);
        gameBounds.size = new Vector3((width - 1) * tileDis, 4.0f, (height - 1) * tileDis);

        //Make a grid of the given dimensions and fill with empty pieces - also instantiate the base grid
        grid = new List<List<Gamepiece>>(width);
        tiles = new List<GameObject>(width * height);
        goals = new List<Gamepiece>();
        startNodes = new List<Gamepiece>();
        statics = new List<Gamepiece>();
        placeables = new List<Gamepiece>();
        for (int i = 0; i < width; i++) {
            grid.Add(new List<Gamepiece>(height));
            for (int j = 0; j < height; j++) {
                //place corners or sides if along the corners or sides. Otherwise place centers
                Vector3 placeLocation = GridToWorldSpace(new Vector2Int(i, j));
                placeLocation.y -= 0.5f; // = new Vector3(transform.position.x + (i - ((width - 1) / 2.0f)) * tileDis, transform.position.y - 0.5f, transform.position.z + j * tileDis + tileDis);
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
                    case 0:         //Color Gate
                        temp = new ColorGate(inWorld, false, this);
                        break;
                    case 1:         //Case 1-4 are nodes of that length
                    case 2:
                    case 3:
                    case 4:
                        temp = new BlackieNode(inWorld, false, this, i);
                        break;
                    case 5:         //Bridge Node
                        temp = new BridgeNode(inWorld, false, this);
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
            if (line.Length == 5) {
                //parse string
                int x = int.Parse(line[0]);
                int y = int.Parse(line[1]);
                int d = int.Parse(line[2]);
                int type = int.Parse(line[3]);
                int extra = int.Parse(line[4]);
                //make a new object of the given type
                int prefabToSpawn = i; //adjust which object to spawn based on the type indication and the "extra" field
                if (i == 1)
                    prefabToSpawn = extra;
                else if (i > 1)
                    prefabToSpawn += 4;
                GameObject inWorld = Instantiate(prefabs[prefabToSpawn], GridToWorldSpace(new Vector2Int(x, y)),transform.rotation);
                inWorld.transform.Rotate(0, 90.0f * d, 0);
                inWorld.transform.Rotate(Vector3.up * d * 90.0f); //rotate it to match the input direction

                Gamepiece temp;
                //for anything that spawns with a color, this determins that color
                Gamepiece.PowerStates spawnstate;
                switch (extra) {
                    case 1:
                        spawnstate = Gamepiece.PowerStates.RED;
                        break;
                    case 2:
                        spawnstate = Gamepiece.PowerStates.GREEN;
                        break;
                    case 3:
                        spawnstate = Gamepiece.PowerStates.BLUE;
                        break;
                    default:
                        spawnstate = Gamepiece.PowerStates.OFF;
                        break;
                }
                //place a different piece depending on which type it is
                switch (type) {
                    case 0:         //Gate node
                        temp = new ColorGate(inWorld, true, this, x, y, d);
                        break;
                    case 1:        //Standard
                        temp = new BlackieNode(inWorld, true, this, x, y, d, extra);
                        break;
                    case 2:         //Bridge node
                        temp = new BridgeNode(inWorld, true, this, x, y, d);
                        break;
                    case 3:         //Source
                        temp = new SourceNode(inWorld, this, spawnstate, x, y);
                        startNodes.Add(temp);
                        break;
                    case 4:         //Goal
                        temp = new GoalNode(inWorld, this, spawnstate, x, y);
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

        //make sure to power things that are supposed to be powered
        CheckPower();
    }


    //takes in a world space location and transforms it into grid coordinates
    public Vector2Int WorldToGridSpace(Vector3 pos) {
        Vector3 inLocalSpace = gameObject.transform.InverseTransformPoint(pos);
        //allign so that grid space 0,0 is however far left of the box
        inLocalSpace /= tileDis;
        inLocalSpace.x += (width - 1)/2.0f;
        inLocalSpace.z -= 1.0f;
        //add 0.5f for rounding
        inLocalSpace.x += 0.5f;
        inLocalSpace.z += 0.5f;
        Vector2Int gridSpace = new Vector2Int((int)inLocalSpace.x, (int)inLocalSpace.z);
        return gridSpace;
    }


    //takes a position in the grid and finds the worldspace position of that point
    public Vector3 GridToWorldSpace(Vector2Int pos) {
        Vector3 worldSpace = new Vector3(pos.x, 0.0f , pos.y);
        worldSpace.z += 1.0f;
        worldSpace.x -= (width - 1) / 2.0f;
        worldSpace *= tileDis;
        worldSpace = gameObject.transform.TransformPoint(worldSpace);
        worldSpace.y = transform.position.y;
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

    public Gamepiece GetAtLocation(Vector2Int loc) {
        return GetAtLocation(loc.x, loc.y);
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
                //remove piece
                ShortAnim(x, y);
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
        for (int i = 0; i < grid.Count; i++) {
            for (int j = 0; j < grid[0].Count; j++) {
                grid[i][j].PrepareForCheck();
            }
        }
        //For each start node, run check power on it. 
        //Check power function on a node tells it which nodes next to it are powered, and which power. 
        //then those nodes repeat
        foreach (Gamepiece g in startNodes) {
            //for each start node, generate a queue of what they're powering
            Queue<PowerTransfer> outputs = new Queue<PowerTransfer>();
            List<PowerTransfer> toQ = g.TransferPower(new PowerTransfer {
                sourceLocation = g.location,
                effectLocation = g.location,
                powerType = g.state
            });
            foreach (PowerTransfer pt in toQ) {
                outputs.Enqueue(pt);
            }
            //then loop through and apply the same function to each node that they power, until nothing more receives power, or a short is found.
            while (outputs.Count > 0) {
                PowerTransfer transfer = outputs.Dequeue();
                toQ = GetAtLocation(transfer.effectLocation.x, transfer.effectLocation.y).TransferPower(transfer);
                foreach (PowerTransfer pt in toQ) {
                    outputs.Enqueue(pt);
                }
                //check for short, and return false if it did
                if (GetAtLocation(transfer.effectLocation.x, transfer.effectLocation.y).DidShort())
                    return false;
            }
        }
        //if nothing shorted, return true
        return true;
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
            progressionNum = 2;
            state = States.READY;
        }
    }

    //starts machine so it can make puzzles
    public void SetUpMachine() {
        if (state == States.READY) {
            progressionNum = 1;
            state = States.GAMEGOING;
        }
    }

    //Called when a short happens. Removes a piece, launches it in the air and spawns a particle effect
    public void ShortAnim(int x, int y) {
        Gamepiece p = RemovePiece(x, y);
        if (p != emptyPiece) {
            gameBounds.enabled = false;
            //Spawn particle systems
            //remove peice
            p.worldObject.GetComponent<WorldGamepiece>().DoForcedRemove();
            gameBounds.enabled = true;
        } else {
            print("Oh shit sorted the circuit but attempted to remove an empty node");
        }
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
        state = States.GAMEGOING;
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

    //used for powercheck. Pass this to a node and it will return a list of them, telling how its powering things. Empty nodes should be skipped.
    public struct PowerTransfer{
        public Vector2Int sourceLocation;
        public Vector2Int effectLocation;
        public Gamepiece.PowerStates powerType;
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

        public List<PowerTransfer> sources; //list of sources of power, used to tell if it should short

        //initial contructor with x,y,dir
        public Gamepiece(GameObject _worldObject, bool _isLocked, BlackieMiniGame2 gameManager, int x, int y, int dir) {
            worldObject = _worldObject;
            isLocked = _isLocked;
            location = new Vector2Int(x, y);
            direction = dir;
            gameRef = gameManager;
        }
        //extra piece constructor where x,y,dir are set later on placement
        public Gamepiece(GameObject _worldObject, bool _isLocked, BlackieMiniGame2 gameManager) {
            worldObject = _worldObject;
            isLocked = _isLocked;
            location = new Vector2Int(-1, -1);
            direction = 0;
            gameRef = gameManager;
        }

        //takes a souce of power targeting this square. Returns a list of all the squares this tries to power.
        public virtual List<PowerTransfer> TransferPower(PowerTransfer source) {
            SetState(source.powerType); //take on the power type of the souce
            List<PowerTransfer> output = new List<PowerTransfer>();
            for (int i = 0; i < 4; i++) {
                //default method will pass power on to anything adjacent to this, as long as that thing wasnt the source and isnt empty.
                Vector2Int target = location + LocalDirectionVector(i);
                if (target != source.sourceLocation && gameRef.GetAtLocation(target.x, target.y) != gameRef.emptyPiece) {
                    output.Add(new PowerTransfer() {
                        sourceLocation = location,
                        effectLocation = target,
                        powerType = state
                    });
                }
            }
            return output;
        }

        //called on each piece when powercheck happens.
        public void PrepareForCheck() {
            if (sources == null)
                sources = new List<PowerTransfer>();
            else
                sources.Clear();
        }

        //checks against sources list to see if there are any issues. Bad power sources results in a short
        public virtual bool DidShort() {
            return false;
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
        public EmptyNode() : base(null, true, null) {
            state = PowerStates.OFF;
        }

        //never power itself or anything else
        public override List<PowerTransfer> TransferPower(PowerTransfer source) {
            return new List<PowerTransfer>();
        }
    }

    //the standard node
    public class BlackieNode : Gamepiece {

        public int length;

        public BlackieNode(GameObject _inWorld, bool _locked, BlackieMiniGame2 gameManager, int x, int y, int dir, int len) : base(_inWorld, _locked, gameManager, x, y, dir) {
            length = len;
        }
        public BlackieNode(GameObject _inWorld, bool _locked, BlackieMiniGame2 gameManager, int len) : base(_inWorld, _locked, gameManager) {
            length = len;
        }

        public override List<PowerTransfer> TransferPower(PowerTransfer source) {
            List<PowerTransfer> output = new List<PowerTransfer>();
            //if the power is not affecting this node or the other end node, then its ignored
            if (source.effectLocation != location || source.effectLocation != location + (LocalDirectionVector(1) * (length - 1))) {
                return output;
            }
            SetState(source.powerType); //take on the power type of the souce
            sources.Add(source); //add this as a source of power
            //for the spots around this node
            for (int i = 0; i < 4; i++) {
                //default method will pass power on to anything adjacent to this, as long as that thing wasnt the source and isnt empty.
                Vector2Int target = location + LocalDirectionVector(i);
                if (target != source.sourceLocation && gameRef.GetAtLocation(target.x, target.y) != gameRef.emptyPiece && gameRef.GetAtLocation(target.x, target.y) != this) {
                    output.Add(new PowerTransfer() {
                        sourceLocation = location,
                        effectLocation = target,
                        powerType = state
                    });
                }
            }
            if (length > 1) {
                //for the spots around the other end node
                for (int i = 0; i < 4; i++) {
                    //default method will pass power on to anything adjacent to this, as long as that thing wasnt the source and isnt empty.
                    Vector2Int target = location + (LocalDirectionVector(1) * (length - 1)) + LocalDirectionVector(i);
                    if (target != source.sourceLocation && gameRef.GetAtLocation(target.x, target.y) != gameRef.emptyPiece && gameRef.GetAtLocation(target.x, target.y) != this) {
                        output.Add(new PowerTransfer() {
                            sourceLocation = location,
                            effectLocation = target,
                            powerType = state
                        });
                    }
                }
            }
            return output;
        }

        //for blackie nodes, it simply shorts if there is more than one source of power to it at all
        public override bool DidShort() {
            return sources.Count > 1;
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
                gameRef.grid[newLoc.x][newLoc.y] = this;
                newLoc += offset;
            }
        }

        public void RemoveSelf() {
            Vector2Int offset = LocalDirectionVector(1); //since when we place it, it extends to our right
            for (int i = 0; i < length; i++) { //clear them all from the grid
                gameRef.grid[location.x][location.y] = gameRef.emptyPiece;
                location += offset;
            }
        }

    }

    //takes two input. If they're the same, it outputs that color. If they're different then it outputs the third color
    public class ColorGate : Gamepiece {

        public ColorGate(GameObject _inWorld, bool _locked, BlackieMiniGame2 gameManager, int x, int y, int dir) : base(_inWorld, _locked, gameManager, x, y, dir) {
            
        }
        public ColorGate(GameObject _inWorld, bool _locked, BlackieMiniGame2 gameManager) : base(_inWorld, _locked, gameManager) {

        }

        //Color gates will power above and below them with power based on their left and right
        public override List<PowerTransfer> TransferPower(PowerTransfer source) {
            List<PowerTransfer> output = new List<PowerTransfer>();
            sources.Add(source);

            PowerTransfer left = source;
            PowerTransfer right = source; //these usally dont remain as source. Just need an initial value :(
            bool leftFound = false;
            bool rightFound = false; //Wow I cant have nullable structs what is this
            //find out if there are power souces to the left and right of this node
            foreach (PowerTransfer p in sources) {
                if (p.sourceLocation == location + LocalDirectionVector(3)) {
                    left = p;
                    leftFound = true;
                }
                if (p.sourceLocation == location + LocalDirectionVector(1)) {
                    right = p;
                    rightFound = true;
                }
            }
            //if there are both left and right nodes, change color accordingly
            if (leftFound && rightFound) {
                //if left and right both have the same power type, this adapts that too
                if (left.powerType == right.powerType) {
                    SetState(left.powerType);
                } else if (left.powerType != PowerStates.RED && right.powerType != PowerStates.RED) { //if neither side is a color, do that color
                    SetState(PowerStates.RED);
                } else if (left.powerType != PowerStates.GREEN && right.powerType != PowerStates.GREEN) {
                    SetState(PowerStates.GREEN);
                } else if (left.powerType != PowerStates.BLUE && right.powerType != PowerStates.BLUE) {
                    SetState(PowerStates.BLUE);
                }
                //then supply power above and below equal to whatever this just got powered by
                Vector2Int target = location + LocalDirectionVector(0);
                if (gameRef.GetAtLocation(target.x, target.y) != gameRef.emptyPiece)
                    output.Add(new PowerTransfer() { sourceLocation = location, effectLocation = target, powerType = state });

                target = location + LocalDirectionVector(2);
                if (gameRef.GetAtLocation(target.x, target.y) != gameRef.emptyPiece)
                    output.Add(new PowerTransfer() { sourceLocation = location, effectLocation = target, powerType = state });
            }
            return output;
        }

        //if anything is providing power from above or below this node, it shorts
        public override bool DidShort() {
            bool hasBadSource = false;
            foreach (PowerTransfer p in sources) {
                if (p.sourceLocation == location + LocalDirectionVector(0) || p.sourceLocation == location + LocalDirectionVector(2))
                    hasBadSource = true;
            }
            return hasBadSource;
        }

    }

    //Connects power from the left/right side and power from the forward/back sides
    public class BridgeNode : Gamepiece{
        public BridgeNode(GameObject _inWorld, bool _locked, BlackieMiniGame2 gameManager, int x, int y, int dir) : base (_inWorld, _locked, gameManager, x, y, dir) {

        }
        public BridgeNode(GameObject _inWorld, bool _locked, BlackieMiniGame2 gameManager) : base(_inWorld, _locked, gameManager) {

        }

        public override List<PowerTransfer> TransferPower(PowerTransfer source) {
            sources.Add(source);
            List<PowerTransfer> output = new List<PowerTransfer>();
            for (int i = 0; i < 4; i++) {
                if (location + LocalDirectionVector(i) == source.sourceLocation && gameRef.GetAtLocation(location + LocalDirectionVector(i + 2)) != gameRef.emptyPiece) {
                    output.Add(new PowerTransfer() {    sourceLocation = location,
                                                        effectLocation = location + LocalDirectionVector(i + 2),
                                                        powerType = source.powerType});

                }
            }
            return output;
        }

        public override bool DidShort() {
            bool hasShort = false;
            for (int i = 0; i < sources.Count; i++) {
                Vector2Int direction = location - sources[i].sourceLocation;
                for (int j = i + 1; j < sources.Count; j++) {
                    //if the two nodes are on opposite sides of this node and both powering it
                    if (sources[j].sourceLocation + direction == location) {
                        hasShort = true;
                    }
                }
            }
            return hasShort;
        }
    }

    //source of power
    public class SourceNode : Gamepiece {
        //Source nodes have no direction
        public SourceNode(GameObject _inWorld, BlackieMiniGame2 gameManager, PowerStates _color, int x, int y) : base (_inWorld, true, gameManager, x, y, 0) {
            state = _color;
        }

        //powers everything next to it.
        public override List<PowerTransfer> TransferPower(PowerTransfer source) {
            if (source.sourceLocation != location)
                sources.Add(source);
            List<PowerTransfer> output = new List<PowerTransfer>();
            for (int i = 0; i < 4; i++) {
                if (gameRef.GetAtLocation(location + LocalDirectionVector(i)) != gameRef.emptyPiece) {
                    output.Add(new PowerTransfer() {    sourceLocation = location,
                                                        effectLocation = location + LocalDirectionVector(i),
                                                        powerType = state});
                }
            }
            return output;
        }

        //source node will recognize a short if anything is trying to power it.
        public override bool DidShort() {
            return sources.Count > 0;
        }

        //Cannot change the state of a source node
        public override void SetState(PowerStates newstate) {
        }

    }

    //goal node. Supply these with power of the right color to win
    public class GoalNode : Gamepiece {

        //the desired color for it to be
        public PowerStates powerColor;

        //goal nodes have no direction
        public GoalNode(GameObject _inWorld, BlackieMiniGame2 gameManager, PowerStates _color, int x, int y) : base (_inWorld, true, gameManager, x, y, 0) {
            powerColor = _color;
            state = PowerStates.OFF;
        }

        //powers nothing, but will be powered if by its own color
        public override List<PowerTransfer> TransferPower(PowerTransfer source) {
            if (source.powerType == powerColor)
                SetState(powerColor);
            return new List<PowerTransfer>();
        }
    }
}
