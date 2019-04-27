using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndNode : GamePieceView
{
    public MeshRenderer colorMesh;

    public override void ChangeColor(int newColor) {
        base.ChangeColor(newColor);
        colorMesh.material = boardView.powerColors[piece.color].material;
        if (newColor == 0) {
            colorMesh.material.color = boardView.powerColors[((BlackieGameBoard.EndPiece)piece).goalColor].maxColor;

        }
    }

    public override void AttachModel(BlackieGameBoard.Piece p) {
        base.AttachModel(p);
        BlackieGameBoard.EndPiece ep = (BlackieGameBoard.EndPiece)p;
        if (ep == null) {
            Debug.LogError("Error: Object with EndNode script was not assigned a startnode");
            return;
        }
        colorMesh.material.color = boardView.powerColors[ep.goalColor].maxColor;
    }
}
