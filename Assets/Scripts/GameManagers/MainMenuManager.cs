using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

    //Save Manager Reference and Audio Settings Reference
    SaveManager mySaveManager;
    [Header("References")]
    public GameObject mainUI;
    public AudioSettingsManager audioSettingsRemote;
    public GameObject saveSlotPrefab;
    public GameObject saveSlotHolder;
    List<GameObject> saveSlots;

    [Header("Buttons")]
    public Button[] myButtons;
    public Button newSaveCnt;

    [Header("Save Information")]
    string newSaveName = "";
    public string newSaveNameGet { get { return newSaveName; } set { newSaveName = value; } }

    [Header("Loading Screen")]
    public string sceneToLoad = "UnleashedBackyard";
    public string[] seasonStrings = { "UnleashedSummer", "UnleashedFall", "UnleashedWinter", "UnleashedSpring" };
    public GameObject loadingScreen;
    public Text loadingText;


    // Use this for initialization
    void Start() {

        mySaveManager = SaveManager.getInstance();
        saveSlots = new List<GameObject>();

        audioSettingsRemote.RemoteStart();
        CreateSlotUI();
        CheckButtonVisibility();

        //reset static classes
        DayNightManager.Reset();
        CrossSceneReferences.Reset();
    }

    private void OnEnable() {
        CheckButtonVisibility();
    }

    public void CheckButtonVisibility() {
        //if last open id is asssigned, set up the continue button
        if (SaveManager.getInstance().GetLastOpenID() > -1) {
            myButtons[0].gameObject.SetActive(true);
        } else {
            myButtons[0].gameObject.SetActive(false);
        }
        //if we have saves available, set up load button
        if (SaveManager.getInstance().GetNumberOfSaves() > 0) {
            myButtons[2].gameObject.SetActive(true);
        } else {
            myButtons[2].gameObject.SetActive(false);
        }
    }

    void CreateSlotUI() {
        for (int i = 0; i < mySaveManager.GetNumberOfSaves(); i++) {
            GameObject newSlot = Instantiate(saveSlotPrefab, saveSlotHolder.gameObject.transform);
            KeyValuePair<int, string> saveInfo = mySaveManager.GetSlotInfoAtIndex(i);

            newSlot.GetComponent<SlotSaveInfo>().FillInfo(saveInfo.Key, saveInfo.Value, this);

            //saveSlots.Add(newSlot); //TODO: use this to properly display UI
        }
    }


    //Used to continue on last save
    public void Continue() {
        mySaveManager.LoadFile(mySaveManager.GetLastOpenID());
        LoadScene(sceneToLoad, seasonStrings[mySaveManager.GetInt(SeasonManager.SEASON_KEY, 0)]);
    }

    //Used For Creating a New Save
    public void CreateNewSaveGame() {
        mySaveManager.CreateFile(newSaveName);
    }

    public void CheckSaveName() {
        if(newSaveNameGet.Length > 2) {
            newSaveCnt.interactable = true;
        }
        else {
            newSaveCnt.interactable = false;
        }
    }

    public void Quit() {

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

    #else
		Application.Quit();

    #endif

    }

    #region loading screen
    public void LoadScene() {
        LoadScene(sceneToLoad, seasonStrings[0]);
    }
    public void LoadScene(string sceneName, string season) {
        loadingScreen.SetActive(true);
        mainUI.SetActive(false);
        StartCoroutine(DotLoading(0.4f));
        StartCoroutine(LoadNewSceneAsync(sceneName, season));
    }


    IEnumerator DotLoading(float duration) {
        int dots = 0;
        while (true) {
            yield return new WaitForSeconds(duration);
            dots = dots == 3 ? 0 : dots + 1;

            string text = "Loading";
            for (int i = 0; i < dots; i++) {
                text += ".";
            }
            loadingText.text = text;
        }
    }

    IEnumerator LoadNewSceneAsync(string name, string season) {
        AsyncOperation asyncScene = SceneManager.LoadSceneAsync(name);
        AsyncOperation asyncSeason = SceneManager.LoadSceneAsync(season, LoadSceneMode.Additive);
        while (!asyncScene.isDone && !asyncSeason.isDone)
            yield return null;
        
        StopCoroutine("DotLoading");
    }
    #endregion
}
