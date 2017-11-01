using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdDive : MonoBehaviour {

    public GameObject objectA;
    public GameObject objectB;



    void Update()
    {
        transform.position = Vector3.Lerp(objectA.transform.position, objectB.transform.position, Mathf.PingPong(Time.time, 1));
    }

}
