using UnityEngine;
using UnityEngine.AI;
using System.Collections; // Required for Coroutines
using Yarn.Unity;

[RequireComponent(typeof(Collider))]
public class InteractableNPC : MonoBehaviour
{
    [Header("Yarn Spinner Settings")]
    [Tooltip("Drag the Dialogue System object from your VN_Engine_Master here")]
    [SerializeField] private DialogueRunner dialogueRunner;
    
    [Tooltip("The exact Yarn node to start when talking to her")]
    [SerializeField] private string yarnStartNode = "Hub_Default";

    [Header("Player Controls")]
    [Tooltip("Drag the Player scripts (Movement, Camera Look) here to disable them")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    [Header("NPC AI Controls")]
    [Tooltip("Drag the child's NavMeshAgent here")]
    [SerializeField] private NavMeshAgent npcAgent;
    [Tooltip("Drag the ChildAIWander script here so we can pause her brain")]
    [SerializeField] private ChildAIWander npcBrain;
    [Tooltip("How fast the NPC rotates to face the player")]
    [SerializeField] private float turnSpeed = 5f;

    private Transform playerTransform;
    private Animator npcAnimator;
    private Coroutine turnCoroutine;

    private void Start()
    {
        // Hook into Yarn's native C# events
        if (dialogueRunner != null)
        {
            dialogueRunner.onNodeStart.AddListener(EngagePlayer);
            dialogueRunner.onDialogueComplete.AddListener(ReleasePlayer);
        }

        // Find the player once
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Auto-grab components
        if (npcAgent == null) npcAgent = GetComponent<NavMeshAgent>();
        if (npcBrain == null) npcBrain = GetComponent<ChildAIWander>();
        npcAnimator = GetComponent<Animator>();
    }

    public void TriggerDialogue()
    {
        if (dialogueRunner != null && !dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue(yarnStartNode);
        }
    }

    private void EngagePlayer(string nodeName)
    {
        // 1. Lock global interactions and player movement
        SpatialPointer3D.CanUsePointer = false;
        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = false;
        }

        // 2. Unlock cursor for VN choices
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3. Hijack the NPC
        if (npcBrain != null) npcBrain.enabled = false; 
        if (npcAgent != null) 
        {
            npcAgent.isStopped = true; 
            npcAgent.velocity = Vector3.zero; 
            npcAgent.ResetPath();
        }

        // 4. Force the idle animation so she doesn't get stuck walking
        if (npcAnimator != null)
        {
            npcAnimator.CrossFade("stand", 0.2f);
        }

        // 5. Start the smooth rotation
        if (playerTransform != null)
        {
            if (turnCoroutine != null) StopCoroutine(turnCoroutine);
            turnCoroutine = StartCoroutine(SmoothTurnRoutine());
        }
    }

    private void ReleasePlayer()
    {
        // 1. Re-enable player movement
        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = true;
        }

        // 2. Relock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SpatialPointer3D.CanUsePointer = true;

        // 3. Release the NPC back to her AI
        if (npcBrain != null) npcBrain.enabled = true;
        if (npcAgent != null) npcAgent.isStopped = false;
        
        // Stop turning if the conversation ends super fast
        if (turnCoroutine != null) StopCoroutine(turnCoroutine);
    }

    private IEnumerator SmoothTurnRoutine()
    {
        // Calculate the flat direction to the player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        directionToPlayer.y = 0; // Ignore height so she doesn't tilt backward

        // Calculate the final mathematical rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Smoothly rotate until she is almost perfectly facing the player
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
            yield return null; // Wait for the next frame
        }

        // Snap the last microscopic decimal to perfect the alignment
        transform.rotation = targetRotation;
    }
}