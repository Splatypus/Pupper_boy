using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;

public class DialogEditor : EditorWindow {

    List<DialogNode> nodes;
    List<Connection> connections;
    GUIStyle nodeStyle;
    GUIStyle selectedNodeStyle;
    GUIStyle inPointStyle;
    GUIStyle outPointStyle;

    ConnectionPoint selectedInPoint;
    ConnectionPoint selectedOutPoint;

    Vector2 offset;

    [MenuItem("Window/Dialog Editor")]
    static void OpenWindow() {
        DialogEditor window = GetWindow<DialogEditor>();
        window.titleContent = new GUIContent("Dialog Writer");
    }

    private void OnEnable()
    {
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
    }

    void OnGUI(){
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);
        DrawNodes();
        DrawConnections();
        DrawConnectionLine(Event.current);
        ProcessEvents(Event.current);
        if (GUI.changed)
            Repaint();
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
        if (connections != null){
            for (int i = 0; i < connections.Count; i++){
                connections[i].Draw();
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
        if (nodes != null) {
            for (int i = 0; i < nodes.Count; i++) {
                nodes[i].Draw();
            }
        }
    }

    void ProcessEvents(Event e) {
        //first do node events
        if (nodes != null) {
            foreach (DialogNode n in nodes) {
                bool guiChanged = n.ProcessEvents(e);
                if (guiChanged) {
                    GUI.changed = true;
                }
            }
        }
        //the other events
        switch (e.type){
            case EventType.MouseDown:
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
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }
    void OnClickAddNode(Vector2 mousePosition){
        if (nodes == null){
            nodes = new List<DialogNode>();
        }
        nodes.Add(new DialogNode(mousePosition, 200, 50, nodeStyle, selectedNodeStyle,inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }
    void OnClickInPoint(ConnectionPoint inPoint){
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
    void OnClickOutPoint(ConnectionPoint outPoint){
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
    void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
    }
    void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }

        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }
    void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }
    void OnClickRemoveNode(DialogNode node){
        if (connections != null){
            //removes connections when a node is deleted.
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < connections.Count; i++){
                if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint){
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++){
                connections.Remove(connectionsToRemove[i]);
            }
            connectionsToRemove = null;
        }

        nodes.Remove(node);
    }
    void OnDrag(Vector2 delta) {
        if (nodes != null) {
            foreach (DialogNode n in nodes) {
                n.Drag(delta);
            }
        }
        offset += delta;
        GUI.changed = true;
    }
}

public class DialogNode {
    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public GUIStyle style;
    public GUIStyle defaultStyle;
    public GUIStyle selectedStyle;

    public UnityAction<DialogNode> OnRemoveNode;

    //constructer
    public DialogNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle _selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, UnityAction<ConnectionPoint> OnClickInPoint, UnityAction<ConnectionPoint> OnClickOutPoint, UnityAction<DialogNode> OnClickRemoveNode){
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
        defaultStyle = nodeStyle;
        selectedStyle = _selectedStyle;
        OnRemoveNode = OnClickRemoveNode;
    }

    //Drags the node
    public void Drag(Vector2 delta) {
        rect.position += delta;
    }

    //draw function
    public void Draw() {
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(rect, title, style);
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

        if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleCap)){
            if (OnClickRemoveConnection != null){
                OnClickRemoveConnection(this);
            }
        }
    }
}
