using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelDesignCreator : EditorWindow
{
    Vector2 offset;
    Vector2 drag;
    GUIStyle gUIStyle;
    GUIStyle wallVerticalGUIStyle;
    GUIStyle wallHorizontalGUIStyle;
    List<List<Node>> nodes;
    Vector2 nodePos;
    MapManager mapManager;

    float _mouseArea = 0.24f;

    [MenuItem("Window/Level Design Creator")]
    private static void OpenWindow()
    {
        LevelDesignCreator window = GetWindow<LevelDesignCreator>();
        window.titleContent = new GUIContent("Level Design Creator");
    }

    private void OnEnable()
    {
        SetUpMapManager();
        gUIStyle = new GUIStyle();
        Texture2D icon = Resources.Load("Sprites/Background") as Texture2D;
        Debug.Log(icon);
        gUIStyle.normal.background = AssetPreview.GetAssetPreview(icon);
        SetUpNodes();
    }

    private void SetUpMapManager()
    {
        mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();

        wallHorizontalGUIStyle = new GUIStyle();
        wallVerticalGUIStyle = new GUIStyle();
        wallHorizontalGUIStyle.normal.background = mapManager.buttonStyleList[0].icon;
        wallVerticalGUIStyle.normal.background = mapManager.buttonStyleList[1].icon;
    }

    private void SetUpNodes()
    {
        nodes = new List<List<Node>>();

        for (int i = 0; i < 10; i++)
        {
            nodes.Add(new List<Node>());
            for (int j = 0; j < 10; j++)
            {
                nodePos.Set(i * 30, j * 30);
                nodes[i].Add(new Node(nodePos, 30, 30, gUIStyle));
                nodes[i][j].verticalWallStyle = wallVerticalGUIStyle;
                nodes[i][j].horizontalWallStyle = wallHorizontalGUIStyle;
            }
        }
    }

    private void OnGUI()
    {
        // GUI
        DrawGrid();
        DrawNodes();
        ProcessNode(Event.current);
        ProcessGrid(Event.current);
        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void ProcessNode(Event e)
    {
        Vector2 mousePosition = (e.mousePosition - offset) / 30;
        if (mousePosition.x < 0 || mousePosition.y < 0 || mousePosition.x > 10 || mousePosition.y > 10)
        {
            return;
        }
        Debug.Log(mousePosition);
        int row = (int)mousePosition.y;
        int column = (int)mousePosition.x;

        if (e.type == EventType.MouseDown)
        {
            int way = WhichWallMouseOn(row, column, mousePosition);
            if (way == -1)
            {
                return;
            }
            CallWall(row, column, way);
            GUI.changed = true;

        }
    }

    private int WhichWallMouseOn(int row, int column, Vector2 mousePosition)
    {
        //left
        if (mousePosition.x > column && mousePosition.x <= column + _mouseArea)
        {
            return 0;
        }
        //right
        if (mousePosition.x >= column + (1 - _mouseArea) && mousePosition.x < column + 1)
        {
            return 2;
        }
        //up
        if (mousePosition.y > row && mousePosition.y <= row + _mouseArea)
        {
            return 1;
        }
        //down
        if (mousePosition.y >= row + (1 - _mouseArea) && mousePosition.y < row + 1)
        {
            return 3;
        }
        return -1;
    }
    private void CallWall(int row, int column, int way) // 0: left, 1: up, 2: right, 3: down
    {
        if (nodes[column][row].IsEnable[way] == true)
        {
            nodes[column][row].IsEnable[way] = false;
        }
        else
        {
            nodes[column][row].IsEnable[way] = true;
        }

    }


    private void DrawNodes()
    {
        GUI.depth = 1;

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                nodes[i][j].DrawBackground();
                nodes[i][j].DrawWall();

            }
        }
    }

    private void ProcessGrid(Event e)
    {
        drag = Vector2.zero;
        switch (e.type)
        {
            case EventType.MouseDrag:
                {
                    if (e.button == 0)
                    {
                        OnMouseDrag(e.delta);
                    }
                }
                break;
        }
    }

    private void OnMouseDrag(Vector2 delta)
    {
        drag = delta;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                nodes[i][j].Drag(delta);
            }
        }
        GUI.changed = true;
    }

    private void DrawGrid()
    {
        int widthDivider = Mathf.CeilToInt(position.width / 30);
        int hightDevider = Mathf.CeilToInt(position.height / 30);
        Handles.BeginGUI();
        Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        offset += drag;
        Vector3 newOffset = new Vector3(offset.x % 30, offset.y % 30, 0);
        for (int i = 0; i < widthDivider; i++)
        {
            Handles.DrawLine(new Vector3(30 * i, -30, 0) + newOffset, new Vector3(30 * i, position.height, 0) + newOffset);
        }
        for (int i = 0; i < hightDevider; i++)
        {
            Handles.DrawLine(new Vector3(-30, 30 * i, 0) + newOffset, new Vector3(position.width, 30 * i, 0) + newOffset);
        }
        Handles.color = Color.white;
        Handles.EndGUI();
    }
}
