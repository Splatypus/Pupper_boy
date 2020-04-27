using System.Collections;
using System.Collections.Generic;
using IL3DN;
using UnityEngine;

public class IL3DNSeasonChange : MonoBehaviour
{

    public IL3DN_ColorManagerTextures colorManager;
    public IL3DN_Snow snowManager;


    void Awake()
    {
        EventManager.OnSeasonChange += OnSeasonChanged;
    }
    
    void OnDestroy()
    {
        EventManager.OnSeasonChange -= OnSeasonChanged;
    }

    // called when a season is changed
    void OnSeasonChanged(SeasonManager.Seasons s)
    {
        //Set colors based on season
        colorManager.SetMaterialColors((int)s);
        //set snow to true in the winter
        snowManager.Snow = (s == SeasonManager.Seasons.WINTER);
    }
}
