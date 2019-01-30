#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FenceGeneration : MonoBehaviour {

    #region variables
    [Header("Horizontal Objects")]
    public GameObject horizontalObject; //objects that are stretched horizontally between posts
    public float lowHeight; //lowest location of a horizontal object
    public float highHeight; //highest
    public float horizontalOffset = 0.15f;
    public int horizontalNum; //number of horizontal objects to split between lowest and highest points

    [Header("Vertical Objects")]
    public GameObject vertObject; //object that is parallel to posts
    public int vertNum; //number to space evenly between posts
    public float vertOffset = -0.15f;
    
    [Header("Collision")]
    public GameObject colliderObject;
    public GameObject firstDigZone;
    public GameObject secondDigzone;
    public bool doesGenerateDigZones = true;
    public float firstOffset, secondOffset;
    public DigZone.Yards firstYard;
    public DigZone.Yards secondYard;
    //public string firstName = "";
    //public string secondName = "";

    [Header("Preset Objects")]
    public GameObject postObject;
    public GameObject partStorage;
    [SerializeField]
    public GameObject sourcePost;
    [SerializeField]
    List<GameObject> linkedPosts;
    [SerializeField]
    List<GameObject> horizontals;
    [SerializeField]
    List<GameObject> verticals;

    #endregion


    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (!Application.isPlaying) {

            //update each connected post so that things are spread evenly like they should be
            bool needToClearNulls = false;
            foreach (GameObject p in linkedPosts) {
                if (p != null) {
                    FenceGeneration f = p.GetComponent<FenceGeneration>();
                    //fist fix any issues with adjusting the number of parts
                    if (f.horizontals.Count != f.horizontalNum)
                        FixList(f.horizontals, f.horizontalNum, f.horizontalObject);
                    if (f.verticals.Count != f.vertNum)
                        FixList(f.verticals, f.vertNum, f.vertObject);

                    //do horizontal objects
                    for (int i = 0; i < f.horizontalNum; i++) {
                        //set them to be in the center of the posts, distributed evenly in height
                        Vector3 newloc = new Vector3(Mathf.Lerp(transform.position.x, p.transform.position.x, 0.5f),
                                                        Mathf.Lerp(transform.position.y, p.transform.position.y, 0.5f),
                                                        Mathf.Lerp(transform.position.z, p.transform.position.z, 0.5f));
                        newloc += Vector3.Cross(transform.position - p.transform.position, Vector3.up).normalized * f.horizontalOffset; //move the location to the side, based on the offset value
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
                        float scaleAmount = Vector3.Distance(transform.position, p.transform.position) / (horizontalObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents.z * 2);
                        currentTrans.localScale = new Vector3(currentTrans.localScale.x, currentTrans.localScale.y, scaleAmount / 100);
                    }

                    //Next do vertical objects
                    for (int i = 0; i < f.vertNum; i++) {
                        Vector3 newLoc = new Vector3(Mathf.Lerp(transform.position.x, p.transform.position.x, (float)(i + 1) / (float)(f.vertNum + 1)), //new location splits them evenly between posts
                                                        Mathf.Lerp(transform.position.y, p.transform.position.y, (float)(i + 1) / (float)(f.vertNum + 1)),
                                                        Mathf.Lerp(transform.position.z, p.transform.position.z, (float)(i + 1) / (float)(f.vertNum + 1)));
                        newLoc += Vector3.Cross(transform.position - p.transform.position, Vector3.up).normalized * f.vertOffset; //move the location to the side, based on the offset value
                        f.verticals[i].transform.position = newLoc;
                        //set rotation to look from one post to the other
                        Vector3 lookVector = new Vector3(p.transform.position.x - transform.position.x, 0.0f, p.transform.position.z - transform.position.z);
                        f.verticals[i].transform.rotation = Quaternion.LookRotation(lookVector, Vector3.up);
                    }

                    //Adjust the fence collider
                    f.colliderObject.transform.position = (transform.position + p.transform.position) / 2.0f; //set to the center of the two posts
                    f.colliderObject.transform.rotation = Quaternion.LookRotation(p.transform.position - transform.position); //look from one to the other
                    BoxCollider col = f.colliderObject.GetComponent<BoxCollider>();
                    col.size = new Vector3(col.size.x, col.size.y, Vector3.Distance(transform.position, p.transform.position) / 100);

                    //Adjust Dig Zone colliders
                    f.firstDigZone.SetActive(f.doesGenerateDigZones);
                    f.secondDigzone.SetActive(f.doesGenerateDigZones);
                    if (f.doesGenerateDigZones) {
                        //first dig zone
                        Vector3 newLoc = (transform.position + p.transform.position) / 2.0f; //set to the center of the two posts
                        newLoc += Vector3.Cross(transform.position - p.transform.position, Vector3.up).normalized * f.firstOffset;
                        f.firstDigZone.transform.position = newLoc;
                        f.firstDigZone.transform.rotation = Quaternion.LookRotation(p.transform.position - transform.position); //look from one to the other
                        col = f.firstDigZone.GetComponent<BoxCollider>();
                        col.size = new Vector3(col.size.x, col.size.y, Vector3.Distance(transform.position, p.transform.position) / 100);

                        //second dig zone
                        newLoc = (transform.position + p.transform.position) / 2.0f; //set to the center of the two posts
                        newLoc += Vector3.Cross(transform.position - p.transform.position, Vector3.up).normalized * f.secondOffset;
                        f.secondDigzone.transform.position = newLoc;
                        f.secondDigzone.transform.rotation = Quaternion.LookRotation(p.transform.position - transform.position); //look from one to the other
                        col = f.secondDigzone.GetComponent<BoxCollider>();
                        col.size = new Vector3(col.size.x, col.size.y, Vector3.Distance(transform.position, p.transform.position) / 100);

                        //then assign digzone names
                        f.firstDigZone.GetComponent<DigZone>().enteringYard = f.firstYard;
                        f.secondDigzone.GetComponent<DigZone>().enteringYard = f.secondYard;
                    }



                } else {
                    needToClearNulls = true;
                }
            }
            //if posts have been improperly delted
            if (needToClearNulls) {
                print("Cleaning up improperly deleted fencepost assests");
                List<GameObject> newLinkedPosts = new List<GameObject>(linkedPosts.Count - 1);
                foreach (GameObject p in linkedPosts) {
                    if (p != null) {
                        newLinkedPosts.Add(p);
                    }
                }
                linkedPosts = newLinkedPosts;
            }
        }
	}

    //adds a new post
    public void AddPost() {
        GameObject temp = Instantiate(postObject, transform.position /*+ new Vector3(1.0f, 0, 1.0f)*/, transform.rotation, transform.parent);
        linkedPosts.Add(temp);
        UnityEditor.PrefabUtility.DisconnectPrefabInstance(gameObject);
        temp.GetComponent<FenceGeneration>().OnCreate(gameObject, partStorage, postObject, lowHeight, highHeight,horizontalOffset, vertOffset, horizontalNum, vertNum, doesGenerateDigZones, firstOffset, secondOffset, firstYard, secondYard);
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

        //colliders dont need to be reset if the gameobject is deleted
        /*//reset box collider
        colliderObject.transform.position = transform.position;
        colliderObject.transform.rotation = transform.rotation;
        BoxCollider col = colliderObject.GetComponent<BoxCollider>();
        col.size = new Vector3(col.size.x, col.size.y, 0.0f);

        //and reset digzone colliders
        firstDigZone.transform.position = transform.position;
        firstDigZone.transform.rotation = transform.rotation;
        col = firstDigZone.GetComponent<BoxCollider>();
        col.size = new Vector3(col.size.x, col.size.y, 0.0f);

        secondDigzone.transform.position = transform.position;
        secondDigzone.transform.rotation = transform.rotation;
        col = secondDigzone.GetComponent<BoxCollider>();
        col.size = new Vector3(col.size.x, col.size.y, 0.0f);*/
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
    public void OnCreate(GameObject selfReference, GameObject parts, GameObject postRef, float lh, float hh, float horzoff, float vertoff, int hnum, int vnum, bool generate, float foff, float soff, DigZone.Yards yard1, DigZone.Yards yard2) {
        sourcePost = selfReference;
        partStorage = parts;
        postObject = postRef;
        lowHeight = lh;
        highHeight = hh;
        horizontalOffset = horzoff;
        vertOffset = vertoff;
        horizontalNum = hnum;
        vertNum = vnum;
        doesGenerateDigZones = generate;
        firstOffset = foff;
        secondOffset = soff;
        firstYard = yard1;
        secondYard = yard2;

        linkedPosts = new List<GameObject>();
        verticals = new List<GameObject>();
        horizontals = new List<GameObject>();
    }
}
#endif