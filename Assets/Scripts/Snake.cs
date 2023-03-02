using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2 direction = Vector2.right;
    private Vector2 input;
    public int initialSize = 4;
    private LinkedList<Transform> segments = new LinkedList<Transform>();
    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        // Only allow turning up or down while moving in the x-axis
        if (direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                input = Vector2.up;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                input = Vector2.down;
            }
        }
        // Only allow turning left or right while moving in the y-axis
        else if (direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                input = Vector2.right;
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                input = Vector2.left;
            }
        }
    }
    float timer = 0.1f;
    private void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= 4 * Time.deltaTime;
        }
        else
        {
            // Set the new direction based on the input
            if (input != Vector2.zero)
            {
                direction = input;
            }

            // Set each segment's position to be the same as the one it follows. We
            // must do this in reverse order so the position is set to the previous
            // position, otherwise they will all be stacked on top of each other.
            LinkedListNode<Transform> node = segments.Last;
            for (int i = segments.Count - 1; i > 0; i--)
            {
                if (node == segments.First) continue;
                node.Value.position = node.Previous.Value.position;
                node = node.Previous;
            }

            // Move the snake in the direction it is facing
            // Round the values to ensure it aligns to the grid
            float x = Mathf.Round(transform.position.x) + direction.x;
            float y = Mathf.Round(transform.position.y) + direction.y;

            transform.position = new Vector2(x, y);
            timer = 0.1f;
        }

    }

    public void Grow(GameObject gameObject = null)
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.name = segments.Count.ToString();


        if (gameObject == null)
        {
            segment.position = segments.Last.Value.position;

            segments.AddLast(segment);
            return;

        }
        else
        {
            segment.GetComponent<SpriteRenderer>().color = GameManager.GetColor(gameObject);

            FoodColor foodColor = GameManager.FoodQueue.Dequeue();
            GameManager.SetColor(gameObject, foodColor.color);
            Destroy(foodColor.gameObject, 0f);

            LinkedListNode<Transform> node = segments.Count > 0 ? segments.First.Next : null;
            for (int i = 1; i < segments.Count; i++)
            {
                SpriteRenderer sprite = node.Value.GetComponent<SpriteRenderer>();
                if (sprite.color == segment.GetComponent<SpriteRenderer>().color)
                {
                    Debug.Log("sprite.color " + sprite.color + " | " + GameManager.GetColor(gameObject));
                    segment.position = node.Value.position;
                    segments.AddAfter(node, segment);
                    return;
                }
                Debug.Log("sprite.color " + sprite.color + " | " + GameManager.GetColor(gameObject));

                node = node.Next;
            }
            segment.position = segments.Last.Value.position;
            segments.AddLast(segment);
        }
    }

    public void ResetState()
    {
        direction = Vector2.right;
        transform.position = Vector3.zero;

        // Start at 1 to skip destroying the head
        LinkedListNode<Transform> node = segments.First;
        for (int i = 0; i < segments.Count; i++)
        {
            if (node.Value == segments.First.Value)
            {
                node = node.Next;
                continue;
            }
            Destroy(node.Value.gameObject);
            node = node.Next;

        }


        // Clear the list but add back this as the head
        segments.Clear();
        segments.AddLast(transform);

        // -1 since the head is already in the list
        for (int i = 0; i < initialSize - 1; i++)
        {
            Grow();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            Grow(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            ResetState();
        }
    }

}
