using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    #region Serialized Fields

    [Header("Movement Settings")]
    [SerializeField] private float maxWalkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f; 
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    [Header("Physics Settings")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float fallMultiplier = 1.5f;

    [Header("Camera & Look Settings")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float lookSmoothing = 0.05f;

    [Header("Strafe Tilt Settings")]
    [Tooltip("How many degrees the camera rolls when strafing left/right.")]
    [SerializeField] private float strafeTiltAngle = 2f;
    [Tooltip("How fast the camera rolls into the tilt.")]
    [SerializeField] private float tiltSmoothing = 5f;

    [Header("Dynamic FOV Settings")]
    [SerializeField] private float normalFov = 60f;
    [SerializeField] private float sprintFov = 75f;
    [SerializeField] private float fovTransitionSpeed = 5f;

    [Header("Headbob & Audio Settings")]
    [SerializeField] private float bobFrequency = 12f;
    [SerializeField] private float sprintBobFrequency = 18f; 
    [SerializeField] private float bobAmplitude = 0.05f;
    [SerializeField] private AudioClip[] footstepSounds;

    [Header("Landing Impact Settings")]
    [Tooltip("How far down the camera dips when landing.")]
    [SerializeField] private float landingDipAmount = 0.15f;
    [Tooltip("How fast the camera recovers from the dip.")]
    [SerializeField] private float landingRecoverySpeed = 7f;
    [Tooltip("Drag your heavy landing sound effect here!")]
    [SerializeField] private AudioClip landingSound;

    #endregion

    #region Private Variables

    private CharacterController controller;
    private AudioSource audioSource;
    private Camera cam; 
    
    private Vector2 moveInput;
    private Vector2 mouseInput;
    private Vector3 currentMoveVelocity;
    private float verticalVelocity;
    private bool isSprinting; 
    
    private float xRotation = 0f;
    private float yRotation = 0f;
    private float currentZRotation = 0f; 
    private Vector2 currentMouseDelta;
    private Vector2 mouseDeltaVelocity;

    // Headbob & Impact state variables
    private float defaultCameraY;
    private float bobTimer;
    private bool stepTaken;
    
    private bool wasGrounded; 
    private float currentLandingOffset = 0f; 

    #endregion

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        
        if (playerCamera != null)
        {
            cam = playerCamera.GetComponent<Camera>(); 
            defaultCameraY = playerCamera.localPosition.y;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yRotation = transform.eulerAngles.y;
        wasGrounded = true;
    }

    private void Update()
    {
        GatherInput();
        HandleCameraRotation();
        HandleDynamicFOV(); 
        HandleMovement();
        HandleHeadbobAndFootsteps(); 
        
        wasGrounded = controller.isGrounded; // Update grounded state at the very end
    }

    private void GatherInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        
        isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    private void HandleCameraRotation()
    {
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, mouseInput, ref mouseDeltaVelocity, lookSmoothing);

        float lookX = currentMouseDelta.x * mouseSensitivity;
        float lookY = currentMouseDelta.y * mouseSensitivity;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        yRotation += lookX;

        float targetZ = -moveInput.x * strafeTiltAngle;
        currentZRotation = Mathf.Lerp(currentZRotation, targetZ, tiltSmoothing * Time.deltaTime);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, currentZRotation);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private void HandleDynamicFOV()
    {
        if (cam == null) return;

        bool isMoving = moveInput.magnitude > 0.1f;
        float targetFov = (isSprinting && isMoving) ? sprintFov : normalFov;
        
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, fovTransitionSpeed * Time.deltaTime);
    }

    private void HandleMovement()
    {
        Vector3 targetDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        
        float currentTargetSpeed = isSprinting ? sprintSpeed : maxWalkSpeed;
        Vector3 targetVelocity = targetDirection * currentTargetSpeed;

        float currentSpeedRate = (moveInput.magnitude > 0) ? acceleration : deceleration;
        currentMoveVelocity = Vector3.Lerp(currentMoveVelocity, targetVelocity, currentSpeedRate * Time.deltaTime);

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0) verticalVelocity = -2f;

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            float currentGravity = (verticalVelocity < 0) ? gravity * fallMultiplier : gravity;
            verticalVelocity += currentGravity * Time.deltaTime;
        }

        Vector3 finalVelocity = currentMoveVelocity + (Vector3.up * verticalVelocity);
        
        // Physically move the player
        controller.Move(finalVelocity * Time.deltaTime);

        // NEW LOCATION: Check for landing right AFTER moving
        if (controller.isGrounded && !wasGrounded)
        {
            // We use the falling velocity from right before we hit the ground
            if (verticalVelocity < -3f)
            {
                currentLandingOffset = -landingDipAmount;
                if (landingSound != null)
                {
                    audioSource.PlayOneShot(landingSound);
                }
            }
        }
    }

    private void HandleHeadbobAndFootsteps()
    {
        currentLandingOffset = Mathf.Lerp(currentLandingOffset, 0f, landingRecoverySpeed * Time.deltaTime);

        if (!controller.isGrounded) return;

        if (Mathf.Abs(currentMoveVelocity.x) > 0.1f || Mathf.Abs(currentMoveVelocity.z) > 0.1f)
        {
            float currentBobFrequency = isSprinting ? sprintBobFrequency : bobFrequency;
            bobTimer += Time.deltaTime * currentBobFrequency;
            
            float newCameraY = defaultCameraY + (Mathf.Sin(bobTimer) * bobAmplitude) + currentLandingOffset;
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, newCameraY, playerCamera.localPosition.z);

            if (Mathf.Sin(bobTimer) < -0.95f && !stepTaken)
            {
                PlayFootstep();
                stepTaken = true; 
            }
            else if (Mathf.Sin(bobTimer) > 0f)
            {
                stepTaken = false;
            }
        }
        else
        {
            bobTimer = 0f;
            float targetY = defaultCameraY + currentLandingOffset;
            float smoothReturn = Mathf.Lerp(playerCamera.localPosition.y, targetY, Time.deltaTime * 5f);
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, smoothReturn, playerCamera.localPosition.z);
        }
    }

    private void PlayFootstep()
    {
        if (footstepSounds == null || footstepSounds.Length == 0) return;

        int randomIndex = Random.Range(0, footstepSounds.Length);
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(footstepSounds[randomIndex]);
    }
}