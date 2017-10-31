using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppyPickup : MonoBehaviour {

    public int num_food_for_poop = 2;
    public float time_for_poop = 10.0f;
    public GameObject poop_obj;

    public int num_food_for_memes = 10;
    int m_num_food = 0;

    private GameObject itemInMouth = null;                                      //reference to item currently in the dog's mouth
    private Vector3 prevPosition = new Vector3(0f, 0f, 0f);                     //when ball is let go, this is used to calculate it's momentum
    [SerializeField] private Transform mouth;                                   //location of the mouth to move items to
    [SerializeField] private Transform butt;                                    //ass
    [SerializeField] private AudioClip[] borks;
    [SerializeField] private AudioClip[] poops;
    private AudioSource m_audio_source;
    private int bork_index = 0;

    private List<GameObject> objectsInRange = new List<GameObject>();           //objects in pickup range
    private BallLauncher launcherInRange = null;                                //ball launcher that is in range (if one exists)
    private ToyBox boxInRange = null;                                           //toy box that is in range (if one exists)
    private FoodDispenser foodInRange = null;

	// Use this for initialization
	void Start () {
        m_audio_source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        //determine if E key has been pressed.  E key is used to pickup and drop objects
        if(Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log("got e");
            //first, if there is an object in the dog's mouth, drop it or load it into a launcher
            if(itemInMouth != null)
            {
                //first check if we're in range of a ball launcher.  If so, load it
                if (launcherInRange && itemInMouth.name == "Tennis Ball")
                {
                    Debug.Log("Loading Ball");
                    launcherInRange.LoadBall(itemInMouth);
                }
                //Next, check if we're in range of the toy box.  If so, toss it in.
                else if(boxInRange && itemInMouth)
                {
                    Debug.Log("Toss in box");
                    boxInRange.TossInBox(itemInMouth);
                }
                //Otherwise, drop it
                else
                {
                    itemInMouth.transform.parent = null;
                    itemInMouth.GetComponent<Rigidbody>().useGravity = true;
                    //calculate the momentum of the ball due to the dog's turning.
                    //If you don't it will always fall straight to the ground
                    itemInMouth.GetComponent<Rigidbody>().velocity = (itemInMouth.transform.position - prevPosition) / Time.deltaTime;

                    Interactable interactable = itemInMouth.GetComponent<Interactable>();
                    if (interactable)
                    {
                        interactable.onDrop();
                    }
                }
                itemInMouth = null;
            }
            else if (foodInRange != null && foodInRange.CanEat())
            {
                foodInRange.EatFood();

                // eat some goddamn food
                m_num_food++;
                if (m_num_food >= num_food_for_memes)
                {
                    //Debug.Log("ENTER MEME ZONE!!!!");
                    FlightMode flight = FindObjectOfType<FlightMode>();
                    flight.can_fly = true;
                }
                if (m_num_food % num_food_for_poop == 0)
                {
                    Invoke("poop", time_for_poop);
                }
                
            }
            //otherwise, see if there are objects in range and pick up the closest one
            else if(objectsInRange.Count > 0)
            {

                itemInMouth = ClosestObject();
                itemInMouth.transform.parent = mouth;
                itemInMouth.transform.localPosition = new Vector3(0f, 0f, 0f);
                itemInMouth.GetComponent<Rigidbody>().useGravity = false;

                // do stuff with interactable object
                Interactable interactable = itemInMouth.GetComponent<Interactable>();
                if(interactable)
                {
                    interactable.onPickup();
                }
            }
            else
            {
                int prev_index = bork_index;
                // bork
                bork_index = Random.Range(0, borks.Length);
                
                while (bork_index == prev_index)
                {
                    bork_index = Random.Range(0, borks.Length);
                }
                
                m_audio_source.clip = borks[bork_index];
                m_audio_source.Play();
            }
        }

        //update prevPosition
        if (itemInMouth != null)
            prevPosition = itemInMouth.transform.position;
	}

    public void poop()
    {
        //Debug.Log("poop!");

        Instantiate(poop_obj, butt.transform.position, butt.transform.rotation);
        int i= Random.Range(0, poops.Length);
        m_audio_source.clip = poops[i];
        m_audio_source.Play();
    }

    private void LateUpdate()
    {
        if(itemInMouth != null)
        {
            itemInMouth.transform.rotation = mouth.rotation;
            itemInMouth.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
            
    }




    void OnTriggerEnter(Collider other)
    {
        //if it's a pickup item, add it to the list
        if (other.tag == "Pickup" && !objectsInRange.Contains(other.gameObject))
        {
            Debug.Log("Object In");
            objectsInRange.Add(other.gameObject);
        }
        //if it's a ball launcher, store it in launcherInRange
        else if (other.tag == "Launcher")
        {
            Debug.Log("Launcher in range");
            launcherInRange = other.GetComponent<BallLauncher>();
        }
        //if it's a toy box, store it in boxInRange
        else if (other.tag == "Box")
        {
            Debug.Log("Box In Range");
            boxInRange = other.GetComponent<ToyBox>();
        }
        else if (other.tag == "Food")
        {
            foodInRange = other.GetComponent<FoodDispenser>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if it's a pickup item, remove it from the list
        if (other.tag == "Pickup" && objectsInRange.Contains(other.gameObject))
        {
            Debug.Log("Object Out");
            objectsInRange.Remove(other.gameObject);
        }
        //if it's a ball launcher, set launcherInRange to null
        else if (other.tag == "Launcher")
        {
            Debug.Log("Launcher out of range");
            launcherInRange = null;
		} 
        //if it's a toy box, set boxInRange to null
		else if (other.tag == "Box")
		{
			Debug.Log("Box out of range");
			boxInRange = null;
		}
        else if (other.tag == "Food")
        {
            foodInRange = null;
        }
    }




    GameObject ClosestObject()
    {
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        foreach(GameObject go in objectsInRange)
        {
            float dist = (mouth.position - go.transform.position).magnitude;
            if (dist < minDist)
            {
                closest = go;
                minDist = dist;
            }
        }

        return closest;
    }

}
