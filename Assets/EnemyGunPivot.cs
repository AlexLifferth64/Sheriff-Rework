using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGunPivot : MonoBehaviour
{
    public GameObject player;
    public Vector2 lookDir;
    public float angle;
    Quaternion rotation;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        lookDir = (Vector2)player.transform.position - (Vector2)transform.position;
        angle = (Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg) - 90;

        if (angle < 180)
        {
            rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            //Debug.Log("yeeeeeeeee");
        }
        else
        {
            rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            //Debug.Log("nooooooooo");
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1);
    }
}
