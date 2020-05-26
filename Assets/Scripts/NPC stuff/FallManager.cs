using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallManager : MonoBehaviour
{
    [HideInInspector] public bool tiffanyDone = false;
    [HideInInspector] public bool rexDone = false;
    [HideInInspector] public bool socksDone = false;
    [HideInInspector] public bool SBDone = false;
    [HideInInspector] public bool chipDone = false;
    [HideInInspector] public bool bubblesDone = false;


    public bool areAllDone() {
        return tiffanyDone && rexDone && socksDone && SBDone && chipDone && bubblesDone;
    }
}
