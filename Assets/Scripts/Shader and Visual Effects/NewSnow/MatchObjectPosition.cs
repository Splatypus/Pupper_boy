using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * UNUSED: CONTENT ADDED TO SNOWPHYSICS CLASS
 * **/

//Matches the snow camera movement to the player, however, only moves the camera in pixel increments. 
public class MatchObjectPosition : MonoBehaviour {

    public GameObject player;
    private Vector3 initialPlayerPosition;

    private float worldSpaceToPixelMult;

	// Use this for initialization
	void Start () {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        Camera c = gameObject.GetComponent<Camera>();
        worldSpaceToPixelMult = (c.orthographicSize*2)/ c.targetTexture.width;
        initialPlayerPosition = player.transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 playerOffset = player.transform.position - initialPlayerPosition;
        transform.position = new Vector3(   player.transform.position.x - playerOffset.x % worldSpaceToPixelMult, 
                                            transform.position.y,
                                            player.transform.position.z - playerOffset.z % worldSpaceToPixelMult);
	}
}
