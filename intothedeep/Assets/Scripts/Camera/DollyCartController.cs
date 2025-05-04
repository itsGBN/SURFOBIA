using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class DollyCartController : MonoBehaviour
{
    [SerializeField] CinemachineDollyCart dollyCart;
    [SerializeField] CinemachineDollyCart priorCart;

    public float neededSpeed;

    public void StartCart()
    {
        dollyCart.m_Speed = neededSpeed;
        ResetCart();
    }

    void ResetCart()
    {
        priorCart.m_Speed = 0f;
        priorCart.m_Position = 0;
    }

}
