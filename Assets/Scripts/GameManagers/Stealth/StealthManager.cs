using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthManager : MonoBehaviour
{
    [Header("Respawn Info")]
    public GameObject respawnPoint;
    public float fadeDuration = 2.0f;
    public GameObject exclamationPoint;
    public AnimationCurve popInCurve;
    public float distanceAbove = 1.0f;


    public void FoundPlayer(StealthAgent agent, GameObject player) {
        //on seen events
        OnSeen(player);
        //fade out screen for respawn, then do respawn events
        ScreenEffects.GetInstance().FadeToBlack(fadeDuration, ()=> {OnRespawn(player);} );

    }

    //Events that happen when the player is seen
    public virtual void OnSeen(GameObject player) {
        //lock movement
        player.GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.MovementLock);
        //Display exclamation
        StartCoroutine(PopIn(fadeDuration, exclamationPoint, player));
    }

    //Events that happen when the player is respawned
    public virtual void OnRespawn(GameObject player) {

        //Move to respawn point
        player.transform.position = respawnPoint.transform.position;
        player.transform.rotation = respawnPoint.transform.rotation;
        Camera.main.GetComponent<FreeCameraLook>().CenterCamera();
        ScreenEffects.GetInstance().ReverseFade(fadeDuration);

        //unlock movement
        player.GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.Walking);
    }


    IEnumerator PopIn(float duration, GameObject scaledObject, GameObject player) {
        //enable
        scaledObject.SetActive(true);
        //set position
        scaledObject.transform.position = player.transform.position + new Vector3(0, distanceAbove, 0);
        //scale over time
        float startTime = Time.time;
        Vector3 initialScale = scaledObject.transform.localScale;
        while(Time.time < startTime + duration) {
            //scale it from zero to its initial size, using a defined curve based on time
            scaledObject.transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, popInCurve.Evaluate((Time.time-startTime/duration)));
            yield return null;
        }
        scaledObject.transform.localScale = initialScale;
        //disable
        scaledObject.SetActive(false);
    }
}
