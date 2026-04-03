using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("How close the player needs to be to interact")]
    [SerializeField] private float interactRange = 3f;
    
    [Tooltip("How thick the detection beam is. Larger = easier to target while moving.")]
    [SerializeField] private float detectionRadius = 0.5f;

    [Tooltip("The button used to interact")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("UI Settings")]
    [Tooltip("Drag the CanvasGroup of your Interact_Prompt UI here")]
    [SerializeField] private CanvasGroup interactPromptGroup;
    
    [Tooltip("How fast the prompt fades in and out")]
    [SerializeField] private float fadeSpeed = 10f;

    private bool _isLookingAtInteractable = false;
    private InteractableMonitor _currentMonitor = null;

    private void Update()
    {
        // Safety check: Don't allow interacting or showing UI if the game is paused (Time.timeScale == 0)
        // or if a menu/dialogue is currently running.
        if (Time.timeScale == 0f || !SpatialPointer3D.CanUsePointer)
        {
            FadePrompt(false);
            return;
        }

        // 1. Shoot the thick beam to find targets
        CheckForInteractable();
        
        // 2. Smoothly fade the UI
        FadePrompt(_isLookingAtInteractable);

        // 3. Handle the actual key press
        if (_isLookingAtInteractable && _currentMonitor != null)
        {
            if (Input.GetKeyDown(interactKey))
            {
                _currentMonitor.TriggerMonitor();
                _isLookingAtInteractable = false; // Hide prompt instantly so it doesn't overlap the terminal UI
            }
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        
        // Shoot a thick cylinder that passes through objects, grabbing everything in its path
        RaycastHit[] hits = Physics.SphereCastAll(ray, detectionRadius, interactRange);

        // Default to false. It only turns true if the monitor is inside the beam.
        _isLookingAtInteractable = false; 
        _currentMonitor = null;

        foreach (RaycastHit hit in hits)
        {
            // Check for a Monitor
            InteractableMonitor monitor = hit.collider.GetComponent<InteractableMonitor>();
            if (monitor != null)
            {
                _isLookingAtInteractable = true;
                _currentMonitor = monitor;
                return; 
            }
            
            // Check for the Child NPC
            InteractableNPC npc = hit.collider.GetComponent<InteractableNPC>();
            if (npc != null)
            {
                _isLookingAtInteractable = true;
                
                // If they press E while looking at her, trigger the dialogue!
                if (Input.GetKeyDown(interactKey))
                {
                    npc.TriggerDialogue();
                    _isLookingAtInteractable = false; // Hide prompt instantly
                }
                return;
            }
        }
    }

    private void FadePrompt(bool show)
    {
        if (interactPromptGroup == null) return;

        // Smoothly fade between 0 (invisible) and 1 (visible)
        float targetAlpha = show ? 1f : 0f;
        interactPromptGroup.alpha = Mathf.Lerp(interactPromptGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
    }
}