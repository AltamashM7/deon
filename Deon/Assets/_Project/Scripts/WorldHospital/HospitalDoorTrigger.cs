using UnityEngine;
using Yarn.Unity;

public class HospitalDoorTrigger : MonoBehaviour
{
    [Tooltip("Drag the Surgery_SpawnPoint from the surgery room here")]
    public Transform surgeryRoomSpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        // Ensure only the Player triggers this
        if (other.CompareTag("Player"))
        {
            // 1. Disable the character controller safely
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // 2. Teleport the player down to the surgery room
            other.transform.position = surgeryRoomSpawnPoint.position;
            other.transform.rotation = surgeryRoomSpawnPoint.rotation;

            // 3. Re-enable the controller
            if (cc != null) cc.enabled = true;

            // 4. Fire the Final Decision dialogue
            FindAnyObjectByType<DialogueRunner>().StartDialogue("World1_FinalDecision");
            
            // 5. Turn off this trigger so it only happens once
            GetComponent<Collider>().enabled = false;
        }
    }
}