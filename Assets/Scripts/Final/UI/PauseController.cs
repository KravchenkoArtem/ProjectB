using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseController : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject tutorPanel;

    [SerializeField]
    private Button pauseButton;
    private bool pauseActive;

    [SerializeField]
    private Slider sliderMusic;
    [SerializeField]
    private Slider sliderSFX;
    [SerializeField]
    private Toggle toggleSound;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.Instance;
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found!");
        }
    }

    private void Start()
    {
        OptionsSettings.Instance.SetValueUI(sliderMusic, sliderSFX, toggleSound);
        audioManager.StopSound(1);
        audioManager.PlaySound(2);
    }

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

    public void OnSFXValue(float value)
    {
        OptionsSettings.Instance.OnSFXValue(value);
    }

    public void OnMusicValue(float value)
    {
        OptionsSettings.Instance.OnMusicValue(value);
    }

    public void OnSoundToggle()
    {
        OptionsSettings.Instance.OnSoundToggle();
    }

    public void TransitionToOptions()
    {
        animator.SetBool("Options", true);
    }

    public void TransitionToPause()
    {
        OptionsSettings.Instance.GetValueUI(sliderMusic.value, sliderSFX.value, toggleSound.isOn);
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
