using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayNigtLightChange : MonoBehaviour {

    public Light lightSource;
    public Color dayColor;
    public Color nightColor;
    public Vector3 dayRotation;
    public Vector3 nightRotation;
    public float dayIntensity;
    public float nightIntensity;

	// Use this for initialization
	void Start () {
        if (lightSource == null)
            lightSource = gameObject.GetComponent<Light>();

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
        lightSource.color = dayColor;
        lightSource.transform.rotation = Quaternion.Euler(dayRotation);
        lightSource.intensity = dayIntensity;
    }
    public void OnNight() {
        lightSource.color = nightColor;
        lightSource.transform.rotation = Quaternion.Euler(nightRotation);
        lightSource.intensity = nightIntensity;
    }
}
