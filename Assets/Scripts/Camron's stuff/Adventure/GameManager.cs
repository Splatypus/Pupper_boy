using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    //character organization and rounds
    int round = 0;
    Character[] allies = new Character[4];
    Character[] enemies = new Character[4];
    Queue<Character> turnOrder = new Queue<Character>();



    //start a new round
    public void NextRound() {
        //generate turn order
        round++;
        //do first turn
        NextTurn();
    }


    //start each turn
    public void NextTurn() {
        Character currentCharacter = turnOrder.Dequeue();
        if (currentCharacter != null) {
            currentCharacter.TurnAction();
        } else {
            //if no character left in the turn order, then start the next round
            NextRound();
        }
    }

}
