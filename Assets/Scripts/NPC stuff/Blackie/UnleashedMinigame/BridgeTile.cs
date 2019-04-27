using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeTile : GamePieceView {
    public MeshRenderer bridgePipeMesh;
    public ParticleSystem bridgeParticles;

    int oldBridgeColor = -1;

    public override void ChangeColor(int newColor) {
        base.ChangeColor(newColor);

        newColor = ((BlackieGameBoard.BridgePiece)piece).horizontalColor;
        if (newColor == oldBridgeColor)
            return;
        oldBridgeColor = newColor;

        //change pipe and rock color
        Material colorMat = boardView.powerColors[newColor].material;
        if (bridgePipeMesh != null) {
            bridgePipeMesh.material = colorMat;
        }

        //trigger particle effect
        if (bridgeParticles != null && newColor != 0) {
            ParticleSystem.MainModule mm = bridgeParticles.main;
            ParticleSystem.MinMaxGradient color = mm.startColor;
            color.colorMin = boardView.powerColors[newColor].minColor;
            color.colorMax = boardView.powerColors[newColor].maxColor;
            mm.startColor = color;
            bridgeParticles
.Play();
        }
    }

    public override void AttachModel(BlackieGameBoard.Piece p) {
        base.AttachModel(p);
    }
}
