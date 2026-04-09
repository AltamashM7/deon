using System.Collections; // NEW: Required for Coroutines
using UnityEngine;
using UnityEngine.Video; // Required for video commands
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VideoPlayer))]
public class EndingCutsceneManager : MonoBehaviour
{
    [Header("Next Scene Settings")]
    [Tooltip("The exact name of the scene to load when the video finishes")]
    public string nextSceneName = "Credits";

    // --- NEW: A reference to the UI canvas displaying the video ---
    [Header("Cutscene UI")]
    [Tooltip("Drag your Cutscene_Canvas or Video_Screen GameObject here")]
    public GameObject cutsceneScreen;

    private VideoPlayer _videoPlayer;

    private void Start()
    {
        // 1. Force the video screen to hide while the scene loads
        if (cutsceneScreen != null)
        {
            cutsceneScreen.SetActive(false);
        }

        // 2. Kill the BGM so it doesn't overlap the finale!
        if (MusicManager.Instance != null) 
        {
            MusicManager.Instance.StopMusic();
        }

        _videoPlayer = GetComponent<VideoPlayer>();
        
        // 3. Start the flicker-free video prep
        StartCoroutine(PrepareAndPlayEnding());
    }

    private IEnumerator PrepareAndPlayEnding()
    {
        // Tell the engine to start buffering the video into memory
        _videoPlayer.Prepare();
        
        while (!_videoPlayer.isPrepared)
        {
            yield return null; // Wait here until the video is 100% ready
        }

        // Turn the Video Screen Canvas ON so we can see the texture
        if (cutsceneScreen != null)
        {
            cutsceneScreen.SetActive(true);
        }

        // Play the video!
        _videoPlayer.Play();
        
        // Wait exactly 1 frame to ensure it has physically rendered to the screen
        yield return new WaitForEndOfFrame();

        // Tell the script to listen for the exact moment the video finishes
        _videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // Stop listening to prevent memory leaks
        vp.loopPointReached -= OnVideoFinished;

        // --- FIX: Flush the render texture from memory so it goes blank! ---
        if (vp.targetTexture != null)
        {
            vp.targetTexture.Release();
        }
        
        // Hide the canvas cleanly before swapping scenes
        if (cutsceneScreen != null)
        {
            cutsceneScreen.SetActive(false);
        }

        // Load the Credits scene!
        SceneManager.LoadScene(nextSceneName);
    }
}