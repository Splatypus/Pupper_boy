using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonTerrainChange : MonoBehaviour
{
    [Tooltip("Set to the terrain on this same object if left null")]
    public Terrain target;
    public TerrainData summer, fall, winter, spring;

    private TerrainData defaultData;

    // Use this for initialization
    void Start() {
        if (target == null)
            target = gameObject.GetComponent<Terrain>();

        defaultData = target.terrainData;
        EventManager.OnSeasonChange += OnSeasonChanged;
    }

    // called when a season is changed
    void OnSeasonChanged(SeasonManager.Seasons s) {
        //assign material to the given material for the season we changed to. Defaults to the original material if none are given
        TerrainData newData = null;
        switch (s) {
            case SeasonManager.Seasons.SUMMER:
                newData = summer;
                break;
            case SeasonManager.Seasons.FALL:
                newData = fall;
                break;
            case SeasonManager.Seasons.WINTER:
                newData = winter;
                break;
            case SeasonManager.Seasons.SPRING:
                newData = spring;
                break;
            default:
                break;
        }

        target.terrainData = (newData != null ? newData : defaultData);
        target.Flush();
    }
}
