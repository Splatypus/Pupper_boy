﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlotSaveInfo : MonoBehaviour {

    [Tooltip("Save slot this UI element is tied to.")]
    public int slotNumber;

    public void LoadGame() {
        FindObjectOfType<SaveManager>().LoadGameFromSlot(slotNumber);

        SceneManager.LoadSceneAsync(1);
    }
}