using System.Collections;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    private BoxCollider2D playerCollider;
    public float initColliderSizeX;
    public float initColliderSizeY;

    public Transform groundCheck;
    public Transform rightCheck;
    public Transform leftCheck;
    public bool isGrounded = true;
    public bool isOnRightWall = false;
    public bool isOnLeftWall = false;

    private bool onJumpable = false;
    [SerializeField] private bool onNoWall = false;

    public float defaultGravity = 10;

    //[SerializeField] private float timeFromRightWall = 0.1f;
    //[SerializeField] private float timeFromLeftWall = 0.1f;

    public int initJumpSpeed;
    private float jumpSpeed;
    public int runSpeed;
    public int runCap;
    [SerializeField] private int wallRunCap = 45;

    private Collider2D jumpableObj;

    //public float recentHighVelX;
    //public float recentHighVelY;

    //public float lastFrameVelX;
    //public float lastFrameVelY;

    private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter = 0f;
    private bool isJumping = false;

    [SerializeField] private float standingSlideTime = 1; // Has 0 friction when just landed and sliding
    [SerializeField] private float standingSlideCounter = 0;

    //private bool isWallJumping = false;
    public float initJumpNum = 0.15f;
    private float jumpNum = 0.15f;
    public int totalJumps = 1;
    //private int totalWallJumps = 3;

    public string xDir;
    public string yDir;
    public string lastXdir = "right";

    public string jump = "none"; // "up2"
    public bool slide;
    public bool isGliding = false; // When true, x deceleration is off;
    public bool canWallRun = false;
    public bool isWallRunning = false;

    public bool canMove = true;

    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask jumpable;
    [SerializeField] private LayerMask noWall;

    //public GameObject WallJumpParticles;
    //public GameObject CritWallJumpParticles;
    public ParticleSystem jumpParticles;
    //[SerializeField] private ParticleSystem slideParticles;

    //public AudioSource jumpSound;

    DodgeRoll dodgeroll;
    Melee melee;
    LadderMovement ladder;

    //https://www.youtube.com/watch?v=RFix_Kg2Di0 jump buffer and coyote time

    private void Start()
    {
        //DontDestroyOnLoad(transform);

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<BoxCollider2D>();
        dodgeroll = GetComponent<DodgeRoll>();
        melee = GetComponent<Melee>();
        ladder = GetComponent<LadderMovement>();

        //combatScript = GetComponent<Combat>();
        //gunScript = GetComponent<Gun>();

        initColliderSizeX = playerCollider.size.x;
        initColliderSizeY = playerCollider.size.y;
        //recentHighVelX = initJumpSpeed;

        jump = "none";
        //jumpSound.time = 0.12f;
    }

    private void Update()
    {
        //DEBUGGING
        if(Input.GetKeyDown(KeyCode.U))
        {
            rb.velocity = new Vector2(-35, rb.velocity.y);
        }
        else if(Input.GetKeyDown(KeyCode.I))
        {
            rb.velocity = new Vector2(35, rb.velocity.y);
        }
        else if(Input.GetKeyDown(KeyCode.J))
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - 5);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + 5);
        }

        // WASD input: Gets input from the user and sets "xDir" and "jump" accordingly
        xDir = "none";

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            xDir = "left";
            lastXdir = "left";

            isGliding = true;
            sr.flipX = true;

            //playerCollider.offset = new Vector2(0.0625f, playerCollider.offset.y);
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if(rb.velocity.x > 0 && canMove)
                isGliding = false;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            xDir = "right";
            lastXdir = "right";

            isGliding = true;
            sr.flipX = false;
            
            //playerCollider.offset = new Vector2(-0.0625f, playerCollider.offset.y);
        }
        else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            if(rb.velocity.x < 0 && canMove)
                isGliding = false;
        }

        
        
        if (Input.GetKeyDown(KeyCode.Space)) // MAYBE CHANGE TO CAN JUMP
        {
            jumpBufferCounter = jumpBufferTime;
            jump = "up";
        }
        else// if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpBufferCounter -= Time.deltaTime;
            jump = "none";
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            yDir = "up";
            //isWallRunning = !isWallRunning;
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            yDir = "up";
        }
        else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!isGrounded && !isOnLeftWall && !isOnRightWall)
                isWallRunning = !isWallRunning;
            else
                isWallRunning = false;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            slide = true;
            isGliding = true;
            yDir = "down";

            if (isGrounded)
            {
                if(standingSlideCounter >= 0)
                    standingSlideCounter -= Time.deltaTime;

                /*if (Mathf.Abs(rb.velocity.x) > 1)
                {
                    playerCollider.size = new Vector2(initColliderSizeY, initColliderSizeX - 0.05f); // if player is moving, collider widens

                    if (rb.velocity.x < 0) // left
                        playerCollider.offset = new Vector2(initColliderSizeX / 2, (-initColliderSizeX / 2) - 0.025f);
                    else // right
                        playerCollider.offset = new Vector2(initColliderSizeX / -2, (-initColliderSizeX / 2) - 0.025f);
                }
                else
                {*/
                playerCollider.size = new Vector2(initColliderSizeX, initColliderSizeX - 0.05f);
                playerCollider.offset = new Vector2(0, (-initColliderSizeX / 2) - 0.025f);
            }
            else
            {
                playerCollider.size = new Vector2(initColliderSizeX, initColliderSizeY);
                playerCollider.offset = new Vector2(0, 0.0625f); // Change to something more flexible?
            }
        }
        else
        {
            playerCollider.size = new Vector2(initColliderSizeX, initColliderSizeY);
            playerCollider.offset = new Vector2(0, 0.0625f); // Change to something more flexible?

            slide = false;
            yDir = "none";
        }

        //isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(initColliderSizeX - 0.05f, 0.01f), 0, ground);
        isOnRightWall = Physics2D.OverlapBox(new Vector2(rightCheck.position.x, rightCheck.position.y + +0.0625f), new Vector2(0.01f, initColliderSizeY - 0.05f), 0, ground) && !isGrounded; // 0.0625f is from Collider offset
        isOnLeftWall = Physics2D.OverlapBox(new Vector2(leftCheck.position.x, leftCheck.position.y + 0.0625f), new Vector2(0.01f, initColliderSizeY - 0.05f), 0, ground) && !isGrounded;
        //onNoWall = Physics2D.OverlapBox(groundCheck.position, new Vector2(initColliderSizeX - 0.05f, 0.01f), 0, noWall);

        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(initColliderSizeX - 0.05f, 0.01f), 0, ground) || ladder.GetIsOnLadder();


        jumpableObj = Physics2D.OverlapBox(groundCheck.position, new Vector2(initColliderSizeX - 0.05f, 0.01f), 0, jumpable);
        onJumpable = false;

        if(jumpableObj)
        {
            onJumpable = true;
        }

        if (rb.velocity.y < -7 && !dodgeroll.isRolling && !ladder.GetIsOnLadder()) // Caps wall sliding speed
        {
            if (isOnLeftWall || isOnRightWall)
            {
                if (isOnLeftWall && xDir == "left")
                    rb.velocity = new Vector2(rb.velocity.x, -5);
                else if (isOnRightWall && xDir == "right")
                    rb.velocity = new Vector2(rb.velocity.x, -5);
            }
            else if (rb.velocity.y < -150)
                rb.gravityScale = 0;
        }

        if (isGrounded || isOnLeftWall || isOnRightWall)
        {
            //totalJumps = 1;
            if (isGrounded)
            {
                if (Mathf.Abs(GameObject.Find("Camera").GetComponent<CameraBehavior>().yPos - transform.position.y) > 4 && !GameObject.Find("Camera").GetComponent<CameraBehavior>().mouseControl)
                {
                    GameObject.Find("Camera").GetComponent<CameraBehavior>().yPos = transform.position.y;
                }

                if (!slide)
                    rb.gravityScale = 0;
                else if(!ladder.GetIsOnLadder())
                    rb.gravityScale = defaultGravity;

                isGliding = false;
                dodgeroll.canSuperRoll = false;
                GetComponent<Gun>().airShots = 2;
                //groundObj = Physics2D.OverlapBox(groundCheck.position, new Vector2(initColliderSizeX - 0.05f, 0.01f), 0, ground).gameObject;
            }
        }
        else if(!ladder.GetIsOnLadder())
        {
            rb.gravityScale = defaultGravity;
        }
    }

    private void FixedUpdate()
    {
        //Adding forces based on "xDir" and "jump"
        if (!dodgeroll.isRolling)
        {
            if (xDir == "left" && !slide && canMove)// && !combatScript.isPunching && !isWallRunning) // If holding left and not sliding
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
            else if (xDir == "right" && !slide && canMove)// && !combatScript.isPunching && !isWallRunning)
            {
                if (rb.velocity.x < 0 && rb.velocity.x <= runCap)
                    rb.AddForce(2 * runSpeed * Time.deltaTime * transform.right, ForceMode2D.Impulse); // changing directions is faster by doubling run speed
                else if (rb.velocity.x <= runCap - 4)
                    rb.AddForce(runSpeed * Time.deltaTime * transform.right, ForceMode2D.Impulse);
                else if (rb.velocity.x <= runCap + 4)
                    rb.velocity = new Vector2(runCap, rb.velocity.y);
                else if (!isGliding)
                    rb.AddForce(runSpeed / 2 * Time.deltaTime * -transform.right, ForceMode2D.Impulse); // Slows player if their velocity is higher than the run cap
            }
            else if (Mathf.Abs(rb.velocity.x) > 1) // Deceleration
            {
                if (!slide && isGliding == false && jumpBufferCounter < 0)
                {
                    if (rb.velocity.x < 0)
                        rb.AddForce(runSpeed * Time.deltaTime * transform.right, ForceMode2D.Impulse);
                    else if (rb.velocity.x > 0)
                        rb.AddForce(runSpeed * Time.deltaTime * -transform.right, ForceMode2D.Impulse);
                }
                else if (isGrounded && jumpBufferCounter < 0 && standingSlideCounter < 0) // AND SLIDING
                {
                    if (rb.velocity.x < 0)
                        rb.AddForce(runSpeed / 10 * Time.deltaTime * transform.right, ForceMode2D.Impulse);
                    else if (rb.velocity.x > 0)
                        rb.AddForce(runSpeed / 10 * Time.deltaTime * -transform.right, ForceMode2D.Impulse);
                }
            }
            else if(!GetComponent<Gun>().isGrappling)
                rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (jumpBufferCounter > 0/* jump == "up"*/ && (isGrounded || isOnLeftWall || isOnRightWall || onJumpable) && !dodgeroll.isRolling)//jump == "up" || jumpBufferCounter > 0f) && (isGrounded == true || coyoteTimeCounter > 0f))
        {
            Jump();
        }

            /*if (isJumping && jump == "up2" && jumpNum > 0)
            {
                rb.gravityScale = 0;
                jumpNum -= Time.deltaTime;
            }
            else if (isJumping)
            {
                rb.gravityScale = defaultGravity;
                //if(jump == "none")
                //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
                isJumping = false;
                jumpNum = initJumpNum;
                jumpSpeed = initJumpSpeed;
            }*/
            //stairRay = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.08f), Vector2.right, 0.25f, ground);
            /*Debug.DrawRay(new Vector2(rightCheck.position.x + 0.015f, rightCheck.position.y + 1), new Vector2(0, -1.7f), Color.red);
            Debug.DrawRay(transform.position, new Vector2(2, 0), Color.red);*/
        }

    public void Jump()
    {
        
        jumpBufferCounter = 0;
        standingSlideCounter = standingSlideTime;

        isGliding = true;
        totalJumps--;
        jump = "none";
        jumpSpeed = initJumpSpeed;

        //leftRay = Physics2D.Raycast(rb.transform.position, Vector2.left, 1, ground);
        //rightRay = Physics2D.Raycast(rb.transform.position, Vector2.right, 1, ground);
        jumpParticles.Play();
        //jumpSound.Play();

        //StartCoroutine(ShortHop());

        if (isOnLeftWall)// && totalWallJumps > 0)
        {
                //isJumping = true;
            canMove = false;
            sr.flipX = false;
            lastXdir = "right";

            rb.velocity = new Vector2(runCap + 3, initJumpSpeed);

            StartCoroutine(WallMoveTime());
        }
        else if (isOnRightWall/* || timeFromRightWall > 0)*/)
        {
            //isJumping = true;
            canMove = false;
            sr.flipX = true;
            lastXdir = "left";

            rb.velocity = new Vector2(-runCap - 3, initJumpSpeed);

            StartCoroutine(WallMoveTime());
        }
        else if (onJumpable)
        {
            if (jumpableObj.CompareTag("Flask"))
            {
                rb.velocity = new Vector2(rb.velocity.x + jumpableObj.GetComponent<Rigidbody2D>().velocity.x, jumpSpeed);
                jumpableObj.GetComponent<Rigidbody2D>().velocity = new Vector2(jumpableObj.GetComponent<Rigidbody2D>().velocity.x, jumpableObj.GetComponent<Rigidbody2D>().velocity.y - 10);
                jumpableObj.transform.position = groundCheck.transform.position;
            }
        }
        else if(onNoWall) // Ground objects that you can't wall jump off of
        {
            if(slide && xDir == "none")
            {
                transform.position = new Vector2(transform.position.x, transform.position.y - 0.1f);
            }
        }
        else //&& timeFromLeftWall <= 0 && timeFromRightWall <= 0) // add this and make the whole jump thing an if else for optimization
        {
            //isJumping = true;

            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            /*else
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpSpeed);*/
        }
    }

    public float GetJumpBufferCounter()
    {
        return jumpBufferCounter;
    }

    private IEnumerator ShortHop()
    {
        yield return new WaitForSeconds(0.015f);
        if(!Input.GetKey(KeyCode.Space) || Input.GetKeyUp(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
        }
    }

    public IEnumerator TimePause(float waitSec)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(waitSec);
        Time.timeScale = 1;
    }

    public IEnumerator WallMoveTime()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        canMove = true;
    }
}