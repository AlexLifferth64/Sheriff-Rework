using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMovement : MonoBehaviour
{
    [SerializeField] private float ladderSpeed = 7;

    [SerializeField] private bool isClimbingLadder = false;
    private bool isCollidingWithLadder = false;

    private Rigidbody2D rb;

    SimpleMovement movement;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<SimpleMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isCollidingWithLadder)
        {
            if (movement.yDir == "up")
            {
                isClimbingLadder = true;

                movement.canMove = false;
                rb.gravityScale = 0;

                if (movement.GetJumpBufferCounter() > 0)
                {
                    Debug.Log("Ladder Jump");
                    rb.velocity = new Vector2(rb.velocity.x, ladderSpeed);
                }
                else
                    rb.velocity = new Vector2(0, ladderSpeed);

                //rb.transform.position = new Vector2(collision.transform.position.x, rb.transform.position.y);
            }
            else if(Input.GetKeyDown(KeyCode.S))//movement.yDir == "down")
            {
                isClimbingLadder = true;

                movement.canMove = false;
                rb.gravityScale = 0;

                if (movement.GetJumpBufferCounter() > 0)
                    rb.velocity = new Vector2(rb.velocity.x, -ladderSpeed);
                else
                    rb.velocity = new Vector2(0, -ladderSpeed);
                //rb.transform.position = new Vector2(collision.transform.position.x, rb.transform.position.y);

            }
            else if(isClimbingLadder)
            {
                if (movement.yDir == "down")
                {
                    isClimbingLadder = true;

                    movement.canMove = false;
                    rb.gravityScale = 0;

                    if (movement.GetJumpBufferCounter() > 0)
                        rb.velocity = new Vector2(rb.velocity.x, -ladderSpeed);
                    else
                        rb.velocity = new Vector2(0, -ladderSpeed);
                    //rb.transform.position = new Vector2(collision.transform.position.x, rb.transform.position.y);

                }
                else
                {
                    rb.gravityScale = 0;
                    rb.velocity = new Vector2(0, 0);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {

                movement.Jump();
                isClimbingLadder = false;

                movement.canMove = true;
                rb.gravityScale = movement.defaultGravity;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isCollidingWithLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isClimbingLadder = false;
        movement.canMove = true;
        rb.gravityScale = movement.defaultGravity;
        isCollidingWithLadder = false;
    }

    public bool GetIsOnLadder()
    {
        return isClimbingLadder;
    }
}
