using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayNightMaterialChange : MonoBehaviour {

    public MeshRenderer target;
    public Material dayMat;
    public Material nightMat;

	// Use this for initialization
	void Start () {
        if (target == null)
            target = gameObject.GetComponent<MeshRenderer>();

        //set initial day night setting
        DayNightManager dayNight = DayNightManager.Instance;
        if (dayNight.currentTime == DayNightManager.Times.DAY) {
            OnDay();
        } else {
            OnNight();
        }
        //set up triggers
        dayNight.AddTrigger(DayNightManager.Times.DAY, new UnityAction(OnDay));
        dayNight.AddTrigger(DayNightManager.Times.NIGHT, new UnityAction(OnNight));
    }

    //called when we enter either day or night
    public void OnDay() {
        target.material = dayMat;
    }
    public void OnNight() {
        target.material = nightMat;
    }
}
