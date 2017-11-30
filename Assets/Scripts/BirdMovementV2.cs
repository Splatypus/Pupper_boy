using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovementV2 : MonoBehaviour {

    // state enum stuff
    public enum BirdState { Wander, FlyAway, FlyDown, BathMode };
    public BirdState curState = BirdState.Wander;

    #region Wander Info
    // private vars
    int numWanderHops = 0;

    // public vars
    public float WanderHopForce;
    public float WanderMaxAngleChange;
    public int WanderNumHopsToTurnAround;
    [SerializeField] private PhysicMaterial moveMat;
    [SerializeField] private PhysicMaterial waitMat;
    #endregion

    // private variables
    Animator anim;
    Rigidbody rb;
    BoxCollider col;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
    }

#region Animation Events
    void have_friction()
    {
        col.material = waitMat;
    }

    void small_hop_forward()
    {
        rb.AddForce(transform.right * -WanderHopForce);
    }
    
    void hop_forward()
    {
        if (curState == BirdState.Wander)
        {
            col.material = moveMat;
            if (numWanderHops >= WanderNumHopsToTurnAround)
            {
                transform.right = -transform.right;
                numWanderHops = 0;
            }
            else
            {
                transform.Rotate(transform.up, Random.Range(-WanderMaxAngleChange, WanderMaxAngleChange));
            }

            rb.AddForce(transform.right * -WanderHopForce);
            numWanderHops++;
        }

    }
#endregion
}
