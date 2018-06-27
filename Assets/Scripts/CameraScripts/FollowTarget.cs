using UnityEngine;

public abstract class FollowTarget : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 5.0f;

    virtual protected void Start() {
        if (target == null) {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void FixedUpdate() {
        if (target != null && (target.GetComponent<Rigidbody>() != null && !target.GetComponent<Rigidbody>().isKinematic)) {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * moveSpeed);
        }
    }

    public Transform Target { get { return this.target; } }
}
