using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//When placed in a scene will check the current season on start and send off the proper trigger
public class SeasonDetector : MonoBehaviour
{
    // On start, trigger the season change so everything that cares knows it happened
    void Start() {
        EventManager.Instance.TriggerSeasonChange(SeasonManager.Instance.GetSeason());
    }
}
