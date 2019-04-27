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
    //move
    public virtual bool CanMove(float direction) {
        return true;
    }

    public virtual void AfterMove(float direction) {

    }

    //rotate
    public virtual bool CanRotate(bool isRight) {
        return true;
    }

    public virtual void AfterRotate(bool wasRight) {
    }

    #region interface methods
    public virtual void OnFocus() {
        foreach (MeshRenderer r in meshes) {
            r.material.SetFloat("_IsFocused", 1);
        }
    }

    public virtual void OnDefocus() {
        foreach (MeshRenderer r in meshes) {
            r.material.SetFloat("_IsFocused", 0);
        }

    }

    public virtual void OnPickup(PuppyPickup source) {
        source.itemInMouth = gameObject;
        DraggingController control = (DraggingController)GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.Dragging);
        control.Init(this);
        GetComponent<Collider>().enabled = false;
    }

    public virtual void OnDrop(Vector3 currentVelocity) {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.Walking);
        GetComponent<Collider>().enabled = true;
    }
    #endregion

}
