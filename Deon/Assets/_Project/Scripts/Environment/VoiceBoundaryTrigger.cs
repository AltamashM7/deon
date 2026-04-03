using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VoiceBoundaryTrigger : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Drag your voice line audio clip here.")]
    public AudioClip voiceClip;
    
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Force it to be a 2D sound so it plays clearly in the player's ears 
        // regardless of where they crossed the boundary
        audioSource.spatialBlend = 0f; 
    }

    void OnTriggerExit(Collider other)
    {
        // 1. Check if the object leaving the zone is the player
        if (other.CompareTag("Player"))
        {
            // 2. Check if a clip is assigned AND if the audio source is currently quiet
            if (voiceClip != null && !audioSource.isPlaying)
            {
                // We use .clip and .Play() instead of PlayOneShot because 
                // PlayOneShot doesn't correctly flag .isPlaying as true!
                audioSource.clip = voiceClip;
                audioSource.Play();
            }
        }
    }
}