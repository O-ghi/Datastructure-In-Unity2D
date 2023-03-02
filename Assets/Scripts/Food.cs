using UnityEngine;

public class Food : MonoBehaviour
{
    public Collider2D gridArea;
    public SpriteRenderer sprite;
    private void Start()
    {
        sprite = transform.GetComponent<SpriteRenderer>();
        RandomizePosition();
    }

    public void RandomizePosition()
    {
        Bounds bounds = gridArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        x = Mathf.Round(x);
        y = Mathf.Round(y);

        transform.position = new Vector2(x, y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RandomizePosition();
    }
    public void SetColor(Color color)
    {
        sprite.color = color;
    }
    public Color GetColor()
    {
        return sprite.color;
    }
}
