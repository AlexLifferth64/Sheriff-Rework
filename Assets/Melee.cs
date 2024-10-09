using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Melee : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    public bool isAttacking = false;
    public bool isParrying = false;
    public bool isWallCling = false;

    /*[SerializeField] GameObject grenadePrefab;
    private GameObject grenade;*/

    [SerializeField] private GameObject leftHitbox;
    [SerializeField] private GameObject rightHitbox;

    [SerializeField] private GameObject leftWallClingHitbox;
    [SerializeField] private GameObject rightWallClingHitbox;

    /*private Vector2 mousePos;
    private Vector2 lookDir;
    private Camera cam;*/
    [SerializeField] private LayerMask enemy;
    [SerializeField] private LayerMask ground;

    SimpleMovement movementScript;
    DodgeRoll dodgeRoll;
    Gun gun;
    private RevolverBarrel barrel;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        movementScript = gameObject.GetComponent<SimpleMovement>();
        dodgeRoll = GetComponent<DodgeRoll>();
        gun = GetComponent<Gun>();
        barrel = GameObject.Find("Barrel").GetComponent<RevolverBarrel>();
        //cam = GameObject.Find("Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Mouse3)) && !isAttacking && !dodgeRoll.isRolling)
        {
            StartCoroutine(Punch());
        }

        /*if(isAttacking)
        {
            if(movementScript.isGrounded)
            {

            }
        }*/

        if (leftHitbox.active || rightHitbox.active)
        {
            Collider2D[] objects;
            Collider2D[] wallClingObjects;

            if (leftHitbox.active)
            {
                objects = Physics2D.OverlapCircleAll(leftHitbox.transform.position, leftHitbox.transform.localScale.x / 2, enemy);
                wallClingObjects = Physics2D.OverlapBoxAll(leftWallClingHitbox.transform.position, new Vector2(leftWallClingHitbox.transform.localScale.x / 2, leftHitbox.transform.localScale.x / 2), 0, ground);
            }
            else // rightHitbox.active
            {
                objects = Physics2D.OverlapCircleAll(rightHitbox.transform.position, rightHitbox.transform.localScale.x / 2, enemy);
                wallClingObjects = Physics2D.OverlapBoxAll(rightWallClingHitbox.transform.position, new Vector2(rightWallClingHitbox.transform.localScale.x / 2, leftHitbox.transform.localScale.x / 2), 0, ground);
            }

            foreach (Collider2D obj in objects)
            {
                if (obj.CompareTag("Enemy"))
                {
                    obj.GetComponent<Health>().AdjustHealth(false, 5);

                    if(rb.velocity.y < -30)
                    {
                        Debug.Log("MARKET GARDEN: " + rb.velocity.y);
                        dodgeRoll.canSuperRoll = true;
                        StopKnife();
                    }
                    movementScript.canMove = true;


                    StartCoroutine(TimePause(0.15f));
                }
                else if(obj.CompareTag("Flask"))
                {
                    if (leftHitbox.active)
                    {
                        obj.GetComponent<Rigidbody2D>().velocity = new Vector2(-45, 5);
                        obj.GetComponent<Rigidbody2D>().angularVelocity = 2500;
                    }
                    else
                    {
                        obj.GetComponent<Rigidbody2D>().velocity = new Vector2(45, 5);
                        obj.GetComponent<Rigidbody2D>().angularVelocity = -2500;
                    }
                    //StopKnife();
                    movementScript.canMove = true;


                    StartCoroutine(TimePause(0.15f));

                    if (rb.velocity.y < -30)
                    {
                        Debug.Log("MARKET GARDEN: " + rb.velocity.y);
                        dodgeRoll.canSuperRoll = true;
                    }
                }
            }

            /*if(wallClingObjects.Length != 0 && !movementScript.isGrounded && !isWallCling)
            {
                isWallCling = true;
                movementScript.rb.velocity = Vector2.zero;

                Debug.Log("Wall Cling");
            }*/

        }

        /*if(isWallCling)
        {
            if(movementScript.jump != "none")
            {
                isWallCling = false;
                movementScript.rb.gravityScale = movementScript.defaultGravity;
            }
            else
            {
                movementScript.rb.gravityScale = 0;
            }
        }*/
    }

    IEnumerator Punch()
    {
        isAttacking = true;
        movementScript.isGliding = true;
        movementScript.canMove = false;

        gun.EnableGunArm(false);

        //GameObject.Find("Player Relative Mouse").GetComponent<PlayerRelativeMouse>().ResetMousePos();
        //GameObject.Find("Player Relative Mouse").GetComponent<PlayerRelativeMouse>().lockMouse = true;

        //yield return new WaitForSeconds(0.05f);

        isParrying = true;

        //GetComponent<SpriteRenderer>().color = Color.yellow; // For debugging

        yield return new WaitForSeconds(0.15f);

        isParrying = false;

        //GetComponent<SpriteRenderer>().color = Color.white; // For debugging

        yield return new WaitForSeconds(0.26f);

        if (movementScript.lastXdir == "left")
            leftHitbox.SetActive(true);
        else if (movementScript.lastXdir == "right")
            rightHitbox.SetActive(true);
        
        //GetComponent<SpriteRenderer>().color = Color.green; // For debugging

        yield return new WaitForSeconds(0.14f);

        leftHitbox.SetActive(false);
        rightHitbox.SetActive(false);

        //GetComponent<SpriteRenderer>().color = Color.white; // For debugging

        yield return new WaitForSeconds(0.16f);

        isAttacking = false;
        movementScript.canMove = true;

        gun.EnableGunArm(true);

        //GameObject.Find("Player Relative Mouse").GetComponent<PlayerRelativeMouse>().lockMouse = false;
    }

    IEnumerator AirPunch()
    {
        isAttacking = true;
        movementScript.isGliding = true;
        movementScript.canMove = false;

        //yield return new WaitForSeconds(0.05f);

        //isParrying = true;

        if (movementScript.lastXdir == "left")
            leftHitbox.SetActive(true);
        else if (movementScript.lastXdir == "right")
            rightHitbox.SetActive(true);

        GetComponent<SpriteRenderer>().color = Color.green; // For debugging

        yield return new WaitForSeconds(0.18f);

        leftHitbox.SetActive(false);
        rightHitbox.SetActive(false);

        GetComponent<SpriteRenderer>().color = Color.white; // For debugging

        yield return new WaitForSeconds(0.12f);

        isAttacking = false;
        movementScript.canMove = true;
    }

    public void StopKnife()
    {
        StopAllCoroutines();

        isAttacking = false;
        isParrying = false;
        movementScript.canMove = true;
        GameObject.Find("Player Relative Mouse").GetComponent<PlayerRelativeMouse>().lockMouse = false;

        GetComponent<SpriteRenderer>().color = Color.white; // For debugging

        leftHitbox.SetActive(false);
        rightHitbox.SetActive(false);
        gun.EnableGunArm(true);
    }

    IEnumerator TimePause(float waitSec)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(waitSec);
        Time.timeScale = 1;
    }
    /*IEnumerator TimeStretch(float duration)
    {
        if (!timeStretch)
        {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(0);
            timeStretch = true;
        }
    }*/
}
