using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Dialog2 : InteractableObject, ISerializationCallbackReceiver {

    //player references
    PlayerDialog pdialog;
    PlayerControllerManager controlman;

    //display info
    public string characterName;
    public Sprite image;

    //camera references
    public float dynamicCameraDistance = 4.0f;
    public float dynamicCameraHeight = 2.5f;
    public float dynamicCameraTime = 1.0f;

    //Progression info
    public DialogNodeStart startNode = null;
    DialogNode currentNode;
    public int progressionNum;

    //EditorInfo
    public List<DialogNode> nodes;
    public List<Connection> connections;
    public List<UnityEvent> functions;

    // Use this for initialization
    public virtual void Start () {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        controlman = player.GetComponent<PlayerControllerManager>();
        pdialog = player.GetComponent<PlayerDialog>(); //find player dialog script on the player and set this to refrence it
        //serialize this later
        foreach (DialogNode n in nodes) {
            if (n is DialogNodeStart)
                startNode = (DialogNodeStart)n;
        }
        currentNode = startNode;
    }

    public override void OnInteract() {
        //if this NPC has no more dialog, do nothing
        if (currentNode.connections == null || currentNode.connections.Count == 0) {
            return;
        }
        //set up dialog nodes
        if (currentNode is DialogNodeBreak) {
            bool canContinue = false;
            int numChoices = 0; //number of choice nodes attached. If 0, then continue to the next node no matter what
            foreach (DialogNode c in currentNode.connections) {
                if (c is DialogNodeChoice) {
                    numChoices++;
                    if (((DialogNodeChoice)c).num == progressionNum) {
                        ChangeNode(c);
                        canContinue = true;
                    }
                }
            }
            if (numChoices == 0) {
                ChangeNode(currentNode.connections[0]);
            } else if (!canContinue) {
                return;
            } else {
                progressionNum = 0; //reset progression num if its been used
            }
        } else if (currentNode is DialogNodeStart) {
            //procede to the next node
            if (currentNode.connections != null) {
                int numChoices = 0; //number of choice nodes attached. If 0, then continue to the next node no matter what
                foreach (DialogNode c in currentNode.connections) {
                    if (c is DialogNodeChoice) {
                        numChoices++;
                        if (((DialogNodeChoice)c).num == progressionNum) {
                            ChangeNode(c);
                        }
                    }
                }
                if (numChoices == 0) {
                    ChangeNode(currentNode.connections[0]);
                } else {
                    progressionNum = 0; //reset progression num if its been used
                }
            } else {
                Debug.LogError("Start node placed with no out connection. Dialog bugging out.");
            }
        }

        //make two vectors pointing away from the plane created by the two dogs talking. Then move the camera to the closer of the two.
        Vector3 midpoint = Vector3.Lerp(pdialog.transform.position, transform.position, 0.5f);
        Vector3 position1 = midpoint + Vector3.Cross(transform.position - pdialog.transform.position, Vector3.up).normalized * dynamicCameraDistance + Vector3.up * dynamicCameraHeight;
        Vector3 position2 = midpoint + Vector3.Cross(pdialog.transform.position - transform.position, Vector3.up).normalized * dynamicCameraDistance + Vector3.up * dynamicCameraHeight;
        
        Vector3 cameraPosition = Vector3.zero;
        if (Vector3.Distance(position1, Camera.main.transform.position) < Vector3.Distance(position2, Camera.main.transform.position))
            cameraPosition = position1;
        else
            cameraPosition = position2;
        //Vector3 cameraPosition = Vector3.Distance(position1, Camera.main.transform.position) < Vector3.Distance(position2, Camera.main.transform.position) ? position1 : position2;
        Camera.main.GetComponent<FreeCameraLook>().MoveToPosition(cameraPosition, midpoint, dynamicCameraTime);

        //change player mode to dialog mode when they interact with this npc
        controlman.ChangeMode(PlayerControllerManager.Modes.Dialog);
        pdialog.npcDialog = this;
        //Assign image and name
        pdialog.imageObject.sprite = image;
        pdialog.nameTextObject.text = characterName;
    }

    public void OnEnd() {
        controlman.ChangeMode(PlayerControllerManager.Modes.Walking);
        Camera.main.GetComponent<FreeCameraLook>().RestoreCamera(dynamicCameraTime);
    }

    //should be called to swap the current node. 
    void ChangeNode(DialogNode node) {
        //progress current node
        currentNode = node;
        //different effects depending on which node we've reached
        if (node is DialogNodeDialog) { //dialog
            //Diaplay up new dialog
            SendDialog();
        } else if (node is DialogNodeChoice) { //choice
            //procede to whatever comes after this choice. If nothing is after it, display error.
            if (node.connections != null) {
                ChangeNode(node.connections[0]);
            } else {
                Debug.LogError("Choice node placed with no out connection. Dialog bugging out.");
            }
        } else if (node is DialogNodeFunction) {  //function
            //Run the function specified by the given number. If the number is out of bounds of the function array, dont run anything. Then procede to the next node if it exists.
            int num = ((DialogNodeFunction)node).functionNum;
            if ( num < functions.Count && num >= 0) {
                functions[num].Invoke();
            }
            if(node.connections != null) {
                ChangeNode(node.connections[0]);
            } else {
                Debug.LogError("Function node placed with no out connection. Dialog bugging out.");
            }
        } else if (node is DialogNodeBreak) {  //break
            //if dialog is going, end it.
            OnEnd();
        } else if (node is DialogNodeStart) { //start
            //procede to the next node
            if(node.connections != null) {
                ChangeNode(node.connections[0]);
            } else {
                Debug.LogError("Start node placed with no out connection. Dialog bugging out.");
            }
        }
    }

    public void SendDialog() {
        //if this node is a dialog node, then send over the text contents
        if (currentNode is DialogNodeDialog) {
            pdialog.SetDialog(((DialogNodeDialog)currentNode).text);
            //if the connected nodes are all choices, set up those as well
            foreach (DialogNode n in currentNode.connections) {
                if (n is DialogNodeChoice) {
                    pdialog.AddOption(((DialogNodeChoice)n).num, ((DialogNodeChoice)n).text);
                }
            }
        }
    }

    //procede to the next dialog. PlayerDialogScript prevents this from being called while buttons are active, so it shouldnt skip over choices
    public void Next() {
        if (currentNode is DialogNodeDialog && currentNode.connections != null) {
            ChangeNode(currentNode.connections[0]);
        }
    }

    public void OnChoiceMade(int choice) {
        foreach (DialogNode n in currentNode.connections) {
            if (n is DialogNodeChoice) {
                if (((DialogNodeChoice)n).num == choice && n.connections != null) {
                    ChangeNode(n);
                }
            }
        }
    }

    #region serialization
    //Serialization
    [HideInInspector, SerializeField]
    List<SerializedNode> serializedNodes;
    [HideInInspector, SerializeField]
    List<SerializedConnection> serializedConnections;
    [System.Serializable]
    public enum NodeType { DIALOG, CHOICE, FUNCTION, BREAK, START}

    [System.Serializable]
    public struct SerializedNode {
        public float x, y, width, height;
        public string text;
        public int num;
        public NodeType type;
    }
    [System.Serializable]
    public struct SerializedConnection {
        public int outIndex, inIndex;
    }

    public void OnBeforeSerialize(){
        //Setup node list
        if (serializedNodes == null)
            serializedNodes = new List<SerializedNode>();
        serializedNodes.Clear();

        //loop over the nodes, serialize, and add them
        if (nodes != null) {
            for (int i = 0; i < nodes.Count; i++) {
                SerializedNode s = new SerializedNode {
                    x = nodes[i].rect.x,
                    y = nodes[i].rect.y,
                    width = nodes[i].rect.width,
                    height = nodes[i].rect.height,
                    text = "",
                    num = -1
                };
                //check type of node and add details specific to that type
                if (nodes[i] is DialogNodeDialog) { //dialog
                    s.type = NodeType.DIALOG;
                    s.text = ((DialogNodeDialog)nodes[i]).text;
                } else if (nodes[i] is DialogNodeChoice) { //choice
                    s.type = NodeType.CHOICE;
                    s.num = ((DialogNodeChoice)nodes[i]).num;
                    s.text = ((DialogNodeChoice)nodes[i]).text;
                } else if (nodes[i] is DialogNodeFunction) {  //function
                    s.type = NodeType.FUNCTION;
                    s.num = ((DialogNodeFunction)nodes[i]).functionNum;
                } else if (nodes[i] is DialogNodeBreak) {  //break
                    s.type = NodeType.BREAK;
                } else if (nodes[i] is DialogNodeStart) { //start
                    s.type = NodeType.START;
                }
                //add the fully serialized node to the list
                nodes[i].index = serializedNodes.Count; //stores its index in the serialized node list so that connections can be serialized
                serializedNodes.Add(s);
            }
        }


        //then do connections
        if (serializedConnections == null)
            serializedConnections = new List<SerializedConnection>();
        serializedConnections.Clear();

        if (connections != null) {
            for (int i = 0; i < connections.Count; i++) {
                SerializedConnection s = new SerializedConnection { };
                s.inIndex = connections[i].inPoint.node.index;
                s.outIndex = connections[i].outPoint.node.index;
                serializedConnections.Add(s);
            }
        }
    }

    public void OnAfterDeserialize(){

        //Remake or clear nodes list
        if (nodes == null)
            nodes = new List<DialogNode>();
        nodes.Clear();

        foreach (SerializedNode s in serializedNodes) {
            //Create new nodes based on the serialized ones
            DialogNode temp;
            switch (s.type) {
                case NodeType.DIALOG:
                    temp = new DialogNodeDialog(new Vector2(s.x, s.y), s.width, s.height) {
                        text = s.text
                    };
                    nodes.Add(temp);
                    break;
                case NodeType.CHOICE:
                    temp = new DialogNodeChoice(new Vector2(s.x, s.y), s.width, s.height) {
                        text = s.text,
                        num = s.num
                    };
                    nodes.Add(temp);
                    break;
                case NodeType.FUNCTION:
                    temp = new DialogNodeFunction(new Vector2(s.x, s.y), s.width, s.height) {
                        functionNum = s.num
                    };
                    nodes.Add(temp);
                    break;
                case NodeType.BREAK:
                    temp = new DialogNodeBreak(new Vector2(s.x, s.y), s.width, s.height);
                    nodes.Add(temp);
                    break;
                case NodeType.START:
                    temp = new DialogNodeStart(new Vector2(s.x, s.y), s.width, s.height);
                    nodes.Add(temp);
                    break;
                default:
                    break;
            }
        }

        //Then do the list of connections
        if (connections == null)
            connections = new List<Connection>();
        connections.Clear();

        foreach (SerializedConnection s in serializedConnections) {
            connections.Add(new Connection(nodes[s.inIndex].inPoint, nodes[s.outIndex].outPoint));
            nodes[s.outIndex].connections.Add(nodes[s.inIndex]);
        }

    }
    #endregion

    #region Nodes
    public class DialogNode
    {
        #if UNITY_EDITOR
        public DialogEditorWindow window;
        #endif
        //DrawingStuff
        public Rect rect;
        public string title;
        public bool isDragged;
        public bool isSelected;
        //connections
        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;
        //style
        public GUIStyle style;
        public GUIStyle defaultStyle;
        public GUIStyle selectedStyle;

        //public UnityAction<DialogNode> OnRemoveNode;

        public List<DialogNode> connections;
        public int index; //WARNING: index is only its accurate position in the nodes list right before and after serialization.


        //public DialogLinkedNode linkedNode;

        #if UNITY_EDITOR
        //constructer
        public DialogNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, DialogEditorWindow windowRef)
        {
            rect = new Rect(position.x, position.y, width, height);
            style = nodeStyle;
            inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle);
            outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle);
            defaultStyle = nodeStyle;
            selectedStyle = _selectedStyle;
            connections = new List<DialogNode>();
            window = windowRef;
        }
        #endif
        //serialization constructor
        public DialogNode(Vector2 position, float width, float height)
        {
            rect = new Rect(position.x, position.y, width, height);
            style = null;
            inPoint = new ConnectionPoint(this, ConnectionPointType.In, null);
            outPoint = new ConnectionPoint(this, ConnectionPointType.Out, null);
            defaultStyle = null;
            selectedStyle = null;
            connections = new List<DialogNode>();
        }

        //Drags the node
        public void Drag(Vector2 delta)
        {
            rect.position += delta;
        }

        #if UNITY_EDITOR
        //draw function
        public virtual void Draw()
        {
            inPoint.Draw();
            outPoint.Draw();
            GUI.Box(rect, "", style);
            EditorGUI.LabelField(new Rect(rect.x + 20, rect.y + 10, rect.width, 15), title);
        }

        public bool ProcessEvents(Event e)
        {
            switch (e.type) {
                case EventType.MouseDown:
                    //on left click, check if is inside bounds
                    if (e.button == 0) {
                        if (rect.Contains(e.mousePosition)) {
                            isDragged = true;
                            GUI.changed = true;
                            style = selectedStyle;
                            isSelected = true;
                        } else {
                            GUI.changed = true;
                            style = defaultStyle;
                            isSelected = false;
                            //GUI.FocusControl("");
                        }
                    }
                    //on right click in bounds, open menu
                    if (e.button == 1 && isSelected && rect.Contains(e.mousePosition)) {
                        ProcessContextMenu();
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged) {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }
            return false;
        }

        //opens menu
        private void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
            genericMenu.ShowAsContext();
        }

        //removes node
        private void OnClickRemoveNode()
        {
            window.OnClickRemoveNode(this);
        }
        #endif

        //Connects this node to a new one in the linked dialog representation
        public virtual void LinkNode(DialogNode d)
        {
            connections.Add(d);
        }

    }

    public class DialogNodeDialog : DialogNode
    {

        public string text;
        //node editor constructor
        #if UNITY_EDITOR
        public DialogNodeDialog(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, DialogEditorWindow windowRef)
            : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle, windowRef)
        {
            text = "";
            title = "Dialog";
        }
        #endif
        //Serializer constructor
        public DialogNodeDialog(Vector2 position, float width, float height)
            : base(position, width, height)
        {
            text = "";
            title = "Dialog";
        }

        #if UNITY_EDITOR
        public override void Draw()
        {
            base.Draw();
            Rect textAreaRect = new Rect(rect.x + 20, rect.y + 35, rect.width - 40, rect.height - 55);
            text = EditorGUI.TextArea(textAreaRect, text);
        }
        #endif

    }

    public class DialogNodeChoice : DialogNode{

        public string text;
        public int num;

        #if UNITY_EDITOR
        public DialogNodeChoice(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, DialogEditorWindow windowRef)
            : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle, windowRef)
        {
            text = "";
            title = "Choice";
            num = 0;
        }
        #endif

        //Serializer constructor
        public DialogNodeChoice(Vector2 position, float width, float height)
            : base(position, width, height) {
            text = "";
            title = "Choice";
            num = 0;
        }

#if UNITY_EDITOR
        public override void Draw()
        {
            base.Draw();
            Rect textAreaRect = new Rect(rect.x + 60, rect.y + 30, rect.width - 75, rect.height - 45);
            text = EditorGUI.TextField(textAreaRect, text);
            num = EditorGUI.IntField(new Rect(rect.x + 15, rect.y + 30, 30, 25), num);
        }
        #endif
    }

    public class DialogNodeFunction : DialogNode
    {
        public int functionNum;

        #if UNITY_EDITOR
        public DialogNodeFunction(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, DialogEditorWindow windowRef)
            : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle, windowRef)
        {
            functionNum = 0;
            title = "Function";
        }
        #endif
        //Serializer constructor
        public DialogNodeFunction(Vector2 position, float width, float height)
            : base(position, width, height) {
            functionNum = 0;
            title = "Function";
        }

        #if UNITY_EDITOR
        public override void Draw()
        {
            base.Draw();
            Rect textAreaRect = new Rect(rect.x + 15, rect.y + 30, rect.width - 30, rect.height - 45);
            functionNum = EditorGUI.IntField(textAreaRect, functionNum);
        }
        #endif
    }

    public class DialogNodeBreak : DialogNode
    {
        #if UNITY_EDITOR
        public DialogNodeBreak(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, DialogEditorWindow windowRef)
            : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle, windowRef)
        {
            title = "Break";
        }
        #endif

        //Serializer constructor
        public DialogNodeBreak(Vector2 position, float width, float height)
            : base(position, width, height) {
            title = "Break";
        }

        #if UNITY_EDITOR
        public override void Draw()
        {
            base.Draw();
        }
        #endif

    }

    public class DialogNodeStart : DialogNode
    {

        #if UNITY_EDITOR
        public DialogNodeStart(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, DialogEditorWindow windowRef)
            : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle, windowRef)
        {
            title = "Start";
        }
        #endif
        //Serializer constructor
        public DialogNodeStart(Vector2 position, float width, float height)
            : base(position, width, height) {
            title = "Start";
        }

    }

    public enum ConnectionPointType { In, Out }
    public class ConnectionPoint
    {
        public Rect rect;
        public ConnectionPointType type;
        public DialogNode node;
        public GUIStyle style;

        public ConnectionPoint(DialogNode node, ConnectionPointType type, GUIStyle style)
        {
            this.node = node;
            this.type = type;
            this.style = style;
            rect = new Rect(0, 0, 10f, 20f);
        }
        //without styles
        public ConnectionPoint(DialogNode node, ConnectionPointType type)
        {
            this.node = node;
            this.type = type;
            rect = new Rect(0, 0, 10f, 20f);
        }
#if UNITY_EDITOR
        public void Draw()
        {
            rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;

            switch (type)
            {
                case ConnectionPointType.In:
                    rect.x = node.rect.x - rect.width + 8f;
                    break;

                case ConnectionPointType.Out:
                    rect.x = node.rect.x + node.rect.width - 8f;
                    break;
            }

            if (GUI.Button(rect, "", style))
            {
                switch (type)
                {
                    case ConnectionPointType.In:
                        node.window.OnClickInPoint(this);
                        break;

                    case ConnectionPointType.Out:
                        node.window.OnClickOutPoint(this);
                        break;
                }
            }
        }
#endif
    }

    public class Connection
    {
        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;
        #if UNITY_EDITOR
        public DialogEditorWindow window;
        #endif

        #if UNITY_EDITOR
        public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, DialogEditorWindow windowRef)
        {
            this.inPoint = inPoint;
            this.outPoint = outPoint;
            window = windowRef;
        }
        #endif

        public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint) {
            this.inPoint = inPoint;
            this.outPoint = outPoint;
        }

        #if UNITY_EDITOR
        public void Draw()
        {
            Handles.DrawBezier(
                inPoint.rect.center,
                outPoint.rect.center,
                inPoint.rect.center + Vector2.left * 50f,
                outPoint.rect.center - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
            {
                window.OnClickRemoveConnection(this);
            }
        }
        #endif
    }
    #endregion
}

