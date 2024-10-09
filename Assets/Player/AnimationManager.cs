using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;
    public string currentState;

    private Coroutine armShift;

    private SimpleMovement MovementScript;
    private DodgeRoll RollScript;
    private Melee MeleeScript;

    private Transform playerGunPivot;

    // Start is called before the first frame update
    void Start()
    {
        playerGunPivot = GameObject.Find("PlayerGunPivot").transform;

        animator = GetComponent<Animator>();
        MovementScript = GetComponent<SimpleMovement>();
        RollScript = GetComponent<DodgeRoll>();
        MeleeScript = GetComponent<Melee>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*if (CombatScript.isPunching)
            changeAnimationState("Punch");
        else */
        if (currentState != "Land" || currentState != "Idle" || !MovementScript.isGrounded)
        {
            if (RollScript.isRolling)
                changeAnimationState("DodgeRoll");
            else if (MeleeScript.isAttacking)
                changeAnimationState("Knife");
            else if (MovementScript.isGrounded)
            {
                if (currentState == "FallingJump" || MovementScript.xDir == "none" && !MovementScript.slide)
                {
                    changeAnimationState("Land");
                    //StartCoroutine(LandTime());
                }
                else if (!MovementScript.slide)
                {
                    if (currentState == "SlideDown")
                        changeAnimationState("SlideUp");
                    if (MovementScript.xDir == "left" || MovementScript.xDir == "right")
                        changeAnimationState("Walk");
                    else if (currentState != "FallingJump")
                        changeAnimationState("Idle");
                }
                //else if (MovementScript.rb.velocity.x != 0)
                //changeAnimationState("Slide");
                else
                {
                    if(MovementScript.rb.velocity.x > 0)
                        changeAnimationState("SlideDown");
                    else if (MovementScript.rb.velocity.x < 0)
                        changeAnimationState("SlideDown");
                    else
                        changeAnimationState("CrouchDown");
                }
            }
            else
            {
                /*if(MovementScript.rb.velocity.x > 0)
                    MovementScript.sr.flipX = true;
                else if (MovementScript.rb.velocity.x < 0)
                    MovementScript.sr.flipX = false;*/
                /*if (MovementScript.isWallRunning)
                {
                    changeAnimationState("WallRun");
                }
                else if (MovementScript.isOnLeftWall && MovementScript.xDir == "left")
                {
                    MovementScript.sr.flipX = true;
                    changeAnimationState("WallSlide");
                }
                else if (MovementScript.isOnRightWall && MovementScript.xDir == "right")
                {
                    MovementScript.sr.flipX = false;
                    changeAnimationState("WallSlide");
                }
                else*/
                if (MovementScript.rb.velocity.y > 8)
                    changeAnimationState("RisingJump");
                else if (MovementScript.rb.velocity.y < -8)
                    changeAnimationState("FallingJump");
                else
                    changeAnimationState("MiddleJump");
            }
        }
    }

    private IEnumerator ArmCrouchDown()
    {
        float frameTime = 1 / (0.35f * 60); // How long a frame is displayed with 0.35 being the speed of the animation playing
        //Transform prm = GameObject.Find("Player Relative Mouse").transform;

        playerGunPivot.transform.localPosition = new Vector2(0, 0);
        //prm.localPosition = new Vector2(prm.transform.localPosition.x, transform.localPosition.y - 0.25f);

        yield return new WaitForSeconds(frameTime);
        playerGunPivot.transform.localPosition = new Vector2(0, -0.125f);
        //prm.localPosition = new Vector2(prm.transform.localPosition.x, transform.localPosition.y - 0.125f);


        yield return new WaitForSeconds(frameTime);
        playerGunPivot.transform.localPosition = new Vector2(0, -0.25f);
        //prm.localPosition = new Vector2(prm.transform.localPosition.x, transform.localPosition.y - 0.125f);
    }

    private IEnumerator ArmCrouchUp()
    {
        float frameTime = 1 / (0.35f * 60); // How long a frame is displayed with 0.35 being the speed of the animation playing
        //Transform prm = GameObject.Find("Player Relative Mouse").transform;

        playerGunPivot.transform.localPosition = new Vector2(0, -0.125f);
        //prm.localPosition = new Vector2(prm.transform.localPosition.x, transform.localPosition.y - 0.25f);

        yield return new WaitForSeconds(frameTime);
        playerGunPivot.transform.localPosition = new Vector2(0, 0);
        //prm.localPosition = new Vector2(prm.transform.localPosition.x, transform.localPosition.y - 0.125f);


        yield return new WaitForSeconds(frameTime);
        playerGunPivot.transform.localPosition = new Vector2(0, 0.25f);
        //prm.localPosition = new Vector2(prm.transform.localPosition.x, transform.localPosition.y - 0.125f);
    }

    private IEnumerator ArmSlideDown()
    {
        float frameTime = 1 / (0.35f * 60); // How long a frame is displayed with 0.35 being the speed of the animation playing
        //Transform prm = GameObject.Find("Player Relative Mouse").transform;

        playerGunPivot.transform.localPosition = new Vector2(0, 0.125f);

        yield return new WaitForSeconds(frameTime);
        playerGunPivot.transform.localPosition = new Vector2(0, -0.125f);

        yield return new WaitForSeconds(frameTime);
        playerGunPivot.transform.localPosition = new Vector2(0, -0.5f);


        yield return new WaitForSeconds(frameTime);
        playerGunPivot.transform.localPosition = new Vector2(0, -0.625f);
    }

    private void changeAnimationState(string newState)
    {
        if (currentState == newState)
            return;

        animator.Play(newState);

        currentState = newState;

        if (currentState == "CrouchDown")
            armShift = StartCoroutine(ArmCrouchDown());
        else if (currentState == "SlideDown")
            armShift = StartCoroutine(ArmSlideDown());
        else if (playerGunPivot.transform.localPosition.y != 0.25f)
        {
            StopCoroutine(armShift);
            if(MovementScript.isGrounded)
                armShift = StartCoroutine(ArmCrouchUp());
            else
                playerGunPivot.transform.localPosition = new Vector2(0, 0.25f);
        }
    }
    IEnumerator LandTime()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        currentState = "Idle";
    }
}
