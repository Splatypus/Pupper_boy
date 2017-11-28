using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Icons { Afraid, Angry, Bird, Corgi, Exclamation, Happy, Pomeranian, Question, Sad, Squirrell, Terrier}
// need to get sprites that match up to these, probably in an array that we can index into with enum int val

public class IconManager : MonoBehaviour {

    [SerializeField]
    private GameObject singleIconBubble;
    [SerializeField]
    private Image singleIcon;

    [SerializeField]
    private GameObject doubleIconBubble;
    [SerializeField]
    private Image doubleIcon1;
    [SerializeField]
    private Image doubleIcon2;

    // Use this for initialization
    void Start () {
        

        singleIconBubble.SetActive(false);
        doubleIconBubble.SetActive(false);
    }
	
	public void set_single_icon(Icons icon)
    {

    }
}
