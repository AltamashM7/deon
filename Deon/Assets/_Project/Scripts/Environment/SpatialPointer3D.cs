using UnityEngine;

public class SpatialPointer3D : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag the Arrow_Pivot here (the empty object we made)")]
    [SerializeField] private Transform arrowPivot;
    
    [Tooltip("Drag the actual 3D RightArrow mesh here so we can grab its material")]
    [SerializeField] private Renderer arrowRenderer;
    
    [Tooltip("Drag the desk/pc parent object here")]
    [SerializeField] private Transform objectiveTarget;

    [Header("Settings")]
    [Tooltip("The key the player presses to ping the objective")]
    [SerializeField] private KeyCode pingKey = KeyCode.F;
    
    [Tooltip("How many seconds the arrow stays fully visible")]
    [SerializeField] private float visibleDuration = 2f;
    
    [Tooltip("How fast the arrow fades in and out")]
    [SerializeField] private float fadeSpeed = 3f;

    // The Safety Lock for Farhan's VN and computer interactions
    public static bool CanUsePointer = true;
    
    private Material arrowMat;
    private Color matColor;
    private float visibleTimer = 0f;

    private enum PointerState { Hidden, FadingIn, Visible, FadingOut }
    private PointerState currentState = PointerState.Hidden;

    private void Start()
    {
        if (arrowRenderer != null)
        {
            // We use .material (not .sharedMaterial) so it creates a unique clone
            // for runtime, preventing us from permanently altering your project asset!
            arrowMat = arrowRenderer.material;
            matColor = arrowMat.color;
            
            // Force it to be completely invisible at start
            matColor.a = 0f;
            arrowMat.color = matColor;
            arrowRenderer.enabled = false; 
        }
    }

    private void Update()
    {
        // 1. Check for player input
        if (CanUsePointer && currentState == PointerState.Hidden && Input.GetKeyDown(pingKey))
        {
            currentState = PointerState.FadingIn;
            arrowRenderer.enabled = true; // Turn the mesh back on
            
            // Reset alpha just in case
            matColor.a = 0f;
            arrowMat.color = matColor;
        }

        // 2. Only calculate rotation and fading if it is currently active
        if (currentState != PointerState.Hidden)
        {
            arrowPivot.LookAt(objectiveTarget);
            HandleFading();
        }
    }

    private void HandleFading()
    {
        switch (currentState)
        {
            case PointerState.FadingIn:
                matColor.a += Time.deltaTime * fadeSpeed;
                if (matColor.a >= 1f)
                {
                    matColor.a = 1f;
                    currentState = PointerState.Visible;
                    visibleTimer = visibleDuration;
                }
                arrowMat.color = matColor;
                break;

            case PointerState.Visible:
                visibleTimer -= Time.deltaTime;
                if (visibleTimer <= 0f)
                {
                    currentState = PointerState.FadingOut;
                }
                break;

            case PointerState.FadingOut:
                matColor.a -= Time.deltaTime * fadeSpeed;
                if (matColor.a <= 0f)
                {
                    matColor.a = 0f;
                    currentState = PointerState.Hidden;
                    arrowRenderer.enabled = false; // Turn mesh off completely to save GPU power
                }
                arrowMat.color = matColor;
                break;
        }
    }
}