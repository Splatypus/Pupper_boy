using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class restartLevel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.P) && Input.GetKeyDown(KeyCode.LeftControl))
        {
            SceneManager.LoadScene("Backyard");
        }
        if (Input.GetKeyDown(KeyCode.Escape) && Input.GetKeyDown(KeyCode.LeftControl))
        {
            SceneManager.LoadScene("title_screen");
        }
	}
}
