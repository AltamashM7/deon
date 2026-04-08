using UnityEngine;
using Yarn.Unity;

public class World1_Manager : MonoBehaviour
{
    [Tooltip("Drag your global Dialogue Runner here")]
    public DialogueRunner dialogueRunner;
    
    [Tooltip("Drag the invisible Hospital Door cube here")]
    public Collider hospitalDoorTrigger;
    
    private bool talkedToJunior = false;
    private bool talkedToPatient = false;
    private PlayerInteractor playerUI;

    private void Start()
    {
        // 1. Lock the door at the start!
        if (hospitalDoorTrigger != null) hospitalDoorTrigger.enabled = false; 
        
        playerUI = FindAnyObjectByType<PlayerInteractor>();

        // 2. Teach Yarn how to report back to this script
        if (dialogueRunner != null)
        {
            dialogueRunner.AddCommandHandler<string>("complete_objective", MarkObjectiveComplete);
        }

        // 3. Show your custom starting hint for 6 seconds
        if (playerUI != null) 
        {
            playerUI.ShowSystemHint("This is breaktime, you should talk with the others.", 4f);
        }
    }

    private void MarkObjectiveComplete(string npcName)
    {
        if (npcName == "junior") talkedToJunior = true;
        if (npcName == "patient") talkedToPatient = true;

        // If both are true, the break is over!
        if (talkedToJunior && talkedToPatient)
        {
            if (playerUI != null) 
            {
                playerUI.ShowSystemHint("The break is over. Return to the Hospital.", 4f);
            }
            
            // Unlock the climax door
            if (hospitalDoorTrigger != null) hospitalDoorTrigger.enabled = true; 
        }
    }
}