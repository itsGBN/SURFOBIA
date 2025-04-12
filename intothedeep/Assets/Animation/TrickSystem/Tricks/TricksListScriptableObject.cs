using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Tricks", menuName = "TricksList")]
public class TricksListScriptableObject : ScriptableObject
{
    public List<string> playerInputList = new List<string>();
    public List<InputActionReference> testList = new List<InputActionReference>();
    
    public string trickName;
    public float pointsAmount;

    //so for the scriptable object, you can add them to 'test list', and it will move it to player input list
    public void ConvertTestListToPlayerInputList()
    {
        playerInputList.Clear();
        foreach (var actionReference in testList)
        {
            if (actionReference != null && actionReference.action != null)
            {
                foreach (var control in actionReference.action.controls)
                {
                    playerInputList.Add(control.name);
                }
                
            }
        }
    }
}
