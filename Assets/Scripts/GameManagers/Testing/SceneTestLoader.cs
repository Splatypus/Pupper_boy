using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script to run some specific things before a scene loads
 * 
 * **/

public class SceneTestLoader : MonoBehaviour
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadGameobject() {

        //only create a test save if we're loading into the backyard with no save files
        if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("UnleashedBackyard")) {
            return;
        }

        SaveManager save = SaveManager.getInstance();

        if (save.GetLastOpenID() != -1) {
            save.LoadFile(save.GetLastOpenID());
        } else {
            save.CreateFile("TestFile pls ignore");
        }
    }
}
