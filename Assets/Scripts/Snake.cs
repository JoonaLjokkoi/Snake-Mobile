using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    Vector2 direction;
    Vector2 previousGridPosition; // Added variable to keep track of the previous grid position
    bool canTurn;
    public GameObject segment;
    public List<GameObject> segments = new List<GameObject>();
    int points = 0;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI gameOverText;
    public Button tryAgainButton;
    public GameObject exitButton;

    public float speed = 0.05f;

    public Food food;
    public AudioClip foodClip; 
    public AudioClip obstacleClip; 

    private AudioSource foodAudioSource;
    private AudioSource obstacleAudioSource;

    public void OnExitButtonClicked()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Exit Play mode in the Editor
    #else
            Application.Quit(); // Quits the application in standalone builds
    #endif
    }

    // Start is called before the first frame update
    void Start()
    {
        // Sets the camera's aspect ratio for portrait mode
        Camera.main.aspect = 1080f / 1920f;

        // Adjusts the camera's orthographic size to maintain the correct aspect ratio
        Camera.main.orthographicSize = 1920f / 2f / 48f; // Changes 100f to adjust zoom level if needed
 

        reset();
        food.RandomizePosition(); // Randomizes food position initially

        // Gets the AudioSources attached to this GameObject or add them if they don't exist
        foodAudioSource = gameObject.GetComponent<AudioSource>();
        if (foodAudioSource == null)
        {
            foodAudioSource = gameObject.AddComponent<AudioSource>();
        }

        obstacleAudioSource = gameObject.AddComponent<AudioSource>(); // Adds a new AudioSource for obstacles

        // Hides game over text and try again button
        gameOverText.gameObject.SetActive(false);
        tryAgainButton.gameObject.SetActive(false);

    }

    public void reset()
    {
        // Resets snake's position, segments, and points
        transform.position = new Vector2(0, -1);
        transform.rotation = Quaternion.Euler(0, 0, -90);
        direction = Vector2.right;
        Time.timeScale = speed;
        resetSegments();
        points = 0;
        UpdatePointsDisplay();

        // Randomizes food position when resetting
        food.RandomizePosition();

        // Hides game over text and try again button
        gameOverText.gameObject.SetActive(false);
        tryAgainButton.gameObject.SetActive(false);
        exitButton.SetActive(false);
    }



        public int GetPoints()
    {
        return points;
    }

    void resetSegments()
    {
        // destroys segments
        for (int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }

        segments.Clear(); // clear list
        segments.Add(gameObject); // add head

        // puts the initial segments on top of the head
        for (int i = 0; i < 2; i++)
        {
            grow();
        }
    }

        void UpdatePointsDisplay()
    {
        int displayedPoints = Mathf.Max(0, points - 0); // Ensures displayed points are non-negative and the starting points will be zero
        pointsText.text = displayedPoints.ToString();
    }


        void grow()
    {
        GameObject newSegment = Instantiate(segment);
        newSegment.transform.position = segments[segments.Count - 1].transform.position;
        segments.Add(newSegment);

        points++; // Increases the points when the snake eats food
        UpdatePointsDisplay(); // Updates the points display text
    }


     // Update is called once per frame
    void Update()
    {
        // No need to call getUserInput() anymore
        
    }

        public void OnUpButtonClicked()
    {
        if (direction != Vector2.down && canTurn)
        {
            direction = Vector2.up;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            canTurn = false;
        }
    }

    public void OnDownButtonClicked()
    {
        if (direction != Vector2.up && canTurn)
        {
            direction = Vector2.down;
            transform.rotation = Quaternion.Euler(0, 0, 180);
            canTurn = false;
        }
    }

    public void OnRightButtonClicked()
    {
        if (direction != Vector2.left && canTurn)
        {
            direction = Vector2.right;
            transform.rotation = Quaternion.Euler(0, 0, -90);
            canTurn = false;
        }
    }

    public void OnLeftButtonClicked()
    {
        if (direction != Vector2.right && canTurn)
        {
            direction = Vector2.left;
            transform.rotation = Quaternion.Euler(0, 0, 90);
            canTurn = false;
        }
    }


    void getUserInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && direction != Vector2.down && canTurn)
        {
            direction = Vector2.up;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            canTurn = false;
        }
        else if (Input.GetKeyDown(KeyCode.S) && direction != Vector2.up && canTurn)
        {
            direction = Vector2.down;
            transform.rotation = Quaternion.Euler(0, 0, 180);
            canTurn = false;
        }
        else if (Input.GetKeyDown(KeyCode.D) && direction != Vector2.left && canTurn)
        {
            direction = Vector2.right;
            transform.rotation = Quaternion.Euler(0, 0, -90);
            canTurn = false;
        }
        else if (Input.GetKeyDown(KeyCode.A) && direction != Vector2.right && canTurn)
        {
            direction = Vector2.left;
            transform.rotation = Quaternion.Euler(0, 0, 90);
            canTurn = false;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            reset();
        }
    }

    void FixedUpdate()
    {
        moveSegments();
        moveSnake();
    }

    void moveSegments()
    {
        // puts on top of the one in front
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].transform.position = segments[i - 1].transform.position;
        }
    }

    void moveSnake()
    {
        float x = transform.position.x + direction.x;
        float y = transform.position.y + direction.y;

        // Checks if the snake is moving to a new grid square
        Vector2 currentGridPosition = new Vector2(Mathf.Round(x), Mathf.Round(y));
        if (currentGridPosition != previousGridPosition)
        {
            previousGridPosition = currentGridPosition;
            canTurn = true;

            // Updates the snake's head rotation based on the current direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        transform.position = new Vector2(x, y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Obstacle")
        {
            Time.timeScale = 0; // Pauses the game

            // Plays obstacle collision sound
            obstacleAudioSource.PlayOneShot(obstacleClip);

            // Shows the game over text and try again button
            gameOverText.gameObject.SetActive(true);
            tryAgainButton.gameObject.SetActive(true);
            exitButton.SetActive(true);
        }
        else if (other.tag == "Food")
        {
            grow(); // Calls the grow method when the snake collides with the food

            // Plays food collection sound
            foodAudioSource.PlayOneShot(foodClip);
        }
    }
}