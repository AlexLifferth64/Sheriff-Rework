using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Flask : MonoBehaviour
{
    [SerializeField] float drinkTime = 1;

    Coroutine drinkCo;
    private bool isDrinking = false;
    public int flasks = 3;

    [SerializeField] private GameObject flaskPrefab;
    private GameObject flaskObj;

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.C) && flasks > 0 && !isDrinking)
        {
            isDrinking = true;
            flasks--;

            GetComponent<SimpleMovement>().canMove = false;
            drinkCo = StartCoroutine(Drink(drinkTime));
        }
        else if(Input.GetKeyDown(KeyCode.Mouse0) && isDrinking)
        {
            StopCoroutine(drinkCo);
            flaskObj = Instantiate(flaskPrefab, transform.position, new Quaternion(0,0,0,1));
            flaskObj.transform.SetParent(GameObject.Find("Player").transform);
            flaskObj.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
            
            GetComponent<SimpleMovement>().canMove = true;
            isDrinking = false;

        }*/

        if ((Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Mouse4) || Input.GetKeyDown(KeyCode.N)) && flasks > 0 && !isDrinking)
        {
            if(GetComponent<SimpleMovement>().isGrounded)
            {
                isDrinking = true;

                GetComponent<SimpleMovement>().canMove = false;
                drinkCo = StartCoroutine(Drink(drinkTime));
            }
            else
            {
                flaskObj = Instantiate(flaskPrefab, transform.position, GameObject.Find("PlayerGunPivot").transform.rotation);
                flaskObj.transform.SetParent(transform);

                flaskObj.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;

                GameObject.Find("Player").GetComponent<Gun>().shotTime = Time.time - 0.1f; // 0.3 delay
            }

            flasks--;
        }
    }

    IEnumerator Drink(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<Health>().AdjustHealth(true, 1);
        isDrinking = false;
        GetComponent<SimpleMovement>().canMove = true;
    }
}
