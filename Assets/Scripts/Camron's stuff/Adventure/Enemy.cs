using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    public override void TurnAction() {
        //generate list of usable abilities and randomly select one
        gameManager.NextTurn();
    }

}
