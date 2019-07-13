using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffects : MonoBehaviour {

    //singleton pattern
    static ScreenEffects instance;
    public static ScreenEffects GetInstance() {
        return instance;
    }

    public Material mat;

    public Image imageRenderer;

    //screen fade effect
    float fadePercent = 0.0f;

    void Awake() {
        instance = this;
    }

    void Start() {
        mat.SetFloat("_FadePercent", fadePercent);
    }

    public bool AreEffectsActive() {
        return fadePercent > 0.0f;
    }

    //fades the entire screen to black over the given duration. Calls onComplete when finished
    public void FadeToBlack(float duration, System.Action onComplete = null) {
        StartFade(duration, Color.black, onComplete);
    }
    //fades the entire screen to the given color over the given duration. Calls onComplete when finsihed
    public void StartFade(float duration, Color color, System.Action onComplete = null) {
        StopAllCoroutines();
        imageRenderer.color = color;
        StartCoroutine(ProgressFade(duration, true, onComplete));
    }
    //reverses any current fade effect on the screen, returning it to normal over the given duration. Calls onComplete when finished
    public void ReverseFade(float duration, System.Action onComplete = null) {
        StopAllCoroutines();
        StartCoroutine(ProgressFade(duration, false, onComplete));
    }

    public void SetFadeAmount(float newFadeAmount) {
        StopAllCoroutines();
        fadePercent = newFadeAmount;
        imageRenderer.color = new Color(imageRenderer.color.r,
                                        imageRenderer.color.g,
                                        imageRenderer.color.b,
                                        fadePercent);
        imageRenderer.gameObject.SetActive(fadePercent > 0.0f);
    }

    public IEnumerator ProgressFade(float duration, bool isIncreasing, System.Action onComplete = null) {
        duration *= isIncreasing ? 1 - fadePercent : fadePercent; //shorten duration if already partially faded
        imageRenderer.gameObject.SetActive(true);
        float startTime = Time.time;
        float initialPercent = fadePercent;
        while (Time.time <= startTime + duration) {
            //lerp fade percent to 100 or 0 based on if were fading in or out, and the percentage of the duration we've completed
            fadePercent = Mathf.Lerp(initialPercent, isIncreasing ? 1.0f : 0.0f, (Time.time - startTime) / duration);
            imageRenderer.color = new Color(imageRenderer.color.r,
                                        imageRenderer.color.g,
                                        imageRenderer.color.b,
                                        fadePercent); //mat.SetFloat("_FadePercent", fadePercent);
            //yield for next frame
            yield return new WaitForEndOfFrame();
        }
        //set it at the end to make sure we get an exact value
        fadePercent = isIncreasing ? 1.0f : 0.0f;
        imageRenderer.color = new Color(imageRenderer.color.r,
                                        imageRenderer.color.g,
                                        imageRenderer.color.b,
                                        fadePercent);//mat.SetFloat("_FadePercent", fadePercent);
        if (!isIncreasing) {
            imageRenderer.gameObject.SetActive(false);
        }
        onComplete?.Invoke();
    }

    /*===SCENT MODE===
    //initialize variables
    public void InitializeScentMode(int active, float effectDuration, float effectDistance) {
        mat.SetFloat("_RunRingPass", active);
        mat.SetFloat("_RingPassTimeLength", effectDuration);
        mat.SetFloat("_RingMaxDistance", effectDistance);
    }
    //updates runtime variables in scent mode shader
    public void UpdateScentMode(Vector3 cameraPos, Vector3 playerPos, Vector3[] cameraFrustrum) {
        mat.SetVector("_CameraPosition", cameraPos);
        mat.SetVector("_DoggoPosition", playerPos);
        mat.SetFloat("_GameTime", Time.time);
        mat.SetMatrix("_ViewFrustum", (new Matrix4x4(cameraFrustrum[0], cameraFrustrum[1], cameraFrustrum[2], cameraFrustrum[3])).transpose);
    }
    //sets the state of scent mode, 0 is inactive, 1 is outward, 2 is inward
    public void SetScentActive(int active) {
        mat.SetFloat("_RunRingPass", active);
    }
    //sets the time at which the effect started
    public void SetScentStartTime(float time) {
        mat.SetFloat("_StartingTime", time);
    }*/
}
