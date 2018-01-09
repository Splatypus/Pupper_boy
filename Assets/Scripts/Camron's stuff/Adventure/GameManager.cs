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
        //do first turn if there are still characters, otherwise end combat
        if (turnOrder.Count > 0) {
            NextTurn();
        } else {
            EndCombat();
        }
    }


    //start each turn
    public void NextTurn() {
        if (turnOrder.Count <= 0) {
            NextRound();
        } else {
            Character currentCharacter = turnOrder.Dequeue();
            //set up UI for new character here
            currentCharacter.OnTurnStart();
        }
    }

    //ABILITY USE
    //takes an int to determine which button was clicked, then actiavtes taht ability
    public void UseAbility(int number) {

        NextRound();
    }

    //ends current combat or something
    public void EndCombat() {

    }

}
