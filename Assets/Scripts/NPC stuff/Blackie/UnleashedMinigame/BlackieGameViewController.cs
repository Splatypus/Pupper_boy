using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlackieGameViewController : AIbase, BlackieGameBoard.IListener 
{
    [Header("References")]
    public ChipAI chip;

    [Header("Game Files")]
    public TextAsset[] files;

    [Header("Prefabs")]
    public GameObject basePrefab;
    public GameObject blankTilePrefab;
    public GameObject sourceTilePrefab;
    public GameObject endTilePrefab;
    public GameObject lineTilePrefab;
    public GameObject elbowTilePrefab;
    public GameObject tTilePrefab;
    public GameObject crossTilePrefab;
    public GameObject bridgeTilePrefab;
    public GameObject[] stonePrefabs;

    [Header("Materials")]
    public PowerColor[] powerColors;
    [SerializeField] Material baseDefault;
    [SerializeField] Material baseImmobile;

    [Header("Build Variables")]
    public float baseDistance;
    public float pieceHeight;
    [Range(0, 1)] public float stoneSpawnChance;

    [HideInInspector] public BlackieGameBoard game { get; private set; }

    //runtime references
    private List<GameObject> bases;
    private List<GameObject> pieces;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        game = new BlackieGameBoard(this);
    }

    #region listener functions
    public void OnVictory() {
        print("You win!");
        foreach (GameObject g in bases) {
            Destroy(g);
        }
        foreach (GameObject g in pieces) {
            Destroy(g);
        }
        chip.FinishGame();
        StartCoroutine(StartDialogNextFrame()); //Since destroying a draggable returns you to walking, this has to be called on the next frame... for whatever reason.
    }
    public void OnFileLoaded() {
        GenerateBase();
    }
    #endregion

    public void LoadFile(int index) {
        //clear bases
        if (bases != null) {
            for (int i = 0; i < bases.Count; i++) {
                Destroy(bases[i]);
            }
        }
        bases = new List<GameObject>();
        //clear pieces
        if (pieces != null) {
            for (int i = 0; i < pieces.Count; i++) {
                Destroy(pieces[i]);
            }
        }
        pieces = new List<GameObject>();

        game.LoadBoard(files[index]);
    }

    //draws the base tiles
    void GenerateBase() {
        bases.Clear();
        pieces.Clear();
        for (int x = 0; x < game.GetWidth(); x++) {
            for (int y = 0; y < game.GetHeight(); y++) {
                GenerateBoardSpace(x, y);
            }
        }
    }

    void GenerateBoardSpace(int x, int y) {
        if (game.GetSpace(x, y) != null) {
            //instantiate it in its spot, as a child to this gameObject, accounting for an x offset so that this gameobject is placed in the center of the x axis
            Vector3 placePosition = new Vector3(
                                            baseDistance * (x - (game.GetWidth() - 1.0f) / 2.0f),
                                            0,
                                            baseDistance * (y + 1.5f)
                                            );
            placePosition = transform.rotation * placePosition;
            placePosition += transform.position;

            bases.Add(Instantiate(basePrefab, placePosition, transform.rotation, transform));

            //then make a tile on top of it, if one is supposed to be here
            BlackieGameBoard.Piece p = game.GetPiece(x, y);
            if (p != null) {
                GameObject worldPiece;
                Quaternion rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + p.GetRotation(), 0);
                switch (p) {
                    case BlackieGameBoard.SourcePiece sp:
                        worldPiece = sourceTilePrefab;
                        break;
                    case BlackieGameBoard.EndPiece endp:
                        worldPiece = endTilePrefab;
                        break;
                    case BlackieGameBoard.LinePiece linep:
                        worldPiece = lineTilePrefab;
                        break;
                    case BlackieGameBoard.ElbowPiece elp:
                        worldPiece = elbowTilePrefab;
                        break;
                    case BlackieGameBoard.TPiece tp:
                        worldPiece = tTilePrefab;
                        break;
                    case BlackieGameBoard.CrossPiece cp:
                        worldPiece = crossTilePrefab;
                        break;
                    case BlackieGameBoard.BridgePiece bp:
                        worldPiece = bridgeTilePrefab;
                        break;
                    default:  //default assume blank piece
                        worldPiece = blankTilePrefab;
                        break;
                }
                //create this piece with its starting rotation
                worldPiece = Instantiate(worldPiece, placePosition + new Vector3(0, pieceHeight, 0), rotation, transform);
                GamePieceView pieceView = worldPiece.GetComponent<GamePieceView>();
                //if its locked, change its base material to a darker mat
                if (p.isLocked)
                    pieceView.baseMesh.material = baseImmobile;
                //then have a chance to add a set of stones at a random rotation
                if (Random.Range(0.0f, 1.0f) <= stoneSpawnChance) {
                    GameObject stone = Instantiate( stonePrefabs[Random.Range(0, stonePrefabs.Length - 1)], 
                                                    worldPiece.transform.position, 
                                                    worldPiece.transform.rotation * Quaternion.Euler(0, 90 * Random.Range(0,3), 0),
                                                    worldPiece.transform
                                                    );
                    pieceView.SetStone(stone);
                }


                //assign it for callbacks
                pieceView.boardView = this;
                pieceView.AttachModel(p);
                //add to list
                pieces.Add(worldPiece);
            }
        }
    }

    //all the information regarding a color of power flow
    [Serializable]
    public class PowerColor {
        public Color minColor;
        public Color maxColor;
        public Material material;
    }

    IEnumerator StartDialogNextFrame() {
        yield return new WaitForEndOfFrame();
        Debug.Log("Chip talking");
        chip.OnInteract();
    }
}
