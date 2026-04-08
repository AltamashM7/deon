using System.Collections; // NEW: Required for Coroutines
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video; 
using Unity.Cinemachine;

public class MenuCameraManager : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineCamera vCam_01_Wide;
    public CinemachineCamera vCam_02_Ren;
    public CinemachineCamera vCam_03_Akane;

    [Header("UI Panels")]
    public GameObject panel_01_Begin;
    public GameObject panel_02_RenSelect;
    public GameObject panel_03_AkaneConfirm;

    [Header("Buttons")]
    public Button btn_Begin;
    public Button btn_StartGame;
    public Button btn_ExitGame;
    public Button btn_Yes;
    public Button btn_No;

    [Header("Settings")]
    public string hubSceneName = "HubOverhaul"; 

    [Header("Cutscene Settings")] 
    [Tooltip("Drag your Intro_Cutscene VideoPlayer object here")]
    public VideoPlayer introCutscenePlayer;

    private void Start()
    {
        // 1. Automatically wire up all the button clicks
        btn_Begin.onClick.AddListener(() => SetMenuState(2));
        btn_StartGame.onClick.AddListener(() => SetMenuState(3));
        btn_ExitGame.onClick.AddListener(ExitApplication);
        
        // MODIFIED: btn_Yes now triggers the Fade Coroutine
        btn_Yes.onClick.AddListener(PlayIntroCutscene); 
        
        btn_No.onClick.AddListener(() => SetMenuState(2)); // "No" sends them back to Ren

        // 2. Initialize the starting state (Wide Shot)
        SetMenuState(1);
    }

    // The Master State Controller
    private void SetMenuState(int stateIndex)
    {
        // Step A: Reset all cameras to background priority (10)
        vCam_01_Wide.Priority = 10;
        vCam_02_Ren.Priority = 10;
        vCam_03_Akane.Priority = 10;

        // Step B: Turn off all UI Panels
        panel_01_Begin.SetActive(false);
        panel_02_RenSelect.SetActive(false);
        panel_03_AkaneConfirm.SetActive(false);

        // Step C: Elevate the requested camera to active priority (20) and show its UI
        switch (stateIndex)
        {
            case 1: // Phase 1: Wide Shot
                vCam_01_Wide.Priority = 20;
                panel_01_Begin.SetActive(true);
                break;

            case 2: // Phase 2: Ren's Menu
                vCam_02_Ren.Priority = 20;
                panel_02_RenSelect.SetActive(true);
                break;

            case 3: // Phase 3: Akane's Confirmation
                vCam_03_Akane.Priority = 20;
                panel_03_AkaneConfirm.SetActive(true);
                break;
        }
    }

    // --- NEW: Fade and Cutscene Logic ---
    private void PlayIntroCutscene()
    {
        // Disable the button instantly so the player can't double-click it during the fade
        btn_Yes.interactable = false;
        
        // Start the fading sequence
        StartCoroutine(FadeAndPlaySequence());
    }

    private IEnumerator FadeAndPlaySequence()
    {
        // 1. Generate a black overlay purely via code
        GameObject fadeObj = new GameObject("FadeOverlay");
        Canvas canvas = fadeObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // Ensure it renders on top of EVERYTHING

        Image fadeImage = fadeObj.AddComponent<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 0f); // Start completely transparent

        // 2. Smoothly fade it in over 1.5 seconds
        float fadeDuration = 1.5f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0f, 0f, 0f, timer / fadeDuration);
            yield return null;
        }

        // Lock it to solid black
        fadeImage.color = Color.black;

        // Give the player half a second to sit in the darkness
        yield return new WaitForSeconds(0.5f);

        // 3. Start the cinematic!
        if (introCutscenePlayer != null)
        {
            // Kill the Main Menu music so it doesn't overlap the cinematic
            if (MusicManager.Instance != null) 
            {
                MusicManager.Instance.StopMusic();
            }

            // Hide Akane's confirmation UI so the video is clearly visible
            panel_03_AkaneConfirm.SetActive(false);

            // Destroy the black fade canvas so it doesn't block our view of the video!
            Destroy(fadeObj);

            // Turn on the video player and listen for when it finishes
            introCutscenePlayer.gameObject.SetActive(true);
            introCutscenePlayer.loopPointReached += OnIntroFinished;
            
            introCutscenePlayer.Play();
        }
        else
        {
            // Failsafe: If no video is assigned, just load the hub instantly
            StartHubLevel();
        }
    }

    private void OnIntroFinished(VideoPlayer vp)
    {
        // Stop listening to prevent memory leaks
        vp.loopPointReached -= OnIntroFinished;
        
        // The cinematic is over, teleport to the Hub!
        StartHubLevel();
    }

    private void StartHubLevel()
    {
        // Loads your first-person Hub scene
        SceneManager.LoadScene(hubSceneName); 
    }

    private void ExitApplication()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}