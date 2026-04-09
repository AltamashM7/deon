using UnityEngine;
using Yarn.Unity;

[RequireComponent(typeof(Collider2D))]
public class TeleportDoor2D : MonoBehaviour
{
    [Header("Teleportation")]
    [Tooltip("Drag the Transform (Spawn Point) from the Child's secret room here")]
    public Transform destinationPoint;

    [Header("Feedback")]
    [Tooltip("The dialogue node to play if the player tries to leave too early")]
    public string lockedDialogueNode = "World2_DoorLocked";
    public DialogueRunner dialogueRunner;

    [Header("UI")]
    [Tooltip("Optional: A 'Press Z to Enter' UI prompt")]
    public GameObject interactPrompt;

    private bool _playerInRange = false;
    private Transform _playerTransform;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    private void Update()
    {
        if (!_playerInRange) return;

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.A))
        {
            TryTeleport();
        }
    }

    private void TryTeleport()
    {
        // Check with the Manager to see if the player talked to everyone
        if (World2_Manager.Instance != null && World2_Manager.Instance.AreAllObjectivesComplete())
        {
            // Unlocked! Teleport them.
            if (_playerTransform != null)
            {
                _playerTransform.position = destinationPoint.position;
            }
        }
        else
        {
            // Locked! Play the warning dialogue.
            if (dialogueRunner != null && !string.IsNullOrEmpty(lockedDialogueNode))
            {
                if (!dialogueRunner.IsDialogueRunning)
                {
                    dialogueRunner.StartDialogue(lockedDialogueNode);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerInRange = true;
            _playerTransform = collision.transform; // Save the player's transform to teleport them
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerInRange = false;
            _playerTransform = null;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }
}