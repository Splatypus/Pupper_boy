using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDraggable : MonoBehaviour, PuppyPickup.IPickupItem
{
    [Header("Draggability Obtions")]
    public float moveSpeed;
    public float rotationSpeed;
    public float setMoveAmount; //if 0, free move
    public float setRotateAmount; //if 0, free rotate

    public AnimationCurve dragLerpCurve;
    public AnimationCurve rotateLerpCurve;

    [Header("Visual")]
    public MeshRenderer[] meshes;


    //control functions
    public virtual bool CanMove(bool isForward) {
        return true;
    }

    public virtual bool CanRotate(bool isRight) {
        return true;
    }

    #region interface methods
    public void OnFocus() {
        foreach (MeshRenderer r in meshes) {
            r.material.SetFloat("_IsFocused", 1);
        }
    }

    public void OnDefocus() {
        foreach (MeshRenderer r in meshes) {
            r.material.SetFloat("_IsFocused", 0);
        }

    }

    public void OnPickup(PuppyPickup source) {
        source.itemInMouth = gameObject;
        DraggingController control = (DraggingController)GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.Dragging);
        control.Init(this);
        GetComponent<Collider>().enabled = false;
    }

    public void OnDrop(Vector3 currentVelocity) {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.Walking);
        GetComponent<Collider>().enabled = true;
    }
    #endregion

}
