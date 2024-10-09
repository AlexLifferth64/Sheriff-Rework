using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trapdoor : MonoBehaviour
{
    private void Awake()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Animator animator = GetComponent<Animator>();

        if(collider.enabled)
            animator.Play("Close", -1, 0);
        else
            animator.Play("Open", -1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
