using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ScentManager : MonoBehaviour {

    //set variables
    public Material mat;
    public float duration;
    public float maxDistance = 300;
    public List<ScentObject> scentObjects = new List<ScentObject>();
    public bool isUnlocked = false; //has the player aquired this skill yet

    //internal variables
    Transform playerTransform;
    float startTime;
    public bool isEnabled;
    Vector3[] points;
    bool shaderActive = false;

    //static ref
    public static ScentManager Instance;

    private void Awake() {
        Instance = this;
    }

    void Start() {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
        mat.SetFloat("_RunRingPass", 0);
        mat.SetFloat("_RingPassTimeLength", duration);
        mat.SetFloat("_RingMaxDistance", maxDistance);
        isEnabled = false;
        startTime = Time.time - duration;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        points = new Vector3[4];
    }

    void Update() {

        //enable or disable scent objects
        float t = (Time.time - startTime) / duration;
        if (t <= 1.0f) {//if t is greater than 1, the effect isnt running, so don't bother running anything
            if (!isEnabled)
                t = (1 - t);
            t = Mathf.Pow(t, 2.7f);
            //then actually toggle active
            foreach (ScentObject g in scentObjects) {
                //TODO: Fix this null check to actually remove the object when it should
                if (isEnabled && g != null && !g.isActive && Vector3.Distance(g.transform.position, playerTransform.position) < t * maxDistance) {
                    g.StartScent();
                } else if (!isEnabled && g != null && g.isActive && Vector3.Distance(g.transform.position, playerTransform.position) > t * maxDistance) {
                    g.EndScent();
                }
            }
        } else if (!isEnabled && shaderActive) {//the animation is no longer running in this case, so if this variable is true, toggle it
            shaderActive = false;
            mat.SetFloat("_RunRingPass", 0.0f); //set the shader to 0 so that it saves processing stuff
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        //if the effect is running, send the needed info to the shader
        if (startTime + duration > Time.time) {
            mat.SetVector("_CameraPosition", transform.position);
            mat.SetVector("_DoggoPosition", playerTransform.position);
            mat.SetFloat("_GameTime", Time.time);

            //set up camera frustum
            Camera.main.CalculateFrustumCorners(new Rect(0, 0, 1, 1), Camera.main.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, points);
            for (int i = 0; i < 4; i++) {
                points[i] = transform.localToWorldMatrix * points[i];
                Debug.DrawRay(transform.position, points[i], Color.blue);
            }
            mat.SetMatrix("_ViewFrustum", (new Matrix4x4(points[0], points[1], points[2], points[3])).transpose);
        }

        Graphics.Blit(source, destination, mat);
        //mat is the material which contains the shader
        //we are passing the destination RenderTexture to
    }

    //if the effect is available to the player, toggle its enable/disable state
    public void ToggleEffect() {
        if (isUnlocked) {
            if (!isEnabled)
                EnableEffect();
            else
                DisableEffect();
        }
    }
    //Should be called when player input tries to enable scent mode. Check to make sure that its unlocked and not already enabled, then enables it if it can. Returns true if swap was made
    public bool InputEnable() {
        if (isUnlocked && !isEnabled) {
            EnableEffect();
            return true;
        }
        return false;
    }
    //Same thing but for disabling the effect
    public bool InputDisable() {
        if (isUnlocked && isEnabled) {
            DisableEffect();
            return true;
        }
        return false;
    }

    //starts expanding the effect
    public void EnableEffect() {
        isEnabled = true;
        shaderActive = true;
        mat.SetFloat("_RunRingPass", 1); //run outward pass

        if (startTime + duration < Time.time) {
            startTime = Time.time;
            mat.SetFloat("_StartingTime", startTime); //set start time normally if it completed animation
        } else {
            startTime = Time.time + Time.time - startTime - duration;
            mat.SetFloat("_StartingTime", startTime);
        }
    }

    //starts shrinking the effect
    public void DisableEffect() {
        isEnabled = false;
        mat.SetFloat("_RunRingPass", 2);  //run inward pass

        if (startTime + duration < Time.time) {
            startTime = Time.time;
            mat.SetFloat("_StartingTime", startTime); //set start time normally if it completed animation
        } 
        else {
            startTime = Time.time + Time.time - startTime - duration;
            mat.SetFloat("_StartingTime", startTime);
        }
    }

}
