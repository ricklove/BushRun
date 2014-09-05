using UnityEngine;
using System.Collections.Generic;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;

    void Awake()
    {
        Instance = this;
    }

    private string[] backgroundMusic = {
    //"ANIMAL Bird Singing Spring (mono)"
    };
    private string[] ambientSounds = {
        "ANIMAL Bird Singing Spring (mono)"
    };
    private string[] hurtSounds = {
        "BREAK Bright Splinters (stereo)",
        "GRUNT Male Hurt With Cough (mono)",
        "GRUNT Male Quick (mono)",
        "GRUNT Male Quick Deep (mono)",
        "GRUNT Male Subtle Hurt (mono)",
        "GRUNT Male Subtle Quick (mono)",
    };
    private string[] cheerSounds = {
//        "PUZZLE Success Banjo Four Note Fast Wet (stereo)",
//        "PUZZLE Success Banjo Two Note Fast (stereo)",
//        "PUZZLE Success Brass Fanfare Bright Wet (stereo)",
//        "PUZZLE Success Bright Voice Two Note Fast Delay (stereo)",
//        "PUZZLE Success Guitar 1 Fast Three Note Climb Dry (stereo)",
//        "PUZZLE Success Guitar 1 Fast Two Note Climb Wet (stereo)",
//        "PUZZLE Success Guitar 2 Fast Two Note Dry (stereo)",
//        "PUZZLE Success Piano Three Note Fast Bright Soft Dry (stereo)",
//        "PUZZLE Success Violin Pizzicato Three Note Bright Wet (stereo)",
//        "PUZZLE Success Violin Pizzicato Three Note Dark Wet (stereo)",
//        "PUZZLE Success Violin Pizzicato Three Note Wet (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 01 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 02 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 03 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 04 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 05 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 06 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 07 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 08 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 09 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 10 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 11 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 12 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 13 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 14 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 15 (stereo)",
        "MUSIC EFFECT Solo Xylophone Positive 16 (stereo)",
    };
    private Dictionary<string,AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    private float _timeForNextAmbient = 5;

    // Use this for initialization
    void Start()
    {
    
    }
    
    // Update is called once per frame
    void Update()
    {
        // Ambient Sounds
        if (Time.time > _timeForNextAmbient)
        {
            PlayAnySound(ambientSounds);
            _timeForNextAmbient = Time.time + Random.Range(20, 60);
        }
    }

    public void PlayHurt()
    {
        PlayAnySound(hurtSounds);
    }

    public void PlayCheer()
    {
        PlayAnySound(cheerSounds);
    }

    void PlayAnySound(string[] sounds)
    {
        if (sounds == null || sounds.Length <= 0)
        {
            return;
        }

        PlaySound(sounds [Random.Range(0, sounds.Length)]);
    }

    void PlaySound(string sound)
    {
        // Get clip
        if (!_audioClips.ContainsKey(sound))
        {
            _audioClips [sound] = Resources.Load<AudioClip>("Sounds/" + sound);
        }

        var clip = _audioClips [sound];
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }
}
//public enum SoundType{
//    BackgroundMusic,
//    Ambient,
//    Hurt,
//    Cheer
//}
