using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] Animator mantaAnim;
    [SerializeField] Animator skeletonAnim;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            mantaAnim.SetFloat("Speed", 1);
            skeletonAnim.SetFloat("Speed", 1);
        }

        if(Input.GetKeyUp(KeyCode.W))
        {
            mantaAnim.SetFloat("Speed", 0);
            skeletonAnim.SetFloat("Speed", 0);
        }
    }
}
