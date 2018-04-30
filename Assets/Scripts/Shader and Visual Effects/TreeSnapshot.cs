using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSnapshot : MonoBehaviour {

    public int imageWidth = 128;
    public int imageHeight = 128;
    public float distanceFromTree = 10.0f;
    public string folder;
    public string fileName;
    

    Vector3[] vecs;
    Texture2D tex;
    


    // Use this for initialization
    void Start () {
        vecs = GenerateVectors();
        tex = new Texture2D(128 * 4, 128 * 4, TextureFormat.ARGB32, false);
        StartCoroutine(ConvertToImage(0));

    }

    //generates a half-dome of vectors. 8 vectors in a circle pointing outward, then 7 at a 45 degree upward angle, and then one stright up
    public static Vector3[] GenerateVectors() {
        Vector3[] vecs = new Vector3[16];
        for (int i = 0; i < 16; i++) {
            if (i < 8){
                float turnAmount = i * 45;
                vecs[i] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * turnAmount), 0.0f, Mathf.Sin(Mathf.Deg2Rad * turnAmount)).normalized;
            } else if (i < 15) {
                float turnAmount = i * 360/7;
                vecs[i] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * turnAmount), Mathf.Sin(Mathf.Deg2Rad * 60.0f), Mathf.Sin(Mathf.Deg2Rad * turnAmount)).normalized;
            } else if (i == 15) {
                vecs[i] = Vector3.up;
            }
        }

        return vecs;
    }

    //renders the tree out to an image
    IEnumerator ConvertToImage(int itr) {
        //grab the main camera and mess with it for rendering the object - make sure orthographic
        Camera cam = Camera.main;
        cam.orthographic = true;

        //render to screen rect area equal to out image size
        float rw = imageWidth;
        rw /= Screen.width;
        float rh = imageHeight;
        rh /= Screen.height;
        cam.rect = new Rect(0, 0, rw, rh);

        //grab size of object to render - place/size camera to fit
        Bounds bb = gameObject.GetComponent<Renderer>().bounds;

        //place camera looking at centre of object - and backwards down the z-axis from it
        Vector3 objectCenter = bb.center;
        cam.transform.position = new Vector3(objectCenter.x + vecs[itr].x * distanceFromTree,
                                                objectCenter.y + vecs[itr].y * distanceFromTree,
                                                objectCenter.z + vecs[itr].z * distanceFromTree);    // + (bb.min.z * 2.0f));
        cam.transform.LookAt(objectCenter);
        //make clip planes fairly optimal and enclose whole mesh
        cam.nearClipPlane = 0.5f;
        cam.farClipPlane = Mathf.Abs(Vector3.Distance(cam.transform.position, objectCenter)) * 2;
        //set camera size to just cover entire mesh
        float[] f = { bb.extents.x, bb.extents.y, bb.extents.z };
        cam.orthographicSize = 1.01f * Mathf.Max(f);
        //cam.transform.position.Set(cam.transform.position.x, cam.orthographicSize * 0.05f, cam.transform.position.y);

        //render
        yield return new WaitForEndOfFrame();

        //Texture2D tex = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32, false);
        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), (itr % 4) * imageWidth, (itr / 4) * imageHeight);
        tex.Apply();

        //turn all pixels == background-color to transparent
        Color bCol = cam.backgroundColor;
        Color alpha = bCol;
        alpha.a = 0.0f;
        //alpha = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        for (int y = 0; y < imageHeight; y++)
        {
            for (int x = 0; x < imageWidth; x++)
            {
                Color c = tex.GetPixel(x + ((itr%4) * imageWidth), y + ((itr / 4) * imageWidth));
                if (c.r == bCol.r)
                    tex.SetPixel(x + ((itr % 4) * imageWidth), y + ((itr / 4) * imageWidth), alpha);
            }
        }
        tex.Apply();

        // Encode texture into PNG
        //byte[] bytes = tex.EncodeToPNG();
        //Destroy(tex);
        //System.IO.File.WriteAllBytes(Application.dataPath + "/Resources/" + folder + "/" + fileName + itr + ".png", bytes);

        //do this again for the next angle
        itr += 1;
        if (itr < 16)
        {
            //gameObject.transform.Rotate(new Vector3(0.0f, 360.0f / 16.0f, 0.0f), Space.World);
            StartCoroutine(ConvertToImage(itr));
        }
        else {
            //at the end write the whole sprite sheet to a file
            byte[] bytes = tex.EncodeToPNG();
            Destroy(tex);
            System.IO.File.WriteAllBytes(Application.dataPath + "/Resources/" + folder + "/" + fileName + ".png", bytes);
        }
    }
}
