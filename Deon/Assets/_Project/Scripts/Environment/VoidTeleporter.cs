using UnityEngine;
using System.Collections;

public class VoidTeleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    [Tooltip("Drag the center of the Hub (or an empty GameObject) here.")]
    public Transform centerPoint; 
    
    [Header("Effects")]
    public AudioClip glitchSound;
    private AudioSource audioSource;

    void Start()
    {
        // Dynamically add an audio source so we don't have to set it up in the Inspector
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f; // Forces the sound to be 2D so it plays at full volume everywhere
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the object leaving the boundary is the Player
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportSequence(other.gameObject));
        }
    }

    IEnumerator TeleportSequence(GameObject player)
    {
        // 1. Play the Audio Cue
        if (glitchSound != null)
        {
            audioSource.PlayOneShot(glitchSound);
        }

        // 2. Visual Cue: Violent FOV snap
        Camera playerCam = Camera.main;
        float originalFOV = playerCam.fieldOfView;
        playerCam.fieldOfView = 140f; // Warps the screen aggressively

        // 3. TELEPORTATION (The CharacterController Quirk)
        CharacterController cc = player.GetComponent<CharacterController>();
        
        // You CANNOT change the transform.position of an active CharacterController.
        // It must be disabled first, moved, and then re-enabled.
        if (cc != null) cc.enabled = false;
        
        player.transform.position = centerPoint.position;
        
        if (cc != null) cc.enabled = true;

        // 4. Recover the Visuals
        yield return new WaitForSeconds(0.15f); // Hold the glitch for a fraction of a second
        playerCam.fieldOfView = originalFOV;
    }
}