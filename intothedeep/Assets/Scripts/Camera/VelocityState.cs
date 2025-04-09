using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum to identify whether player is in the air or reaching maximum speed or having speeding up

public enum VelocityState
{
    Landed,
    InAir,
    RegularMaxSpeed,
    RegularSpeed,
    Idle,
}