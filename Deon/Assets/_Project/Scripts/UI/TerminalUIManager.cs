using UnityEngine;
using UnityEngine.UI;
using TMPro; // <-- We need this namespace to talk to TextMeshPro!
using UnityEngine.SceneManagement;

public class TerminalUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject terminalContainer;
    
    // Changed these from 'Text' to 'TextMeshProUGUI'
    [SerializeField] private TextMeshProUGUI titleText; 
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statusText;
    
    [SerializeField] private Image previewImage;
    
    [Header("Buttons")]
    [SerializeField] private Button warpButton;
    [SerializeField] private Button exitButton;

    [Header("Player Controls")]
    [Tooltip("Drag the Player scripts (Movement, Camera Look) here to disable them when the terminal is open")]
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
        // 1. Fill the UI with the data
        titleText.text = "SIMULATION: " + worldData.displayName.ToUpper();
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

        // 2. QoL Status Check using Farhan's engine
        if (ChoiceEngine.Instance != null)
        {
            int score = ChoiceEngine.Instance.GetWorldScore(worldData.worldId);
            statusText.text = (score != -1) ? "STATUS: COMPLETED" : "STATUS: PENDING SIMULATION";
        }

        // 3. Freeze game and unlock cursor
        SpatialPointer3D.CanUsePointer = false; 
        Time.timeScale = 0f; 

        // Disable player movement and camera look scripts to prevent gliding
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

        // Re-enable player movement and camera look scripts
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
        if (!string.IsNullOrEmpty(pendingScene))
        {
            Time.timeScale = 1f; // Must unpause before loading!
            SceneManager.LoadScene(pendingScene);
        }
        else
        {
            Debug.LogWarning("Warp failed: No scene name assigned!");
        }
    }
}