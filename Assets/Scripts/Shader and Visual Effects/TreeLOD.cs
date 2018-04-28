using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLOD : MonoBehaviour {

    public int imageWidth = 128;
    public int imageHeight = 128;
    public float distanceFromTree = 10.0f;
    public string folder;
    public string name;
    


    // Use this for initialization
    void Start () {
        StartCoroutine(ConvertToImage(0));
        
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
        cam.transform.position = new Vector3((bb.max.x + bb.min.x) / 2, (bb.max.y + bb.min.y) / 2, gameObject.transform.position.z - distanceFromTree);// + (bb.min.z * 2.0f));
        //make clip planes fairly optimal and enclose whole mesh
        cam.nearClipPlane = 0.5f;
        cam.farClipPlane = -cam.transform.position.z + 10.0f + bb.max.z;
        //set camera size to just cover entire mesh
        cam.orthographicSize = 1.01f * Mathf.Max((bb.max.y - bb.min.y) / 2.0f, (bb.max.x - bb.min.x) / 2.0f);
        //cam.transform.position.Set(cam.transform.position.x, cam.orthographicSize * 0.05f, cam.transform.position.y);

        //render
        yield return new WaitForEndOfFrame();

        Texture2D tex = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32, false);
        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        tex.Apply();

        //turn all pixels == background-color to transparent
        Color bCol = cam.backgroundColor;
        Color alpha = bCol;
        alpha.a = 0.0f;
        alpha = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        for (int y = 0; y < imageHeight; y++)
        {
            for (int x = 0; x < imageWidth; x++)
            {
                Color c = tex.GetPixel(x, y);
                if (c.r == bCol.r)
                    tex.SetPixel(x, y, alpha);
            }
        }
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);
        System.IO.File.WriteAllBytes(Application.dataPath + "/Resources/" + folder + "/" + name + itr + ".png", bytes);

        //do this again for the next angle
        if (itr < 15)
        {
            gameObject.transform.Rotate(new Vector3(0.0f, 360.0f / 16.0f, 0.0f), Space.World);
            StartCoroutine(ConvertToImage(itr + 1));
        }
    }
}
