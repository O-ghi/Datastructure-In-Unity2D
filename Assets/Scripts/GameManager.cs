using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Transform FoodPool;
    private Transform Content;
    private GameObject FoodObject;

    double _timer = 3f;
    //color
    Color[] _colors = new Color[] { Color.green, Color.red, Color.yellow };
    private void Awake()
    {
        FoodPool = transform.Find("Pool").transform;
        Content = transform.Find("Content").transform;
        FoodObject = transform.Find("Pool/Food").gameObject;
    }

    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= 1 * Time.deltaTime;
            Debug.Log("_timer " + _timer);
        }
        else
        {
            GameObject obj = Instantiate(FoodObject, Content);
            obj.SetActive(true);
            _timer = 3f;
        }
    }
}


public enum FoodColor
{
    green = 0,
    red = 1,
    yellow = 2
}