using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class TargetBehavior : MonoBehaviour
{
    private Animator animator;
    private CircleCollider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        animator.Play("Hit", -1, 0);

        foreach (Transform child in gameObject.transform)
        {
            BoxCollider2D childCollider = child.GetComponent<BoxCollider2D>();
            childCollider.enabled = !childCollider.enabled;

            if (childCollider.enabled)
                childCollider.GetComponent<Animator>().Play("Close", -1, 0);
            else
                childCollider.GetComponent<Animator>().Play("Open", -1, 0);
        }

        

        collider.enabled = false;
        StartCoroutine(ReactivateTime());
    }

    private IEnumerator ReactivateTime()
    {
        yield return new WaitForSeconds(0.86666666666f);
        collider.enabled = true;
    }
}
