using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    private BoxCollider2D playerCollider;
    private float initColliderSizeX;
    private float initColliderSizeY;

    public Transform groundCheck;
    public Transform rightCheck;
    public Transform leftCheck;
    public bool isGrounded = true;
    public bool isOnRightWall = false;
    public bool isOnLeftWall = false;

    private bool firstFrameOnWall = true;
    RaycastHit2D stairRay;
    RaycastHit2D downTileRay;


    [SerializeField] private float timeFromRightWall = 0.1f;
    [SerializeField] private float timeFromLeftWall = 0.1f;

    public int initJumpSpeed;
    private float jumpSpeed;
    public int runSpeed;
    public int runCap;
    [SerializeField] private int wallRunCap = 45;

    public float recentHighVelX;
    public float recentHighVelY;

    public float lastFrameVelX;
    public float lastFrameVelY;

    //private float jumpBufferTime = 0.2f;
    //private float jumpBufferCounter = 0f;
    private bool isJumping = false;
    //private bool isWallJumping = false;
    public float initJumpNum = 0.15f;
    private float jumpNum = 0.15f;
    public int totalJumps = 1;
    //private int totalWallJumps = 3;

    public string xDir;
    public string yDir;
    public string jump = "none"; // "up2"
    public bool slide;
    public bool isGliding = false; // When true, x deceleration is off;
    public bool canWallRun = false;
    public bool isWallRunning = false;

    private float speedJump = 0.2f;

    private bool groundedLastFrame = true;

    RaycastHit2D leftRay;
    RaycastHit2D rightRay;
    RaycastHit2D bottomRay;
    GameObject groundObj;

    public LayerMask ground;

    //public GameObject WallJumpParticles;
    //public GameObject CritWallJumpParticles;
    public ParticleSystem jumpParticles;
    //[SerializeField] private ParticleSystem slideParticles;

    //public AudioSource jumpSound;

    //[SerializeField] CameraBehavior cameraScript;
    //[SerializeField] GrapplingHook grapplingHookScript;
    //Combat combatScript;
    //Gun gunScript;
    //Shotgun shotgunScript;


    //https://www.youtube.com/watch?v=RFix_Kg2Di0 jump buffer and coyote time

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<BoxCollider2D>();
        //combatScript = GetComponent<Combat>();
        //gunScript = GetComponent<Gun>();

        initColliderSizeX = playerCollider.size.x;
        initColliderSizeY = playerCollider.size.y;
        recentHighVelX = initJumpSpeed;

        jump = "up";
        //jumpSound.time = 0.12f;
    }

    private void Update()
    {
        // WASD input: Gets input from the user and sets "xDir" and "jump" accordingly
        xDir = "none";
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            xDir = "left";
            isGliding = true;
            sr.flipX = false;
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            /*if (!combatScript.isPunching)
                isGliding = false;*/
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            xDir = "right";
            isGliding = true;
            sr.flipX = true;
        }
        else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            /*if (!combatScript.isPunching)
                isGliding = false;*/
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            yDir = "up";
            isWallRunning = !isWallRunning;
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            yDir = "up";
        }
        /*else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!isGrounded && !isOnLeftWall && !isOnRightWall)
                isWallRunning = !isWallRunning;
            else
                isWallRunning = false;
        }*/
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            slide = true;
            isGliding = true;
            yDir = "down";
        }
        else
        {
            slide = false;
            yDir = "none";
        }

        if (Input.GetKeyDown(KeyCode.Space))
            jump = "up";
        else if (Input.GetKeyUp(KeyCode.Space))
            jump = "none";

        //isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(initColliderSizeX - 0.05f, 0.01f), 0, ground);
        groundedLastFrame = isGrounded;
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(initColliderSizeX - 0.05f, 0.01f), 0, ground);
        //isOnRightWall = Physics2D.OverlapBox(rightCheck.position, new Vector2(0.01f, initColliderSizeY - 0.05f), 0, ground) && isGrounded == false;
        //isOnLeftWall = Physics2D.OverlapBox(leftCheck.position, new Vector2(0.01f, initColliderSizeY - 0.05f), 0, ground) && isGrounded == false;

        /*if (!isOnRightWall && timeFromRightWall > 0)
            timeFromRightWall -= Time.deltaTime;
        else if (isOnRightWall)
        {
            timeFromRightWall = 0.1f;

        }

        if (!isOnLeftWall && timeFromLeftWall > 0)
            timeFromLeftWall -= Time.deltaTime;
        else if (isOnLeftWall)
        {
            timeFromLeftWall = 0.1f;

        }*/

        if (rb.velocity.y < -12) // Caps wall sliding speed
        {
            if (isOnLeftWall || isOnRightWall)
            {
                if (isOnLeftWall && xDir == "left")
                    rb.velocity = new Vector2(rb.velocity.x, -12);
                else if (isOnRightWall && xDir == "right")
                    rb.velocity = new Vector2(rb.velocity.x, -12);
            }
            else if (rb.velocity.y < -150)
                rb.gravityScale = 0;
        }
        else if (rb.velocity.y > initJumpSpeed && (isOnLeftWall || isOnRightWall))
        {
            if (isOnLeftWall && xDir == "left")
                rb.gravityScale = 0.5f;
            else if (isOnRightWall && xDir == "right")
                rb.gravityScale = 0.5f;
        }
        else
            rb.gravityScale = 14;

        if (isGrounded || isOnLeftWall || isOnRightWall)
        {
            if (isGrounded)
            {
                isGliding = false;
                groundObj = Physics2D.OverlapBox(groundCheck.position, new Vector2(initColliderSizeX - 0.05f, 0.01f), 0, ground).gameObject;
            }

            totalJumps = 1;
            Physics2D.IgnoreLayerCollision(6, 7, false);
            //gunScript.ammo = 2;
            //shotgunScript.ammo = 3;
        }

        lastFrameVelX = rb.velocity.x;
        lastFrameVelY = rb.velocity.y;
    }

    private void FixedUpdate()
    {

        //Adding forces based on "xDir" and "jump"
        if (xDir == "left" && !slide)// && !combatScript.isPunching && !isWallRunning) // If holding left and not sliding
        {
            if (rb.velocity.x > 0 && rb.velocity.x >= -runCap)
                rb.AddForce(2 * runSpeed * Time.deltaTime * -transform.right, ForceMode2D.Impulse); // changing directions is faster by doubling run speed
            else if (rb.velocity.x >= -runCap + 4)
                rb.AddForce(runSpeed * Time.deltaTime * -transform.right, ForceMode2D.Impulse);
            else if (rb.velocity.x >= -runCap - 4)
                rb.velocity = new Vector2(-runCap, rb.velocity.y);
            else if (!isGliding)
                rb.AddForce(runSpeed / 2 * Time.deltaTime * transform.right, ForceMode2D.Impulse); // Slows player if their velocity is higher than the run cap
        }                                                                                           // (making sliding more optimal for conserving momentum)
        else if (xDir == "right" && !slide)// && !combatScript.isPunching && !isWallRunning)
        {
            if (rb.velocity.x < 0 && rb.velocity.x <= runCap)
                rb.AddForce(2 * runSpeed * Time.deltaTime * transform.right, ForceMode2D.Impulse); // changing directions is faster by doubling run speed
            else if (rb.velocity.x <= runCap - 4)
                rb.AddForce(runSpeed * Time.deltaTime * transform.right, ForceMode2D.Impulse);
            else if (rb.velocity.x <= runCap + 4)
                rb.velocity = new Vector2(runCap, rb.velocity.y);
            else if (!isGliding)
                rb.AddForce(runSpeed / 2 * Time.deltaTime * -transform.right, ForceMode2D.Impulse); // Slows player if their velocity is higher than the run cap
        }                                                                                           //  (making sliding more optimal for conserving momentum)
        else if (Mathf.Abs(rb.velocity.x) > 3) // Deceleration
        {
            if (!slide && isGliding == false)
            {
                if (rb.velocity.x < 0)
                    rb.AddForce(runSpeed * Time.deltaTime * transform.right, ForceMode2D.Impulse);
                else if (rb.velocity.x > 0)
                    rb.AddForce(runSpeed * Time.deltaTime * -transform.right, ForceMode2D.Impulse);
            }
            else if (isGrounded)
            {
                if (rb.velocity.x < 0)
                {
                    if (xDir == "right")
                        rb.AddForce(runSpeed / 3 * Time.deltaTime * transform.right, ForceMode2D.Impulse);
                    else
                        rb.AddForce(runSpeed / 8 * Time.deltaTime * transform.right, ForceMode2D.Impulse);
                }
                else if (rb.velocity.x > 0)
                {
                    if (xDir == "left")
                        rb.AddForce(runSpeed / 3 * Time.deltaTime * -transform.right, ForceMode2D.Impulse);
                    else
                        rb.AddForce(runSpeed / 8 * Time.deltaTime * -transform.right, ForceMode2D.Impulse);
                }
            }
        }
        else if (!Input.GetKey(KeyCode.N))//!grapplingHookScript.ropeLength.enabled)
            rb.velocity = new Vector2(0, rb.velocity.y);

        // Standard jump      
        if (jump == "up" && totalJumps > 0)//jump == "up" || jumpBufferCounter > 0f) && (isGrounded == true || coyoteTimeCounter > 0f))
        {
            isGliding = true;
            totalJumps--;
            jump = "up2";
            leftRay = Physics2D.Raycast(rb.transform.position, Vector2.left, 1, ground);
            rightRay = Physics2D.Raycast(rb.transform.position, Vector2.right, 1, ground);
            jumpParticles.Play();
            //jumpSound.time = 0.12f;
            //jumpSound.Play();

            if ((isOnLeftWall || timeFromLeftWall > 0) && leftRay.collider != null)// && totalWallJumps > 0)
            {
                isJumping = true;

                /*if (sinX < initJumpSpeed && angle * Mathf.Rad2Deg != 0)
                {
                    if (sinX >= 0)
                        rb.velocity = new Vector2(cosX, initJumpSpeed);
                    else
                        rb.velocity = new Vector2(cosX, -initJumpSpeed);
                }
                else
                    rb.velocity = new Vector2(cosX, sinX);*/

                //rb.velocity = new Vector2(cosX, initJumpSpeed);
            }
            else if ((isOnRightWall || timeFromRightWall > 0) && rightRay.collider != null)
            {
                isJumping = true;

                /*if (sinX < initJumpSpeed && degAngle != 180)
                {
                    if (sinX >= 0)
                        rb.velocity = new Vector2(cosX, initJumpSpeed);
                    else
                        rb.velocity = new Vector2(cosX, -initJumpSpeed);
                }
                else
                    rb.velocity = new Vector2(cosX, sinX);*/
            }
            else //&& timeFromLeftWall <= 0 && timeFromRightWall <= 0) // add this and make the whole jump thing an if else for optimization
            {
                isJumping = true;

                if (slide)
                {
                    if (xDir == "left")
                    {
                        if (rb.velocity.x > -33)
                            rb.velocity = new Vector2(-recentHighVelY, 0);
                        else
                            rb.velocity = new Vector2(rb.velocity.x - 2, 0);
                        jumpSpeed /= 1.6f;
                        jumpNum = 0.12f;
                        Physics2D.IgnoreLayerCollision(6, 7, true);
                    }
                    else if (xDir == "right")
                    {

                        if (rb.velocity.x < 33)
                            rb.velocity = new Vector2(recentHighVelY, rb.velocity.y);
                        else
                            rb.velocity = new Vector2(rb.velocity.x + 2, rb.velocity.y);
                        jumpSpeed /= 1.6f;
                        jumpNum = 0.12f;
                        Physics2D.IgnoreLayerCollision(6, 7, true);
                    }
                }

                if (rb.velocity.y < 0)
                    rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                else
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpSpeed);

                //downRay = Physics2D.Raycast(rb.transform.position, Vector2.down, 4, ground); // For Particles
                /*if (!isOnLeftWall && !isOnRightWall)
                    Instantiate(WallJumpParticles, new Vector2(transform.position.x, transform.position.y - 1), Quaternion.Euler(0, 0, 90));*/
            }

            timeFromLeftWall = 0;
            timeFromRightWall = 0;
        }

        if (isJumping && jump == "up2" && jumpNum > 0)
        {
            rb.gravityScale = 0;
            jumpNum -= Time.deltaTime;
        }
        else if (isJumping)
        {
            rb.gravityScale = 14;
            //if(jump == "none")
            //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
            isJumping = false;
            jumpNum = initJumpNum;
            jumpSpeed = initJumpSpeed;
        }
        //stairRay = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.08f), Vector2.right, 0.25f, ground);
        /*Debug.DrawRay(new Vector2(rightCheck.position.x + 0.015f, rightCheck.position.y + 1), new Vector2(0, -1.7f), Color.red);
        Debug.DrawRay(transform.position, new Vector2(2, 0), Color.red);*/
    }

    private void StairPosRound(string dir) // Rounds player position to the top of the tile if they collide a tile while in the air if the entire block is below their center of mass
    {
        stairRay = Physics2D.Raycast(new Vector2(rightCheck.position.x + 0.015f, rightCheck.position.y + 1), Vector2.down, 3, ground);
        if (stairRay.distance > 1.5f && !stairRay.collider.CompareTag("DropThroughPlatForm") && rb.velocity.y > 0)
        {
            transform.position = new Vector2(stairRay.point.x - 0.4f, stairRay.point.y + initColliderSizeY / 2);
            rb.velocity = new Vector2(lastFrameVelX, 0);
        }
        firstFrameOnWall = true;
    }

    // Returns string of the direction of keys
    // For instance, if 'w' and 'd', it will return "upright"
    /*private string KeyDirection()
    {
        string keyDir = "";

        if (yDir != "none")
            keyDir += yDir;
        if (xDir != "none")
            keyDir += xDir;
        if (keyDir == "")
            keyDir = "none";

        return keyDir;
    }*/

    public IEnumerator TimePause(float waitSec)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(waitSec);
        Time.timeScale = 1;
    }
}