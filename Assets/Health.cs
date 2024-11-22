using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] int health = 5;

    [SerializeField] private Material FlashMaterial;
    [SerializeField] private Material RedFlashMaterial;

    [SerializeField] private GameObject parryParticles;


    public void AdjustHealth(bool add, int amount)
    {
        if(transform.name == "Player")
        {

            if (!GetComponent<DodgeRoll>().isVulnerable)
            {
                transform.GetComponent<Melee>().StopKnife();

                return;
            }
            else if(GetComponent<Melee>().isParrying)
            {
                Debug.Log("Parry!");
                transform.GetComponent<Melee>().StopKnife();
                transform.GetComponent<Gun>().shotTime = 0;
                Debug.Log(GameObject.Find("Barrel").GetComponent<RevolverBarrel>().barrel[GameObject.Find("Barrel").GetComponent<RevolverBarrel>().barrelNum]);
                GameObject.Find("Barrel").GetComponent<RevolverBarrel>().barrel[GameObject.Find("Barrel").GetComponent<RevolverBarrel>().barrelNum] = true;
                //GameObject.Find("Barrel").GetComponent<RevolverBarrel>().UpdateBarrelVisuals();


                Instantiate(parryParticles, transform.position, Quaternion.identity);

                StartCoroutine(TimePause(0.1f));
                return;
            }
        }


        if (add)
        {
            health += amount;
            //Debug.Log("Recieved Health: " + amount);
        }
        else
        {
            health -= amount;
            //Debug.Log("Took damage: " + amount);

        
            if(gameObject.GetComponent<SpriteRenderer>().material != FlashMaterial || gameObject.GetComponent<SpriteRenderer>().material != RedFlashMaterial)
                StartCoroutine(FlashEffect());
        }

        if (health <= 0)
            Death();
    }

    public void AdjustHealthWithPosition(bool add, int amount, float posX)
    {
        if (transform.name == "Player")
        {
            if (!GetComponent<DodgeRoll>().isVulnerable)
            {
                return;
            }
            else if (GetComponent<Melee>().isParrying)
            {
                /*if ((transform.position.x < posX && GetComponent<SimpleMovement>().lastXdir == "right")
                    || transform.position.x > posX && GetComponent<SimpleMovement>().lastXdir == "left")
                {*/
                    //GetComponent<CapsuleCollider2D>().excludeLayers = LayerMask.GetMask("Bullet");
                    //Instantiate(parryParticles, GameObject.Find("Player").transform);

                GetComponent<Melee>().StopKnife();
                //StartCoroutine(TimePause(0.15f));

                //Debug.Log("Parry!");
                //Instantiate(parryParticles, transform.position, Quaternion.identity);

                StartCoroutine(TimePause(0.15f));
                return;
            }
        }


        if (add)
        {
            health += amount;
            //Debug.Log("Recieved Health: " + amount);
        }
        else
        {
            health -= amount;
            //Debug.Log("Took damage: " + amount);


            if (gameObject.GetComponent<SpriteRenderer>().material != FlashMaterial || gameObject.GetComponent<SpriteRenderer>().material != RedFlashMaterial)
                StartCoroutine(FlashEffect());
        }

        if (health <= 0)
            Death();
    }

    IEnumerator TimedDeath() // For enemies
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        GetComponent<Collider2D>().isTrigger = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

        rb.gravityScale = 0;

        yield return new WaitForSecondsRealtime(0.15f);

        if (transform.CompareTag("Flask"))
            GetComponent<FlaskBehavior>().Shards();
        else
            Destroy(gameObject);
    }

    private void Death()
    {
        if (transform.name == "Player")
        {
            Debug.Log("U Died Lmao");
            health = 3;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1;
        }
        else
            StartCoroutine(TimedDeath());
    }

    IEnumerator TimePause(float waitSec)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(waitSec);
        Time.timeScale = 1;
    }

    IEnumerator FlashEffect()
    {
        Material initialMaterial = gameObject.GetComponent<SpriteRenderer>().material;
        if (transform.name == "Player")
        {
            gameObject.GetComponent<SpriteRenderer>().material = RedFlashMaterial;
            StartCoroutine(TimePause(0.15f));
        }
        else
            gameObject.GetComponent<SpriteRenderer>().material = FlashMaterial;
        
        yield return new WaitForSeconds(0.1f);
        
        gameObject.GetComponent<SpriteRenderer>().material = initialMaterial;
    }
}
