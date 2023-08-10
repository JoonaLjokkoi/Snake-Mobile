using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public GameObject snake; // Reference to the Snake object

    // Start is called before the first frame update
    void Start()
    {
        RandomizePosition();
    }

    public void RandomizePosition()
    {
        Vector2 newPosition;
        do
        {
            int x = Random.Range(-10, 11);
            int y = Random.Range(-9, 15);
            newPosition = new Vector2(x, y);
        } while (IsCollidingWithSnake(newPosition));

        transform.position = newPosition;
    }

    bool IsCollidingWithSnake(Vector2 position)
    {
        // Check if the position collides with any of the snake's segments
        foreach (GameObject segment in snake.GetComponent<Snake>().segments)
        {
            if ((Vector2)segment.transform.position == position)
            {
                return true;
            }
        }
        return false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        RandomizePosition();
    }
}
