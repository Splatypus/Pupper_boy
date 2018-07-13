using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GreyscaleEffect : MonoBehaviour {

    Transform playerTransform;
    public Material mat;
    float startTime;
    public float duration;
    bool isEnabled;
    public Vector3[] points;

    void Start() {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
        mat.SetFloat("_RunRingPass", 0);
        mat.SetFloat("_RingPassTimeLength", duration);
        isEnabled = false;
        startTime = -duration;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        points = new Vector3[4];
    }

    void Update() {
        

        //input
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (!isEnabled)
                EnableEffect();
            else
                DisableEffect();
        }    
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        mat.SetVector("_CameraPosition", transform.position);
        //if (startTime + duration < Time.time) 
        mat.SetVector("_DoggoPosition", playerTransform.position);

        //set up camera frustum
        Camera.main.CalculateFrustumCorners(new Rect(0, 0, 1, 1), Camera.main.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, points);
        for (int i = 0; i < 4; i++) {
            points[i] = transform.localToWorldMatrix * points[i];
            points[i] -= transform.position;
            Debug.DrawRay(transform.position, points[i] + transform.position, Color.blue);
        }
        mat.SetMatrix("_ViewFrustum", (new Matrix4x4(points[0], points[1], points[2], points[3])).transpose);

        Graphics.Blit(source, destination, mat);
        //mat is the material which contains the shader
        //we are passing the destination RenderTexture to
    }

    public void EnableEffect() {
        isEnabled = true;
        mat.SetFloat("_RunRingPass", 1); //run outward pass

        if (startTime + duration < Time.time) {
            startTime = Time.time;
            mat.SetFloat("_StartingTime", startTime); //set start time normally if it completed animation
        } else {
            startTime = Time.time + Time.time - startTime - duration;
            mat.SetFloat("_StartingTime", startTime);
        }
    }

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
