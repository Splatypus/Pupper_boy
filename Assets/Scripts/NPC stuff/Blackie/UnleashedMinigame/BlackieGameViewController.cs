﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackieGameViewController : Dialog2, BlackieGameBoard.IListener 
{

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
    public Material[] powerColors;
    public Material baseDefault;
    public Material baseImmobile;

    [Header("Build Variables")]
    public float baseDistance;
    public float pieceHeight;

    private BlackieGameBoard game;

    //runtime references
    private List<GameObject> bases;
    private List<GameObject> pieces;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        game = new BlackieGameBoard(this);
        bases = new List<GameObject>();
        pieces = new List<GameObject>();
    }

    #region listener functions
    public void OnVictory() {

    }
    public void OnFileLoaded() {
        GenerateBase();
    }
    #endregion

    public void LoadFile(int index) {
        game.LoadBoard(files[index]);
    }

    //draws the base tiles
    void GenerateBase() {
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
                                            transform.position.x + baseDistance * (x - (game.GetWidth() - 1.0f) / 2.0f),
                                            transform.position.y,
                                            transform.position.z + baseDistance * (game.GetHeight() - y + 0.5f)
                                            );

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
                worldPiece = Instantiate(worldPiece, placePosition + new Vector3(0, pieceHeight, 0), rotation, transform);
                pieces.Add(worldPiece);
                //TODO: apply rotation and immobile texture kinda thing
            }
        }
    }
}