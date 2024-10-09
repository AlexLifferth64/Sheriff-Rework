using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    Rigidbody2D rb;
    public Vector2 spawnPoint = new Vector2(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
    }
    public void InitiateRespawn()
    {
        rb.transform.position = spawnPoint;
        rb.velocity = new Vector2(0, 0);
        Time.timeScale = 1;
    }
}

