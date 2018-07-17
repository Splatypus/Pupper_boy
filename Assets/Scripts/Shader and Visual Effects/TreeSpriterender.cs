using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TreeSpriterender : MonoBehaviour {

    public Texture2D spriteSheet;
    public int width;
    public int height;
    SpriteRenderer[] sr;
    SpriteHolder sh;

    int snapshotCount;
    public float LODdis;
    public float blendMult;
    public GameObject meshObject;
    public GameObject spriteObject;

	// Use this for initialization
	void Start () {
        sr = spriteObject.GetComponentsInChildren<SpriteRenderer>();
        sh = SpriteHolder.Instance(spriteSheet, width, height);
        snapshotCount = sh.vecs.Length;

        //adjust location and scale of sprite to equal mesh exactly
        Bounds bb = meshObject.GetComponent<Renderer>().bounds;
        spriteObject.transform.position = bb.center;
        float[] f = { bb.extents.x, bb.extents.y, bb.extents.z };
        spriteObject.transform.localScale *=  Mathf.Max(f)* 2 * 100 / Mathf.Max(height, width);
	}

	
	// Update is called once per frame
	void Update () {
        //if tree is far away
        if (Vector3.Distance(Camera.main.transform.position, transform.position) > LODdis)
        {
            if (meshObject.activeInHierarchy) {
                //remove mesh, fade in sprite
                meshObject.SetActive(false);
                spriteObject.SetActive(true);
            }
            Vector3 cameraLook = transform.position - Camera.main.transform.position; //vector direction from camera to this object
            cameraLook = cameraLook.normalized;
            //find the value of each vector's dot prodict with the camera look, then sort by that result to find the closest 3 vectors
            int[] indexes = new int[snapshotCount];
            float[] results = new float[snapshotCount];
            for (int i = 0; i < snapshotCount; i++)
            {
                indexes[i] = i;
                results[i] = Vector3.Dot(cameraLook, transform.rotation * sh.vecs[i]);
            }
            Array.Sort(results, indexes);


            for (int i = 0; i < 3; i++) {
                sr[i].sprite = sh.sprites[indexes[i]];
                //set alpha of two of the sprites to give a blend effect
                float d = Vector3.Distance(sh.vecs[indexes[0]], -cameraLook);
                if (i != 0)
                    sr[i].color = new Color(sr[i].color.r, sr[i].color.g, sr[i].color.b, d*blendMult / (Vector3.Distance(sh.vecs[indexes[i]], -cameraLook) + d) );
            }

            //then billboard the sprite
            //spriteObject.transform.LookAt(spriteObject.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up); //true billboard
            spriteObject.transform.LookAt(Camera.main.transform.position); //always face camera
        }
        //tree is close
        else {
            if (!meshObject.activeInHierarchy) {
                //spawn mesh, fade out sprite
                meshObject.SetActive(true);
                spriteObject.SetActive(false);
            }
        }
    }

    /* //method to fade sprites in and out to try to reduce popping, but it looks kinda bad
    IEnumerator FadeSprite(SpriteRenderer s, float start, float end, float time, bool fadeIn) {
        isFade = true;
        float startTime = Time.time;
        while (Time.time < startTime + time) {
            s.color = new Color(s.color.r, s.color.g, s.color.b, Mathf.Lerp(start, end, (Time.time - startTime)/time));
            yield return new WaitForSeconds(0.017f);
        }
        isFade = false;
        if (fadeIn) {
            meshObject.SetActive(false);
        } else {
            spriteObject.SetActive(false);
        }
    }*/
}

//singleton class used to hold the sprites and "mesh"
public class SpriteHolder {
    private static SpriteHolder instance;
    public Sprite[] sprites;
    public Vector3[] vecs;

    private SpriteHolder(Texture2D sheet, int width, int height) {
        sprites = new Sprite[36];
        for (int i = 0; i < 36; i++){
            sprites[i] = Sprite.Create(sheet, new Rect((i % 6) * width, (i / 6) * height, width, height), new Vector2(0.5f, 0.5f));
        }
        vecs = TreeSnapshot.GenerateVectors();
    }

    public static SpriteHolder Instance(Texture2D sprite, int width, int height) {
        if (instance == null)
        {
            instance = new SpriteHolder(sprite, width, height);
        }
        return instance;
    }
}