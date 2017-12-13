using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlMenuImageV2 : MonoBehaviour {

    public Image control_image;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            control_image.gameObject.SetActive(!control_image.gameObject.activeSelf);
        }
    }
}
