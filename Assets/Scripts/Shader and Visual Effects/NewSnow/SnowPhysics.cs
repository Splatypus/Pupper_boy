using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnowPhysics : MonoBehaviour {

    [HideInInspector]
    public ComputeShader calculationEngine;
    //public Texture initialInput; Probably taken from terrain height map??
    public Texture cameraInput;

    private int textureIndex;
    public RenderTexture[] texture = new RenderTexture[2];
    public Texture getSnowTexture {
        get { return texture[textureIndex]; }
    }
    public Texture getBuffer {
        get { return texture[(textureIndex + 1) % 2]; }
    }

    private int physicsSimulationID;

    public Material SnowMaterial;
    public float recoveryTime;

    private GameObject player;
    private Vector3 lastPosition;
    public float worldSpaceWidth;
    private float worldSpaceToPixelMult;

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
        player = GameObject.FindGameObjectWithTag("Player");
        lastPosition = player.transform.position;
        worldSpaceToPixelMult = getSnowTexture.width / worldSpaceWidth;
    }

    private void FixedUpdate() {
#if UNITY_EDITOR //this only matters if recoverytime changes during runtime. 
        calculationEngine.SetFloat("RecoverySpeed", recoveryTime);
#endif

        //find out how many pixels the player has moved on the texture
        Vector3 movementDelta = player.transform.position - lastPosition;
        //converts worldspace movement into a number of pixels moved
        float xPixelDelta = movementDelta.x * worldSpaceToPixelMult;
        float zPixelDelta = movementDelta.z * worldSpaceToPixelMult;
        calculationEngine.SetFloat("deltaX", xPixelDelta);
        calculationEngine.SetFloat("deltaY", zPixelDelta);

    
        calculationEngine.SetFloat("ElapsedTime", Time.fixedDeltaTime);
        calculationEngine.SetTexture(physicsSimulationID, "PreviousState", getBuffer);
        calculationEngine.SetTexture(physicsSimulationID, "Result", getSnowTexture);
        calculationEngine.Dispatch(physicsSimulationID, getSnowTexture.width / 8, getSnowTexture.height / 8, 1);

        SnowMaterial.SetTexture("_DispTex", getSnowTexture);
        //SnowMaterial.SetTexture("_NormalMap", getSnowTexture); TODO: Generate and set a real normal map
        textureIndex = (textureIndex+1) %2;

        //update last position
        lastPosition = player.transform.position;
    }
}
