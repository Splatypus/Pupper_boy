using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePieceView : BasicDraggable, BlackieGameBoard.IPieceListener
{

    [HideInInspector] public BlackieGameViewController boardView;
    protected BlackieGameBoard.Piece piece;

    public MeshRenderer baseMesh;
    public MeshRenderer pipeMesh;
    public MeshRenderer stoneMesh;
    public ParticleSystem particles;

    int oldColor = -1;

    #region listener interface
    public virtual void ChangeColor(int newColor) {

        if (newColor == oldColor)
            return;
        oldColor = newColor;

        //change pipe and rock color
        Material colorMat = boardView.powerColors[newColor].material;
        if (pipeMesh != null) {
            pipeMesh.material = colorMat;
        }
        if (stoneMesh != null) {
            stoneMesh.material = colorMat;
        }

        //trigger particle effect
        if (particles != null && newColor != 0) {
            ParticleSystem.MainModule mm = particles.main;
            ParticleSystem.MinMaxGradient color = mm.startColor;
            color.colorMin = boardView.powerColors[newColor].minColor;
            color.colorMax = boardView.powerColors[newColor].maxColor;
            mm.startColor = color;
            particles.Play();
        }
    }
    #endregion

    #region Basic Draggable overrides
    //Movement
    public override bool CanMove(float direction) {
        return boardView.game.CanMovePiece(piece.x, piece.y, AngleConversion(direction) + piece.rotation);
    }
    public override void AfterMove(float direction) {
        boardView.game.MovePiece(piece.x, piece.y, AngleConversion(direction) + piece.rotation);
    }
    
    //Rotation
    public override bool CanRotate(bool isRight) {
        return true;
    }
    public override void AfterRotate(bool wasRight) {
        boardView.game.RotatePiece(piece.x, piece.y, wasRight ? 1 : -1);
    }

    private int AngleConversion(float angle) {
        //convert angle to 0-360 range
        angle = ((angle % 360) + 360) % 360;
        //convert to 0-3
        angle /= 90;
        //round to closest int and return
        angle += 0.5f;
        return (int)angle;
    }
    #endregion

    #region pickup interface
    public override void OnFocus() {
        if (!piece.isLocked) {
            foreach (MeshRenderer r in meshes) {
                r.material.SetFloat("_IsFocused", 1);
            }
        }
    }

    public override void OnDefocus() {
        if (!piece.isLocked) {
            foreach (MeshRenderer r in meshes) {
                r.material.SetFloat("_IsFocused", 0);
            }
        }
    }

    public override void OnPickup(PuppyPickup source) {
        if (!piece.isLocked) {
            source.itemInMouth = gameObject;
            DraggingController control = (DraggingController)GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.Dragging);
            control.Init(this);
            GetComponent<Collider>().enabled = false;
        }
    }

    public override void OnDrop(Vector3 currentVelocity) {
        if (!piece.isLocked) {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.Walking);
            GetComponent<Collider>().enabled = true;
        } else {
            Debug.LogError("ERROR: Attempting to drop a locked puzzle piece. This object should have never been able to be picked up");
        }
    }
    #endregion


    public void SetStone(GameObject stoneObject) {
        stoneMesh = stoneObject.GetComponentInChildren<MeshRenderer>();
    }
    //sets needed references and listeners between a this object and its model
    public virtual void AttachModel(BlackieGameBoard.Piece p) {
        piece = p;
        p.listener = this;
        ChangeColor(p.color);
    }
}
