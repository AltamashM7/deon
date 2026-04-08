using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [Header("Background Music")]
    [Tooltip("The MP3/WAV file you want to play in this scene")]
    public AudioClip trackToPlay;

    private void Start()
    {
        // Tell the immortal manager to play this track!
        if (MusicManager.Instance != null && trackToPlay != null)
        {
            MusicManager.Instance.PlayTrack(trackToPlay);
        }
    }
}