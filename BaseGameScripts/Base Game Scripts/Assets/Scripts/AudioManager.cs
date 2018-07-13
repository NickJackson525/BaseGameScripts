using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#region Enums

//used to determine which audiosource will be used to play a sound
public enum AudioSourceType { Background, Effects, UI }

//list of sound names as enums
public enum Sound
{
    //background
    BackgroundMusic1, BackgroundMusic2, BackgroundMusic3, BackgroundMusic4, BackgroundMusic5,
    InGameBackgroundMusic1, InGameBackgroundMusic2, InGameBackgroundMusic3, InGameBackgroundMusic4,
}

#endregion

public class AudioManager
{
    #region Variables

    //Dictionary that loads all the sounds for the game
    public Dictionary<Sound, AudioClip> SoundClips = new Dictionary<Sound, AudioClip>
    {
        {Sound.BackgroundMusic1, Resources.Load<AudioClip>("")},
        {Sound.BackgroundMusic2, Resources.Load<AudioClip>("")},
        {Sound.BackgroundMusic3, Resources.Load<AudioClip>("")},
        {Sound.BackgroundMusic4, Resources.Load<AudioClip>("")},
        {Sound.BackgroundMusic5, Resources.Load<AudioClip>("")},
        {Sound.InGameBackgroundMusic1, Resources.Load<AudioClip>("")},
        {Sound.InGameBackgroundMusic2, Resources.Load<AudioClip>("")},
        {Sound.InGameBackgroundMusic3, Resources.Load<AudioClip>("")},
        {Sound.InGameBackgroundMusic4, Resources.Load<AudioClip>("")},
    };

    //audio sources for ui, effects, and background music
    public AudioSource UIAudioSource;
    public AudioSource effectsAudioSource;
    public AudioSource backroundAudioSource;
    public bool isMusicMuted = false;
    public bool areEffectsMuted = false;

    private int previousBackgroundTrack = 0; //used so that a background tract isn't played 2 times in a row
    private float backgroundVolume = .5f;    //volume level of background music
    private float UIVolume = .5f;            //volume level of ui sounds
    private float effectVolume = .5f;        //volume level of sound effects

    #endregion

    #region Singleton Constructor

    // create variable for storing singleton that any script can access
    private static AudioManager instance;

    // create GameManager
    private AudioManager()
    {
        //create internal updater object
        UnityEngine.Object.DontDestroyOnLoad(new GameObject("Updater", typeof(Updater)));
    }

    // Property for Singleton
    public static AudioManager Instance
    {
        get { return instance ?? (instance = new AudioManager()); }
    }

    #endregion

    #region Internal Updater Class

    //class to allow the gamemanager singleton to have an update method
    class Updater : MonoBehaviour
    {
        private void Start()
        {
            Instance.Start();
        }

        private void Update()
        {
            Instance.Update();
        }
    }

    #endregion

    #region Start

    private void Start()
    {
        
    }

    #endregion

    #region Update

    private void Update()
    {

    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Plays any sound in the stored dictionary, on the specified audiosource
    /// </summary>
    public void PlaySound(AudioSourceType source, Sound soundToPlay)
    {
        //switch to determine which audiosource to use
        switch (source)
        {
            case AudioSourceType.Background:
                //check that the audiosource exists
                if ((backroundAudioSource != null) && !isMusicMuted)
                {
                    //stop whatever is already playing and play a new sound
                    backroundAudioSource.Stop();
                    backroundAudioSource.PlayOneShot(SoundClips[soundToPlay], backgroundVolume);
                    backroundAudioSource.volume = backgroundVolume;
                }
                break;
            case AudioSourceType.Effects:
                //check that the audiosource exists
                if ((effectsAudioSource != null) && !areEffectsMuted)
                {
                    //stop whatever is already playing and play a new sound
                    effectsAudioSource.PlayOneShot(SoundClips[soundToPlay], effectVolume);
                    effectsAudioSource.volume = effectVolume;
                }
                break;
            case AudioSourceType.UI:
                //check that the audiosource exists and isn't playing a sound already
                if ((UIAudioSource != null) && !areEffectsMuted)
                {
                    //stop whatever is already playing and play a new sound
                    UIAudioSource.PlayOneShot(SoundClips[soundToPlay], UIVolume);
                    UIAudioSource.volume = UIVolume;
                }
                break;
            default:
                //check that the audiosource exists
                if ((effectsAudioSource != null) && !areEffectsMuted)
                {
                    //stop whatever is already playing and play a new sound
                    effectsAudioSource.Stop();
                    effectsAudioSource.PlayOneShot(SoundClips[soundToPlay], effectVolume);
                    effectsAudioSource.volume = effectVolume;
                }
                break;
        }
    }

    /// <summary>
    /// Method to randomly pick a background track to play. It also stops a track from being placed twice in a row.
    /// </summary>
    public void PlayBackgroundMusic()
    {
        int rand = Random.Range(0, 5);

        //loop until a new track number is picked
        while (rand == previousBackgroundTrack)
        {
            rand = Random.Range(0, 5);
        }

        //reset track number
        previousBackgroundTrack = rand;

        PlaySound(AudioSourceType.Background, (Sound)rand);
    }

    /// <summary>
    /// Method to randomly pick an in game background track to play. It also stops a track from being placed twice in a row.
    /// </summary>
    public void PlayInGameBackgroundMusic()
    {
        int rand = Random.Range(5, 9);

        //loop until a new track number is picked
        while (rand == previousBackgroundTrack)
        {
            rand = Random.Range(5, 9);
        }

        //reset track number
        previousBackgroundTrack = rand;

        PlaySound(AudioSourceType.Background, (Sound)rand);
    }

    public void MuteSounds(AudioSourceType source)
    {
        switch (source)
        {
            case AudioSourceType.Background:
                backroundAudioSource.Stop();
                break;
            case AudioSourceType.Effects:
                effectsAudioSource.Stop();
                break;
            case AudioSourceType.UI:
                UIAudioSource.Stop();
                break;
            default:
                effectsAudioSource.Stop();
                break;
        }
    }

    #endregion
}
