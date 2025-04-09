using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//camera mode for camera manager to control

public interface ICameraController 
{
    //decides whether if a camera is suppossed to be active
    //under given game state (ground sliding, rail sliding, cavern wall sliding, etc.)
    bool ShouldApplyState(int gameState);
    
    //apply camera settings (how big the fov is, how big the dampings are, etc.)
    //based on current movement state (jumping, sprinting, etc.)
    //the deltaTime decides the time for blending
    //this changes the current active camera's values, not the camera object
    void ApplySettings(VelocityState velocityState, float deltaTime);
    
    //set the camera to be active
    void SetActive(bool isActive);
}
