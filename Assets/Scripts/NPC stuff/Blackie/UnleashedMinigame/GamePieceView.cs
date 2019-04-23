using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePieceView : MonoBehaviour, BlackieGameBoard.IPieceListener
{

    BlackieGameViewController boardView;
    public MeshRenderer baseMesh;
    public MeshRenderer pipeMesh;
    public MeshRenderer stoneMesh;
    public ParticleSystem particles;

    //interface
    public void ChangeColor(int newColor) {

        //change pipe and rock color
        Material colorMat = newColor == -1 ? boardView.unpoweredColor : boardView.powerColors[newColor].material;
        if (pipeMesh != null) {
            pipeMesh.material = colorMat;
        }
        if (stoneMesh != null) {
            stoneMesh.material = colorMat;
        }

        //trigger particle effect
        if (particles != null && newColor != -1) {
            ParticleSystem.MinMaxGradient color = particles.main.startColor;
            color.colorMin = boardView.powerColors[newColor].minColor;
            color.colorMax = boardView.powerColors[newColor].maxColor;
            particles.Play();
        }
    }

    public void SetStone(GameObject stoneObject) {
        stoneMesh = stoneObject.GetComponentInChildren<MeshRenderer>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }
}
