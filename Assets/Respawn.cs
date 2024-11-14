using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    Rigidbody2D rb;
    public Vector2 spawnPoint = new Vector2(0, 0);

    Transform[] checkpoints;// = gameObject.GetComponentsInChildren<Transform>();
    int checkNum = 0;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        checkpoints = GameObject.Find("Checkpoints").GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // respawn
        {
            InitiateRespawn();
        }
        if (Input.GetKeyDown(KeyCode.Y)) // respawn
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            rb.transform.position = spawnPoint;
            rb.velocity = new Vector2(0, 0);
            Time.timeScale = 1;

        }

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            if (checkNum > 0)
            {
                checkNum--;
                transform.position = checkpoints[checkNum].position;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            if (checkNum < checkpoints.Length - 1)
            {
                checkNum++;
                transform.position = checkpoints[checkNum].position;
            }
        }
    }
    public void InitiateRespawn()
    {
        rb.transform.position = spawnPoint;
        rb.velocity = new Vector2(0, 0);
        Time.timeScale = 1;
    }
}

