using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Transform FoodPool;
    private Transform Content;
    private GameObject FoodObject;

    public static Queue<FoodColor> FoodQueue = new Queue<FoodColor>();
    double _timer = 3f;
    //color
    Color[] _colors = new Color[] { Color.green, Color.red, Color.yellow, Color.white };
    private void Awake()
    {
        FoodPool = transform.Find("Pool").transform;
        Content = transform.Find("Content").transform;
        FoodObject = transform.Find("Pool/Food").gameObject;
    }

    private void Update()
    {
        if (_timer > 0 && FoodQueue.Count > 0)
        {
            _timer -= 1 * Time.deltaTime;
        }
        else
        {
            GameObject obj = Instantiate(FoodObject, Content);
            obj.SetActive(true);
            FoodColor foodColor = new FoodColor()
            {
                gameObject = obj,
                color = _colors[Random.Range(0, 4)]
            };
            FoodQueue.Enqueue(foodColor);
            obj.GetComponent<SpriteRenderer>().color = foodColor.color;
            _timer = 3f;
        }
    }
    public static void SetColor(GameObject gameObject, Color color)
    {
        Food food = gameObject.GetComponent<Food>();
        food.SetColor(color);
    }
    public static Color GetColor(GameObject gameObject)
    {
        Food food = gameObject.GetComponent<Food>();
        return food.GetColor();
    }
}


public class FoodColor
{
    public GameObject gameObject;
    public Color color;
}