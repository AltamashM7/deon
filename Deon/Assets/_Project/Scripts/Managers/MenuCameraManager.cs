using UnityEngine;
using Unity.Cinemachine;

public class MenuCameraManager : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineCamera wideCam;
    public CinemachineCamera renCam;
    public CinemachineCamera akaneCam;

    // These methods will be triggered by your UI Buttons
    public void ShowWide() => ResetPriorities(wideCam);
    public void ShowRen() => ResetPriorities(renCam);
    public void ShowAkane() => ResetPriorities(akaneCam);

    // This handles the math of switching the active camera
    private void ResetPriorities(CinemachineCamera activeCam)
    {
        // 1. Lower all cameras to the background priority
        wideCam.Priority = 10;
        renCam.Priority = 10;
        akaneCam.Priority = 10;

        // 2. Elevate the requested camera to the active priority
        activeCam.Priority = 20;
    }
}