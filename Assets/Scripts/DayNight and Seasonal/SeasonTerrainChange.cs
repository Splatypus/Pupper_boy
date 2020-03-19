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
    public Material summerSnowMat;
    public Material winterSnowMat;


    private TerrainData defaultData;

    // Use this for initialization
    void Awake() {
        if (target == null)
            target = gameObject.GetComponent<Terrain>();

        defaultData = target.terrainData;
        EventManager.OnSeasonChange += OnSeasonChanged;
    }
    private void OnDestroy() {
        EventManager.OnSeasonChange -= OnSeasonChanged;
    }

    // called when a season is changed
    void OnSeasonChanged(SeasonManager.Seasons s) {
        //assign material to the given material for the season we changed to. Defaults to the original material if none are given
        TerrainData newData = null;
        switch (s) {
            case SeasonManager.Seasons.SUMMER:
                newData = summer;
                target.materialTemplate = summerSnowMat;
                break;
            case SeasonManager.Seasons.FALL:
                newData = fall;
                target.materialTemplate = summerSnowMat;
                break;
            case SeasonManager.Seasons.WINTER:
                newData = winter;
                target.materialTemplate = winterSnowMat;
                break;
            case SeasonManager.Seasons.SPRING:
                newData = spring;
                target.materialTemplate = summerSnowMat;
                break;
            default:
                break;
        }
        //change terrain and skybox to their new settings, or back to their default
        target.terrainData = (newData != null ? newData : defaultData);
        target.Flush();

        
    }
}
