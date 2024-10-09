using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverEnemy : MonoBehaviour
{
    private LineRenderer fireLine;
    RaycastHit2D enemySightLine;
    [SerializeField] private bool isAiming = false;
    [SerializeField] private bool isAimingFinal = false;

    //[SerializeField] private Transform playerTransform;
    //[SerializeField] private Transform rotationPoint;

    [SerializeField] LayerMask groundAndPlayerLayer;

    GameObject player;
    [SerializeField] GameObject bulletPrefab;


    // Start is called before the first frame update
    void Start()
    {
        fireLine = GetComponent<LineRenderer>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        enemySightLine = Physics2D.Raycast(transform.position, player.transform.position - transform.position, 100, groundAndPlayerLayer); // Raycast alternative

        if (enemySightLine) // Checking if the raycast is empty prevents errors
        {
            //Debug.DrawLine(transform.position, playerTransform.position, Color.yellow);
            if (enemySightLine.transform.CompareTag("Player"))
            {
                if (!isAiming)
                {
                    StartCoroutine(TimeUntilFire());

                }
                //fireLine.SetPosition(0, transform.position);
                //fireLine.SetPosition(1, enemySightLine.point);
            }
            else
            {
                isAiming = false;
                if (!isAimingFinal)
                    StopAllCoroutines();
                //Debug.DrawLine(transform.position, playerTransform.position, Color.red);
            }
        }
        //enemySightLine = Physics2D.Linecast(transform.position, playerTransform.position, groundAndPlayerLayer);
        /*if (!isAimingFinal)
        {
            enemySightLine = Physics2D.Raycast(transform.position, playerTransform.position - transform.position, 100, groundAndPlayerLayer); // Raycast alternative

            if (enemySightLine) // Checking if the raycast is empty prevents errors
            {
                //Debug.DrawLine(transform.position, playerTransform.position, Color.yellow);
                if (enemySightLine.transform.CompareTag("Player"))
                {
                    if (!isAiming)
                    {
                        StartCoroutine(TimeUntilFire());

                    }
                    fireLine.SetPosition(0, transform.position);
                    fireLine.SetPosition(1, enemySightLine.point);
                }
                else
                {
                    isAiming = false;
                    if (!isAimingFinal)
                        StopAllCoroutines();
                    Debug.DrawLine(transform.position, playerTransform.position, Color.red);
                }
            }
        }
        Debug.DrawLine(transform.position, enemySightLine.point, Color.green);*/
    }

    IEnumerator TimeUntilFire()
    {
        isAiming = true;
        yield return new WaitForSeconds(1);

        isAimingFinal = true;
        Instantiate(bulletPrefab, transform.position, transform.Find("GunPivot").rotation, null);
        Debug.Log("fireREVOLVER");

        yield return new WaitForSeconds(0.25f);


        Instantiate(bulletPrefab, transform.position, transform.Find("GunPivot").rotation, null);

        yield return new WaitForSeconds(0.25f);


        Instantiate(bulletPrefab, transform.position, transform.Find("GunPivot").rotation, null);


        yield return new WaitForSeconds(0.25f);


        //enemySightLine = Physics2D.Raycast(transform.position, tempPlayerPos - (Vector2)transform.position, 100, groundAndPlayerLayer); // Raycast alternative

        //canFire = false;
        //fireLine.SetPosition(0, transform.position);
        //fireLine.SetPosition(1, enemySightLine.point);
        //Debug.Log("fireREVOLVER");

        yield return new WaitForSeconds(0.8f);
        //canFire = true;
        isAiming = false;
        isAimingFinal = false;

        //fireLine.SetPosition(0, transform.position);
        //fireLine.SetPosition(1, playerTransform.position);
    }
}
