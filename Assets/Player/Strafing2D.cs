using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strafing2D : MonoBehaviour
{
    Rigidbody2D rb;

    Vector2 lastFrameMousePos;

    Vector2 lookDir;
    float lookAngle;

    Vector2 velVector;
    float velAngle;

    Camera cam;

    SimpleMovement movement;
    Weapon weapon;

    //LineRenderer lr;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //lr = GetComponent<LineRenderer>(); // For Debugging
        sr = GetComponent<SpriteRenderer>();

        cam = GameObject.Find("Camera").GetComponent<Camera>();
        movement = GetComponent<SimpleMovement>();
        weapon = transform.Find("PlayerGunPivot").GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(lastFrameMousePos);
        //  qDebug.Log((Vector2)cam.ScreenToWorldPoint(Input.mousePosition));
        
        float differenceAngle = weapon.angle - velAngle;
        
        velVector = rb.velocity.normalized;
        velAngle = Mathf.Atan2(velVector.y, velVector.x) * Mathf.Rad2Deg;
        
        if (lastFrameMousePos != (Vector2)Input.mousePosition && !movement.isGrounded && rb.velocity.x != 0 && movement.yDir == "down" && Mathf.Abs(differenceAngle) < 90) //Mouse must be moving to accelerate rb
        {
            if (rb.velocity.x > 0)
                rb.AddForce(10 * Mathf.Cos((weapon.angle - velAngle) * Mathf.Deg2Rad) * Time.deltaTime * transform.right, ForceMode2D.Impulse);
            else
                rb.AddForce(10 * Mathf.Cos((weapon.angle - velAngle) * Mathf.Deg2Rad) * Time.deltaTime * -transform.right, ForceMode2D.Impulse);
            //sr.color = Color.green;
        }/*
        else
        {
            sr.color = Color.red;
        }*/

        //lr.SetPosition(0, transform.position);
        //lr.SetPosition(1, (Vector2)transform.position + (15 * velVector));
        //Debug.DrawLine(transform.position, (Vector2)transform.position + (3 * velVector), Color.red);
        lastFrameMousePos = Input.mousePosition;
    }
}
