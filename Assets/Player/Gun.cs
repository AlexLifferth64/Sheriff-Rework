using System.Collections;
using Unity.Burst.CompilerServices;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Rigidbody2D rb;

    public Animator animator;
    //public Animator outlineAnimator;
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

    public int airShots = 2;
    public bool canFire = true;
    //public GameObject impactEffect;
    public LineRenderer lineRenderer;
    private DistanceJoint2D dj;

    Animator gunAnimator;

    public RaycastHit2D hitInfo;
    public bool hitObject; // Like an enemy or switch


    // Barrel variables
    private int barrelNum = 0; // current index of barrel
    private bool[] barrel = { true, true, true, true, true, true };
    private GameObject[] bullets;
    private bool isReloading;
    private bool isPenalized = false;

    public bool isGrappling = false;

    private bool delayedReloading = false;

    private Vector3 startPos;

    private int shakeCount = 0; // used for barrel shaking effect

    private Transform barrelTransform;

    private Transform player;

    //public AudioSource gunShotSound;

    private SimpleMovement movement;
    public Weapon WeaponScript;
    private Melee melee;

    //https://www.youtube.com/watch?v=wkKsl1Mfp5M

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gunPivot = GameObject.Find("PlayerGunPivot").GetComponent<Transform>();
        gunArm = GameObject.Find("PlayerGunArm").GetComponent<SpriteRenderer>();

        gunAnimator = GameObject.Find("PlayerGunArm").GetComponent<Animator>();

        bullets = new GameObject[6];

        bullets[0] = GameObject.Find("Bullet0");
        bullets[1] = GameObject.Find("Bullet1");
        bullets[2] = GameObject.Find("Bullet2");
        bullets[3] = GameObject.Find("Bullet3");
        bullets[4] = GameObject.Find("Bullet4");
        bullets[5] = GameObject.Find("Bullet5");

        dj = GetComponent<DistanceJoint2D>();

        lineRenderer = GetComponent<LineRenderer>();
        movement = GetComponent<SimpleMovement>();
        melee = GetComponent<Melee>();
        
        barrelTransform = GameObject.Find("Barrel").transform;
        startPos = new Vector3(barrelTransform.position.x, barrelTransform.position.y, 10);


        cam = GameObject.Find("Camera").GetComponent<Camera>(); // SCRIPT WILL NOT WORK IF CAMERA IS NAMED ANYTHING ELSE BUT "Camera"
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.H))
        {
            barrelNum = BarrelNumDeterminer(barrelNum, false);
            //Debug.Log(barrelNum);
            //Debug.Log(barrel[barrelNum]);
        }
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.J))
        {
            barrelNum = BarrelNumDeterminer(barrelNum, true);
            //Debug.Log(barrelNum);
            //Debug.Log(barrel[barrelNum]);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.B))
        {
            if (!barrel[barrelNum])
            {
                if (!delayedReloading)
                    StartCoroutine(DelayedBulletReload(barrelNum));
                else
                    StartCoroutine(PenaltyTime(0.4f));
            }

            if (!melee.isAttacking && barrel[barrelNum] && Time.time >= shotTime + shotDelay && (movement.isGrounded || airShots > 0))
            {
                fire = true;
                shotTime = Time.time;

                
                barrel[barrelNum] = false;
                barrelNum = BarrelNumDeterminer(barrelNum, true);

                Shoot();
                gunAnimator.Play("Fire", -1, 0);

                StartCoroutine(ShotLightTimer());
                StartCoroutine(BarrelFlashEffect(0.4f));
            }
            else if(barrel[barrelNum])
            {
                barrel[barrelNum] = false;
                StartCoroutine(PenaltyTime(0.4f));
            }
        }
        else if(Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.B))
        {
            isGrappling = false;
            dj.enabled = false;
            movement.canMove = true;
        }
        else
        {
            hitObject = false;
        }
        
        if(isPenalized)
        {
            ShakeAffect(-0.3f, 0.3f, shakeCount);
            shakeCount++;
        }
        else
        {
            shakeCount = 0;

        }

        UpdateBarrelVisuals(); // CHANGE SO THAT IT IS ONLY UPDATED WHEN NECESSARY


        //lineRenderer.SetPosition(0, firePoint.position);
        //lineRenderer.SetPosition(1, firePoint.position + (firePoint.right * 50));
    }
    void FixedUpdate()
    {
        /*if (fire == true)
        {
            //gunShotSound.Play();
            //smokeFromGun.Play();
            StartCoroutine(ShotLightTimer());
            mousePos = GameObject.Find("Player Relative Mouse").transform.position;
            lookDir = mousePos - (Vector2)gunPivot.position;
            fire = false;

        }*/
        if (isGrappling)
        {
            lineRenderer.material.SetColor("_Color", new Color(0.4072403f, 0.05126947f, 0.01680738f, 1));
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, dj.connectedAnchor);
            movement.canMove = false;
            movement.isGliding = true;
        }
        else
        {
            if (lineRenderer.material.color.a > 0)
            {
                if (lineRenderer.material.color.a > 0.8)
                    lineRenderer.material.SetColor("_Color", new Color(1, 1, 1, lineRenderer.material.color.a - 0.075f));
                else
                    lineRenderer.material.SetColor("_Color", new Color(1, 1, 0, lineRenderer.material.color.a - 0.075f));
            }
            else
            {
                lineRenderer.material.SetColor("_Color", new Color(1, 1, 1, 0));
            }
        }

        barrelTransform.rotation = Quaternion.Slerp(barrelTransform.rotation, Quaternion.Euler(0, 0, barrelNum * 60), Time.deltaTime * 15);
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

            //barrel[BarrelNumDeterminer(barrelNum, false)] = true; // hotfix to retroactively reload the bullet fired if the player hit something
            // fix later to be not retroactive

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

                barrel[BarrelNumDeterminer(barrelNum, false)] = true;
            }
            else if(hitInfo.collider.CompareTag("Target"))
            {
                hitObject = true;
                StartCoroutine(TimePause(0.08f));

                Debug.Log("TARGET");
                hitInfo.collider.GetComponent<TargetBehavior>().Activate();

                barrel[BarrelNumDeterminer(barrelNum, false)] = true;
            }
            else if (hitInfo.collider.CompareTag("GrapplePoint"))
            {
                hitObject = true;
                StartCoroutine(TimePause(0.08f));

                movement.isGliding = true;
                dj.connectedAnchor = hitInfo.transform.position;
                dj.enabled = true;

                isGrappling = true;

                Debug.Log("GrapplePoint");

                barrel[BarrelNumDeterminer(barrelNum, false)] = true;
            }
            else if (hitInfo.collider.CompareTag("ShotJumpPad"))
            {
                hitObject = true;
                StartCoroutine(TimePause(0.08f));

                movement.isGliding = true;

                if (hitInfo.collider.transform.localScale.x > hitInfo.collider.transform.localScale.y)
                {
                    if (rb.position.y > hitInfo.transform.position.y)
                        rb.velocity = new Vector2(rb.velocity.x, 45);
                    else
                        rb.velocity = new Vector2(rb.velocity.x, -45);
                }
                else
                {
                    if (rb.position.x > hitInfo.transform.position.x)
                        rb.velocity = new Vector2(45, rb.velocity.y);
                    else
                        rb.velocity = new Vector2(-45, rb.velocity.y);
                }

                Debug.Log("ShotJumpPad");

                barrel[BarrelNumDeterminer(barrelNum, false)] = true;
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

    private int BarrelNumDeterminer(int num, bool add) // true adds 1, false subtracts 1
    {
        if (num == 0 && !add)
            return 5;
        else if (num == 5 && add)
            return 0;
        else if (add)
            return num + 1;
        else
            return num - 1;
    }

    IEnumerator PenaltyTime(float time)
    {
        isPenalized = true;
        yield return new WaitForSeconds(time);
        isPenalized = false;

        if (isReloading)
            barrelTransform.position = new Vector3(0, 5, 0) + transform.position;
        else
            barrelTransform.localPosition = startPos;
    }

    private IEnumerator DelayedBulletReload(int i)
    {
        StartCoroutine(GetComponent<DodgeRoll>().Invincibility(0.1f));

        delayedReloading = true;
        yield return new WaitForSeconds(0.25f);
        barrel[i] = true;
        UpdateBarrelVisuals();
        delayedReloading = false;

        StartCoroutine(BarrelFlashEffect(0));

    }

    private void UpdateBarrelVisuals()
    {
        for (int i = 0; i <= 5; i++)
        {
            //Debug.Log(barrel[i]);
            if (barrel[i])
                bullets[i].GetComponent<SpriteRenderer>().color = Color.white;
            else
                bullets[i].GetComponent<SpriteRenderer>().color = Color.black;
        }
    }

    private void ShakeAffect(float lower, float upper, int count)//, float angleRange)
    {
        if (count % 2 == 0)
        {
            if (isReloading)
                barrelTransform.position = new Vector3(0, 5, 0) + player.transform.position;
            else
                barrelTransform.localPosition = startPos;
        }
        else
        {
            float y = Random.Range(lower, upper);
            float x = Random.Range(lower, upper);
            //float angle = Random.Range(-angleRange, angleRange);

            if (isReloading)
                barrelTransform.position = new Vector3(barrelTransform.position.x + x, barrelTransform.position.y + y, 0);
            else
                barrelTransform.localPosition = new Vector3(barrelTransform.localPosition.x + x, barrelTransform.localPosition.y + y, 10);
        }
    }

    private IEnumerator BarrelFlashEffect(float time)
    {
        yield return new WaitForSeconds(time);
        barrelTransform.GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds((1 / (0.25f * 60)) * 2 );
        barrelTransform.GetComponent<SpriteRenderer>().color = new Color32(208, 188, 227, 255);
    }
}