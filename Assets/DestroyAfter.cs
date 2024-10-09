using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float timeDelay;
    float time = 0;

    private void Start()
    {
        time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > time + timeDelay)
            Destroy(gameObject);
    }
}
