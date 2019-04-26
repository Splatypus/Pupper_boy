using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppyPickup : MonoBehaviour {

    [Header("References")]
    [SerializeField] public Transform mouth;                                   //location of the mouth to move items to
    [SerializeField] private Transform butt;                                    //ass

    [Header("Poopin")]
    public int num_food_for_poop = 2;
    public float time_for_poop = 10.0f;
    public GameObject poop_obj;

    [Header("Object Pickup")]
    public GameObject castOrigin;
    public LayerMask mask;
    public float radius;
    [Range(-1,1)] public float minimumDotProductToFocus;
    private GameObject focusedItem;

    [HideInInspector] public GameObject itemInMouth = null;                                      //reference to item currently in the dog's mouth
    private Vector3 prevPosition = new Vector3(0f, 0f, 0f);                     //when ball is let go, this is used to calculate it's momentum
    [SerializeField] private AudioClip[] borks;
    [SerializeField] private AudioClip[] poops;
    private AudioSource m_audio_source;

    private IconManager iconManager;


    // Use this for initialization
    void Start() {
        m_audio_source = GetComponent<AudioSource>();
        iconManager = GetComponentInChildren<IconManager>();
    }

    // Update is called once per frame
    void Update() {
        //update prevPosition
        if (itemInMouth != null) {
            prevPosition = itemInMouth.transform.position;
        } else {
            CheckForItems();
        }
    }

    //casts a sphere to see if there are any items to pick up or push
    void CheckForItems() {
        //Consider using an overlap sphere centered on doggo instead to get a list of all nearby objects, then find the one with the bbest dot product with camera vector
        Collider[] cols = Physics.OverlapSphere(castOrigin.transform.position, radius, mask);
        float bestDot = -1;
        Collider bestCol = null;
        //find the collider within our overlapsphere which has the best dot product with the cameras forward vector (ie, the one we are looking at the most)
        foreach (Collider c in cols) {
            float cameraDotProduct = Vector3.Dot((c.transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward);
            if (cameraDotProduct > bestDot && cameraDotProduct > minimumDotProductToFocus) {
                bestDot = cameraDotProduct;
                bestCol = c;
            }
        }
        //if were now looking at a different object, change focus
        if (focusedItem != bestCol?.gameObject) {
            //if an item was focued, defocus it
            if (focusedItem != null) {
                focusedItem.GetComponent<IPickupItem>()?.OnDefocus();
            }
            //assign the new focused item
            focusedItem = bestCol?.gameObject;
            if (focusedItem != null) {
                focusedItem.GetComponent<IPickupItem>()?.OnFocus();
            }
        }
    }

    //called when the picup button is pressed
    public void DoInputAction() {
        //first, if there is an object in the dog's mouth, drop it
        if (itemInMouth != null) {
            DropItem();
        } 
        //otherwise, notify the focused item that were trying to pick it up
        else if (focusedItem != null) {
            // do stuff with interactable object
            IPickupItem interactable = focusedItem.GetComponent<IPickupItem>();
            interactable?.OnPickup(this);
            focusedItem.GetComponent<IPickupItem>()?.OnDefocus();
            focusedItem = null;

        } else {
            //play a random bork audio clip
            m_audio_source.clip = borks[Random.Range(0, borks.Length)];
            m_audio_source.Play();

            EventManager.Instance.TriggerOnBark(GameObject.FindGameObjectWithTag("Player"));
        }
    }

    private void Dissable_bubble_icon() {
        iconManager.set_single_bubble_active(false);
    }

    //drops the current item
    public void DropItem() {
        itemInMouth.transform.parent = null;
        itemInMouth.GetComponent<IPickupItem>()?.OnDrop((itemInMouth.transform.position - prevPosition) / Time.deltaTime);
        itemInMouth = null;
    }

    public void Poop() {
        //Debug.Log("poop!");

        Instantiate(poop_obj, butt.transform.position, butt.transform.rotation);
        int i = Random.Range(0, poops.Length);
        m_audio_source.clip = poops[i];
        m_audio_source.Play();
    }


    //interface for items that can be picked up
    public interface IPickupItem {

        void OnFocus();
        void OnDefocus();

        void OnPickup(PuppyPickup source);
        void OnDrop(Vector3 currentVelocity);

    }

}
