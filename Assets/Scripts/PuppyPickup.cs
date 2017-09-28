using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppyPickup : MonoBehaviour {

    private GameObject itemInMouth;                 //reference to item currently in the dog's mouth
    [SerializeField] private Transform mouth;       //location of the mouth to move items to

    private List<GameObject> objectsInRange;        //objects in pickup range


	// Use this for initialization
	void Start () {
        itemInMouth = null;
        objectsInRange = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        //determine if E key has been pressed.  E key is used to pickup and drop objects
        if(Input.GetKeyDown(KeyCode.E))
        {
            //first, if there is an object in the dog's mouth, drop it
            if(itemInMouth != null)
            {
                itemInMouth.transform.parent = null;
                itemInMouth.GetComponent<Rigidbody>().useGravity = true;
                itemInMouth = null;
            }
            //otherwise, see if there are objects in range adn pick up the closest one
            else if(objectsInRange.Count > 0)
            {
                itemInMouth = ClosestObject();
                itemInMouth.transform.parent = mouth;
                itemInMouth.transform.localPosition = new Vector3(0f, 0f, 0f);
                itemInMouth.GetComponent<Rigidbody>().useGravity = false;
            }
        }
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
        if (other.tag == "Pickup" && !objectsInRange.Contains(other.gameObject))
        {
            Debug.Log("Object In");
            objectsInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pickup" && objectsInRange.Contains(other.gameObject))
        {
            Debug.Log("Object Out");
            objectsInRange.Remove(other.gameObject);
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
