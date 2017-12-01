using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsSettings : SingletoneAsComponent<OptionsSettings> {

    public static OptionsSettings Instance
    {
        get { return ((OptionsSettings)_Instance); }
        set { _Instance = value; }
    }

    public float CurSliderValueMusic;
    public float CurSliderValueSFX;
    public bool CurToggleValue;

    [SerializeField]
    private AudioMixer mixer;

    public void OnSFXValue(float value)
    {
        mixer.SetFloat("SFX", value);
    }

    public void OnMusicValue(float value)
    {
        mixer.SetFloat("Music", value);
    }

    public void OnSoundToggle()
    {
        AudioManager.Instance.SoundMute = !AudioManager.Instance.SoundMute;
    }

    public void GetValueUI (float newMusicSliderValue, float newSFXSliderValue, bool newToggleValue)
    {
        CurSliderValueMusic = newMusicSliderValue;
        CurSliderValueSFX = newSFXSliderValue;
        CurToggleValue = newToggleValue;
    }

    public void SetValueUI(UnityEngine.UI.Slider newMusicSliderValue, UnityEngine.UI.Slider newSFXSliderValue, UnityEngine.UI.Toggle newToggleValue)
    {
        newMusicSliderValue.value = CurSliderValueMusic;
        newSFXSliderValue.value = CurSliderValueSFX;
        newToggleValue.isOn = CurToggleValue;
        AudioManager.Instance.SoundMute = newToggleValue.isOn;
    }
}
