using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartNode : GamePieceView
{

    public MeshRenderer colorMesh;

    public override void AttachModel(BlackieGameBoard.Piece p) {
        base.AttachModel(p);
        BlackieGameBoard.SourcePiece sp = (BlackieGameBoard.SourcePiece)p;
        if (sp == null) {
            Debug.LogError("Error: Object with StartNode script was not assigned a startnode");
            return;
        }
        //colorMesh.material.color = boardView.powerColors[sp.color].maxColor;
        colorMesh.material = boardView.powerColors[sp.color].material;
    }
}
