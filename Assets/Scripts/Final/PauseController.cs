using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject tutorPanel;

    [SerializeField]
    private Button pauseButton;

    private bool pauseActive;

    public void ClickPauseButton()
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

    private void OnPause()
    {
        pauseButton.interactable = false;
        pauseButton.image.enabled = false;
        animator.SetBool("Paused", true);
        //Time.timeScale = 0;
        pauseActive = true;
    }

    public void OffPause()
    {
        pauseButton.interactable = true;
        pauseButton.image.enabled = true;
        animator.SetBool("Options", false);
        animator.SetBool("Paused", false);
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

    public void ShowPanelTutorial()
    {
        tutorPanel.SetActive(true);
    }

    public void HidePanelTutorial()
    {
        tutorPanel.SetActive(false);
    }
}
