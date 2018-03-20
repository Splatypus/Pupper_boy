using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameracontrol : MonoBehaviour {

    public GameObject Cam1;
    public GameObject Cam2;
   

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Digging()
    {
        Cam2.SetActive(true);
        Cam1.SetActive(false);
    }

    public void MainCam()
    {
        Cam1.SetActive(true);
        Cam2.SetActive(false);
    }
}
