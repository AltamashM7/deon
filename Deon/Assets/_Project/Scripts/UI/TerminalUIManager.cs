using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.SceneManagement;
using UnityEngine.Video; 

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

    [Tooltip("Drag your Cutscene_Canvas or Video_Screen GameObject here")]
    [SerializeField] private GameObject cutsceneScreen;

    [Header("Player Controls")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    private string pendingScene;

    private void Start()
    {
        // Force the video screen to hide when the Hub loads!
        if (cutsceneScreen != null)
        {
            cutsceneScreen.SetActive(false);
        }

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
            if (MusicManager.Instance != null) MusicManager.Instance.StopMusic();

            StartCoroutine(PrepareAndPlayWarpCutscene());
        }
        else
        {
            ExecuteWarp();
        }
    }

    private IEnumerator PrepareAndPlayWarpCutscene()
    {
        warpCutscenePlayer.gameObject.SetActive(true);
        warpCutscenePlayer.enabled = true;

        // Pre-load the video to prevent the starting flicker
        warpCutscenePlayer.Prepare();
        
        while (!warpCutscenePlayer.isPrepared)
        {
            yield return null; 
        }

        // --- FIX 1: Turn on the screen ONLY when the video is 100% ready! ---
        if (cutsceneScreen != null)
        {
            cutsceneScreen.SetActive(true);
        }
        
        warpCutscenePlayer.Play(); 
        warpCutscenePlayer.loopPointReached += OnWarpCutsceneFinished;
    }

    private void OnWarpCutsceneFinished(VideoPlayer vp)
    {
        vp.loopPointReached -= OnWarpCutsceneFinished;
        vp.enabled = false;
        vp.gameObject.SetActive(false);

        // --- FIX 2: Flush the render texture from memory so it goes blank! ---
        if (vp.targetTexture != null)
        {
            vp.targetTexture.Release();
        }

        // Hide the canvas again just to be perfectly clean before the scene swap
        if (cutsceneScreen != null)
        {
            cutsceneScreen.SetActive(false);
        }

        ExecuteWarp();
    }

    private void ExecuteWarp()
    {
        // 1. Unpause time
        Time.timeScale = 1f; 

        // 2. RE-ENABLE THE POINTER
        SpatialPointer3D.CanUsePointer = true; 

        // 3. Re-lock the mouse cursor so you can look around in the new world
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 4. Finally, load the level
        SceneManager.LoadScene(pendingScene);
    }
}