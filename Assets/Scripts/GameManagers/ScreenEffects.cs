using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenEffects : MonoBehaviour {

    //singleton pattern
    static ScreenEffects instance;
    public static ScreenEffects GetInstance() {
        return instance;
    }

    public Material mat;

    //screen fade effect
    float fadePercent = 0.0f;

    void Awake() {
        instance = this;
    }

    void Start() {
        mat.SetFloat("_FadePercent", fadePercent);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        //run the postprocessing effect if active
        if (AreEffectsActive()) {
            Graphics.Blit(source, destination, mat);
        }
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
        StartCoroutine(ProgressFade(duration, true));
    }
    //reverses any current fade effect on the screen, returning it to normal over the given duration. Calls onComplete when finished
    public void ReverseFade(float duration, Event onComplete = null) {
    }

    public IEnumerator ProgressFade(float duration, bool isIncreaing, System.Action onComplete = null) {
        float startTime = Time.time;
        float initialPercent = fadePercent;
        while (Time.time <= startTime + duration) {
            //lerp fade percent to 100 or 0 based on if were fading in or out, and the percentage of the duration we've completed
            fadePercent = Mathf.Lerp(initialPercent, isIncreaing ? 100.0f : 0.0f, (Time.time - startTime) / duration);
            mat.SetFloat("_FadePercent", fadePercent);
            //yield for next frame
            yield return new WaitForEndOfFrame();
        }
        //set it at the end to make sure we get an exact value
        fadePercent = isIncreaing ? 100.0f : 0.0f;
        mat.SetFloat("_FadePercent", fadePercent);
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
