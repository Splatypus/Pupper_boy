using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        }
    }
}
