using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI tutorialText;

    [Header("References")]
    [SerializeField] private TrickListener trickListener;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TestTrick01 testTrick01;

    private Queue<string> tutorialSteps = new Queue<string>();
    private bool isTutorialActive = true;

    void Start()
    {
        InitializeTutorialSteps();
        DisplayNextStep();
    }

    void Update()
    {
        if (!isTutorialActive) return;

        // Check for specific actions to progress the tutorial
        if (tutorialSteps.Peek().Contains("Move") && CheckMovement())
        {
            CompleteCurrentStep();
        }
        else if (tutorialSteps.Peek().Contains("Jump") && CheckJump())
        {
            CompleteCurrentStep();
        }
        else if (tutorialSteps.Peek().Contains("Trick") && CheckTrick())
        {
            CompleteCurrentStep();
        }
        else if (tutorialSteps.Peek().Contains("Grind") && CheckGrind())
        {
            CompleteCurrentStep();
        }

    }

    private void InitializeTutorialSteps()
    {
        tutorialSteps.Enqueue("Move: Use the left joystick to move.");
        tutorialSteps.Enqueue("Jump: Press the jump button to jump.");

        foreach (var trick in trickListener.predefinedTricks)
        {
            trick.ConvertTestListToPlayerInputList();

            // Construct a detailed description of the trick's actions
            string actions = string.Join(", ", trick.playerInputList);
            tutorialSteps.Enqueue($"Trick: Perform the {trick.trickName} trick by doing the following actions: {actions}.");
        }
        tutorialSteps.Enqueue("Grind: Approach a rail and press jump.");
    }

    private void DisplayNextStep()
    {
        if (tutorialSteps.Count > 0)
        {
            tutorialText.text = tutorialSteps.Peek();
        }
        else
        {
            tutorialText.text = "Tutorial Complete!";
            isTutorialActive = false;
        }
    }

    private void CompleteCurrentStep()
    {
        tutorialSteps.Dequeue();
        DisplayNextStep();
    }

    private bool CheckMovement()
    {
        Vector2 movementInput = testTrick01.GetInputs.PS5Map.Move.ReadValue<Vector2>();
        // Debug.Log($"Movement Input: {movementInput.y}");
        return movementInput.y > 0.1f;
    }

    private bool CheckJump()
    {
        return testTrick01.GetInputs.PS5Map.Jump.WasPressedThisFrame();
    }

    private bool CheckGrind()
    {
        if (playerController.currentState is PlayerController.GrindState)
        {
            Debug.Log("Grind detected!");
            return true;
        }

        return false;
    }

    private bool CheckTrick()
    {
        foreach (var trick in trickListener.predefinedTricks)
        {

            trick.ConvertTestListToPlayerInputList();


            if (PlayerInputMatchesTrick(testTrick01.playerInputList, trick.playerInputList))
            {
                Debug.Log($"Trick Completed: {trick.trickName}");
                return true;
            }
        }
        return false;
    }

    private bool PlayerInputMatchesTrick(List<string> playerInputList, List<string> trickInputList)
    {
        if (playerInputList.Count < trickInputList.Count)
            return false;


        for (int i = 0; i < trickInputList.Count; i++)
        {
            if (playerInputList[playerInputList.Count - trickInputList.Count + i] != trickInputList[i])
            {
                return false;
            }
        }
        return true;
    }
}
