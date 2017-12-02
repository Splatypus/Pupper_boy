using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// there is a way to create this enum dynamically, but it looks questionable to me and I think it's better to hard code?
public enum Icons { Afraid, Angry, Bird, Corgi, Exclamation, Happy, Pomeranian, Question, Sad, Squirrell, Terrier, Dig}
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

    // should probably use a sprite sheet but meh
    [SerializeField]
    List<Sprite> icon_sprites;
    
    // TODO: make the in-between stuff work in here
    // TODO: make the negative overlay work on top of any icon

    // Use this for initialization
    void Start () {
        

        singleIconBubble.SetActive(false);
        doubleIconBubble.SetActive(false);
    }
	
	public void set_single_icon(Icons icon)
    {
        singleIcon.sprite = icon_sprites[(int)icon];
    }

    public void set_double_icon_first(Icons icon)
    {
        doubleIcon1.sprite = icon_sprites[(int)icon];
    }

    public void set_double_icon_second(Icons icon)
    {
        doubleIcon2.sprite = icon_sprites[(int)icon];
    }

    public void set_single_bubble_active(bool isActive)
    {
        singleIconBubble.SetActive(isActive);
    }

    public void set_double_bubble_active(bool isActive)
    {
        doubleIconBubble.SetActive(isActive);
    }
}
