/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class DialogWriter : EditorWindow {


    string path = "Assets/Resources/Text/";
    string fileName = "";
    string fileError = "";
    bool fileLoaded = false;

    List<List<string>> conversations = new List<List<string>>();
    int conversationNumber = 0;

    //create the window if its not already open
    [MenuItem("Window/DialogWindow")]
    public static void ShowWindow() {
        DialogWriter window = (DialogWriter)EditorWindow.GetWindow(typeof(DialogWriter));
        window.Show();
    }


    private void OnGUI(){
        //before a specific file is chosen
        if (!fileLoaded)
        {
            fileName = EditorGUILayout.TextField("File Name", fileName);

            //load the file of the given name if it exists
            if (GUILayout.Button("Load"))
            {
                if (System.IO.File.Exists(path + fileName + ".txt"))
                {
                    StreamReader reader = new StreamReader(path + fileName + ".txt");
                    reader.Close();
                    fileLoaded = true;
                }
                else
                {
                    fileError = "File not found";
                }

            }
            //creates a new file of the given name, as long as it doesnt exist
            if (GUILayout.Button("New File"))
            {
                if (System.IO.File.Exists(path + fileName + ".txt"))
                {
                    fileError = "Cannot create new file. A file of that name already exists.";
                }
                else
                {
                    StreamWriter writer = new StreamWriter(path + fileName + ".txt");
                    //writer.WriteLine("test");
                    writer.Close();
                    AssetDatabase.ImportAsset(path + fileName + ".txt");
                    fileLoaded = true;
                }
            }
            if (fileError != "")
                GUILayout.Label(fileError);
        }
        else
        {
            //once a file has been selected
            if (GUILayout.Button("Save")) {     //SAVE

            }

            GUILayout.Label("Convo Editor", EditorStyles.boldLabel);
            //add a new conversation
            if (GUILayout.Button("New Conversation"))
            {
                List<string> convo = new List<string>();            //NEW CONVO
                conversations.Add(convo);
                conversationNumber = conversations.Count - 1;
            }
            //set current convo
            if (conversations.Count > 0){
                string label = "Convo Number (0 to " + (conversations.Count-1) + ")";       //SET CURRENT CONVO
                conversationNumber = Mathf.Clamp(EditorGUILayout.IntField(label, conversationNumber), 0, conversations.Count - 1);
               
                //delete the current one
                if (GUILayout.Button("Delete This Conversation")){      //DELETE CONVO
                    conversations.RemoveAt(conversationNumber);
                    conversationNumber = 0;
                }
            }

            GUILayout.Label("Dialog Writer", EditorStyles.boldLabel);
            //edit dialog
            if (conversationNumber < conversations.Count && GUILayout.Button("Add Text At Start"))  //ADD TEXT BOX TO START
            {
                conversations[conversationNumber].Insert(0, "");
            }
            for (int i = 0; conversationNumber < conversations.Count && i < conversations[conversationNumber].Count; i++)
            {
                GUILayout.Label("Dialog " + i, EditorStyles.boldLabel);
                conversations[conversationNumber][i] = EditorGUILayout.TextField("", conversations[conversationNumber][i]);
                //add a new text box after the this one in the current conversation
                if (GUILayout.Button("Add Text Box After")){                            //ADD TEXT BOX
                    if (i + 1 < conversations[conversationNumber].Count)
                        conversations[conversationNumber].Insert(i + 1, "");
                    else
                        conversations[conversationNumber].Add("");
                }
            }
        }
    }
}
*/