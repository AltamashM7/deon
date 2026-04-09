using System.Collections;
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

    [Tooltip("Drag your Cutscene_Canvas or Video_Screen GameObject here")]
    public GameObject cutsceneScreen;

    // --- NEW: Skip Mechanics ---
    [Header("Skip Settings")]
    [Tooltip("Drag the UI container for the skip prompt here")]
    public GameObject skipPromptUI;
    [Tooltip("Optional: A UI Image set to 'Filled' to show holding progress")]
    public Image skipProgressBar;
    [Tooltip("How many seconds the player must hold space to skip")]
    public float timeToSkip = 2f;
    
    // --- NEW: How long the prompt stays visible before hiding ---
    [Tooltip("How many seconds the prompt stays on screen before fading out")]
    public float promptDisplayDuration = 1f; 

    private bool isIntroPlaying = false;
    private float currentHoldTime = 0f;
    private float promptTimer = 0f; // NEW: The internal countdown clock

    private void Start()
    {
        // Force the video screen to hide when the menu loads!
        if (cutsceneScreen != null)
        {
            cutsceneScreen.SetActive(false);
        }

        // Make sure the skip UI is hidden on start
        if (skipPromptUI != null) skipPromptUI.SetActive(false);
        if (skipProgressBar != null) skipProgressBar.fillAmount = 0f;

        // 1. Automatically wire up all the button clicks
        btn_Begin.onClick.AddListener(() => SetMenuState(2));
        btn_StartGame.onClick.AddListener(() => SetMenuState(3));
        btn_ExitGame.onClick.AddListener(ExitApplication);
        
        btn_Yes.onClick.AddListener(PlayIntroCutscene); 
        
        btn_No.onClick.AddListener(() => SetMenuState(2)); 

        // 2. Initialize the starting state (Wide Shot)
        SetMenuState(1);
    }

    // --- NEW: The Skip Listener & Visibility Logic ---
    private void Update()
    {
        // We only care about the spacebar if the video is actually playing
        if (isIntroPlaying)
        {
            // --- NEW: The Visibility Countdown Logic ---
            if (promptTimer > 0)
            {
                promptTimer -= Time.deltaTime;
                
                // If the timer hits zero, AND the player isn't actively holding space, hide it
                if (promptTimer <= 0 && currentHoldTime <= 0f)
                {
                    if (skipPromptUI != null) skipPromptUI.SetActive(false);
                }
            }

            if (Input.GetKey(KeyCode.Q))
            {
                // Force the UI to turn back on so they can see the progress bar
                if (skipPromptUI != null) skipPromptUI.SetActive(true);
                
                // Keep the timer fresh so the UI doesn't instantly vanish when they let go
                promptTimer = promptDisplayDuration; 

                // Increase the timer
                currentHoldTime += Time.deltaTime;

                // Update the visual progress bar if you assigned one
                if (skipProgressBar != null)
                {
                    skipProgressBar.fillAmount = currentHoldTime / timeToSkip;
                }

                // Did they hold it long enough?
                if (currentHoldTime >= timeToSkip)
                {
                    ExecuteSkip();
                }
            }
            else
            {
                // If they let go, reset the timer and the progress bar instantly
                currentHoldTime = 0f;
                if (skipProgressBar != null)
                {
                    skipProgressBar.fillAmount = 0f;
                }
            }
        }
    }

    // The Master State Controller
    private void SetMenuState(int stateIndex)
    {
        vCam_01_Wide.Priority = 10;
        vCam_02_Ren.Priority = 10;
        vCam_03_Akane.Priority = 10;

        panel_01_Begin.SetActive(false);
        panel_02_RenSelect.SetActive(false);
        panel_03_AkaneConfirm.SetActive(false);

        switch (stateIndex)
        {
            case 1: 
                vCam_01_Wide.Priority = 20;
                panel_01_Begin.SetActive(true);
                break;

            case 2: 
                vCam_02_Ren.Priority = 20;
                panel_02_RenSelect.SetActive(true);
                break;

            case 3: 
                vCam_03_Akane.Priority = 20;
                panel_03_AkaneConfirm.SetActive(true);
                break;
        }
    }

    private void PlayIntroCutscene()
    {
        btn_Yes.interactable = false;
        StartCoroutine(FadeAndPlaySequence());
    }

    private IEnumerator FadeAndPlaySequence()
    {
        GameObject fadeObj = new GameObject("FadeOverlay");
        Canvas canvas = fadeObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; 

        Image fadeImage = fadeObj.AddComponent<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 0f); 

        float fadeDuration = 1.5f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0f, 0f, 0f, timer / fadeDuration);
            yield return null;
        }

        fadeImage.color = Color.black;
        yield return new WaitForSeconds(0.5f);

        if (introCutscenePlayer != null)
        {
            if (MusicManager.Instance != null) 
            {
                MusicManager.Instance.StopMusic();
            }

            panel_03_AkaneConfirm.SetActive(false);

            introCutscenePlayer.gameObject.SetActive(true);
            introCutscenePlayer.Prepare();
            
            while (!introCutscenePlayer.isPrepared)
            {
                yield return null; 
            }

            // --- FIX 1: Turn on the screen ONLY when the video is 100% ready! ---
            if (cutsceneScreen != null)
            {
                cutsceneScreen.SetActive(true);
            }

            // Activate the skip logic and show the prompt!
            isIntroPlaying = true;
            if (skipPromptUI != null) skipPromptUI.SetActive(true);
            
            // Start the countdown timer the moment the video starts
            promptTimer = promptDisplayDuration; 

            introCutscenePlayer.Play();
            
            yield return new WaitForEndOfFrame();

            Destroy(fadeObj);

            introCutscenePlayer.loopPointReached += OnIntroFinished;
        }
        else
        {
            StartHubLevel();
        }
    }

    // The Skip Execution
    private void ExecuteSkip()
    {
        // 1. Lock the logic so it can't trigger twice
        isIntroPlaying = false;

        // 2. Stop the video and clean up the listener
        if (introCutscenePlayer != null)
        {
            introCutscenePlayer.Stop();
            introCutscenePlayer.loopPointReached -= OnIntroFinished;

            // --- FIX 2: Flush the render texture from memory! ---
            if (introCutscenePlayer.targetTexture != null)
            {
                introCutscenePlayer.targetTexture.Release();
            }
        }

        // 3. Move directly to the blackout phase
        FinishCutsceneTransition();
    }

    private void OnIntroFinished(VideoPlayer vp)
    {
        // Video finished naturally! Lock the skip logic.
        isIntroPlaying = false;
        vp.loopPointReached -= OnIntroFinished;

        // --- FIX 2: Flush the render texture from memory! ---
        if (vp.targetTexture != null)
        {
            vp.targetTexture.Release();
        }
        
        FinishCutsceneTransition();
    }

    // Shared Transition Logic
    private void FinishCutsceneTransition()
    {
        // Turn off the skip UI so it doesn't linger during the blackout
        if (skipPromptUI != null) skipPromptUI.SetActive(false);

        // Slam a solid black screen over the camera to prevent the ending flicker!
        GameObject blackout = new GameObject("Blackout");
        Canvas canvas = blackout.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        
        Image bg = blackout.AddComponent<Image>();
        bg.color = Color.black;

        // The cinematic is over, teleport to the Hub!
        StartHubLevel();
    }

    private void StartHubLevel()
    {
        SceneManager.LoadScene(hubSceneName); 
    }

    private void ExitApplication()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}