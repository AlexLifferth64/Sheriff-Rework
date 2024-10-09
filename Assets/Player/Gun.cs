using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Rigidbody2D rb;

    public Animator animator;
    public Animator outlineAnimator;
    private string currentState;
    //public ParticleSystem smokeFromGun;
    public GameObject shotLight;

    private Transform gunPivot;
    private SpriteRenderer gunArm;

    public LayerMask layerMask;
    public Camera cam;
    public Vector2 mousePos;
    public Vector2 lookDir;

    public Transform firePoint;
    public bool fire = false;
    [SerializeField] private float shotDelay;
    public float shotTime;
    //float reloadTime2;

    public int airShots = 2;
    public bool canFire = true;
    //public GameObject impactEffect;
    public LineRenderer lineRenderer;
    public Texture beam1;
    public Texture beam2;
    public Texture beam3;
    //public GameObject decorBullet;
    //public GameObject pistolImpactEffect;
    Animator gunAnimator;

    public RaycastHit2D hitInfo;
    public bool hitObject; // Like an enemy or switch

    //public AudioSource gunShotSound;

    private SimpleMovement movement;
    public Weapon WeaponScript;
    private RevolverBarrel barrel;
    private Melee melee;

    //https://www.youtube.com/watch?v=wkKsl1Mfp5M

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gunPivot = GameObject.Find("PlayerGunPivot").GetComponent<Transform>();
        gunArm = GameObject.Find("PlayerGunArm").GetComponent<SpriteRenderer>();

        gunAnimator = GameObject.Find("PlayerGunArm").GetComponent<Animator>();

        lineRenderer = GetComponent<LineRenderer>();
        movement = GetComponent<SimpleMovement>();
        melee = GetComponent<Melee>();
        barrel = GameObject.Find("Barrel").GetComponent<RevolverBarrel>();
        cam = GameObject.Find("Camera").GetComponent<Camera>(); // SCRIPT WILL NOT WORK IF CAMERA IS NAMED ANYTHING ELSE BUT "Camera"
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.B))
        {
            if (!melee.isAttacking && !barrel.isReloading && barrel.barrel[barrel.barrelNum] && Time.time >= shotTime + shotDelay && (movement.isGrounded || airShots > 0))
            {
                fire = true;
                shotTime = Time.time;
                Shoot();
                gunAnimator.Play("Fire", -1, 0);
                mousePos = GameObject.Find("Player Relative Mouse").transform.position;
                lookDir = mousePos - (Vector2)gunPivot.position;

                StartCoroutine(ShotLightTimer());
                StartCoroutine(BarrelFlashEffect());
            }
        }
        else
        {
            hitObject = false;
        }
        
        //lineRenderer.SetPosition(0, firePoint.position);
        //lineRenderer.SetPosition(1, firePoint.position + (firePoint.right * 50));
    }
    void FixedUpdate()
    {
        if (fire == true)
        {
            //gunShotSound.Play();
            //smokeFromGun.Play();
            StartCoroutine(ShotLightTimer());
            mousePos = GameObject.Find("Player Relative Mouse").transform.position;
            lookDir = mousePos - (Vector2)gunPivot.position;
            fire = false;

        }

        if (lineRenderer.material.color.a > 0)
        {
            if(lineRenderer.material.color.a > 0.8)
                lineRenderer.material.SetColor("_Color", new Color(1, 1, 1, lineRenderer.material.color.a - 0.075f));
            else
                lineRenderer.material.SetColor("_Color", new Color(1, 1, 0, lineRenderer.material.color.a - 0.075f));
        }
        else
        {
            lineRenderer.material.SetColor("_Color", new Color(1, 1, 1, 0));
        }
    }

    IEnumerator ShotLightTimer()
    {
        shotLight.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        shotLight.SetActive(false);
    }

    void Shoot()
    {
        //StartCoroutine(cam.GetComponent<CameraBehavior>().ScreenShake(-0.3f, 0.3f, 0.5f, 0.15f));

        hitInfo = Physics2D.Raycast(gunPivot.position, gunPivot.right, 80, layerMask);
        StartCoroutine(TurnOffRay());
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, firePoint.position);

        //Instantiate(pistolImpactEffect, hitInfo.point, firePoint.rotation);
        if (WeaponScript.angle <= -45 && WeaponScript.angle >= -135)// && !hitInfo.collider.CompareTag("JumpPad")) // Down
        {
            if (rb.velocity.y > 0)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + 4);
            else
                rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        if (hitInfo)
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hitInfo.point);

            if (hitInfo.collider.CompareTag("Enemy") || hitInfo.collider.CompareTag("Flask"))
            {
                //barrel.barrel[barrel.barrelNum] = true;

                hitObject = true;
                StartCoroutine(TimePause(0.08f));

                if (hitInfo.point.y > hitInfo.transform.position.y + 0.5f)
                {
                    hitInfo.collider.GetComponent<Health>().AdjustHealth(false, 5);
                }
                else if (hitInfo.point.y > hitInfo.transform.position.y - 0.5f)
                    hitInfo.collider.GetComponent<Health>().AdjustHealth(false, 2);
                else
                    hitInfo.collider.GetComponent<Health>().AdjustHealth(false, 1);
            }
            else if(hitInfo.collider.CompareTag("Target"))
            {
                hitObject = true;
                StartCoroutine(TimePause(0.08f));

                Debug.Log("TARGET");
                hitInfo.collider.GetComponent<TargetBehavior>().Activate();
            }
            else
            {
                //barrel.barrel[barrel.barrelNum] = false;

                if (!movement.isGrounded)
                    airShots--;
            }
            //GameObject currentDecorBullet = Instantiate(decorBullet);
            //currentDecorBullet.transform.SetPositionAndRotation(hitInfo.point, firePoint.rotation);
        }
        else
        {
            //barrel.barrel[barrel.barrelNum] = false;

            if (!movement.isGrounded)
                airShots--;

            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + (firePoint.right * 50));
        }
    }

    /*private void changeAnimationState(string newState)
    {
        animator.Play(newState);
        outlineAnimator.Play(newState);

        currentState = newState;
    }*/
    public void EnableGunArm(bool enable)
    {
        gunArm.enabled = enable;
    }

    private IEnumerator TurnOffRay()
    {
        //lineRenderer.material.SetTexture("_MainTex", beam1);
        lineRenderer.material.SetColor("_Color", new Color(1, 1, 1, 1));

        yield return new WaitForSeconds(0.01f);
        //lineRenderer.material.SetTexture("_MainTex", beam2);
        yield return new WaitForSeconds(0.02f);
        //lineRenderer.material.SetTexture("_MainTex", beam3);
        yield return new WaitForSeconds(0.03f);
        //lineRenderer.enabled = false;
    }
    private IEnumerator TimePause(float waitSec)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(waitSec);
        Time.timeScale = 1;
    }
    private IEnumerator BarrelFlashEffect()
    {
        yield return new WaitForSeconds(0.4f);
        barrel.GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds((1 / (0.25f * 60)) * 2 );
        barrel.GetComponent<SpriteRenderer>().color = new Color32(208, 188, 227, 255);
    }
}
