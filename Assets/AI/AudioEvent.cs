using UnityEngine;

public class AudioEvent : MonoBehaviour
{
    public int min;
    public int max;
    public AudioSource ad;


    public void Walk()
    {
        int current = Random.Range(min, max);
        ad.PlayOneShot(AudioManager.Instance.WalkSoundClips[current]); 
    }
    public void PlaySound(int Index)
    {
        ad.PlayOneShot(AudioManager.Instance.SoundClips[Index]); 
    }

    void Update()
    {
        ad.volume = AudioManager.Instance.SoundVolumeFloat;

        if(ad.volume <= 0.01f)
            ad.Pause();
        else
            ad.UnPause();
    }
}
