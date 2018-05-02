using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDeformer : MonoBehaviour {

    public GameObject terrainObject;
    public float terrainObjectWidth = 500.0f;
    public float terrainObjectHeight = 500.0f;
    public float amoutPerSecond = 0.01f;
    Terrain terrain;
    float[,] initialHeights;

	// Use this for initialization
	void Start () {
        terrain = terrainObject.GetComponent<Terrain>();
        //save what the inital terrain data was... this allows us to reset it but also allows up to compare new heights to the original to prevent it from displacing too far
        initialHeights = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight);
	}

    //on end, reset the terrain
    private void OnApplicationQuit() {
        terrain.terrainData.SetHeights(0, 0, initialHeights);
    }

    // Update is called once per frame
    void Update () {
        //press c to blow up terrain
        if (Input.GetKey(KeyCode.C)) {
            int x = (int)((transform.position.x - terrainObject.transform.position.x) / terrainObjectWidth * (float)terrain.terrainData.heightmapWidth); //the player's x position on the terrain heightmap
            int y = (int)((transform.position.z - terrainObject.transform.position.z) / terrainObjectHeight * (float)terrain.terrainData.heightmapHeight); //and their y position (or is it z? Thanks for making y up....)
            float[,] heights = terrain.terrainData.GetHeights(x-10, y-10, 20, 20);
            for (int i = 0; i < heights.GetLength(0); i++) {
                for (int j = 0; j < heights.GetLength(1); j++) {
                    heights[i, j] += amoutPerSecond * Time.deltaTime / 500;
                }
            }
            terrain.terrainData.SetHeights(x-10, y-10, heights);
        }
	}
}
