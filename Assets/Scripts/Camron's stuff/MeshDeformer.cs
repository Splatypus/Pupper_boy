using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour {

    //mesh data
    Mesh deformingMesh;
    Vector3[] originalVertices, displacedVertices;

    Vector3[] vertexVelocities;

    //starting data
    public float startHeight;
    public float recoverySpeed;
    public float downSpeed;
    public float cutoffEffectDistance;
    public bool enableDoubleSquareDeformDistance;


    // Use this for initialization
    void Start () {
        //set mesh data
        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++) {
            displacedVertices[i] = originalVertices[i];
        }
        vertexVelocities = new Vector3[originalVertices.Length];

        //so we can compare to square magnitute of vectors rather than magnitute. 
        cutoffEffectDistance *= cutoffEffectDistance;
    }
	
	// Update is called once per frame
	void Update () {
        //update each vertex location
        for (int i = 0; i < displacedVertices.Length; i++) {
            if (displacedVertices[i].y < originalVertices[i].y) {
                displacedVertices[i] += new Vector3(0.0f, recoverySpeed * Time.deltaTime, 0.0f);
            }
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
    }


    public void FlattenPoint(Vector3 point) {
        point -= transform.position;
        for (int i = 0; i < displacedVertices.Length; i++) {
            Vector3 pointToVertex = displacedVertices[i] - point;
            float dis = pointToVertex.sqrMagnitude;
            if (dis < cutoffEffectDistance) {
                if (enableDoubleSquareDeformDistance)
                    dis *= dis;
                float scaledDistance = downSpeed * Time.deltaTime / (1f + dis * dis);
                displacedVertices[i] = new Vector3(displacedVertices[i].x, Mathf.Max(displacedVertices[i].y - scaledDistance, originalVertices[i].y - 1.0f), displacedVertices[i].z);
                deformingMesh.vertices = displacedVertices;
                deformingMesh.RecalculateNormals();
            }
        }
    }


    /*public void AddDeformingForce(Vector3 point, float force) {
        for (int i = 0; i < displacedVertices.Length; i++) {
            AddForceToVertex(i, point, force);
        }
    }

    void AddForceToVertex(int i, Vector3 point, float force) {
        Vector3 pointToVertex = displacedVertices[i] - point;
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        float velocity = attenuatedForce * Time.deltaTime;
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }*/
        


}
