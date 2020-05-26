using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SeasonManager {
    public static readonly string SEASON_KEY = "Season";

    public enum Seasons { SUMMER, FALL, WINTER, SPRING }
    public static readonly string[] sceneNames = { "UnleashedSummer", "UnleashedFall", "UnleashedWinter", "UnleashedSpring" };
    Seasons currentSeason = Seasons.SUMMER;

    //singleton
    private static SeasonManager instance;
    public static SeasonManager Instance {
        get {
            if (instance == null) {
                instance = new SeasonManager();
            }
            return instance;
        }
    }

    //get and set seasons
    public Seasons GetSeason() {
        return currentSeason;
    }
    public void SetSeason(Seasons newSeason, bool save = true) {
        //change season and trigger events
        currentSeason = newSeason;
        //Load in the correct season
        string[] scenes = { "UnleashedBackyard", sceneNames[(int)currentSeason] };
        LoadingScreen.LoadingScreenToScenes(scenes);

        if (save){
            //save scene change
            SaveManager.getInstance().PutInt(SEASON_KEY, (int)currentSeason);
            SaveManager.getInstance().SaveFile();
        }
        
    }
    public void SetSeason(int newSeason, bool save = true) {
        if (newSeason < 0 || newSeason >= 4) {
            Debug.LogError("Set Season called on invalid season number");
            return;
        }
        SetSeason((Seasons)newSeason, save);
    }
}
