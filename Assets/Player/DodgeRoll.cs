using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeRoll : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D c2d;

    [SerializeField] private float rollSpeed = 20;
    [SerializeField] private float vulnerableRollSpeed = 10;
    [SerializeField] private float invinceTime = 0.3f;
    [SerializeField] private float vulnTime = 0.3f;

    private float initRollSpeed;

    public bool canSuperRoll = false;
    public bool isRolling = false;
    public bool isVulnerable = true;

    private Coroutine rollCo;

    SimpleMovement movement;
    Melee melee;
    Gun gun;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<SimpleMovement>();
        melee = GetComponent<Melee>();
        gun = GetComponent<Gun>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Mouse1)
            || Input.GetKeyDown(KeyCode.M))
            && !isRolling && !melee.isAttacking)
        {
            if (!canSuperRoll)
            {
                rollCo = StartCoroutine(Roll());
                //StartCoroutine(Roll());
            }
            else
            {
                canSuperRoll = false;

                float superVel = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y / 2);
                if (movement.lastXdir == "left")
                    rb.velocity = new Vector2(-superVel, 0);
                else // right
                    rb.velocity = new Vector2(superVel, 0);
            }
        }
        else if(isRolling)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {

            }
            if (gun.fire && gun.hitObject)
            {
                //Debug.Log("STOP ROLLING");
                StopCoroutine(rollCo);
                isRolling = false;
                //isVulnerable = true; // Makes player vulnerable immediately CHANGE LATER

                //GetComponent<SpriteRenderer>().color = Color.white; // For debugging
                gun.EnableGunArm(true);
            }
            else
            {
                if (!isVulnerable)
                    rb.velocity = new Vector2(initRollSpeed, rb.velocity.y);
                else
                {
                    if(initRollSpeed > 0)
                        rb.velocity = new Vector2(vulnerableRollSpeed, rb.velocity.y);
                    else
                        rb.velocity = new Vector2(-vulnerableRollSpeed, rb.velocity.y);
                }
            }
        }
    }

    public IEnumerator Roll()
    {
        /*if (movement.yDir == "down")
            rb.velocity = new Vector2(0, rb.velocity.y);
        else */
        StartCoroutine(Invincibility(invinceTime));

        gun.EnableGunArm(false);

        if (movement.lastXdir == "right")
        {
            if (movement.rb.velocity.x > rollSpeed - 2)
            {
                initRollSpeed = rb.velocity.x + 5;

                if (movement.rb.velocity.y < 0)
                    rb.velocity = new Vector2(initRollSpeed, 0); // sets yvel to 0 only if falling
                else
                    rb.velocity = new Vector2(initRollSpeed, rb.velocity.y);
            }
            else
            {
                initRollSpeed = rollSpeed;

                if (movement.rb.velocity.y < 0)
                    rb.velocity = new Vector2(rollSpeed, 0);
                else
                    rb.velocity = new Vector2(rollSpeed, rb.velocity.y);
            }
        }
        else if (movement.lastXdir == "left")
        {
            if (movement.rb.velocity.x < -rollSpeed + 2)
            {
                initRollSpeed = rb.velocity.x - 5;

                if (movement.rb.velocity.y < 0)
                    rb.velocity = new Vector2(initRollSpeed, 0);
                else
                    rb.velocity = new Vector2(initRollSpeed, rb.velocity.y);
            }
            else
            {
                initRollSpeed = -rollSpeed;

                if (movement.rb.velocity.y < 0)
                    rb.velocity = new Vector2(-rollSpeed, 0);
                else
                    rb.velocity = new Vector2(-rollSpeed, rb.velocity.y);
            }
        }

        movement.isGliding = true;
        isRolling = true;
        //isVulnerable = false;

        //GetComponent<SpriteRenderer>().color = Color.green; // For debugging

        yield return new WaitForSeconds(invinceTime);

        //isVulnerable = true;

        if (rb.velocity.x > 0)
            rb.velocity = new Vector2(vulnerableRollSpeed, rb.velocity.y);
        else if (rb.velocity.x < 0)
            rb.velocity = new Vector2(-vulnerableRollSpeed, rb.velocity.y);
        else if (rb.velocity.x == 0)
            rb.velocity = new Vector2(0, rb.velocity.y);

        //GetComponent<SpriteRenderer>().color = Color.red; // For debugging

        yield return new WaitForSeconds(vulnTime);
        isRolling = false;
        //isVulnerable = true;
        //GameObject.Find("Player Relative Mouse").GetComponent<PlayerRelativeMouse>().ResetMousePos();

        //GetComponent<SpriteRenderer>().color = Color.white; // For debugging

        gun.EnableGunArm(true);

        /*if(!movement.slide)
            rb.velocity = new Vector2(0, 0);*/


    }

    public IEnumerator Invincibility(float time) // separate enumerator so that player is still invincible when roll is canceled
    {
        isVulnerable = false;
        yield return new WaitForSeconds(time);

        isVulnerable = true;
        //yield return new WaitForSeconds(vulnTime);
    }
}


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeRoll : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D c2d;

    [SerializeField] private float rollSpeed = 20;
    [SerializeField] private float vulnerableRollSpeed = 10;
    [SerializeField] private float invinceTime = 0.3f;
    [SerializeField] private float vulnTime = 0.3f;

    private float rollTimeStart = 0;
    private float rollTimeCurrent = 0;


    public bool canSuperRoll = false;
    public bool isRolling = false;
    public bool isVulnerable = true;
    private bool cancelRoll = false;

    SimpleMovement movement;
    Gun gun;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<SimpleMovement>();
        gun = GetComponent<Gun>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Mouse1)
            || Input.GetKeyDown(KeyCode.M))
            && !isRolling)
        {
            if(!canSuperRoll)
                StartCoroutine(Roll());
            else
            {
                canSuperRoll = false;

                float superVel = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y / 2);
                if (movement.lastXdir == "left")
                    rb.velocity = new Vector2(-superVel, 0);
                else // right
                    rb.velocity = new Vector2(superVel, 0);
            }
        }
        else if(isRolling)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {

            }
            if (gun.fire && gun.hitObject)
            {
                //StopAllCoroutines();
                //Debug.Log("STOP ROLLING");
                cancelRoll = true;
                isRolling = false;
                //isVulnerable = true;

                GetComponent<SpriteRenderer>().color = Color.white; // For debugging
            }
            else
            {

            }
        }
    }

    public IEnumerator Roll()
    {
        cancelRoll = false;
        /*if (movement.yDir == "down")
            rb.velocity = new Vector2(0, rb.velocity.y);
        else
if (movement.lastXdir == "right")
{
    if (movement.rb.velocity.x > rollSpeed - 2)
    {
        if (movement.rb.velocity.y < 0)
            rb.velocity = new Vector2(rb.velocity.x + 5, 0); // sets yvel to 0 only if falling
        else
            rb.velocity = new Vector2(rb.velocity.x + 5, rb.velocity.y);
    }
    else
    {
        if (movement.rb.velocity.y < 0)
            rb.velocity = new Vector2(rollSpeed, 0);
        else
            rb.velocity = new Vector2(rollSpeed, rb.velocity.y);
    }
}
else if (movement.lastXdir == "left")
{
    if (movement.rb.velocity.x < -rollSpeed + 2)
    {
        if (movement.rb.velocity.y < 0)
            rb.velocity = new Vector2(rb.velocity.x - 5, 0);
        else
            rb.velocity = new Vector2(rb.velocity.x - 5, rb.velocity.y);
    }
    else
    {
        if (movement.rb.velocity.y < 0)
            rb.velocity = new Vector2(-rollSpeed, 0);
        else
            rb.velocity = new Vector2(-rollSpeed, rb.velocity.y);
    }
}

movement.isGliding = true;
isRolling = true;
isVulnerable = false;

GetComponent<SpriteRenderer>().color = Color.green; // For debugging

yield return new WaitForSeconds(invinceTime);

if (!cancelRoll)
{
    isVulnerable = true;

    if (rb.velocity.x > 0)
        rb.velocity = new Vector2(vulnerableRollSpeed, rb.velocity.y);
    else if (rb.velocity.x < 0)
        rb.velocity = new Vector2(-vulnerableRollSpeed, rb.velocity.y);
    else if (rb.velocity.x == 0)
        rb.velocity = new Vector2(0, rb.velocity.y);

    GetComponent<SpriteRenderer>().color = Color.red; // For debugging

    yield return new WaitForSeconds(vulnTime);
    isRolling = false;
    isVulnerable = true;
    //GameObject.Find("Player Relative Mouse").GetComponent<PlayerRelativeMouse>().ResetMousePos();

    GetComponent<SpriteRenderer>().color = Color.white; // For debugging
}
else
{
    isRolling = false;
    isVulnerable = true;
    //c2d.excludeLayers |= (1 << 8);
    //GameObject.Find("Player Relative Mouse").GetComponent<PlayerRelativeMouse>().ResetMousePos();

    GetComponent<SpriteRenderer>().color = Color.white; // For debugging
}

        /*if(!movement.slide)
            rb.velocity = new Vector2(0, 0);

        
    }
}
*/