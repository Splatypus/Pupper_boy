using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

    public static TutorialManager instance;

    public GameObject popup;
    Text text;

    public void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    public void Start() {
        text = popup.GetComponentInChildren<Text>();
    }

    public void EnableWithText(string t) {
        popup.SetActive(true);
        text.text = t;
    }

    public void DisableTutorial() {
        popup.SetActive(false);
    }

}
