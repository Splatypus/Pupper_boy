using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Dialog : InteractableObject {

   
    PlayerDialog pdialog;
    PlayerControllerManager controlman;
    List<List<string>> dialogTexts;
    Dictionary<int, List<string>> options; //list of dialog options
    public int conversationNumber; //this indicates which conversation you're on. 
    public int textBoxNumber; //this indicates which text box you're on within the full array of dialog. 
    public TextAsset file;
    public string nameText;
    public Sprite image;

    public GameObject playercam;
    public GameObject npccam;

    // Use this for initialization
    public void Start () {
        ParseFile(file);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        controlman = player.GetComponent<PlayerControllerManager>();
        pdialog = player.GetComponent<PlayerDialog>(); //find player dialog script on the player and set this to refrence it
	}

    //sets up all the dialog stuff we need by reading from a TextAsset file
    public void ParseFile(TextAsset f) {
        string text = f.ToString();
        if (text == "")
            text = System.Text.Encoding.Default.GetString(f.bytes);
        string[] data = text.Split('\n');
        //loop over every line in the file. Look for //Break which seperates conversations, and //Option which designates dialog options
        int convoNum = -1;
        dialogTexts = new List<List<string>>();
        options = new Dictionary<int, List<string>>();
        for (int i = 0; i < data.Length; i++) {
            if (data[i].StartsWith("//Break")) { //a '//Break' in the file seperates conversations
                convoNum++;
                dialogTexts.Add(new List<string>());

            } else if (data[i].StartsWith("//Option")) { //adds to option dict. This is when player is offered a dialog choice
                string[] opts = data[i].Split('|');
                List<string> toList = new List<string>(opts.Length - 1);
                for (int j = 1; j < opts.Length; j++) {
                    toList.Add(opts[j]);
                }
                options.Add(convoNum, toList);

            } else {    //if no flag, just add it to the current conversation
                dialogTexts[convoNum].Add(data[i]);
            }
        }
    }

    //sends the current dialog stuff over to the player. Sends options stuff too if needed
    public void SendDialog() {
        pdialog.SetDialog(dialogTexts[conversationNumber][textBoxNumber]);
        //after sending over the text, check to see if we also need to send an options thingy
        if (textBoxNumber + 1 == dialogTexts[conversationNumber].Count && options.ContainsKey(conversationNumber))
        {
            List<string> s;
            options.TryGetValue(conversationNumber, out s);
            pdialog.AddOptions(s);
        }
    }

    public override void OnInteract() {
        npccam.SetActive(true);
        playercam.SetActive(false);
        //change player mode to dialog mode when they interact with this npc
        controlman.ChangeMode(PlayerControllerManager.Modes.Dialog);
        pdialog.npcDialog = this;
        //Assign image and name
        pdialog.imageObject.sprite = image;
        pdialog.nameTextObject.text = nameText;
        //set the dialog
        textBoxNumber = 0;
        SendDialog();
    }

    //goes on to the next dialog window. Called from the player dialog when more text is needed.
    public void Next() {
        //either display the next dialog or close window
        textBoxNumber += 1;
        if (textBoxNumber < dialogTexts[conversationNumber].Count) {
            SendDialog();
        } else { 
            controlman.ChangeMode(PlayerControllerManager.Modes.Walking);
            OnEndOfDialog(conversationNumber);
        }
    }

    //called when the end of a dialog section is reached
    public virtual void OnEndOfDialog(int dialogNum) {
        playercam.SetActive(true);
        npccam.SetActive(false);
    }

    //called when a choice is made if players are given options
    //dialog num is the dialog section this choice is given after. choice is which option was selected. 
    public virtual void OnChoiceMade(int choice) {
        playercam.SetActive(true);
        npccam.SetActive(false);
        controlman.ChangeMode(PlayerControllerManager.Modes.Walking);
        OnEndOfDialog(conversationNumber);
    }

    //sets the current conversation number to whatever you pass in. False return if failed or reached the end 
    public bool SetConversationNumber(int c) { 
        if (c < 0 || c > dialogTexts.Count) {
            return false;
        }
        conversationNumber = c;
        return true;
    }

    //if no arguements, it increments it by 1 rather than setting
    public bool SetConversationNumber() {
        if (conversationNumber + 1 > dialogTexts.Count) {
            return false;
        }
        conversationNumber += 1;
        return true;
    }

}
