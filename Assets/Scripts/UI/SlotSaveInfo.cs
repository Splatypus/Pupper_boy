using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlotSaveInfo : MonoBehaviour {

    [Tooltip("Save slot this UI element is tied to.")]
    public int idNumber;
    public string fileName;
    MainMenuManager manager;

    public void LoadGame() {
        SaveManager.getInstance().LoadFile(idNumber);

        manager.LoadScene();
    }

    public void DeleteSave() {
        SaveManager.getInstance().DeleteFile(idNumber);
        Destroy(gameObject);
    }

    public void FillInfo(int id, string name, MainMenuManager manager) {
        this.idNumber = id;
        this.fileName = name;
        this.manager = manager;

        GetComponentInChildren<Text>().text = name;
    }
}
