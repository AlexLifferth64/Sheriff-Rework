using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverBarrel : MonoBehaviour
{
    public int barrelNum = 0;
    public bool[] barrel = {true, true, true, true, true, true};
    public GameObject[] bullets;
    public bool isReloading;
    public bool isPenalized = false;

    private IEnumerator delayedReloadEnum = null;
    private bool delayedReloading = false;

    private Vector3 startPos;

    private int shakeCount = 0; // used for barrel shaking effect

    private Gun gun;
    private Transform player;

    private void Start()
    {
        bullets = new GameObject[6];

        bullets[0] = GameObject.Find("Bullet0");
        bullets[1] = GameObject.Find("Bullet1");
        bullets[2] = GameObject.Find("Bullet2");
        bullets[3] = GameObject.Find("Bullet3");
        bullets[4] = GameObject.Find("Bullet4");
        bullets[5] = GameObject.Find("Bullet5");

        startPos = new Vector3(transform.position.x, transform.position.y, 10);

        gun = GameObject.Find("Player").GetComponent<Gun>();
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isPenalized)
        {
            if (!isReloading)
            {
                //isPenalized = true;
                isPenalized = false;
                isReloading = true;

                //StartCoroutine(BarrelPenaltyTime(0.01f));
            }
            else
            {
                isReloading = false;


                GameObject.Find("Player Relative Mouse").GetComponent<PlayerRelativeMouse>().lockMouse = false;
            }
        }

        if (isReloading)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, player.position.y + 5, 0), 0.35f);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, 0.35f);
        }


        /*if (!isReloading)
            SetBarrelAndBulletSpriteRenderers(false);
        else
            SetBarrelAndBulletSpriteRenderers(true);*/

        if (!isPenalized)
        {
            shakeCount = 0;

            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                bullets[BarrelNumDeterminer(barrelNum, false)].GetComponent<SpriteRenderer>().color = Color.yellow;
            }

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

            /*if(Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.Mouse3)) // Quick Reload
            {
                if (barrel[barrelNum])
                    StartCoroutine(PenaltyTime(0.4f));
                else if(delayedReloadEnum != null)
                {
                    delayedReloadEnum = DelayedBulletReload(barrelNum);
                }
                UpdateBarrelVisuals();
            }*/
            Debug.Log(delayedReloadEnum);

            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.B))
            {
                if (!barrel[barrelNum])
                {
                    if (!delayedReloading)
                        StartCoroutine(DelayedBulletReload(barrelNum));
                    else
                        StartCoroutine(PenaltyTime(0.4f));
                }
                else
                {
                    barrel[barrelNum] = false;
                    barrelNum = BarrelNumDeterminer(barrelNum, true);
                }
                /*if (isReloading)
                {
                    if (barrel[barrelNum])
                        StartCoroutine(PenaltyTime(0.4f));
                    else if(!delayedReloading)
                    {
                        StartCoroutine(DelayedBulletReload(barrelNum));
                    }
                    else
                        StartCoroutine(PenaltyTime(0.4f));
                }
                else */

                /*else
                {
                    barrelNum = BarrelNumDeterminer(barrelNum, true);
                    StartCoroutine(PenaltyTime(0.3f));
                }*/

                UpdateBarrelVisuals();
                //Debug.Log(barrel[0] + " " + barrel[1] + " " + barrel[2] + " " + barrel[3] + " " + barrel[4] + " " + barrel[5]);
            }
        }
        else
        {
            ShakeAffect(-0.3f, 0.3f, shakeCount);
            shakeCount++;
        }
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, barrelNum * 60), Time.deltaTime * 15);
    }

    public void UpdateBarrelVisuals()
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

    /*private void SetBarrelAndBulletSpriteRenderers(bool enable)
    {
        GetComponent<SpriteRenderer>().enabled = enable;

        foreach(GameObject bullet in bullets)
        {
            bullet.GetComponent<SpriteRenderer>().enabled = enable;
        }
    }*/

    IEnumerator PenaltyTime(float time)
    {
        isPenalized = true;
        yield return new WaitForSeconds(time);
        isPenalized = false;

        if (isReloading)
            transform.position = new Vector3(0, 5, 0) + player.transform.position;
        else
            transform.localPosition = startPos;
    }

    private IEnumerator DelayedBulletReload(int i)
    {
        StartCoroutine(player.GetComponent<DodgeRoll>().Invincibility(0.1f));

        delayedReloading = true;
        yield return new WaitForSeconds(0.25f);
        barrel[i] = true;
        UpdateBarrelVisuals();
        delayedReloading = false;

        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds((1 / (0.25f * 60)) * 2);
        GetComponent<SpriteRenderer>().color = new Color32(208, 188, 227, 255);

    }

    private void ShakeAffect(float lower, float upper, int count)//, float angleRange)
    {
        if(count % 2 == 0)
        {
            if(isReloading)
                transform.position = new Vector3(0, 5, 0) + player.transform.position;
            else
                transform.localPosition = startPos;
        }
        else
        {
            float y = Random.Range(lower, upper);
            float x = Random.Range(lower, upper);
            //float angle = Random.Range(-angleRange, angleRange);

            if(isReloading)
                transform.position = new Vector3(transform.position.x + x, transform.position.y + y, 0);
            else
                transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y + y, 10);
        }
    }
}
