using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MaterialChange : MonoBehaviour {

    public MeshRenderer target;
    public Material defaultMat;
    public matTimePair[] otherMats;

    [System.Serializable]
    public struct matTimePair {
        public DayNightManager.Times time;
        public Material mat;
    }

    // Use this for initialization
    void Start() {
        if (target == null)
            target = gameObject.GetComponent<MeshRenderer>();
        
        SetMaterial();
        //set up triggers
        DayNightManager.Instance.AddTrigger(new UnityAction(SetMaterial));
    }

    //set material based off the time
    public void SetMaterial() {
        DayNightManager.Times currentTime = DayNightManager.Instance.currentTime;
        bool didMatch = false;
        //loop thru out list of materials and see if any are assigned to the current time
        foreach (matTimePair m in otherMats) {
            if (!didMatch && currentTime == m.time) {
                target.material = m.mat;
                didMatch = true;
            }
        }
        //if none are, set to default
        if (!didMatch) {
            target.material = defaultMat;
        }
    }
}
