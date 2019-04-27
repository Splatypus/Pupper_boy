using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxChange : MonoBehaviour
{

    [Header("Skybox")]
    public Season Summer;
    public Season Fall, Winter, Spring;

    public Color dayFog;
    public Color nightFog;

    private Material defaultSkybox;
    private Season currentSeason;
    private bool isDay = true;

    private void Awake() {
        defaultSkybox = RenderSettings.skybox;
        dayFog = RenderSettings.fogColor;
        EventManager.OnSeasonChange += OnSeasonChanged;
        EventManager.OnDay += OnDay;
        EventManager.OnNight += OnNight;
    }

    private void OnDestroy() {
        EventManager.OnSeasonChange -= OnSeasonChanged;
        EventManager.OnDay -= OnDay;
        EventManager.OnNight -= OnNight;
    }

    public void OnSeasonChanged(SeasonManager.Seasons s) {
        Material newSkyBox = null;
        switch (s) {
            case SeasonManager.Seasons.SUMMER:
                currentSeason = Summer;
                newSkyBox = GetSkybox(Summer, isDay);
                break;
            case SeasonManager.Seasons.FALL:
                currentSeason = Fall;
                newSkyBox = GetSkybox(Fall, isDay);
                break;
            case SeasonManager.Seasons.WINTER:
                currentSeason = Winter;
                newSkyBox = GetSkybox(Winter, isDay);
                break;
            case SeasonManager.Seasons.SPRING:
                currentSeason = Spring;
                newSkyBox = GetSkybox(Spring, isDay);
                break;
            default:
                break;
        }

        RenderSettings.skybox = (newSkyBox ?? defaultSkybox);
    }

    public void OnDay() {
        isDay = true;
        RenderSettings.skybox = (currentSeason.dayBox ?? defaultSkybox);
        RenderSettings.fogColor = dayFog;
    }

    public void OnNight() {
        isDay = false;
        RenderSettings.skybox = (currentSeason.nightBox ?? defaultSkybox);
        RenderSettings.fogColor = nightFog;
    }

    public Material GetSkybox(Season s, bool isDay) {
        return ((isDay ? s.dayBox : s.nightBox) ?? defaultSkybox);
    }

    [Serializable]
    public class Season {
        public Material dayBox;
        public Material nightBox;
    }

}
