using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    private AudioSource source;

    public string Name;
    public AudioClip Clip;

    [Range(0f, 1f)]
    public float Volume = 0.7f;

    [Range(0.2f, 2f)]
    public float DefaultPitch = 1f;


    private float pitch = 1f;
    public float Pitch {
        get { return pitch; }
        set
        {
            pitch = Math.Min(2f, Math.Max(value, 0f));
        }
    }


    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = Clip;
        source.pitch = DefaultPitch;
    }

    public void Play()
    {
        source.volume = Volume;
        source.pitch = Pitch;
        source.Play();
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    public Sound[] Sounds;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one AudioManager in the scene!");
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        for (int i = 0; i < Sounds.Length; i++)
        {
            GameObject go = new GameObject("Sound_" + i + "_" + Sounds[i].Name);
            go.transform.SetParent(Instance.transform);
            Sounds[i].SetSource(go.AddComponent<AudioSource>());
        }
    }

    public void PlaySound(string name)
    {
        var sound = Sounds.FirstOrDefault(x => x.Name == name);
        sound.Play();
    }
    
    public void PlaySound(string name, float pitch)
    {
        var sound = Sounds.FirstOrDefault(x => x.Name == name);
        sound.Pitch = pitch;
        sound.Play();
    }
}
