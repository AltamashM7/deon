using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ChildAIWander : MonoBehaviour
{
    [Header("Wander Settings")]
    [Tooltip("The central point the NPC will wander around. Drag the 'pc' parent object here.")]
    [SerializeField] private Transform centerDesks;
    
    [Tooltip("How far from the center the NPC can walk.")]
    [SerializeField] private float wanderRadius = 10f;
    
    [Tooltip("Minimum time the NPC will stand idle before picking a new destination.")]
    [SerializeField] private float minIdleTime = 3f;
    
    [Tooltip("Maximum time the NPC will stand idle.")]
    [SerializeField] private float maxIdleTime = 7f;

    // Component References
    private NavMeshAgent agent;
    private Animator anim;

    // Internal State Tracking
    private float idleTimer;
    private float currentIdleDuration;
    private bool isIdle = true;
    private int currentAnimState; // <-- Added to track the current animation

    // Performance Optimization: Animator Hashes
    // String comparisons ("walk", "stand") are slow. Hashes are calculated once and read instantly.
    private readonly int walkStateHash = Animator.StringToHash("walk");
    private readonly int standStateHash = Animator.StringToHash("stand");
    private readonly float animationCrossfadeTime = 0.2f;

    private void Awake()
    {
        // Cache components in Awake (Industry Standard)
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        // Safety Check: Prevent the script from crashing if you forget to assign the center point
        if (centerDesks == null)
        {
            Debug.LogWarning($"[{gameObject.name}] ChildAIWander: centerDesks is not assigned! NPC will stand still.");
            enabled = false;
            return;
        }

        StartIdle();
    }

    private void Update()
    {
        HandleAnimation();

        // Simple State Machine
        if (isIdle)
        {
            HandleIdleState();
        }
        else
        {
            HandleMovementState();
        }
    }

private void HandleAnimation()
    {
        // FIX: We stop relying on the NavMeshAgent's physical velocity, 
        // which can micro-stutter. We use our 100% stable logic state instead.
        bool isMoving = !isIdle;

        // Determine which animation we SHOULD be playing
        int targetState = isMoving ? walkStateHash : standStateHash;

        // ONLY trigger CrossFade if we are changing to a NEW state
        if (currentAnimState != targetState)
        {
            anim.CrossFadeInFixedTime(targetState, animationCrossfadeTime);
            currentAnimState = targetState; // Save the new state so we don't trigger it again next frame
        }
    }

    private void HandleIdleState()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= currentIdleDuration)
        {
            PickNewDestination();
        }
    }

    private void HandleMovementState()
    {
        // Check if the agent has reached its destination, or if the path is blocked
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartIdle();
        }
    }

    private void StartIdle()
    {
        isIdle = true;
        idleTimer = 0f;
        currentIdleDuration = Random.Range(minIdleTime, maxIdleTime);
        agent.ResetPath(); // Force the physics engine to clear any residual velocity
    }

    private void PickNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += centerDesks.position;

        NavMeshHit hit;
        
        // Sample the NavMesh to ensure the random point is actually on the walkable floor
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
        {
            agent.SetDestination(hit.position);
            isIdle = false;
        }
        else
        {
            // Fallback: If the point is off the map, stay idle and try again later
            StartIdle(); 
        }
    }
}