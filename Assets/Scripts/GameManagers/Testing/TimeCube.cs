using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCube : Dialog2
{
    public void ChangeSeason(int season){
        SeasonManager.Instance.SetSeason(season);
    }
}
