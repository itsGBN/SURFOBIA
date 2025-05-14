using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    public static Vector3 checkpointPosition = Vector3.zero; // Static variable to store the checkpoint position
    public static Quaternion checkpointRotation = Quaternion.identity;
    // Start is called before the first frame update
    void Awake()
    {
        // if (checkpointPosition == null)
        // {
        //     checkpointPosition = transform.position;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            checkpointPosition = other.transform.position;
            checkpointRotation = other.transform.rotation;
            Debug.Log("Checkpoint reached: " + checkpointPosition);
        }
    }

    public static void RestartCheckpoint()
    {
        checkpointPosition = new Vector3(-231f, 91.46f, 29.88f);
    }
}
