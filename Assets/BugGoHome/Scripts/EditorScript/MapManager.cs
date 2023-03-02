using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public ButtonStyle[] buttonStyleList;
}

[System.Serializable]
public struct ButtonStyle
{
    public Texture2D icon;
    public string text;
    [HideInInspector]
    public GUIStyle nodeStyle;
}
