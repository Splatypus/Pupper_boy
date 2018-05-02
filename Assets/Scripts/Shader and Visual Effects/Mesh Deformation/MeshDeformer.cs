using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour {

    //mesh data
    Mesh deformingMesh;
    Vector3[] originalVertices, displacedVertices;

    //starting data
    public float startHeight;
    public float minHeight;
    public float recoverySpeed;
    public float downSpeed;
    public float cutoffEffectDistance;
    public bool enableDoubleSquareDeformDistance;

    //easy tiling
    public GameObject tiledObject;
    public int spawnNorth, spawnSouth, spawnEast, spawnWest;
    public float spawnHeightPadding = 4.0f;


    // Use this for initialization
    void Start() {
        //set mesh data
        deformingMesh = GetComponent<MeshFilter>().mesh;
        //makes changing mesh often more efficient
        deformingMesh.MarkDynamic();
        //start the mesh a set distance above terrain
        RaycastHit hit;
        int layermask = 1 << 8;
        originalVertices = deformingMesh.vertices;
        for (int i = 0; i < originalVertices.Length; i++) {
            if (Physics.Raycast(originalVertices[i] + transform.position, Vector3.down, out hit, 10.0f, layermask)) {
                originalVertices[i] = new Vector3(originalVertices[i].x, originalVertices[i].y - hit.distance + startHeight, originalVertices[i].z);
            }
        }
        //originalVertices = deformingMesh.vertices;
        //set up displaced ver array
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++) {
            displacedVertices[i] = originalVertices[i];
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
        deformingMesh.RecalculateBounds();
        //so we can compare to square magnitute of vectors rather than magnitute. 
        cutoffEffectDistance *= cutoffEffectDistance;

        //tile more if we got em
        if (tiledObject) { 
            for (int i = -spawnSouth; i < spawnNorth; i++) {
                for (int j = -spawnWest; j < spawnEast; j++) {
                    Vector3 newlocation = new Vector3(transform.position.x + 10 * i, transform.position.y, transform.position.z + 10 * j);
                    if (Physics.Raycast(newlocation + new Vector3(0.0f, 30.0f, 0.0f), Vector3.down, out hit, 60.0f, layermask) && (i != 0 || j != 0)) {
                        Instantiate(tiledObject, newlocation + new Vector3(0.0f, 30.0f + spawnHeightPadding - hit.distance, 0.0f), transform.rotation);
                    }
                }
            }
        }

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
            Vector2 pointToVertex = new Vector2(displacedVertices[i].x - point.x, displacedVertices[i].z - point.z);
            float dis = pointToVertex.sqrMagnitude;
            if (dis < cutoffEffectDistance) {
                /*if (enableDoubleSquareDeformDistance)
                    dis *= dis;
                float scaledDistance = downSpeed * Time.deltaTime / (1f + dis * dis);
                displacedVertices[i] = new Vector3(displacedVertices[i].x, Mathf.Max(displacedVertices[i].y - scaledDistance, originalVertices[i].y - startHeight + minHeight), displacedVertices[i].z);*/
                displacedVertices[i].y = Mathf.Max(Mathf.Min(displacedVertices[i].y, point.y), originalVertices[i].y - startHeight + minHeight);
            }
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
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
