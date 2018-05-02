using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocation : MonoBehaviour {

    Material m;
    GameObject player;

	// Use this for initialization
	void Start () {
        m = gameObject.GetComponent<MeshRenderer>().material;
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        m.SetVector("_PlayerLoc", player.transform.position);

    }
}
