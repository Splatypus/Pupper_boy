using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlMenu : MonoBehaviour {
    bool isshown = false;
    Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        isshown = true;
        text.text = "WASD: Move\nMouse: Turn Camera\nE: Bark/Pickup/Drop\nQ: Dig\nShift: Sprint\nF: ???";
    }
	
	// Update is called once per frame
	void Update () {
        if (isshown == false)
        {
            
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                text.text = "WASD: Move\nMouse: Turn Camera\nE: Bark/Pickup/Drop\nQ: Dig\nShift: Sprint\nF: ???";
                isshown = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                text.text = "";
                isshown = false;
            }
        }
            
	}
}
