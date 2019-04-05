﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SeasonManager : MonoBehaviour {

    public static SeasonManager Instance;
    public enum Seasons { SUMMER, FALL, WINTER, SPRING }
    public string[] sceneNames;
    Seasons currentSeason = Seasons.SUMMER;

    //Game Object singleton
    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    //get and set seasons
    public Seasons GetSeason() {
        return currentSeason;
    }
    public void SetSeason(Seasons newSeason) {
        if (currentSeason == newSeason) {
            return;
        }
        //first, start unloading the old scene
        SceneManager.UnloadSceneAsync(sceneNames[(int)currentSeason]);
        //change season and trigger events
        currentSeason = newSeason;
        EventManager.Instance.TriggerSeasonChange(currentSeason);
        //Load in the correct season
        StartCoroutine(LoadSceneAsync(sceneNames[(int)currentSeason]));
    }
    public void SetSeason(int newSeason) {
        if (newSeason < 0 || newSeason >= 4) {
            return;
        }
        SetSeason((Seasons)newSeason);
    }

    //async call to load a scene aditively based on a given scene name
    IEnumerator LoadSceneAsync(string name) {
        AsyncOperation async = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        while (!async.isDone) {
            yield return null;
        }
        OnSceneLoaded();
    }
    

    //called whenever an async scene finishes loading
    public void OnSceneLoaded() {

    }
}