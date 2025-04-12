using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;


public class TestTrick01 : MonoBehaviour
{
    private float count;

    //!LIST OF MOVES: Dash, Jump upwards, 
    public float TimerLimit = 1;
    private float Timer = 0f;
    public List<string> playerInputList = new List<string>();

    //this bool activates the entire program to run checks on inputs
    bool isChecking = true;

    public bool timerOn;


    #region CONTROLLER
    private PS5Input GetInputs;

    private void Awake()
    {
        GetInputs = new PS5Input();
    }

    private void OnEnable()
    {
        GetInputs.Enable();
    }

    private void OnDisable()
    {
        GetInputs.Disable();
    }
    #endregion CONTROLLER


// Update is called once per frame
void Update()
    {
        //! RN: it will record if an input is taken in, putting it into a list, and then keep checking if the next input is different than the last, on a timer == if it is, it will record the neew input 
        if (isChecking)
        {

            if (timerOn) Timer += Time.deltaTime;
            if (Timer >= TimerLimit)
            {
                Timer = 0f;
                timerOn = false;

                //Adding a blank space to list
                playerInputList.Clear();

                //playerInputList.Clear();
                Debug.Log("outOfTime!!");
            }
            Vector2 trickValue = GetInputs.PS5Map.TrickStick.ReadValue<Vector2>();

            //Joystick UP
            if (trickValue.y >= 0.8f && (playerInputList.Count == 0 || "up" != playerInputList[playerInputList.Count - 1]))
            {
                playerInputList.Add("up");
                timerOn = true;
                //Debug.Log("up");
            }

            //Joystick DOWN
            if (trickValue.y <= -0.8f  && (playerInputList.Count == 0 || "down" != playerInputList[playerInputList.Count - 1]))
            {
                playerInputList.Add("down");
                timerOn = true;
                //Debug.Log("down");
            }

            //Joystick LEFT
            if (trickValue.x <= -0.8f && (playerInputList.Count == 0 || "left" != playerInputList[playerInputList.Count - 1]))
            {
                playerInputList.Add("left");
                timerOn = true;
                //Debug.Log("left");
            }

            //Joystick RIGHT
            if (trickValue.x >= 0.8f  && (playerInputList.Count == 0 || "right" != playerInputList[playerInputList.Count - 1]))
            {
                playerInputList.Add("right");
                timerOn = true;
                //Debug.Log("right");
            }

            //this works by reading all inputs, and if the last input is different, itll record: this makes sure it doesnt record the same input twice
            InputSystem.onAnyButtonPress
           .CallOnce(ctrl =>
           {

               if (playerInputList.Count == 0 || ctrl.name != playerInputList[playerInputList.Count - 1])
               {
                   playerInputList.Add(ctrl.name);
                   //Timer++;
                   timerOn = true;
                   //Debug.Log("added!");

               }
               else if (ctrl.name == playerInputList[playerInputList.Count - 1])
               {
                   //SameInput
               }
           });

            //Removes items 0 - 9 when list is over 20, keeps it from being too large
            if (playerInputList.Count > 20)
            {
                playerInputList.RemoveRange(0, 10);
            }
        }



    }




}