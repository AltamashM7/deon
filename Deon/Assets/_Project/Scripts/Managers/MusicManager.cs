using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    // Singleton so other scripts can instantly talk to it
    public static MusicManager Instance { get; private set; }
    
    private AudioSource _audioSource;

    private void Awake()
    {
        // If a MusicManager already exists, destroy this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Make this immortal!
        
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true; // Ensure BGM loops forever
        _audioSource.playOnAwake = false;
    }

    public void PlayTrack(AudioClip newClip)
    {
        if (newClip == null) return;
        
        // If this exact track is already playing, don't restart it!
        // (This prevents the Hub music from restarting if you just re-load the Hub)
        if (_audioSource.clip == newClip && _audioSource.isPlaying) return;

        _audioSource.clip = newClip;
        _audioSource.Play();
    }

    public void StopMusic()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }

    // --- NEW: Pause and Resume functionality ---
    public void PauseMusic()
    {
        if (_audioSource != null && _audioSource.isPlaying) 
        {
            _audioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (_audioSource != null) 
        {
            _audioSource.UnPause();
        }
    }
}