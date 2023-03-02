using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Transform FoodPool;
    private GameObject FoodObject;


    //color
    Color[] _colors = new Color[] { Color.green, Color.red, Color.yellow };
    private void Awake()
    {
        FoodPool = transform.Find("Pool").transform;
        FoodObject = transform.Find("Pool/Food").gameObject;
    }

    private void Update()
    {

    }
}


public enum FoodColor
{
    green = 0,
    red = 1,
    yellow = 2
}