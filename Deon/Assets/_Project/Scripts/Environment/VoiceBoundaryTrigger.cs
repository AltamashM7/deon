using UnityEngine;
using System.Collections; // Required for Coroutines

[RequireComponent(typeof(AudioSource))]
public class VoiceBoundaryTrigger : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Drag your voice line audio clip here.")]
    public AudioClip voiceClip;
    
    private AudioSource audioSource;

    [Header("UI Hint Settings")]
    [Tooltip("Drag the CanvasGroup attached to your Hint Image here")]
    public CanvasGroup hintCanvasGroup;
    
    [Tooltip("How long the hint stays fully visible")]
    public float displayDuration = 4f;
    
    [Tooltip("How fast the hint fades in and out")]
    public float fadeSpeed = 2f;

    // The lock that guarantees the UI only shows once per game
    private bool hasHintBeenShown = false; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Force it to be a 2D sound so it plays clearly in the player's ears 
        // regardless of where they crossed the boundary
        audioSource.spatialBlend = 0f; 

        // Ensure the hint starts invisible if a CanvasGroup is assigned
        if (hintCanvasGroup != null)
        {
            hintCanvasGroup.alpha = 0f;
        }
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

                // 3. Show the UI hint ONLY if it hasn't been shown yet
                if (!hasHintBeenShown && hintCanvasGroup != null)
                {
                    hasHintBeenShown = true; // Lock it permanently for this session
                    StartCoroutine(ShowHintSequence());
                }
            }
        }
    }

    private IEnumerator ShowHintSequence()
    {
        // Fade In
        while (hintCanvasGroup.alpha < 1f)
        {
            hintCanvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        hintCanvasGroup.alpha = 1f;

        // Wait
        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        while (hintCanvasGroup.alpha > 0f)
        {
            hintCanvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        hintCanvasGroup.alpha = 0f;
    }
}