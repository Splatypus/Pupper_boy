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
        isshown = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (isshown == false)
        {
            
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                text.text = "WASD: Move\nE: Bark/Pickup/Drop\nQ: Chase Tail\nSpace: jump\nF: ???";
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
