using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class IntroSignal : MonoBehaviour
{
    public GameObject imageObject;

    public PlayerController player;
    public Animator animator;

    [SerializeField] private CinemachineDollyCart dollyCart_1;
    [SerializeField] private CinemachineDollyCart dollyCart_2;
    
    public float dolly_1_Speed;
    public float dolly_2_Speed;
    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDollyCart(int cart)
    {
        Debug.Log("Dolly ");
        switch (cart)
        {
            case 1:
                dollyCart_1.enabled = true;
                Debug.Log("Dolly cart 1 enabled");
                break;
            case 2:
                dollyCart_2.enabled = true;
                break;
        }
        
    }

    public void closeimage()
    {
        imageObject.SetActive(false);
        player.introDirectorEnds();
        
    }

    public void fadeInAnim()
    {
        if (animator != null)
        {
            animator.Play("FadeIn");
        }
    }
    
    public void fadeOutAnim()
    {
        if (animator != null)
        {
            animator.Play("FadeOut");
        }
    }
    
}
