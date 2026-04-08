using UnityEngine;
using TMPro; 

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("How close the player needs to be to interact")]
    [SerializeField] private float interactRange = 3f;
    
    [Tooltip("How thick the detection beam is. Larger = easier to target while moving.")]
    [SerializeField] private float detectionRadius = 0.5f;

    [Tooltip("The button used to interact")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("UI - Interact Prompt")]
    [Tooltip("Drag the CanvasGroup containing your E button and - Interact text here")]
    [SerializeField] private CanvasGroup interactPromptGroup;

    [Header("UI - System Hints")]
    [Tooltip("Drag the NEW CanvasGroup you created for system messages here")]
    [SerializeField] private CanvasGroup systemHintGroup;
    
    [Tooltip("Drag the TextMeshPro object inside the System Hint Group here")]
    [SerializeField] private TMP_Text systemHintText;

    [Header("Global UI Settings")]
    [SerializeField] private float fadeSpeed = 10f;

    private bool _isLookingAtInteractable = false;
    private InteractableMonitor _currentMonitor = null;
    private float _systemHintTimer = 0f;

    private void Update()
    {
        // Safety check: Don't allow interacting or showing UI if the game is paused 
        if (Time.timeScale == 0f || !SpatialPointer3D.CanUsePointer)
        {
            // FORCE the alpha to 0 instantly so it doesn't get "stuck" when time stops
            if (interactPromptGroup != null) interactPromptGroup.alpha = 0f;
            if (systemHintGroup != null) systemHintGroup.alpha = 0f;
            return;
        }

        // 1. Shoot the thick beam to find targets
        CheckForInteractable();
        
        // 2. Handle System Hint Timer & UI Fading (Independent of looking at things)
        if (_systemHintTimer > 0f)
        {
            _systemHintTimer -= Time.deltaTime;
            FadeGroup(systemHintGroup, true); 
        }
        else
        {
            FadeGroup(systemHintGroup, false);
        }

        // 3. Handle standard Interact Prompt (Independent of system hints)
        FadeGroup(interactPromptGroup, _isLookingAtInteractable);

        // 4. Handle the actual key press for Monitors
        if (_isLookingAtInteractable && _currentMonitor != null)
        {
            if (Input.GetKeyDown(interactKey))
            {
                _currentMonitor.TriggerMonitor();
                _isLookingAtInteractable = false; 
            }
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, detectionRadius, interactRange);

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
                if (Input.GetKeyDown(interactKey))
                {
                    npc.TriggerDialogue();
                    _isLookingAtInteractable = false; 
                }
                return;
            }

            // Check for the Interactable Door
            InteractableDoor door = hit.collider.GetComponent<InteractableDoor>();
            if (door != null)
            {
                _isLookingAtInteractable = true;
                if (Input.GetKeyDown(interactKey))
                {
                    door.TeleportPlayer(transform.root.gameObject);
                    _isLookingAtInteractable = false; 
                }
                return;
            }
        }
    }

    // Helper method to safely fade any canvas group
    private void FadeGroup(CanvasGroup group, bool show)
    {
        if (group == null) return;

        float targetAlpha = show ? 1f : 0f;
        group.alpha = Mathf.Lerp(group.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
    }

    // --- METHOD FOR SYSTEM HINTS ---
    public void ShowSystemHint(string message, float duration = 4f)
    {
        if (systemHintText != null)
        {
            systemHintText.text = message;
        }
        
        _systemHintTimer = duration; 
        
        if (systemHintGroup != null)
        {
            systemHintGroup.alpha = 1f;
        }
    }
}