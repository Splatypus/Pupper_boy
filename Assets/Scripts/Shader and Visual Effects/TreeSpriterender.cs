using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TreeSpriterender : MonoBehaviour {

    Vector3[] vecs;
    public Texture2D spriteSheet;
    public int width;
    public int height;
    Sprite[] sprites;
    SpriteRenderer[] sr;

    public float LODdis;
    public float blendMult;
    public GameObject meshObject;
    public GameObject spriteObject;

	// Use this for initialization
	void Start () {
        vecs = TreeSnapshot.GenerateVectors();
        sr = spriteObject.GetComponentsInChildren<SpriteRenderer>();
        sprites = new Sprite[16];
        SetUpSprites();

        //adjust location and scale of sprite to equal mesh exactly
        Bounds bb = meshObject.GetComponent<Renderer>().bounds;
        spriteObject.transform.position = bb.center;
        float[] f = { bb.extents.x, bb.extents.y, bb.extents.z };
        spriteObject.transform.localScale *=  Mathf.Max(f)* 2 * 100 / Mathf.Max(height, width);
	}

    void SetUpSprites() {
        //generate 16 sprites from a sprite sheet
        for (int i = 0; i < 16; i++) {
            sprites[i] = Sprite.Create(spriteSheet, new Rect((i%4) * width, (i/4) * height, width, height), new Vector2(0.5f,0.5f));
        }
    }
	
	// Update is called once per frame
	void Update () {
        //if tree is far away
        if (Vector3.Distance(Camera.main.transform.position, transform.position) > LODdis)
        {
            if (meshObject.activeInHierarchy) { 
                meshObject.SetActive(false);
                spriteObject.SetActive(true);
            }
            Vector3 cameraLook = transform.position - Camera.main.transform.position; //vector direction from camera to this object
            cameraLook = cameraLook.normalized;
            //find the value of each vector's dot prodict with the camera look, then sort by that result to find the closest 3 vectors
            int[] indexes = new int[16];
            float[] results = new float[16];
            for (int i = 0; i < 16; i++)
            {
                indexes[i] = i;
                results[i] = Vector3.Dot(cameraLook, transform.rotation * vecs[i]);
            }
            Array.Sort(results, indexes);


            for (int i = 0; i < 3; i++) {
                sr[i].sprite = sprites[indexes[i]];
                //set alpha of two of the sprites to give a blend effect
                float d = Vector3.Distance(vecs[indexes[0]], -cameraLook);
                if (i != 0)
                    sr[i].color = new Color(sr[i].color.r, sr[i].color.g, sr[i].color.b, d*blendMult / (Vector3.Distance(vecs[indexes[i]], -cameraLook) + d) );
            }

            //then billboard the sprite
            spriteObject.transform.LookAt(spriteObject.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }
        //tree is close
        else {
            if (!meshObject.activeInHierarchy) {
                meshObject.SetActive(true);
                spriteObject.SetActive(false);
            }
        }
    }
}
