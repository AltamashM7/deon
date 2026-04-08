using UnityEngine;
using UnityEngine.Video; // Required for video commands
using Yarn.Unity;

public class InteractableDoor : MonoBehaviour
{
    [Header("Teleport Settings")]
    [Tooltip("Drag the Surgery_SpawnPoint from the surgery room here")]
    public Transform surgeryRoomSpawnPoint;

    [Header("Cutscene Settings")]
    [Tooltip("Drag the Door_Cutscene VideoPlayer object here")]
    public VideoPlayer cutscenePlayer;

    private GameObject _playerObject;

    public void TeleportPlayer(GameObject playerObject)
    {
        _playerObject = playerObject;

        // 1. Lock the player so they cannot move or interact during the cutscene
        CharacterController cc = _playerObject.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Turn off the PlayerMovement script to prevent the "inactive controller" error
        PlayerMovement pm = _playerObject.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        SpatialPointer3D.CanUsePointer = false;

        // Turn off the door's collider so it can't be clicked twice
        GetComponent<Collider>().enabled = false;

        // 2. Play the video if one is assigned
        if (cutscenePlayer != null)
        {
            // --- NEW: PAUSE THE MUSIC! ---
            if (MusicManager.Instance != null) MusicManager.Instance.PauseMusic();

            cutscenePlayer.gameObject.SetActive(true); // Turn the video object on
            
            // Tell the script to listen for the exact moment the video finishes
            cutscenePlayer.loopPointReached += OnCutsceneFinished; 
            
            cutscenePlayer.Play();
        }
        else
        {
            // Failsafe: If no video is assigned, just teleport instantly
            FinishTeleport();
        }
    }

    // This runs automatically the millisecond the video ends
    private void OnCutsceneFinished(VideoPlayer vp)
    {
        // Stop listening to the event (prevents memory leaks)
        vp.loopPointReached -= OnCutsceneFinished;

        // Hide the video player again
        vp.gameObject.SetActive(false);

        // Move to the next step
        FinishTeleport();
    }

    private void FinishTeleport()
    {
        // 3. Teleport the player
        _playerObject.transform.position = surgeryRoomSpawnPoint.position;
        _playerObject.transform.rotation = surgeryRoomSpawnPoint.rotation;

        // 4. Re-enable the controller, movement, and interactions so they can explore the room
        CharacterController cc = _playerObject.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = true;

        // Wake the PlayerMovement script back up
        PlayerMovement pm = _playerObject.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = true;

        SpatialPointer3D.CanUsePointer = true;

        // --- NEW: RESUME THE MUSIC! ---
        if (MusicManager.Instance != null) MusicManager.Instance.ResumeMusic();

        // Note: The StartDialogue command was removed! The player must now 
        // walk up to the Surgery Doctor and press E to start the climax.
    }
}