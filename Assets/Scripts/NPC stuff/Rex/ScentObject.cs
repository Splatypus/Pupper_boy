using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//add this to an object to have it be enabled and diabled when scent mode hits it
public class ScentObject : MonoBehaviour {

    public bool isActive = true; //is it doing scent stuff
    public bool colorParent = false; //should the parent object remain colored for this?

    int returnLayer; //the layer in which the parent object originally existed

    // Use this for initialization
    void Start() {
        ScentManager.Instance.scentObjects.Add(this);
        gameObject.layer = 9;
        //reserve the layer that the parent is in so we know what to reset to
        if (colorParent) {
            returnLayer = transform.parent.gameObject.layer;
        }
        //enable or disable depending on if scent is active
        if (ScentManager.Instance.isEnabled)
            StartScent();
        else
            EndScent(); 
    }

    //called when inside scent detection range
    public virtual void StartScent() {
        gameObject.SetActive(true);
        isActive = true;

        if (colorParent) {
            RecursiveLayerChange(transform.parent.gameObject, 9);
        }
    }

    //and when it leaves detection range
    public virtual void EndScent(){
        gameObject.SetActive(false);
        isActive = false;

        if (colorParent) {
            RecursiveLayerChange(transform.parent.gameObject, returnLayer);
        }
    }

    //changes the layer of each child of an object
    void RecursiveLayerChange(GameObject g, int layer) {
        g.layer = layer;
        foreach (Transform t in g.transform) {
            RecursiveLayerChange(t.gameObject, layer);
        }
    }
}
