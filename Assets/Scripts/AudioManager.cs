using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource Song;
    public AudioClip[] SongClips;

    public AudioSource Sound;
    public AudioClip[] SoundClips;
    public AudioClip[] WalkSoundClips;

    public AudioSource BiomeSound;
    public AudioClip[] BiomeSoundClips;

    [HideInInspector] public float SoundVolumeFloat;
    private float SongVolumeFloat;
    private float OldSoundVolumeFloat;
    private float OldSongVolumeFloat;
    private float MoveTowardsFloatSound;
    private float MoveTowardsFloatSong;
    private float MaxVelositySound;
    private float MaxVelositySong;

    public Slider SoundVolumeSlider;
    public Slider SongVolumeSlider;

    private int Paused;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;

        SoundVolumeFloat = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
        SongVolumeFloat = PlayerPrefs.GetFloat("SongVolume", 0.35f);
        OldSongVolumeFloat = SongVolumeFloat;
        OldSoundVolumeFloat = SoundVolumeFloat;
        MaxVelositySong = SongVolumeFloat;
        MaxVelositySound = SoundVolumeFloat;

        SoundVolumeSlider.value = SoundVolumeFloat;
        SongVolumeSlider.value = SongVolumeFloat;
        MoveTowardsFloatSong = SongVolumeFloat;
        MoveTowardsFloatSound = SoundVolumeFloat;


        Song.clip = SongClips[Random.Range(0, 2)];
        Song.Play();
    }

    // Update is called once per frame
    void Update()
    {   
        Save();

        MoveTowardsFloatSong = Mathf.MoveTowards(MoveTowardsFloatSong, SongVolumeFloat, Time.unscaledDeltaTime / 1.2f);
        MoveTowardsFloatSound = Mathf.MoveTowards(MoveTowardsFloatSound, SoundVolumeFloat, Time.unscaledDeltaTime / 1.5f);

        Sound.volume = MoveTowardsFloatSound;
        BiomeSound.volume = MoveTowardsFloatSound;
        Song.volume = MoveTowardsFloatSong;

        if(Paused == 1 && MoveTowardsFloatSound == 0 || Paused == 1 && MoveTowardsFloatSong == 0)
        {
            Paused = 2;
            Song.Pause();
            Sound.Pause();
            BiomeSound.Pause();
        }

        
        if (!Song.isPlaying && Paused == 0)
        {
            if(Song.clip == SongClips[0])
            {
                Song.clip = SongClips[1];
                Song.Play();
            }
            else if(Song.clip == SongClips[1])
            {
                Song.clip = SongClips[0];
                Song.Play();
            }
        }
    }

    public void PlaySound(int Index)
    {
        Sound.PlayOneShot(SoundClips[Index]);
    }
    public void PlayWalkSound()
    {
        int Index = Random.Range(0, 9);
        Sound.PlayOneShot(WalkSoundClips[Index]);
    }
    public void PlayBiomeSound(int Index, bool Stop)
    {
        if(!Stop)
        {
            BiomeSound.clip = BiomeSoundClips[Index];
            BiomeSound.Play();
        }
        else
            BiomeSound.Stop();
    }

    public void Save()
    {
        if(OldSoundVolumeFloat != SoundVolumeFloat)
        {
            OldSoundVolumeFloat = SoundVolumeFloat;
            PlayerPrefs.SetFloat("SoundVolume", MaxVelositySound);
        }
        
        if(OldSongVolumeFloat != SongVolumeFloat)
        {
            OldSongVolumeFloat = SongVolumeFloat;
            PlayerPrefs.SetFloat("SongVolume", MaxVelositySong);
        }
    }

    public void PauseContinue(bool Continue)
    {
        if(Continue)
        {
            Paused = 0;

            Song.UnPause();
            Sound.UnPause();
            BiomeSound.UnPause();

            SoundVolumeFloat = SoundVolumeSlider.value;
            SongVolumeFloat = SongVolumeSlider.value;
            MaxVelositySound = SoundVolumeSlider.value;
            MaxVelositySong = SongVolumeSlider.value;
        }
        else
        {
            Paused = 1;
            SongVolumeFloat = 0;
            SoundVolumeFloat = 0;
        }
    }
}
