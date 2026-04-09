using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Drag the Pause_Background object here")]
    [SerializeField] private GameObject pauseMenuContainer;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Player Controls")]
    [Tooltip("Drag the Player scripts (Movement, Camera Look) here to disable them when paused")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    [Header("Settings")]
    [Tooltip("The exact name of your Main Menu scene")]
    [SerializeField] private string mainMenuSceneName = "Main_Menu";

    [Header("Audio")]
    [Tooltip("Drag your Main Menu MP3/WAV here to reset the music when quitting")]
    [SerializeField] private AudioClip mainMenuMusic;

    private bool isPaused = false;

    private void Start()
    {
        // Wire up the buttons automatically
        continueButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
        
        pauseMenuContainer.SetActive(false); // Ensure it starts hidden
    }

    private void Update()
    {
        // Only allow pausing if the player isn't already locked in a terminal or VN dialogue
        // (SpatialPointer3D.CanUsePointer acts as our master safety lock)
        if (Input.GetKeyDown(KeyCode.Escape) && SpatialPointer3D.CanUsePointer)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        pauseMenuContainer.SetActive(true);
        Time.timeScale = 0f; // Freeze the game world
        
        // Disable movement scripts to prevent the camera gliding bug
        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = false;
        }

        // Unlock the cursor so the player can click the buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Lock other interactions while paused
        SpatialPointer3D.CanUsePointer = false;
    }

    private void ResumeGame()
    {
        isPaused = false;
        pauseMenuContainer.SetActive(false);
        Time.timeScale = 1f; // Unfreeze the game world
        
        // Re-enable movement scripts
        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = true;
        }

        // Relock the cursor for First Person movement
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Re-enable interactions
        SpatialPointer3D.CanUsePointer = true;
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f; // You MUST unpause time before changing scenes, or the Main Menu will be frozen!
        
        // Ensure cursor is unlocked for the main menu UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // --- NEW FIX: Unlock the master interaction switch before leaving! ---
        SpatialPointer3D.CanUsePointer = true; 
        
        // Reset the music back to the Main Menu theme!
        if (MusicManager.Instance != null && mainMenuMusic != null)
        {
            MusicManager.Instance.PlayTrack(mainMenuMusic);
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}