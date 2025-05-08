using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCam : MonoBehaviour
{
    public Transform target;

    public bool signal;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target is not null)
        {
            this.transform.position = target.position;

            if (signal)
            {
                //shake head
                animator.SetTrigger("shakeHead");
            }
        }
    }
}
