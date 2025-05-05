using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ArrowPoint : MonoBehaviour
{
    public GameObject target;

    public GameObject childObj;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameState == GameManager.GameState.READY)
        {
            childObj.SetActive(false);
        }
        else
        {
            childObj.SetActive(true);
        }
        if (target != null)
        {

            Vector3 direction = target.transform.position - transform.position;

            // Ignore the vertical component (y-axis) to constrain to the horizontal plane
            direction.y = 0;

            // Check if the direction is valid (non-zero)
            if (direction.sqrMagnitude > 0.001f)
            {
                // Rotate the object to face the target in world space
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                // Rotate the object to face the target in local space
               // transform.localRotation = Quaternion.LookRotation(direction, Vector3.up);
            }

        }
        
    }
}
