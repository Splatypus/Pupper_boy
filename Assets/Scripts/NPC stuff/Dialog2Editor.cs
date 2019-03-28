#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Dialog2), true)]
public class Dialog2Editor : Editor {

    DialogEditorWindow window = null;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();
        Dialog2 myDialog = (Dialog2)target;
        if (GUILayout.Button("Open Node Editor") && window == null)
        {
            window = EditorWindow.GetWindow<DialogEditorWindow>();
            window.SetupWindow(myDialog);
            //UnityEditor.PrefabUtility.DisconnectPrefabInstance(myDialog.gameObject); //No longer used in 2018 forward
        }
    }
}
#endif