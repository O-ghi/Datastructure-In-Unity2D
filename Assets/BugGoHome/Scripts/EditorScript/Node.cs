using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    Rect rect;
    public List<Rect> rectWallList = new List<Rect>();

    public bool[] IsEnable = { false, false, false, false }; // 0: left, 1: up, 2: right, 3: down

    public GUIStyle backgroundstyle;
    public GUIStyle horizontalWallStyle;
    public GUIStyle verticalWallStyle;


    public Node(Vector2 position, float width, float height, GUIStyle defaultStyle)
    {
        rect = new Rect(position.x, position.y, width, height);
        backgroundstyle = defaultStyle;
        rectWallList = new List<Rect>();
        rectWallList.Add(new Rect(position.x, position.y, width / 7, height)); // 0
        rectWallList.Add(new Rect(position.x, position.y, width, height / 7)); // 1
        rectWallList.Add(new Rect(position.x + 26, position.y, width / 7, height)); // 2
        rectWallList.Add(new Rect(position.x, position.y + 26, width, height / 7)); // 3

    }

    public void Drag(Vector2 deltal)
    {
        rect.position += deltal;
        for (int i = 0; i < rectWallList.Count; i++)
        {
            var _rect = rectWallList[i];
            _rect.position = rectWallList[i].position + deltal;
            rectWallList[i] = _rect;
        }
    }
    public void DrawBackground()
    {
        GUI.Box(rect, "", backgroundstyle);
    }

    public void SetStyle(GUIStyle nodeStyle)
    {
        backgroundstyle = nodeStyle;
    }

    public void DrawWall()
    {
        for (int i = 0; i < IsEnable.Length; i++)
        {
            if (IsEnable[i] == true)
            {
                if (i % 2 == 0)
                {
                    GUI.Box(rectWallList[i], "", verticalWallStyle);
                }
                else
                {
                    GUI.Box(rectWallList[i], "", horizontalWallStyle);
                }
            }
        }
    }
}
