using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToMultiScene : MonoBehaviour
{

    public string key;

    private void Awake() {
        CrossSceneReferences.GetInstance().PutObject(key, gameObject);
    }
}
