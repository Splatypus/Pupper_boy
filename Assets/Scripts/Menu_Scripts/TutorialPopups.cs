using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopups : MonoBehaviour {

    public GameObject popup;
    public Text text;

    public void EnableWithText(string t) {
        popup.SetActive(true);
        text.text = t;
    }

    public void OnDisable() {
        popup.SetActive(false);
    }
}
