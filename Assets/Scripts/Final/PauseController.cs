using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour {

    private SceneController sceneController;

    [SerializeField]
    private Animator animator;

    private void Start()
    {
        sceneController = SceneController.Instance;
        if (sceneController == null)
        {
            Debug.LogError("SceneController not found!");
        }
    }

    private bool pauseActive;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseActive = !pauseActive;
            if (pauseActive)
            {
                OnPause();
            }
            else
            {
                OffPause();
            }
        }
    }

    private void OnPause()
    {
        animator.SetBool("Paused", true);
        //Grid.instance.IsPause = true; // кривая пауза.
        //Time.timeScale = 0;
        pauseActive = true;
    }

    public void OffPause()
    {
        animator.SetBool("Options", false);
        animator.SetBool("Paused", false);
        //Grid.instance.IsPause = false;
        //Time.timeScale = 1;
        pauseActive = false;
    }

    public void TransitionToOptions()
    {
        animator.SetBool("Options", true);
    }

    public void TransitionToPause()
    {
        animator.SetBool("Options", false);
    }
}
