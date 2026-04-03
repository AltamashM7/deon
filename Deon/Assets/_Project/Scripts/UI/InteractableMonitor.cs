using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractableMonitor : MonoBehaviour
{
    [Tooltip("Drag the specific WorldDefinition asset here (e.g., Hospital, Utopia)")]
    public WorldDefinition myWorldData;
    
    private TerminalUIManager uiManager;

    private void Start()
    {
        // FIX: Replaced the obsolete code with Unity's new, faster search API
        uiManager = FindAnyObjectByType<TerminalUIManager>();
    }

    // Call this from your Player's Raycast script when looking at the monitor and pressing 'E'
    public void TriggerMonitor()
    {
        if (myWorldData != null && uiManager != null)
        {
            uiManager.OpenTerminal(myWorldData);
        }
    }
}