using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VideoPlayer))]
public class EndingCutsceneManager : MonoBehaviour
{
    [Header("Next Scene Settings")]
    [Tooltip("The exact name of the scene to load when the video finishes")]
    public string nextSceneName = "Credits";

    private VideoPlayer _videoPlayer;

    private void Start()
    {
        // --- NEW: KILL THE BGM FOR THE FINALE! ---
        // This ensures the background music doesn't play over your ending MP4
        if (MusicManager.Instance != null) 
        {
            MusicManager.Instance.StopMusic();
        }

        _videoPlayer = GetComponent<VideoPlayer>();
        
        // Tell the script to listen for the exact moment the video finishes
        _videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // Stop listening to prevent memory leaks
        vp.loopPointReached -= OnVideoFinished;
        
        // Load the Credits scene!
        SceneManager.LoadScene(nextSceneName);
    }
}