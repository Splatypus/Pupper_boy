using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//UNUSED: See TutorialManager/EventManager for new tutorial system

public class TutorialPopups : MonoBehaviour {

    public GameObject popup;
    public Text text;
    public bool digTutorial;
    public string digString;
    public bool talkTutorial;
    public string talkString;
    public enum TutorialTypes {DIG, TALK, CLOSED};
    public TutorialTypes state = TutorialTypes.CLOSED;

    public void EnableWithText(string t) {
        popup.SetActive(true);
        text.text = t;
    }

    public void DisableTutorial() {
        popup.SetActive(false);
        state = TutorialTypes.CLOSED;
    }


    public void DoDigTutorial() {
        if (digTutorial) {
            state = TutorialTypes.DIG;
            EnableWithText(digString);
        }
    }

    public void DoTalkTutorial() {
        if (talkTutorial) {
            state = TutorialTypes.TALK;
            EnableWithText(talkString);
        }
    }

    public void CompleteTutorial() {
       
        switch (state) {
            case TutorialTypes.DIG:
                digTutorial = false;
                break;
            case TutorialTypes.TALK:
                talkTutorial = false;
                break;
            default:
                break;
        }

        DisableTutorial();
    }
}
