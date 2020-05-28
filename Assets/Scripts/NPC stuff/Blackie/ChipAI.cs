using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipAI : AIbase
{
    public BlackieGameViewController game;

    public void StartGame(int index) {
        game.LoadFile(index, () => {
            progressionNum = 1;
            OnInteract();
        });
    }
}
