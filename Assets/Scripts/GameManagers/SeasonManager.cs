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
    public AsyncOperation SetSeason(Seasons newSeason) {
        //first, start unloading the old scene
        if (SceneManager.GetSceneByName(sceneNames[(int)currentSeason]).isLoaded) {
            SceneManager.UnloadSceneAsync(sceneNames[(int)currentSeason]);
        }
        //change season and trigger events
        currentSeason = newSeason;
        //Load in the correct season
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneNames[(int)currentSeason], LoadSceneMode.Additive);
        //save scene change
        SaveManager.getInstance().PutInt(SEASON_KEY, (int)currentSeason);
        SaveManager.getInstance().SaveFile();

        return async;
    }
    public AsyncOperation SetSeason(int newSeason) {
        if (newSeason < 0 || newSeason >= 4) {
            return null;
        }
        return SetSeason((Seasons)newSeason);
    }
}
