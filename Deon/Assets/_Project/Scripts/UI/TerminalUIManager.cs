using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.SceneManagement;
using UnityEngine.Video; // Added for cutscene support

public class TerminalUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject terminalContainer;
    [SerializeField] private TextMeshProUGUI titleText; 
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image previewImage;
    
    [Header("Buttons")]
    [SerializeField] private Button warpButton;
    [SerializeField] private Button exitButton;

    [Header("Cutscene Settings")]
    [Tooltip("Drag the Hub_Cutscene VideoPlayer object here")]
    [SerializeField] private VideoPlayer warpCutscenePlayer;

    [Header("Player Controls")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    private string pendingScene;

    private void Start()
    {
        warpButton.onClick.AddListener(WarpToWorld);
        exitButton.onClick.AddListener(CloseTerminal);
        terminalContainer.SetActive(false); 
    }

    public void OpenTerminal(WorldDefinition worldData)
    {
        // 1. Fill the UI
        titleText.text = "World: " + worldData.displayName.ToUpper();
        descriptionText.text = worldData.worldDescription;
        pendingScene = worldData.sceneToLoad;
        
        if (worldData.worldPreviewImage != null)
        {
            previewImage.sprite = worldData.worldPreviewImage;
            previewImage.enabled = true;
        }
        else
        {
            previewImage.enabled = false;
        }

        // 2. Status Check & Lockout
        if (ChoiceEngine.Instance != null)
        {
            int score = ChoiceEngine.Instance.GetWorldScore(worldData.worldId);
            bool isCompleted = (score != 0);
            
            statusText.text = isCompleted ? "STATUS: COMPLETED" : "STATUS: PENDING";
            
            // If the world is completed, make the button unclickable!
            warpButton.interactable = !isCompleted; 
        }

        // 3. Freeze game and unlock cursor
        SpatialPointer3D.CanUsePointer = false; 
        Time.timeScale = 0f; 

        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        terminalContainer.SetActive(true);
    }

    private void CloseTerminal()
    {
        terminalContainer.SetActive(false);
        Time.timeScale = 1f;

        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SpatialPointer3D.CanUsePointer = true;
    }

    private void WarpToWorld()
    {
        if (string.IsNullOrEmpty(pendingScene)) return;

        // Hide the terminal UI so the video can play cleanly
        terminalContainer.SetActive(false);

        // Play the cutscene if we have one attached
        if (warpCutscenePlayer != null)
        {
            warpCutscenePlayer.gameObject.SetActive(true);
            warpCutscenePlayer.enabled = true;
            warpCutscenePlayer.loopPointReached += OnWarpCutsceneFinished;
            
            // Note: We use unscaledTime here in case Time.timeScale is still 0
            warpCutscenePlayer.Play(); 
        }
        else
        {
            ExecuteWarp();
        }
    }

    private void OnWarpCutsceneFinished(VideoPlayer vp)
    {
        vp.loopPointReached -= OnWarpCutsceneFinished;
        vp.enabled = false;
        vp.gameObject.SetActive(false);
        ExecuteWarp();
    }

    private void ExecuteWarp()
    {
        // 1. Unpause time
        Time.timeScale = 1f; 

        // 2. RE-ENABLE THE POINTER (This is what broke your UI!)
        SpatialPointer3D.CanUsePointer = true; 

        // 3. Re-lock the mouse cursor so you can look around in the new world
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 4. Finally, load the level
        SceneManager.LoadScene(pendingScene);
    }
}