using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightSave : MonoBehaviour
{
    void Start()
    {
        DayNightManager.Instance.LoadData();   
    }
}
