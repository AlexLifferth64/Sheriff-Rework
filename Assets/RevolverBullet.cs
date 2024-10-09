using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverBullet : MonoBehaviour
{
    [SerializeField] int speed = 20;

    private Rigidbody2D rb;

    //[SerializeField] GameObject parryParticles;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = speed * transform.up;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) // Will not collide with enemy unless player has parried the bullet
        {
            /*if(collision.GetComponent<Melee>().isParrying)
            {
                rb.velocity = -rb.velocity * 1.5f;
                GetComponent<CapsuleCollider2D>().excludeLayers = LayerMask.GetMask("Bullet");
                //collision.GetComponent<Melee>().StopKnife();
                StartCoroutine(TimePause(0.05f));
                *//*if((transform.position.x < collision.transform.position.x && collision.GetComponent<SimpleMovement>().lastXdir == "right")
                    || transform.position.x > collision.transform.position.x && collision.GetComponent<SimpleMovement>().lastXdir == "left")
                {
                    
                    //Instantiate(parryParticles, GameObject.Find("Player").transform);
                }
                else
                {
                    collision.GetComponent<Health>().AdjustHealth(false, 1); // FIX AND COMPRESS
                    Destroy(gameObject);
                }*//*
            }
            else */if (collision.GetComponent<DodgeRoll>().isVulnerable)
            {
                collision.GetComponent<Health>().AdjustHealth(false, 1);
                Destroy(gameObject);
            }
        }
        else if(collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Health>().AdjustHealth(false, 2);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    IEnumerator TimePause(float waitSec)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(waitSec);
        Time.timeScale = 1;
    }
}
