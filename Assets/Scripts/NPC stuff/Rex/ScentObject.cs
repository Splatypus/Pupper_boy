using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//add this to an object to have it be enabled and diabled when scent mode hits it
public class ScentObject : MonoBehaviour {

    public bool isActive = true; //is it doing scent stuff
    public int layer = 9;
    public GameObject[] effectedObjects;

    int[] returnLayers; //the layer in which the parent object originally existed

    // Use this for initialization
    void Start() {
        ScentManager.Instance.scentObjects.Add(this);
        returnLayers = new int[effectedObjects.Length];
        //reserve the layer that the parent is in so we know what to reset to
        for (int i = 0; i < effectedObjects.Length; i++) {
            returnLayers[i] = effectedObjects[i].layer;
        }
        //enable or disable depending on if scent is active
        if (ScentManager.Instance.isEnabled)
            StartScent();
        else
            EndScent(); 
    }

    private void OnDestroy() {
        ScentManager.Instance.scentObjects.Remove(this);
    }

    //called when inside scent detection range
    public virtual void StartScent() {
        gameObject.SetActive(true);
        isActive = true;

        for (int i = 0; i < effectedObjects.Length; i++) {
            effectedObjects[i].layer = layer;
        }
    }

    //and when it leaves detection range
    public virtual void EndScent(){
        gameObject.SetActive(false);
        isActive = false;

        for (int i = 0; i < effectedObjects.Length; i++) {
            effectedObjects[i].layer = returnLayers[i];
        }
    }
}
