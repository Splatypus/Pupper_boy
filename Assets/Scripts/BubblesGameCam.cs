using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblesGameCam : MonoBehaviour {

    public GameObject playerCam;
    public GameObject gameCam;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerCharacter")
        {
            playerCam.SetActive(false);
            gameCam.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "PlayerCharacter")
        {
            playerCam.SetActive(true);
            gameCam.SetActive(false);
        }

    }
}
