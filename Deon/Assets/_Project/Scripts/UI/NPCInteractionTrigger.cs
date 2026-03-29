using UnityEngine;
using Yarn.Unity; // Yarn Spinner namespace

/// <summary>
/// Place this on an NPC GameObject (with a trigger Collider2D).
/// When the player enters the zone and presses Z or A, it activates
/// the VN Dialogue UI and starts the assigned Yarn dialogue node.
///
/// Conforms to DEON Project Structure:
///   Scripts/UI/NPCInteractionTrigger.cs
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class NPCInteractionTrigger : MonoBehaviour
{
    [Header("Yarn Spinner")]
    [Tooltip("The Yarn DialogueRunner in this scene.")]
    [SerializeField] private DialogueRunner dialogueRunner;

    [Tooltip("The exact node name in your .yarn file to start. E.g. 'NPC_Lawyer_Intro'")]
    [SerializeField] private string yarnStartNode = "NPC_Default";

    [Header("Prompt UI")]
    [Tooltip("Optional: a small 'Press Z to Talk' prompt GameObject above the NPC.")]
    [SerializeField] private GameObject interactPrompt;

    // --- Private State ---
    private bool _playerInRange;
    private PlayerController2D _player;

    private void Start()
    {
        // Ensure the Collider2D on this GameObject is a trigger
        GetComponent<Collider2D>().isTrigger = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void Update()
    {
        if (!_playerInRange) return;

        // Interaction keys: Z or A (as specified in Jira Task 2.2)
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.A))
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        if (dialogueRunner == null)
        {
            Debug.LogError("[NPCInteractionTrigger] DialogueRunner is not assigned on " + gameObject.name);
            return;
        }

        if (dialogueRunner.IsDialogueRunning) return; // Prevent double-trigger

        // Lock player movement
        if (_player != null)
            _player.IsDialogueActive = true;

        // Hide prompt
        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        // Hook into Yarn's completion event to unlock the player when done
        dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);

        // Start the Yarn conversation
        dialogueRunner.StartDialogue(yarnStartNode);
    } 

    private void OnDialogueComplete()
    {
        // Unlock player
        if (_player != null)
            _player.IsDialogueActive = false;

        // Unsubscribe to avoid duplicate calls on next conversation
        dialogueRunner.onDialogueComplete.RemoveListener(OnDialogueComplete);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _playerInRange = true;
        _player = other.GetComponent<PlayerController2D>();

        if (interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _playerInRange = false;
        _player = null;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}