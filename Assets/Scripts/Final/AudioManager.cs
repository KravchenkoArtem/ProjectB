using UnityEngine;
using System;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    public AudioClip Clip;
    public AudioMixerGroup AudioMixer;

    [Range(0.0f, 1.0f)]
    public float Volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float Pitch = 1.0f;
    public bool Loop;

    public int NumberAudio;
    private AudioSource source;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = Clip;
        source.loop = Loop;
        source.outputAudioMixerGroup = AudioMixer;
    }

    public void Play()
    {
        source.volume = Volume;
        source.pitch = Pitch;
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }
}

public class AudioManager : SingletoneAsComponent<AudioManager>
{
    public static AudioManager Instance
    {
        get { return ((AudioManager)_Instance); }
        set { _Instance = value; }
    }

    private bool soundMute;
    public bool SoundMute
    {
        get { return soundMute; }
        set
        {
            soundMute = value;
            MuteSwitch(soundMute);
        }
    }

    [SerializeField]
    private AudioMixerSnapshot play;
    [SerializeField]
    private AudioMixerSnapshot pause;
    [SerializeField]
    Sound[] sounds;

    private void Awake()
    {
        if (sounds != null)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                GameObject go = new GameObject(string.Format("Sound_{0}_{1}", i, sounds[i].NumberAudio));
                go.transform.SetParent(transform);
                sounds[i].SetSource(go.AddComponent<AudioSource>());
            }
        }
    }

    public void PlaySound(int num)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].NumberAudio == num)
            {
                sounds[i].Play();
                return;
            }
        }
    }

    public void StopSound (int num)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].NumberAudio == num)
            {
                sounds[i].Stop();
                return;
            }
        }
        Debug.LogWarning("AudioManager: Sound not found in list, " + num);
    }

    private void MuteSwitch(bool interactable)
    {
        if (interactable)
        {
            play.TransitionTo(0.1f);
        }
        else
        {
            pause.TransitionTo(0.1f);
        }
    }
}
