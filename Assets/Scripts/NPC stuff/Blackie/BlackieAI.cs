using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackieAI : AIbase {


    public GameObject rewardSpawn; //location at which the reward is spawned
    public GameObject reward;
    public bool hasDoneReward = false;

    public BlackieMiniGame2 blackieGameRef;

    void Awake() {
        //Saving.Instance.AddCallback(new UnityEngine.Events.UnityAction(OnLoad));
    }

    //icons to display if in range of player
    public override void OnInRange() {
        if (!hasDoneReward)
            Display(0);
        else
            Display(1);
    }

    //loads the puzzle indicated into the puzzle machine
    public void SetUpPuzzleMachine(int puzzleNum) {
        blackieGameRef.puzzleNumber = puzzleNum;
        blackieGameRef.SetUpMachine();
    }

    //spawns reward
    public void SpawnReward() {
        if (!hasDoneReward) {
            hasDoneReward = true;
            Instantiate(reward, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
        }
    }

    //when you finish conversation, save the data
    public override void OnEnd() {
        base.OnEnd();
        //Save();
    }

    //called by game machine when a game ends
    public void FinishedGame() {
        progressionNum = 1;
    }

    //called when saved data is loaded
    void OnLoad() {
        //currentNode = nodes[Saving.Instance.data.blackieConversationNumber];
        Debug.Log("Loaded to node index " + Saving.Instance.data.blackieConversationNumber);
    }

    //Writes this data to the save file, then saves
    void Save() {
        //this probably works right? Index should only ever be changed from the editor.
        //Saving.Instance.data.blackieConversationNumber = currentNode.index;
        //Saving.Instance.Save();
        Debug.Log("Saving node index " + currentNode.index);
    }

}
