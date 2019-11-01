using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// there is a way to create this enum dynamically, but it looks questionable to me and I think it's better to hard code?
public enum Icons { Afraid, Angry, Bird, Corgi, Exclamation, Happy, Pomeranian, Question, Sad, Squirrell, Terrier, Dig, Flight}
// need to get sprites that match up to these, probably in an array that we can index into with enum int val

public class IconManager : MonoBehaviour {

    [SerializeField]
    private GameObject iconCanvas;
    [SerializeField]
    private Image singleIcon;

    // should probably use a sprite sheet but meh
    [SerializeField]
    List<Sprite> icon_sprites;

    // Use this for initialization
    void Start () {
        iconCanvas.SetActive(false);
    }
	
	public void set_single_icon(Icons icon)
    {
        singleIcon.sprite = icon_sprites[(int)icon];
    }

    public void set_single_bubble_active(bool isActive)
    {
        iconCanvas.SetActive(isActive);
    }
}
