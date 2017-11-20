using UnityEngine;
using System;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    public int NumberAudio;
    public AudioClip Clip;
    public AudioMixerGroup AudioMixer;
    [Range(0.0f, 1.0f)]
    public float Volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float Pitch = 1.0f;


    [Range(0.1f, 0.5f)]
    public float RandomValue = 0.1f;
    [Range(0.1f, 0.5f)]
    public float RandomPitch = 0.1f;

    public bool Loop;

    private AudioSource source;

    public void SetSource (AudioSource _source)
    {
        source = _source;
        source.clip = Clip;
        source.loop = Loop;
        source.outputAudioMixerGroup = AudioMixer;
    }


}

public class AudioManager : MonoBehaviour
{
        
}
