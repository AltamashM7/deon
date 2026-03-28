using UnityEngine;

/// <summary>
/// Place this on an NPC GameObject (with a trigger Collider2D).
/// Yarn Spinner references are stubbed out until the package is installed.
/// Re-enable them after Yarn Spinner is confirmed working in the project.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class NPCInteractionTrigger : MonoBehaviour
{
    [Header("Yarn Spinner (wire up after package is installed)")]
    [Tooltip("The exact node name in your .yarn file to start.")]
    [SerializeField] private string yarnStartNode = "NPC_Default";

    [Header("Prompt UI")]
    [Tooltip("Optional: a small 'Press Z to Talk' prompt above the NPC.")]
    [SerializeField] private GameObject interactPrompt;

    private bool _playerInRange;
    private PlayerController2D _player;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void Update()
    {
        if (!_playerInRange) return;

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.A))
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        // Lock player movement
        if (_player != null)
            _player.IsDialogueActive = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        // --- YARN SPINNER HOOK (uncomment once package is installed) ---
        // dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
        // dialogueRunner.StartDialogue(yarnStartNode);

        // Temporary: auto-unlock after 3 seconds so you can test movement
        Invoke(nameof(OnDialogueComplete), 3f);

        Debug.Log("[NPC] Dialogue triggered for node: " + yarnStartNode);
    }

    private void OnDialogueComplete()
    {
        if (_player != null)
            _player.IsDialogueActive = false;

        Debug.Log("[NPC] Dialogue complete — player unlocked.");
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