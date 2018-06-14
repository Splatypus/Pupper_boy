using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class Dialog2 : MonoBehaviour, ISerializationCallbackReceiver {

    //player references
    PlayerDialog pdialog;
    PlayerControllerManager controlman;

    //display info
    public string characterName;
    public Sprite image;

    //camera references
    public GameObject playercam;
    public GameObject npccam;

    //Progression info
    public DialogNodeStart startNode = null;
    public int progressionNum;

    //EditorInfo
    public List<DialogNode> nodes;
    public List<Connection> connections;
    public List<UnityEvent> functions;

    // Use this for initialization
    void Start () {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        controlman = player.GetComponent<PlayerControllerManager>();
        pdialog = player.GetComponent<PlayerDialog>(); //find player dialog script on the player and set this to refrence it
    }

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
                    temp = new DialogNodeChoice(new Vector2(s.x, s.y), s.width, s.height, null, null, null, null, null) {
                        text = s.text,
                        num = s.num
                    };
                    nodes.Add(temp);
                    break;
                case NodeType.FUNCTION:
                    temp = new DialogNodeFunction(new Vector2(s.x, s.y), s.width, s.height, null, null, null, null, null) {
                        functionNum = s.num
                    };
                    nodes.Add(temp);
                    break;
                case NodeType.BREAK:
                    temp = new DialogNodeBreak(new Vector2(s.x, s.y), s.width, s.height, null, null, null, null, null);
                    nodes.Add(temp);
                    break;
                case NodeType.START:
                    temp = new DialogNodeStart(new Vector2(s.x, s.y), s.width, s.height, null, null, null, null, null);
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
            connections.Add(new Connection(nodes[s.inIndex].inPoint, nodes[s.outIndex].outPoint, null));
            nodes[s.outIndex].connections.Add(nodes[s.inIndex]);
        }

    }


    #region Nodes
    public class DialogNode
    {
        public DialogEditorWindow window;
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
            window = null;
        }

        //Drags the node
        public void Drag(Vector2 delta)
        {
            rect.position += delta;
        }

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
        public DialogNodeDialog(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, DialogEditorWindow windowRef)
            : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle, windowRef)
        {
            text = "";
            title = "Dialog";
        }
        //Serializer constructor
        public DialogNodeDialog(Vector2 position, float width, float height)
            : base(position, width, height)
        {
            text = "";
            title = "Dialog";
        }

        public override void Draw()
        {
            base.Draw();
            Rect textAreaRect = new Rect(rect.x + 20, rect.y + 35, rect.width - 40, rect.height - 55);
            text = EditorGUI.TextArea(textAreaRect, text);
        }

    }

    public class DialogNodeChoice : DialogNode
    {

        public string text;
        public int num;

        public DialogNodeChoice(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, DialogEditorWindow windowRef)
            : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle, windowRef)
        {
            text = "";
            title = "Choice";
            num = 0;
        }

        public override void Draw()
        {
            base.Draw();
            Rect textAreaRect = new Rect(rect.x + 60, rect.y + 30, rect.width - 75, rect.height - 45);
            text = EditorGUI.TextField(textAreaRect, text);
            num = EditorGUI.IntField(new Rect(rect.x + 15, rect.y + 30, 30, 25), num);
        }
    }

    public class DialogNodeFunction : DialogNode
    {
        public int functionNum;

        public DialogNodeFunction(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, DialogEditorWindow windowRef)
            : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle, windowRef)
        {
            functionNum = 0;
            title = "Function";
        }

        public override void Draw()
        {
            base.Draw();
            Rect textAreaRect = new Rect(rect.x + 15, rect.y + 30, rect.width - 30, rect.height - 45);
            functionNum = EditorGUI.IntField(textAreaRect, functionNum);
        }

    }

    public class DialogNodeBreak : DialogNode
    {
        public DialogNodeBreak(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, DialogEditorWindow windowRef)
            : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle, windowRef)
        {
            title = "Break";
        }

        public override void Draw()
        {
            base.Draw();
        }

    }

    public class DialogNodeStart : DialogNode
    {

        public DialogNodeStart(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, DialogEditorWindow windowRef)
            : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle, windowRef)
        {
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
    }

    public class Connection
    {
        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;
        public DialogEditorWindow window;

        public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, DialogEditorWindow windowRef)
        {
            this.inPoint = inPoint;
            this.outPoint = outPoint;
            window = windowRef;
        }

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
    }
    #endregion
}

