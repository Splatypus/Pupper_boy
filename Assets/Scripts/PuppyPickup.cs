using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppyPickup : MonoBehaviour {

    private GameObject itemInMouth = null;                                      //reference to item currently in the dog's mouth
    private Vector3 prevPosition = new Vector3(0f, 0f, 0f);                     //when ball is let go, this is used to calculate it's momentum
    [SerializeField] private Transform mouth;                                   //location of the mouth to move items to

    private List<GameObject> objectsInRange = new List<GameObject>();           //objects in pickup range
    private BallLauncher launcherInRange = null;                                //ball launcher that is in range (if one exists)


	// Use this for initialization
	void Start () {
        //itemInMouth = null;
        //objectsInRange = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        //determine if E key has been pressed.  E key is used to pickup and drop objects
        if(Input.GetKeyDown(KeyCode.E))
        {
            //first, if there is an object in the dog's mouth, drop it or load it into a launcher
            if(itemInMouth != null)
            {
                //first check if we're in range of a ball launcher.  If so, load it
                if (launcherInRange && itemInMouth.name == "Tennis Ball")
                {
                    Debug.Log("Loading Ball");
                    launcherInRange.LoadBall(itemInMouth);
                }
                //Otherwise, drop it
                else
                {
                    itemInMouth.transform.parent = null;
                    itemInMouth.GetComponent<Rigidbody>().useGravity = true;
                    //calculate the momentum of the ball due to the dog's turning.
                    //If you don't it will always fall straight to the ground
                    itemInMouth.GetComponent<Rigidbody>().velocity = (itemInMouth.transform.position - prevPosition) / Time.deltaTime;
                }
                itemInMouth = null;
            }

            //otherwise, see if there are objects in range and pick up the closest one
            else if(objectsInRange.Count > 0)
            {
                itemInMouth = ClosestObject();
                itemInMouth.transform.parent = mouth;
                itemInMouth.transform.localPosition = new Vector3(0f, 0f, 0f);
                itemInMouth.GetComponent<Rigidbody>().useGravity = false;
            }
        }

        //update prevPosition
        if (itemInMouth != null)
            prevPosition = itemInMouth.transform.position;
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
