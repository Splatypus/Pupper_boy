using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameracontrol : MonoBehaviour {

    public GameObject Cam1;
    public GameObject Cam2;
    public bool isCam1 = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isCam1 ==true)
        {
            Cam1.SetActive(true);
            Cam2.SetActive(false);
        }
        else
        {
            Cam2.SetActive(true);
            Cam1.SetActive(false);
        }
	}
}
