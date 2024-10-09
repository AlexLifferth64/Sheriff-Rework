using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEyeSniperEnemy : MonoBehaviour
{
    private LineRenderer fireLine;
    RaycastHit2D enemySightLine;
    [SerializeField] private bool isAiming = false;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform rotationPoint;

    [SerializeField] LayerMask groundAndPlayerLayer;

    GameObject Player;


    // Start is called before the first frame update
    void Start()
    {
        fireLine = GetComponent<LineRenderer>();
        Player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        enemySightLine = Physics2D.Linecast(transform.position, playerTransform.position, groundAndPlayerLayer);
        //RaycastHit2D enemySightLine = Physics2D.Raycast(transform.position, playerTransform.position - transform.position, groundAndPlayerLayer); // Raycast alternative

        if (enemySightLine) // Checking if the raycast is empty prevents errors
        {
            Debug.DrawLine(transform.position, playerTransform.position, Color.yellow);
            if (enemySightLine.transform.CompareTag("Player"))
            {
                if (!isAiming)
                {
                    StartCoroutine(TimeUntilFire());
                    Debug.DrawLine(transform.position, playerTransform.position, Color.green);
                }
                fireLine.SetPosition(0, transform.position);
                fireLine.SetPosition(1, enemySightLine.point);
            }
            else
            {
                isAiming = false;
                StopAllCoroutines();
                Debug.DrawLine(transform.position, playerTransform.position, Color.red);
            }
            //

            //
        }
        else
        {
            //
        }

    }

    IEnumerator TimeUntilFire()
    {
        isAiming = true;
        yield return new WaitForSeconds(3);

        //canFire = false;
        fireLine.SetPosition(0, transform.position);
        fireLine.SetPosition(1, enemySightLine.point);
        Debug.Log("fire");
        Player.GetComponent<Health>().AdjustHealth(false, 1);

        yield return new WaitForSeconds(0.5f);
        //canFire = true;
        isAiming = false;

        //fireLine.SetPosition(0, transform.position);
        //fireLine.SetPosition(1, playerTransform.position);
    }
}
