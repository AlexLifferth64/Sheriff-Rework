using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastSniperEnemy : MonoBehaviour
{
    private LineRenderer fireLine;
    RaycastHit2D enemySightLine;
    [SerializeField] private bool isAiming = false;
    [SerializeField] private bool isAimingFinal = false;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform rotationPoint;

    [SerializeField] LayerMask groundAndPlayerLayer;

    GameObject player;
    

    // Start is called before the first frame update
    void Start()
    {
        fireLine = GetComponent<LineRenderer>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //enemySightLine = Physics2D.Linecast(transform.position, playerTransform.position, groundAndPlayerLayer);
        if (!isAimingFinal)
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
                    //Debug.DrawLine(transform.position, playerTransform.position, Color.red);
                }
            }
        }
        //Debug.DrawLine(transform.position, enemySightLine.point, Color.green);
    }

    IEnumerator TimeUntilFire()
    {
        fireLine.startColor = Color.red;
        fireLine.endColor = Color.red;

        isAiming = true;
        yield return new WaitForSeconds(2);
        
        isAimingFinal = true;
        Vector2 tempPlayerPos = player.transform.position;

        StartCoroutine(FluctuateLineMaterial(0.05f, 0.4f, fireLine, Color.clear));

        yield return new WaitForSeconds(0.4f);
        
        enemySightLine = Physics2D.Raycast(transform.position, tempPlayerPos - (Vector2)transform.position, 100, groundAndPlayerLayer); // Raycast alternative

        //canFire = false;
        fireLine.SetPosition(0, transform.position);
        fireLine.SetPosition(1, enemySightLine.point);
        Debug.Log("fire");
        if (enemySightLine)
        {
            if (enemySightLine.transform.CompareTag("Player"))
                player.GetComponent<Health>().AdjustHealth(false, 1);
        }
        
        yield return new WaitForSeconds(1.5f);
        //canFire = true;
        isAiming = false;
        isAimingFinal = false;

        //fireLine.SetPosition(0, transform.position);
        //fireLine.SetPosition(1, playerTransform.position);
    }

    IEnumerator FluctuateLineMaterial(float frequency, float duration, LineRenderer lr, Color newColor)
    {
        float repeatTimes = duration / frequency;
        Color initColor = lr.startColor;

        for (int i = 0; i < (int)repeatTimes; i++)
        {
            if(i % 2 == 0)
            {
                lr.startColor = newColor;
                lr.endColor = newColor;
            }
            else
            {
                lr.startColor = initColor;
                lr.endColor = initColor;
            }

            yield return new WaitForSeconds(frequency);
        }
        fireLine.startColor = newColor;
        fireLine.endColor = newColor;
    }
}
