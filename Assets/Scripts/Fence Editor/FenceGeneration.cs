using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FenceGeneration : MonoBehaviour {

    #region presets
    public GameObject postObject;
    public GameObject horizontalObject; //objects that are stretched horizontally between posts
    public float lowHeight; //lowest location of a horizontal object
    public float highHeight; //highest
    public int horizontalNum; //number of horizontal objects to split between lowest and highest points
    public GameObject vertObject; //object that is parallel to posts
    public int vertNum; //number to space evenly between posts
    public GameObject partStorage;
    #endregion

    public GameObject sourcePost;

    [SerializeField]
    List<GameObject> linkedPosts = new List<GameObject>();
    [SerializeField]
    List<GameObject> horizontals = new List<GameObject>();
    [SerializeField]
    List<GameObject> verticals = new List<GameObject>();

    // Use this for initialization
    void Start () {
        //linkedPosts = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        //update each connected post so that things are spread evenly like they should be
        bool needToClearNulls = false;
        foreach (GameObject p in linkedPosts) {
            if(p != null) {
                FenceGeneration f = p.GetComponent<FenceGeneration>();
                //fist fix any issues with adjusting the number of parts
                FixList(f.horizontals, f.horizontalNum, f.horizontalObject);
                FixList(f.verticals, f.vertNum, f.vertObject);

                //do horizontal objects
                for (int i = 0; i < f.horizontalNum; i++) {
                    //set them to be in the center of the posts, distributed evenly in height
                    Vector3 newloc = new Vector3(   Mathf.Lerp(transform.position.x, p.transform.position.x, 0.5f), 
                                                    Mathf.Lerp(transform.position.y, p.transform.position.y, 0.5f), 
                                                    Mathf.Lerp(transform.position.z, p.transform.position.z, 0.5f));
                    float lerpT = 0;
                    if (f.horizontalNum == 1)
                        lerpT = 0.5f;
                    else
                        lerpT = (float)i / (float)(f.horizontalNum - 1);
                    newloc.y += Mathf.Lerp(f.lowHeight, f.highHeight, lerpT);
                    Transform currentTrans = f.horizontals[i].transform;
                    currentTrans.position = newloc;
                    //set rotation to look from one post to the other
                    currentTrans.rotation = Quaternion.LookRotation(p.transform.position - transform.position);
                    //set scale so they connect well
                    float scaleAmount = Vector3.Distance(transform.position, p.transform.position)/(horizontalObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents.z * 2);
                    currentTrans.localScale = new Vector3(currentTrans.localScale.x, currentTrans.localScale.y, scaleAmount/130);
                }

                //Next do vertical objects
                for (int i = 0; i < f.vertNum; i++) {
                    Vector3 newLoc = new Vector3(   Mathf.Lerp(transform.position.x, p.transform.position.x, (float)(i + 1) / (float)(f.vertNum + 1)),
                                                    Mathf.Lerp(transform.position.y, p.transform.position.y, (float)(i + 1) / (float)(f.vertNum + 1)),
                                                    Mathf.Lerp(transform.position.z, p.transform.position.z, (float)(i + 1) / (float)(f.vertNum + 1)));
                    f.verticals[i].transform.position = newLoc;
                    //set rotation to look from one post to the other
                    Vector3 lookVector = new Vector3(p.transform.position.x - transform.position.x, 0.0f, p.transform.position.z - transform.position.z);
                    f.verticals[i].transform.rotation = Quaternion.LookRotation(lookVector, Vector3.up);
                }
            } else {
                needToClearNulls = true;
            }
        }
        //if posts have been improperly delted
        if (needToClearNulls) {
            List<GameObject> newLinkedPosts = new List<GameObject>(linkedPosts.Count - 1);
            foreach (GameObject p in linkedPosts) {
                if (p != null) {
                    newLinkedPosts.Add(p);
                }
            }
            linkedPosts = newLinkedPosts;
        }
	}

    //adds a new post slightly offset
    public void AddPost() {
        GameObject temp = Instantiate(postObject, transform.position + new Vector3(1.0f, 0, 1.0f), transform.rotation, transform.parent);
        linkedPosts.Add(temp);
        temp.GetComponent<FenceGeneration>().OnCreate(gameObject, partStorage, postObject, lowHeight, highHeight, horizontalNum, vertNum);
        UnityEditor.Selection.activeTransform = temp.transform;
    }

    //Remove this post and clean everything up
    public void Remove() {
        ClearParts();
        if (sourcePost != null) {
            sourcePost.GetComponent<FenceGeneration>().linkedPosts.Remove(gameObject);
        }
        foreach (GameObject p in linkedPosts) {
            p.GetComponent<FenceGeneration>().sourcePost = null;
            p.GetComponent<FenceGeneration>().ClearParts();
        }
        DestroyImmediate(gameObject);
    }

    //removes all parts connected to this post. If it needs them, part will be reInstantiated.
    public void ClearParts() {
        foreach (GameObject h in horizontals){
            DestroyImmediate(h);
        }
        foreach (GameObject v in verticals){
            DestroyImmediate(v);
        }
    }

    //If you have a list of some object, and a target number of items, it either removes until it has the target num, or adds fillObject onto the end until it has target num
    public void FixList(List<GameObject> l, int targetNum, GameObject fillObject) {
        if (l.Count > targetNum) {
            //destroy and remove extra
            for (int i = targetNum; i < l.Count; i++) {
                DestroyImmediate(l[i]);
            }
            l.RemoveRange(targetNum, l.Count - targetNum);
        } else if (l.Count < targetNum) {
            //add in the difference
            for (int i = l.Count; i < targetNum; i++) {
                GameObject temp = Instantiate(fillObject, partStorage.transform);
                l.Add(temp);
            }
        }
    }

    //called when this is created
    public void OnCreate(GameObject selfReference, GameObject parts, GameObject postRef, float lh, float hh, int hnum, int vnum) {
        sourcePost = selfReference;
        partStorage = parts;
        postObject = postRef;
        highHeight = hh;
        lowHeight = lh;
        horizontalNum = hnum;
        vertNum = vnum;
        linkedPosts = new List<GameObject>();
        verticals = new List<GameObject>();
        horizontals = new List<GameObject>();
    }
}
