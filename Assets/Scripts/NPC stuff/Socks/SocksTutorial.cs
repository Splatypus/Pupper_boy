using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocksTutorial : Dialog2 {
    protected override string DIALOG_PROGRESS_SAVE_KEY { get { return "SocksSummerProgression";} }
    protected override string PROGRESSION_NUM_SAVE_KEY { get { return "SocksSummerPN"; } }
    protected override string CHARACTER_STATE_SAVE_KEY {get { return "SocksSummerObjectives"; } }
    readonly int FINISHED_TUTORIAL = 7;
    readonly int MET_TIFFANY = 8;

    [Header("Objective Info")]
    public GameObject[] lookTargets;
    public GameObject[] moveTargetLocations;
    public GameObject moveTargetObject;
    public GameObject[] itemTargetLocations;
    public GameObject itemTargetObject;

    [Header("CameraStuff")]
    public InSceneCameraReference cameraReference;
    public GameObject[] cameraFocusLocations;

    new void Start() {
        base.Start();
        //customCameraLocation = cameraReference.getCamera();

        if (characterState != MET_TIFFANY) {
            EventManager.OnTalk += OnMetTiffany;
        }

        StartCoroutine(AfterStart());
    }
    //Since other things are getting set up in start functions, this need to initiate dialog after that has already happened
    IEnumerator AfterStart() {
        yield return new WaitForEndOfFrame();
        TriggerInteractFromcharacterState();
    }

    new void OnDestroy() {
        base.OnDestroy();
        EventManager.OnTalk -= OnMetTiffany;
    }

    //points the camera at cameraFocusLocations[index]
    public void PointCamera(int index) {
        //FreeCameraLook cam = Camera.main.GetComponent<FreeCameraLook>();
        //cam.MoveToPosition(cam.transform.position + new Vector3(0.0f, 1.0f, 0.0f), cameraFocusLocations[index].transform.position, 1.0f);
        cameraReference.MoveToPosition(cameraReference.getCamera().transform.position + new Vector3(0.0f, 1.0f, 0.0f), cameraFocusLocations[index].transform.position, 1.0f);
    }

    #region nomovement functions
    //ends dialog without enabling the movement controller. Used for teaching mouse commands.
    public void BreakDialogWithNoMovement() {
        StartCoroutine(DisableMovementAtEndOfFrame());
    }

    //waits until end of frame, then sets movement mode to be locked
    IEnumerator DisableMovementAtEndOfFrame() {
        yield return new WaitForEndOfFrame();
        PlayerControllerManager pcm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerManager>();
        pcm.ChangeMode(PlayerControllerManager.Modes.MovementLock);
    }
    #endregion
    
    //summons a look target at the location designated by the index in the location list
    public void SpawnLookTarget(int index) {
        TutorialLookTarget t = lookTargets[index].GetComponent<TutorialLookTarget>();
        t.enabled = true;
        t.owner = this;
    }

    //summons a move target at the location designated by the index in the location list
    public void SpawnMoveTarget(int index) {
        GameObject o = Instantiate(moveTargetObject, moveTargetLocations[index].transform.position, moveTargetLocations[index].transform.rotation);
        o.GetComponent<TutorialMoveTarget>().owner = this;
    }

    //same as above two functions. Yes these should all share the same function or whatever, but its not important enough for me to change it
    public void SpawnItemTarget(int index) {
        GameObject o = Instantiate(itemTargetObject, itemTargetLocations[index].transform.position, itemTargetLocations[index].transform.rotation);
        o.GetComponent<TutorialMoveTarget>().owner = this;
    }

    //called whenever an objective is finished, such as looking at a thing or moving to the right spot
    public void ObjectiveComplete() {
        characterState++;
        if (characterState > 3)
            progressionNum = 1;
        //trigger new dialog if needed
        TriggerInteractFromcharacterState();
    }

    //Triggers the OnInteract function based on what the current objective count is
   void TriggerInteractFromcharacterState() {
        if (characterState <= 7) { //123 are looking at object, 456 are movement, 7 is retreiving an item
            
            //save progress.
            SaveManager.getInstance().PutInt(CHARACTER_STATE_SAVE_KEY, characterState);
            SaveManager.getInstance().PutInt(DIALOG_PROGRESS_SAVE_KEY, currentNode.index);
            SaveManager.getInstance().PutInt(PROGRESSION_NUM_SAVE_KEY, progressionNum);
            SaveManager.getInstance().SaveFile();

            OnInteract();
        }
    }

    //remove auto-saving of dialog.
    //sock's dialog will save after objectives are finished, since otherwise the forced OnInteract would cause desync
    public override void SaveDialogProgress() {
        //save progress after chatting after the last objective
        if (characterState == 7) {
            characterState++;
            SaveManager.getInstance().PutInt(CHARACTER_STATE_SAVE_KEY, characterState);
            SaveManager.getInstance().PutInt(DIALOG_PROGRESS_SAVE_KEY, currentNode.index);
            SaveManager.getInstance().SaveFile();
        }
        //and then from now on treat dialog normally
        else if (characterState > 7) {
            base.SaveDialogProgress();
        }
    }

    //finds the current fence manager and unlocks fences of the given type
    public void UnlockFences(int y) {
        FenceUnlockManager.Instance.EnableIntoYard(y);
    }

    //progresses dialog when tiffany is talked to
    void OnMetTiffany(GameObject npc) {
        if (npc.GetComponent<TiffyAI>() && characterState > FINISHED_TUTORIAL) {
            EventManager.OnTalk -= OnMetTiffany;

            characterState = MET_TIFFANY;
            progressionNum = 1;
            SaveDialogProgress();
        }
    }

    //sets the progression number to properly branch for the "whats new" dialog option
    public void SetProgressionForWhatsNew() {
        progressionNum = Random.Range(0, 7);
    }

    //called when a player enters a zone theyre suppsoed to bring an item to. If they brought it, tell them to drop it, otherwise tell them to go get it
    public void PlayerEnteredItemZone(bool hasItem) {
        //if the player brought the item, we just set the dialog progression number so that they get a message telling them to drop it
        if (hasItem) {
            progressionNum = 2;
        //if they didnt bring the item, similarly, set the progression number but this time to branch to dialog telling them to go get it
        } else {
            progressionNum = 0;
        }
        //then open up dialog
        OnInteract();
    }

   


}
