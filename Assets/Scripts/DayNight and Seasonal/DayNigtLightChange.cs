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
	void Awake () {
        if (lightSource == null)
            lightSource = gameObject.GetComponent<Light>();

        //set initial day night setting
        DayNightManager dayNight = DayNightManager.Instance;
        if (dayNight.IsDay()) {
            OnDay();
        } else {
            OnNight();
        }
        //set up triggers
        EventManager.OnDay += OnDay;
        EventManager.OnNight += OnNight;

    }

    private void OnDestroy() {
        EventManager.OnDay -= OnDay;
        EventManager.OnNight -= OnNight;
    }

    //called when we enter either day or night
    public void OnDay() {
        lightSource = gameObject.GetComponent<Light>(); //reference is reset since it was deleting itself for some reason on reload
        lightSource.color = dayColor;
        lightSource.transform.rotation = Quaternion.Euler(dayRotation);
        lightSource.intensity = dayIntensity;
    }
    public void OnNight() {
        lightSource = gameObject.GetComponent<Light>();
        lightSource.color = nightColor;
        lightSource.transform.rotation = Quaternion.Euler(nightRotation);
        lightSource.intensity = nightIntensity;
    }

    /*void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            StartCoroutine(LerpTimeCo());
        }
    }

    IEnumerator LerpTimeCo() {
        float startTime = Time.time;
        while (lightSource.color != nightColor) {
            float t = 3;
            lightSource.color = Color.Lerp(dayColor, nightColor, (Time.time - startTime)/t);
            lightSource.transform.rotation = Quaternion.Slerp(Quaternion.Euler(dayRotation), Quaternion.Euler(nightRotation), (Time.time - startTime) / t);
            lightSource.intensity = Mathf.Lerp(dayIntensity, nightIntensity, (Time.time - startTime) / t);
            yield return new WaitForFixedUpdate();
        }
        DayNightManager.Instance.SetTime(DayNightManager.Times.NIGHT);
    }*/

}
