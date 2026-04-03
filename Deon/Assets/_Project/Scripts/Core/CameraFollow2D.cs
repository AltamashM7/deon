using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Tracking")]
    [Tooltip("The player object to follow. If left empty, it will auto-find the 'Player' tag.")]
    public Transform target;

    [Header("Smoothness")]
    [Tooltip("How smoothly the camera catches up (lower number = faster, higher = floatier)")]
    public float smoothTime = 0.25f;

    [Header("Positioning")]
    [Tooltip("Keep Z at -10 so the camera stays pulled back from the 2D plane!")]
    public Vector3 offset = new Vector3(0f, 1.5f, -10f);

    // This variable is required for SmoothDamp to calculate momentum, but we don't need to touch it
    private Vector3 _currentVelocity = Vector3.zero;

    private void Start()
    {
        // Quality of Life: Auto-target the player if Arihant forgot to assign it in the Inspector
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("[CameraFollow2D] Could not find any object tagged 'Player' to follow!");
            }
        }
    }

    private void LateUpdate()
    {
        // If there is still no target, do nothing
        if (target == null) return;

        // Where the camera *wants* to be
        Vector3 targetPosition = target.position + offset;

        // Smoothly glide from where we are right now, to where we want to be
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);
    }
}