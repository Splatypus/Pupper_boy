using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackieGameViewController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject basePrefab;
    public GameObject blankTilePrefab;
    public GameObject lineTilePrefab;
    public GameObject elbowTilePrefab;
    public GameObject TTilePrefab;
    public GameObject crpssTilePrefab;
    public GameObject bridgeTilePrefab;
    public GameObject[] stonePrefabs;

    private BlackieGameBoard game;

    // Start is called before the first frame update
    void Start()
    {
        game = new BlackieGameBoard();
    }

    void GenerateBase() {

    }
}
