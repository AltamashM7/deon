using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("How close the player needs to be to interact")]
    [SerializeField] private float interactRange = 3f;
    
    [Tooltip("The button used to interact")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private void Update()
    {
        // Safety check: Don't allow clicking if the game is paused (Time.timeScale == 0)
        // or if Farhan's VN dialogue is currently running.
        if (Time.timeScale == 0f) return;

        if (Input.GetKeyDown(interactKey))
        {
            AttemptInteraction();
        }
    }

    private void AttemptInteraction()
    {
        // Shoot an invisible raycast from the exact center of the player's camera forward
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // Did the laser hit something with an InteractableMonitor script?
            InteractableMonitor monitor = hit.collider.GetComponent<InteractableMonitor>();
            
            if (monitor != null)
            {
                // We found a monitor! Trigger it.
                monitor.TriggerMonitor();
            }
        }
    }
}