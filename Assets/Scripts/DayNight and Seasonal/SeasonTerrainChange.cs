using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonTerrainChange : MonoBehaviour
{
    [Tooltip("Set to the terrain on this same object if left null")]
    public Terrain target;
    [Header("Terrain Data")]
    public TerrainData summer;
    public TerrainData fall, winter, spring;
    [Header("Terrain Material")]
    public Material winterSnowMat;
    [Header("Skybox")]
    public Material summerSky;
    public Material fallSky, winterSky, springSky;

    private TerrainData defaultData;
    private Material defaultSkybox;

    // Use this for initialization
    void Start() {
        if (target == null)
            target = gameObject.GetComponent<Terrain>();

        defaultData = target.terrainData;
        defaultSkybox = RenderSettings.skybox;
        EventManager.OnSeasonChange += OnSeasonChanged;
    }

    // called when a season is changed
    void OnSeasonChanged(SeasonManager.Seasons s) {
        //assign material to the given material for the season we changed to. Defaults to the original material if none are given
        TerrainData newData = null;
        Material newSkyBox = null;
        switch (s) {
            case SeasonManager.Seasons.SUMMER:
                newSkyBox = summerSky;
                newData = summer;
                target.materialType = Terrain.MaterialType.BuiltInStandard;
                break;
            case SeasonManager.Seasons.FALL:
                newSkyBox = fallSky;
                newData = fall;
                target.materialType = Terrain.MaterialType.BuiltInStandard;
                break;
            case SeasonManager.Seasons.WINTER:
                newSkyBox = winterSky;
                newData = winter;
                target.materialType = Terrain.MaterialType.Custom;
                target.materialTemplate = winterSnowMat;
                break;
            case SeasonManager.Seasons.SPRING:
                newSkyBox = springSky;
                newData = spring;
                target.materialType = Terrain.MaterialType.BuiltInStandard;
                break;
            default:
                break;
        }
        //change terrain and skybox to their new settings, or back to their default
        target.terrainData = (newData != null ? newData : defaultData);
        target.Flush();

        RenderSettings.skybox = (newSkyBox != null ? newSkyBox : defaultSkybox);
    }
}
