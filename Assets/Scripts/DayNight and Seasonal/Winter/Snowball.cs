using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour
{
    public float minScale;
    public float maxScale;
    public float scaleRate;
    public float massMult;
    public float shrinkRate;


    Vector3 previousLocation;

    // Start is called before the first frame update
    void Start()
    {
        previousLocation = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float scale = transform.localScale.x;
        scale *= 1 + Time.fixedDeltaTime * 
                       (Vector3.Distance(previousLocation, transform.position) *  scaleRate * (maxScale - scale) / maxScale - //grow
                       (scale < minScale ? 0 : shrinkRate * scale/maxScale)) ;  //shrink
        previousLocation = transform.position;
        GetComponent<Rigidbody>().mass = scale * scale * massMult;
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
