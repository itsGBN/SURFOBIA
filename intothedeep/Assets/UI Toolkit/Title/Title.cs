using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Title : MonoBehaviour
{
    private UIDocument uIDocument;
    private VisualElement Trans;
    private bool isTrans;

    private void Awake()
    {
        //Refernce UI 
        uIDocument = GetComponent<UIDocument>();
        //Reference Visual Elements
        Trans = uIDocument.rootVisualElement.Q("Trans") as VisualElement;
        //Miscellaneous
        isTrans = false;
    }

    private void Update()
    {
        if (Input.anyKeyDown && !isTrans)
        {
            isTrans = true;
            StartCoroutine(StartTrans());
        }
    }

    IEnumerator StartTrans()
    {
        Trans.style.opacity = 1;
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(1);
        GameManager.instance.UpdateState(GameManager.GameState.READY);
    }

}
