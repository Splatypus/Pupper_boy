using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayNightMaterialChange : MonoBehaviour {

    [Tooltip("Set to the meshrenderer on this same object if left null")]
    public MeshRenderer target;
    [Tooltip("Set to the current material if left null")]
    public Material dayMat;
    public Material nightMat;

	// Use this for initialization
	void Start () {
        if (target == null)
            target = gameObject.GetComponent<MeshRenderer>();
        if (dayMat == null)
            dayMat = target.material;

        //New method using Universal trigger system
        EventManager.OnDay += OnDay;
        EventManager.OnNight += OnNight;
    }

    //Remove event references to prevent memory leaks
    private void OnDestroy() {
        EventManager.OnDay -= OnDay;
        EventManager.OnNight -= OnNight;
    }

    //called when we enter either day or night
    public void OnDay() {
        target.material = dayMat;
    }
    public void OnNight() {
        target.material = nightMat;
    }
}
