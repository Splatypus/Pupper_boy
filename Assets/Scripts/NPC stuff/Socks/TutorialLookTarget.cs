using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLookTarget : MonoBehaviour {

    [HideInInspector]public SocksTutorial owner;
    public float dotCutoff;

    public float lookTimeNeeded;
    float totalLookTime = 0.0f;

    public GameObject targetObject;
    public string multiSceneKey = "";
    List<Shader> originalShader = new List<Shader>();
    List<Material> mats = new List<Material>();
    public Shader shinyShader;
    public Color startColor;
    public Color endColor;

	// Use this for initialization
	void Start () {
        
    }

    private void OnEnable() {
        if (targetObject == null) {
            targetObject = CrossSceneReferences.GetInstance().GetObject(multiSceneKey);
        }
        //set its shader
        Component[] components = targetObject.GetComponentsInChildren(typeof(MeshRenderer));
        for (int i = 0; i < components.Length; i++) {
            MeshRenderer mr = (MeshRenderer)components[i];
            Material[] materials = mr.materials;
            for (int j = 0; j < materials.Length; j++) {
                originalShader.Add(materials[j].shader);
                mats.Add(materials[j]);
                materials[j].shader = shinyShader;
                materials[j].SetColor("_GlowColor", startColor);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        //if the player is looking in the right direction
        if (Vector3.Dot((targetObject.transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward) > dotCutoff) {
            //add time looking at the object
            totalLookTime += Time.deltaTime;
            //then change its color based on how long you've looked at it
            for (int i = 0; i < mats.Count; i++) {
                mats[i].SetColor("_GlowColor", Color.Lerp(startColor, endColor, totalLookTime / lookTimeNeeded));
            }

            //then complete this objective if its been looked at for long enough
            if (totalLookTime > lookTimeNeeded) {
                Finished();
            }
        }
	}

    void Finished() {
        owner.ObjectiveComplete();
        //return shader to normal
        for (int i = 0; i < mats.Count; i++) {
            mats[i].shader = originalShader[i];
        }
        Destroy(gameObject);
    }
}
