using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocksTutorial : Dialog2 {
    protected override string PROGRESSION_SAVE_KEY { get { return "SocksSummerProgression";} }
    private readonly string OBJECTIVE_COUNT_KEY = "SocksSummerObjectives";

    [Header("Objective Info")]
    public GameObject[] lookTargetLocations;
    public GameObject lookTargetObject;
    public GameObject[] moveTargetLocations;
    public GameObject moveTargetObject;
    public GameObject[] itemTargetLocations;
    public GameObject itemTargetObject;
    public int objectiveCount = 0;

    [Header("CameraStuff")]
    public InSceneCameraReference cameraReference;
    public GameObject[] cameraFocusLocations;

    new void Start() {
        base.Start();
        customCameraLocation = cameraReference.getCamera();


        objectiveCount = SaveManager.getInstance().GetInt(OBJECTIVE_COUNT_KEY, 0);
        
        StartCoroutine(AfterStart());
    }
    //Since other things are getting set up in start functions, this need to initiate dialog after that has already happened
    IEnumerator AfterStart() {
        yield return new WaitForEndOfFrame();
        TriggerInteractFromObjectiveCount();
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
        GameObject o = Instantiate(lookTargetObject, lookTargetLocations[index].transform.position, lookTargetLocations[index].transform.rotation);
        o.GetComponent<TutorialLookTarget>().owner = this;
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
        objectiveCount++;

        //trigger new dialog if needed
        TriggerInteractFromObjectiveCount();
    }

    //Triggers the OnInteract function based on what the current objective count is
    public void TriggerInteractFromObjectiveCount() {
        if (objectiveCount <= 2 || objectiveCount == 5 || objectiveCount == 6 || objectiveCount == 7 || objectiveCount == 10 || objectiveCount == 11) { //looking at targets 3 and 4 do nothing, since 3,4,5 spawn all at once, same as spawns 8,9,10
            
            //save progress.
            SaveManager.getInstance().PutInt(OBJECTIVE_COUNT_KEY, objectiveCount);
            SaveManager.getInstance().PutInt(PROGRESSION_SAVE_KEY, currentNode.index);
            SaveManager.getInstance().SaveFile();

            OnInteract();
        }
    }

    //remove auto-saving of dialog.
    //sock's dialog will save after objectives are finished, since otherwise the forced OnInteract would cause desync
    public override void SaveDialogProgress() {
        if (objectiveCount == 11) {
            objectiveCount++;
            SaveManager.getInstance().PutInt(OBJECTIVE_COUNT_KEY, objectiveCount);
            SaveManager.getInstance().PutInt(PROGRESSION_SAVE_KEY, currentNode.index);
            SaveManager.getInstance().SaveFile();
        }
    }

    //finds the current fence manager and unlocks fences of the given type
    public void UnlockFences(int y) {
        FenceUnlockManager.Instance.EnableIntoYard(y);
    }

    //called when a player enters a zone theyre suppsoed to bring an item to. If they brought it, tell them to drop it, otherwise tell them to go get it
    public void PlayerEnteredItemZone(bool hasItem) {
        //if the player brought the item, we just set the dialog progression number so that they get a message telling them to drop it
        if (hasItem) {
            progressionNum = 1;
        //if they didnt bring the item, similarly, set the progression number but this time to branch to dialog telling them to go get it
        } else {
            progressionNum = 2;
        }
        //then open up dialog
        OnInteract();
    }

   


}
