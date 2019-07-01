    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiffyAI : AIbase {
    protected override string DIALOG_PROGRESS_SAVE_KEY { get { return "TiffanySummerProgression"; } }
    protected override string PROGRESSION_NUM_SAVE_KEY { get { return "TiffanySummerPN"; } }
    protected override string CHARACTER_STATE_SAVE_KEY { get { return "TiffanySummerState"; } }
    int FIRST_MET = 0;
    int HIDING = 1;
    int RESCUED = 2;
    int HAPPY = 3;

    Animator[] anim;
    public float moveDistance;
    public float walkTime;
    public float birdCount;

    public GameObject rewardSpawn; //location to spawn reward
    public GameObject reward; //what to spawn

    public GameObject bandanaCollar;
    public GameObject defaultCollar;

    new public void Start() {
        base.Start();
        anim = GetComponentsInChildren<Animator>();

        if (characterState == FIRST_MET) {
            EventManager.OnFenceDig += OnDoggoEntersYard;
        } else if (characterState == RESCUED || characterState == HAPPY) {
            StartCoroutine(WalkOut());
        }
    }

    //remove eventmanager triggers
    new void OnDestroy() {
        base.OnDestroy();
        EventManager.OnFenceDig -= OnDoggoEntersYard;
    }

    //triggers the first time doggo enters tiffany's yard
    public void OnDoggoEntersYard(GameObject yard) {
        if (yard.GetComponent<DigZone>().enteringYard == DigZone.Yards.Tiffany) {
            EventManager.OnFenceDig -= OnDoggoEntersYard;
            characterState = HIDING;
            OnInteract();
        }
    }

    public override void OnInRange(GameObject player) {
        if (characterState == HIDING) {
            Display(0);
        } else if (characterState == RESCUED) {
            Display(1);
        } else if (characterState == HAPPY) {
            Display(2);
        }
    }

    //called when a toy is brought in range. If its the bandana, progress the quest
    public override void ToyInRange(BasicToy toy) {
        base.ToyInRange(toy);
        //check to make sure the quest characterState and toy tag are correct. If so, delete the bandana and do her stuff
        if (toy.HasTag(BasicToy.Tag.TiffyQuestItem) && characterState == RESCUED) {
            progressionNum = 1;
        }
    }

    public void AfterItemTaken() {
        characterState = HAPPY;
        Display(2);
        if (bandanaCollar != null && defaultCollar != null) {
            bandanaCollar.SetActive(true);
            defaultCollar.SetActive(false);
        }
    }

    public void SpawnReward() {
        Instantiate(reward, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
    }

    public void Saved() {
        characterState = RESCUED;
        progressionNum = 1; //go to the next dialog set
        Display(1);
        StartCoroutine(WalkOut());
    }

    //moves tiffany out of her dog house
    IEnumerator WalkOut() {
        //start anims
        foreach (Animator a in anim) {
            a.SetFloat("Forward", 0.8f);
            a.SetTrigger("isSaved");
        }

        //move
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        Vector3 endPosiotion = startPosition + gameObject.transform.forward * moveDistance;
        while (startTime + walkTime > Time.time) {
            transform.position = Vector3.Lerp(startPosition, endPosiotion, (Time.time - startTime)/walkTime);
            yield return new WaitForFixedUpdate();
        }

        //end anims
        foreach (Animator a in anim) {
            a.SetFloat("Forward", 0.0f);
        }
    }
   
    //called when a bird is scared off. If shes hiding, check if all birds have been spooked away, then trigger actions if they have
    public void BirdScared() {
        if (characterState == HIDING) {
            birdCount--;
            if (birdCount == 0) {
                Saved();
            }
        }
    }

    //unlocks fences to designated area
    public void UnlockFences(int y) {
        FenceUnlockManager.Instance.EnableIntoYard(y);
    }

}
