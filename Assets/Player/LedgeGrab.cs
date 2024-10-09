using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrab : MonoBehaviour
{
    SimpleMovement movement;

    RaycastHit2D rightLedgeRay;
    RaycastHit2D leftLedgeRay;

    public GameObject rightLedgeGrabBox;
    public GameObject leftLedgeGrabBox;

    private bool isLedgeGrabbing;

    public LayerMask ground;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<SimpleMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(new Vector2(transform.position.x + movement.initColliderSizeX / 2, transform.position.y + 0.6f + movement.initColliderSizeY / 2), new Vector2(transform.position.x + 0.4f + movement.initColliderSizeX / 2, transform.position.y + 0.4f + movement.initColliderSizeY / 2), Color.green);
        if (movement.rb.velocity.y <= 0)
        {
            if (!isLedgeGrabbing)
            {
                if (movement.isOnRightWall)
                {
                    rightLedgeRay = Physics2D.Raycast(new Vector2(transform.position.x + movement.initColliderSizeX / 2, transform.position.y + 0.4f + movement.initColliderSizeY / 2), transform.right, 0.1f, ground); // Raycast alternative
                    if (rightLedgeRay.collider == null)
                    {
                        rightLedgeGrabBox.SetActive(true);
                        isLedgeGrabbing = true;
                        movement.rb.velocity = new Vector2(0, -10);
                    }
                }
                else if (movement.isOnLeftWall)
                {
                    leftLedgeRay = Physics2D.Raycast(new Vector2(transform.position.x - movement.initColliderSizeX / 2, transform.position.y + 0.4f + movement.initColliderSizeY / 2), -transform.right, 0.1f, ground); // Raycast alternative

                    if (leftLedgeRay.collider == null)
                    {
                        leftLedgeGrabBox.SetActive(true);
                        isLedgeGrabbing = true;
                    }
                }
            }
        }

        if (isLedgeGrabbing)
        {
            movement.canMove = false;
            if(Input.GetKey(KeyCode.W))
            {
                if(leftLedgeGrabBox.active)
                    transform.position += new Vector3(-1, 2.3f, 0);
                else
                    transform.position += new Vector3(1, 2.3f, 0);

                isLedgeGrabbing = false;

                DoneLedgeGrabbing();
            }
            else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Space))
            {
                DoneLedgeGrabbing();
            }
        }
    }

    private void DoneLedgeGrabbing()
    {
        rightLedgeGrabBox.SetActive(false);
        leftLedgeGrabBox.SetActive(false);
        isLedgeGrabbing = false;
        movement.canMove = true;
    }
}
