using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnowPhysics : MonoBehaviour {

    [HideInInspector]
    public ComputeShader calculationEngine;
    //public Texture initialInput; Probably taken from terrain height map??
    public Texture cameraInput;

    //textures
    private int textureIndex;
    public RenderTexture[] texture = new RenderTexture[2];
    public Texture getSnowTexture {
        get { return texture[textureIndex]; }
    }
    public Texture getBuffer {
        get { return texture[(textureIndex + 1) % 2]; }
    }
    //compute shader
    private int physicsSimulationID;
    //material
    public Material SnowMaterial;
    public float recoveryTime;
    //player movement tracking
    private GameObject player;
    private Vector3 initialPlayerPosition;
    private float worldSpaceToPixelMult;
    //terrain for heightmap
    public Texture terrainHeightmap;

	// Use this for initialization
	void Awake () {
        physicsSimulationID = calculationEngine.FindKernel("SnowPhysicsUpdate");
        int overwrite = calculationEngine.FindKernel("SnowFlashInput");
        texture = new RenderTexture[2];
        for (int i = 0; i < texture.Length; ++i) {
            texture[i] = new RenderTexture(cameraInput.width, cameraInput.height, 24);
            texture[i].format = RenderTextureFormat.RFloat;
            texture[i].wrapMode = TextureWrapMode.Clamp;
            texture[i].filterMode = FilterMode.Point;
            texture[i].enableRandomWrite = true;
            texture[i].Create();
        }
        //assign textures for overwrite
        calculationEngine.SetTexture(overwrite, "Input", cameraInput);
        calculationEngine.SetTexture(overwrite, "PreviousState", getSnowTexture);
        calculationEngine.SetTexture(overwrite, "Result", getBuffer);
        calculationEngine.Dispatch(overwrite, getSnowTexture.width / 8, getSnowTexture.height / 8, 1);

        calculationEngine.SetFloat("Width", getSnowTexture.width);
        calculationEngine.SetFloat("Height", getSnowTexture.height);
        calculationEngine.SetFloat("RecoveryTime", recoveryTime);
        calculationEngine.SetTexture(physicsSimulationID, "Input", cameraInput);
    }

    private void Start() {
        //assign player reference
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        //find initial position and calculate the number of pixels per worldpsace unit
        Camera c = gameObject.GetComponent<Camera>();
        worldSpaceToPixelMult = c.targetTexture.width/(c.orthographicSize * 2) ;
        initialPlayerPosition = player.transform.position;
        //set snow shader variables
        SnowMaterial.SetFloat("_WorldToPixel", worldSpaceToPixelMult);
        SnowMaterial.SetFloat("_Range", c.farClipPlane - c.nearClipPlane);
        SnowMaterial.SetFloat("_CameraWidth", getSnowTexture.width);

        //get terrain heightmap
        SnowMaterial.SetTexture("_TerrainHeightMap", terrainHeightmap);

    }

    private void FixedUpdate() {
#if UNITY_EDITOR //this only matters if recoverytime changes during runtime. 
        calculationEngine.SetFloat("RecoverySpeed", recoveryTime);
#endif

        //move the camera in pixel increments 
        Vector3 playerOffset = player.transform.position - initialPlayerPosition;
        Vector3 newPosition = new Vector3(player.transform.position.x - playerOffset.x % (1/worldSpaceToPixelMult),
                                            transform.position.y,
                                            player.transform.position.z - playerOffset.z % (1/worldSpaceToPixelMult));
        //converts worldspace movement into a number of pixels moved (for compute shader)
        Vector3 movementDelta = newPosition - transform.position;
        calculationEngine.SetInt("deltaX", (int)(Mathf.Floor(movementDelta.x * worldSpaceToPixelMult + 0.1f)) );
        calculationEngine.SetInt("deltaY", (int)(Mathf.Floor(movementDelta.z * worldSpaceToPixelMult + 0.1f)) );
        transform.position = newPosition;
        SnowMaterial.SetFloat("_CameraLocationX", newPosition.x);
        SnowMaterial.SetFloat("_CameraLocationZ", newPosition.z);

        //feed data to compute shader for snowfall
        calculationEngine.SetFloat("ElapsedTime", Time.fixedDeltaTime);
        calculationEngine.SetTexture(physicsSimulationID, "PreviousState", getBuffer);
        calculationEngine.SetTexture(physicsSimulationID, "Result", getSnowTexture);
        calculationEngine.Dispatch(physicsSimulationID, getSnowTexture.width / 8, getSnowTexture.height / 8, 1);

        SnowMaterial.SetTexture("_DispTex", getSnowTexture);
        //SnowMaterial.SetTexture("_NormalMap", getSnowTexture); TODO: Generate and set a real normal map
        textureIndex = (textureIndex+1) %2;
    }
}
