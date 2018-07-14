using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ScentManager : MonoBehaviour {

    //set variables
    public Material mat;
    public float duration;
    public float maxDistance = 300;
    public List<GameObject> scentObjects = new List<GameObject>();

    //internal variables
    Transform playerTransform;
    float startTime;
    bool isEnabled;
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
        startTime = -duration;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        points = new Vector3[4];

        foreach (GameObject g in scentObjects) {
            g.SetActive(false);
        }
    }

    void Update() {
        
        //input
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (!isEnabled)
                EnableEffect();
            else
                DisableEffect();
        }

        //enable or disable scent objects
        float t = (Time.time - startTime) / duration;
        if (t <= 1.0f) {//if t is greater than 1, the effect isnt running, so don't bother running anything
            if (!isEnabled)
                t = (1 - t);
            t = Mathf.Pow(t, 2.7f);
            //then actually toggle active
            foreach (GameObject g in scentObjects) {
                if (isEnabled && !g.activeInHierarchy && Vector3.Distance(g.transform.position, playerTransform.position) < t * maxDistance) {
                    g.SetActive(true);
                } else if (!isEnabled && g.activeInHierarchy && Vector3.Distance(g.transform.position, playerTransform.position) > t * maxDistance) {
                    g.SetActive(false);
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

    //Unused right now. Might want to use rather than the current update
    IEnumerator EnableWhenInRange(GameObject g) {
        yield return new WaitUntil(() => Vector3.Distance(playerTransform.position, g.transform.position) > 2);
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
