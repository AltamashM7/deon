using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Cinemachine; // The new Cinemachine 3.0 namespace

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
    public string hubSceneName = "HubOverhaul"; // Change this to your actual scene name

    private void Start()
    {
        // 1. Automatically wire up all the button clicks
        btn_Begin.onClick.AddListener(() => SetMenuState(2));
        btn_StartGame.onClick.AddListener(() => SetMenuState(3));
        btn_ExitGame.onClick.AddListener(ExitApplication);
        btn_Yes.onClick.AddListener(StartHubLevel);
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