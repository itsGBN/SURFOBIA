using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ControlScheme", menuName = "New ControlScheme")]
public class ControlScheme : ScriptableObject
{
    [SerializeField] public string FORWARD;
    [SerializeField] public string BRAKE;
    [SerializeField] public string JUMP;
}
