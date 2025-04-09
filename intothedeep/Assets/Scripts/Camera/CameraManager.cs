using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private readonly List<ICameraController> _cameraControllers = new List<ICameraController>();

    public void RegisterCamera(ICameraController cameraController)
    {
        if (cameraController != null)
        {
            if (!_cameraControllers.Contains(cameraController))
            {
                _cameraControllers.Add(cameraController);
            }
        }
    }

    public void SetActiveCamera(ICameraController activeController)
    {
        foreach (var controller in _cameraControllers)
        {
            controller.SetActive(controller == activeController);
        }
    }

    public void ApplyState(VelocityState velocityState, float deltaTime)
    {
        foreach (var controller in _cameraControllers)
        {
            controller.ApplySettings(velocityState, deltaTime);
        }
    }

}
