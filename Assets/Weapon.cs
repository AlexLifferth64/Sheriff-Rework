using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Weapon : MonoBehaviour
{
    public Animator animator;

    GameObject player;

    public Camera cam;
    private SpriteRenderer gunSr;
    [SerializeField] private Transform firePoint;
    
    private Transform gunPivot;

    public float angle;
    public Vector2 mousePos;
    public Vector2 lookDir;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player"); // SAME HERE

        cam = GameObject.Find("Camera").GetComponent<Camera>(); // SCRIPT WILL NOT WORK IF CAMERA IS NAMED ANYTHING ELSE BUT "Camera"
        gunPivot = GameObject.Find("PlayerGunPivot").GetComponent<Transform>();

        gunSr = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = GameObject.Find("Player Relative Mouse").transform.position;
        lookDir = mousePos - (Vector2)gunPivot.position;

        angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if(!player.GetComponent<SpriteRenderer>().flipX)
        {
            if(angle > 135 || angle < -135)
                gunSr.flipY = true;
            else
                gunSr.flipY = false;
        }
        else
        {
            if(angle < 45 && angle > -45)
                gunSr.flipY = false;
            else
                gunSr.flipY = true;
        }
    }
}
