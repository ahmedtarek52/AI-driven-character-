using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Background Music")]
    public AudioSource musicSource;
    public AudioClip[] backgroundMusicClips;
    private int currentMusicIndex = 0;

    [Header("Sound Effects")]
    public AudioSource sfxSource;
    public AudioClip shootSound;
    public AudioClip hitSound;
    public AudioClip deathSound;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (backgroundMusicClips.Length > 0)
        {
            PlayBackgroundMusic();
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusicClips.Length > 0)
        {
            musicSource.clip = backgroundMusicClips[currentMusicIndex];
            musicSource.Play();
        }
    }


    public void PlayShootSound()
    {
        if (shootSound != null)
        {
            sfxSource.PlayOneShot(shootSound);
        }
    }
    public void PlayHitSound()
    {
        if (shootSound != null)
        {
            sfxSource.PlayOneShot(hitSound);
        }
    }

    public void PlayDeathSound()
    {
        if (deathSound != null)
        {
            sfxSource.PlayOneShot(deathSound);
        }
    }


} 