using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlaskBehavior : MonoBehaviour
{
    [SerializeField] int speed = 20;

    private Rigidbody2D rb;
    private Rigidbody2D parentRb;

    [SerializeField] private GameObject shard0;
    [SerializeField] private GameObject shard1;
    [SerializeField] private GameObject shard2;
    [SerializeField] private GameObject shard3;
    [SerializeField] private GameObject shard4;
    [SerializeField] private GameObject shard5;

    private GameObject[] shards;
    
    [SerializeField] private GameObject flaskParticles;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb.velocity.x < 0)
        {
            rb.angularVelocity = 1000;
        }
        else if (rb.velocity.x > 0)
        {
            rb.angularVelocity = -1000;
        }

        rb.velocity = new Vector2(rb.velocity.x * 1.25f, rb.velocity.y);

        if (rb.velocity.y > 0)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 1.5f);
        else
            rb.velocity = new Vector2(rb.velocity.x, 22);

        shards = new GameObject[6];
        shards[0] = shard0;
        shards[1] = shard1;
        shards[2] = shard2;
        shards[3] = shard3;
        shards[4] = shard4;
        shards[5] = shard5;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("FLASK COORD  " + transform.position);
        //Debug.Log(collision.contacts[0].point);

        if(transform.position.y - collision.contacts[0].point.y > 0.5f)
        {
            Shards();
        }
        //Shards();
    }

    public void Shards()
    {
        foreach (GameObject obj in shards)
        {
            GameObject activeObj = Instantiate(obj, transform.position, obj.transform.rotation);
            Rigidbody2D objRb = activeObj.GetComponent<Rigidbody2D>();

            objRb.velocity = GetComponent<Rigidbody2D>().velocity;
            objRb.angularVelocity = 9999;// GetComponent<Rigidbody2D>().angularVelocity;
            objRb.transform.rotation = GetComponent<Rigidbody2D>().transform.rotation;
        }
        Instantiate(flaskParticles, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
