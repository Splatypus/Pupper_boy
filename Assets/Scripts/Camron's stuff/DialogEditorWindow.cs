#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;

public class DialogEditorWindow : EditorWindow {

    public Dialog2 connectedDialog;
    
    //moved to dialog
    //List<DialogNode> nodes;
    //List<Connection> connections;
    GUIStyle nodeStyle;
    GUIStyle selectedNodeStyle;
    GUIStyle inPointStyle;
    GUIStyle outPointStyle;

    Dialog2.ConnectionPoint selectedInPoint;
    Dialog2.ConnectionPoint selectedOutPoint;

    Vector2 offset;

    //TextAsset textFile;

    DialogEditorWindow window;

    //[MenuItem("Window/Dialog Editor")]
    public void SetupWindow(Dialog2 dialogRef) {
        connectedDialog = dialogRef;
        //update the window reference for each node
        if (connectedDialog.nodes != null) {
            foreach (Dialog2.DialogNode n in connectedDialog.nodes) {
                n.window = this;
                n.style = nodeStyle;
                n.inPoint.style = inPointStyle;
                n.outPoint.style = outPointStyle;
                n.selectedStyle = selectedNodeStyle;
                n.defaultStyle = nodeStyle;
            }
        }
        if (connectedDialog.connections != null) {
            foreach (Dialog2.Connection c in connectedDialog.connections) {
                c.window = this;
            }
        }
    }

    private void OnEnable()
    {
        window = GetWindow<DialogEditorWindow>();
        //Standard node style
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        //selected node
        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        //in point style
        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);
        //out point
        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

        EditorStyles.textField.wordWrap = true;
    }

    private void OnDisable(){
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
    }

    void OnGUI(){
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);
        DrawNodes();
        DrawConnections();
        DrawConnectionLine(Event.current);
        ProcessEvents(Event.current);
        /*textFile = (TextAsset)EditorGUI.ObjectField(new Rect(10, 10, 200, 20), "Text File:", textFile, typeof(TextAsset), false);
        if (GUI.Button(new Rect(220, 10, 50, 20), "Load")) {
            //well lol fuck me
        }
        if (GUI.Button(new Rect(280, 10, 50, 20), "Save")){
            SaveToFile(textFile);
        }*/
        if (GUI.changed) {
            Repaint();
        }
    }

    void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor){
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        //offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++){
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++){
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    void DrawConnections(){
        if (connectedDialog.connections != null){
            for (int i = 0; i < connectedDialog.connections.Count; i++){
                connectedDialog.connections[i].Draw();
            }
        }
    }

    void DrawConnectionLine(Event e) {
        if (selectedInPoint != null && selectedOutPoint == null){
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null){
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    void DrawNodes() {
        if (connectedDialog.nodes != null) {
            for (int i = connectedDialog.nodes.Count-1; i >= 0; i--) {
                connectedDialog.nodes[i].Draw();
            }
        }
    }

    void ProcessEvents(Event e) {
        //first do node events
        if (connectedDialog.nodes != null) {
            foreach (Dialog2.DialogNode n in connectedDialog.nodes) {
                bool guiChanged = n.ProcessEvents(e);
                if (guiChanged) {
                    GUI.changed = true;
                }
            }
        }
        //the other events
        switch (e.type){
            case EventType.MouseDown:

                GUI.FocusControl(null);
                if (e.button == 0){
                    ClearConnectionSelection();
                }

                if (e.button == 1){
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0){
                    OnDrag(e.delta);
                }
                break;
        }
    }

    void ProcessContextMenu(Vector2 mousePosition){
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Text"), false, () => OnClickAddNode(mousePosition));
        genericMenu.AddItem(new GUIContent("Add Choice"), false, () => OnClickChoiceNode(mousePosition));
        genericMenu.AddItem(new GUIContent("Add Function Call"), false, () => OnClickFunctionNode(mousePosition));
        genericMenu.AddItem(new GUIContent("Add Break"), false, () => OnClickBreakNode(mousePosition));
        genericMenu.AddItem(new GUIContent("Add Start"), false, () => OnClickStartNode(mousePosition));
        genericMenu.ShowAsContext();
    }
    void OnClickAddNode(Vector2 mousePosition){
        if (connectedDialog.nodes == null){
            connectedDialog.nodes = new List<Dialog2.DialogNode>();
        }
        connectedDialog.nodes.Add(new Dialog2.DialogNodeDialog(mousePosition, 200, 150, nodeStyle, selectedNodeStyle,inPointStyle, outPointStyle, this));
    }
    void OnClickChoiceNode(Vector2 mousePosition) {
        if (connectedDialog.nodes == null){
            connectedDialog.nodes = new List<Dialog2.DialogNode>();
        }
        connectedDialog.nodes.Add(new Dialog2.DialogNodeChoice(mousePosition, 180, 70, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, this));
    }
    void OnClickFunctionNode(Vector2 mousePosition) {
         if (connectedDialog.nodes == null){
            connectedDialog.nodes = new List<Dialog2.DialogNode>();
        }
        connectedDialog.nodes.Add(new Dialog2.DialogNodeFunction(mousePosition, 80, 70, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, this));
    }
    void OnClickBreakNode(Vector2 mousePosition)
    {
        if (connectedDialog.nodes == null)
        {
            connectedDialog.nodes = new List<Dialog2.DialogNode>();
        }
        connectedDialog.nodes.Add(new Dialog2.DialogNodeBreak(mousePosition, 70, 40, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, this));
    }
    void OnClickStartNode(Vector2 mousePosition) {
        if (connectedDialog.nodes == null)
        {
            connectedDialog.nodes = new List<Dialog2.DialogNode>();
        }
        bool hasStartNode = false;
        foreach (Dialog2.DialogNode n in connectedDialog.nodes) {
            if (n.GetType() == typeof(Dialog2.DialogNodeStart)) {
                hasStartNode = true;
            }
        }
        if (hasStartNode) { 
            Debug.LogError("Only 1 start node may exist at a time in a dialog set");
        } else {
            Dialog2.DialogNodeStart temp = new Dialog2.DialogNodeStart(mousePosition, 70, 70, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, this);
            connectedDialog.nodes.Add(temp);
            connectedDialog.startNode = temp;
        }
    }
    public void OnClickInPoint(Dialog2.ConnectionPoint inPoint){
        selectedInPoint = inPoint;

        if (selectedOutPoint != null){
            if (selectedOutPoint.node != selectedInPoint.node){
                CreateConnection();
                ClearConnectionSelection();
            } else{
                ClearConnectionSelection();
            }
        }
    }
    public void OnClickOutPoint(Dialog2.ConnectionPoint outPoint){
        selectedOutPoint = outPoint;

        if (selectedInPoint != null){
            if (selectedOutPoint.node != selectedInPoint.node){
                CreateConnection();
                ClearConnectionSelection();
            }else{
                ClearConnectionSelection();
            }
        }
    }
    public void OnClickRemoveConnection(Dialog2.Connection connection)
    {
        //remove it from general connections list
        connectedDialog.connections.Remove(connection);
        //remove it from each node's list of things it connects to
        connection.outPoint.node.connections.Remove(connection.inPoint.node);
    }
    void CreateConnection()
    {
        if (connectedDialog.connections == null)
            connectedDialog.connections = new List<Dialog2.Connection>();

        connectedDialog.connections.Add(new Dialog2.Connection(selectedInPoint, selectedOutPoint, this));
        //Link the two nodes in the dialog script
        selectedOutPoint.node.LinkNode(selectedInPoint.node);
    }
    void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }
    public void OnClickRemoveNode(Dialog2.DialogNode node){
        if (connectedDialog.connections != null){
            //removes connections when a node is deleted.
            List<Dialog2.Connection> connectionsToRemove = new List<Dialog2.Connection>();

            for (int i = 0; i < connectedDialog.connections.Count; i++){
                if (connectedDialog.connections[i].inPoint == node.inPoint || connectedDialog.connections[i].outPoint == node.outPoint){
                    connectionsToRemove.Add(connectedDialog.connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++){
                OnClickRemoveConnection(connectionsToRemove[i]);
            }
            connectionsToRemove = null;
        }

        connectedDialog.nodes.Remove(node);
    }
    void OnDrag(Vector2 delta) {
        if (connectedDialog.nodes != null) {
            foreach (Dialog2.DialogNode n in connectedDialog.nodes) {
                n.Drag(delta);
            }
        }
        offset += delta;
        GUI.changed = true;
    }
}
/*
public class DialogNode {
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

    public UnityAction<DialogNode> OnRemoveNode;

    public List<DialogNode> connections;


    //public DialogLinkedNode linkedNode;

    //constructer
    public DialogNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, UnityAction<ConnectionPoint> OnClickInPoint, UnityAction<ConnectionPoint> OnClickOutPoint, UnityAction<DialogNode> OnClickRemoveNode){
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
        defaultStyle = nodeStyle;
        selectedStyle = _selectedStyle;
        OnRemoveNode = OnClickRemoveNode;
        connections = new List<DialogNode>();
    }

    //Drags the node
    public void Drag(Vector2 delta) {
        rect.position += delta;
    }

    //draw function
    public virtual void Draw() {
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(rect, "", style);
        EditorGUI.LabelField(new Rect(rect.x + 20, rect.y + 10, rect.width, 15), title);
    }

    public bool ProcessEvents(Event e) {
        switch (e.type){
            case EventType.MouseDown:
                //on left click, check if is inside bounds
                if (e.button == 0){
                    if (rect.Contains(e.mousePosition)){
                        isDragged = true;
                        GUI.changed = true;
                        style = selectedStyle;
                        isSelected = true;
                    }
                    else{
                        GUI.changed = true;
                        style = defaultStyle;
                        isSelected = false;
                        //GUI.FocusControl("");
                    }
                }
                //on right click in bounds, open menu
                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition)){
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged){
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
        return false;
    }

    //opens menu
    private void ProcessContextMenu(){
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    //removes node
    private void OnClickRemoveNode(){
        if (OnRemoveNode != null){
            OnRemoveNode(this);
        }
    }

    //Connects this node to a new one in the linked dialog representation
    public virtual void LinkNode(DialogNode d) {
        connections.Add(d);
    }

}

public class DialogNodeDialog : DialogNode {

    string text;

    public DialogNodeDialog(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, UnityAction<ConnectionPoint> OnClickInPoint, UnityAction<ConnectionPoint> OnClickOutPoint, UnityAction<DialogNode> OnClickRemoveNode) 
        : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle,OnClickInPoint, OnClickOutPoint, OnClickRemoveNode) {
        text = "";
        title = "Dialog2";
    }

    public override void Draw(){
        base.Draw();
        Rect textAreaRect = new Rect(rect.x + 20, rect.y + 35, rect.width - 40, rect.height - 55);
        text = EditorGUI.TextArea(textAreaRect, text);
    }
}

public class DialogNodeChoice : DialogNode
{

    public string text;
    public int num;

    public DialogNodeChoice(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, UnityAction<ConnectionPoint> OnClickInPoint, UnityAction<ConnectionPoint> OnClickOutPoint, UnityAction<DialogNode> OnClickRemoveNode)
        : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode)
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

    public DialogNodeFunction(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, UnityAction<ConnectionPoint> OnClickInPoint, UnityAction<ConnectionPoint> OnClickOutPoint, UnityAction<DialogNode> OnClickRemoveNode)
        : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode)
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
    public DialogNodeBreak(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, UnityAction<ConnectionPoint> OnClickInPoint, UnityAction<ConnectionPoint> OnClickOutPoint, UnityAction<DialogNode> OnClickRemoveNode)
        : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode)
    {
        title = "Break";
    }

    public override void Draw()
    {
        base.Draw();
    }

}

public class DialogNodeStart : DialogNode {

    public DialogNodeStart(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, UnityAction<ConnectionPoint> OnClickInPoint, UnityAction<ConnectionPoint> OnClickOutPoint, UnityAction<DialogNode> OnClickRemoveNode)
        : base(position, width, height, nodeStyle, _selectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode)
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
    public UnityAction<ConnectionPoint> OnClickConnectionPoint;

    public ConnectionPoint(DialogNode node, ConnectionPointType type, GUIStyle style, UnityAction<ConnectionPoint> OnClickConnectionPoint){
        this.node = node;
        this.type = type;
        this.style = style;
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        rect = new Rect(0, 0, 10f, 20f);
    }

    public void Draw(){
        rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;

        switch (type){
            case ConnectionPointType.In:
                rect.x = node.rect.x - rect.width + 8f;
                break;

            case ConnectionPointType.Out:
                rect.x = node.rect.x + node.rect.width - 8f;
                break;
        }

        if (GUI.Button(rect, "", style)){
            if (OnClickConnectionPoint != null){
                OnClickConnectionPoint(this);
            }
        }
    }
}

public class Connection
{
    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;
    public UnityAction<Connection> OnClickRemoveConnection;

    public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, UnityAction<Connection> OnClickRemoveConnection)
    {
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        this.OnClickRemoveConnection = OnClickRemoveConnection;
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

        if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap)){
            if (OnClickRemoveConnection != null){
                OnClickRemoveConnection(this);
            }
        }
    }
}
*/
#endif