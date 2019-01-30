using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiffanyBirds : BirdMovementV2 {

    public TiffyAI tiffanyReference;

    //when the bird is scared off, let tiffany know
    public override void StartFlight() {
        base.StartFlight();
        tiffanyReference.BirdScared();
    }
}
