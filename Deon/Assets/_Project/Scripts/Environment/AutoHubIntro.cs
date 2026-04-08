using UnityEngine;
using Yarn.Unity;

public class AutoHubIntro : MonoBehaviour
{
    [Header("Yarn References")]
    [Tooltip("Drag your Dialogue System / Dialogue Runner here")]
    public DialogueRunner dialogueRunner;

    // The magic word 'static' means these variables live outside the scene.
    // They survive scene reloads and will not reset until you quit the game!
    private static bool _hasPlayedIntro = false;
    private static bool _hasPlayedHospitalReaction = false;

    private void Start()
    {
        // Wait half a second for the scene to fully load
        Invoke(nameof(CheckAndPlayDialogue), 0.5f);
    }

    private void CheckAndPlayDialogue()
    {
        if (dialogueRunner == null) return;

        // 1. Is this our very first time in the Hub?
        if (!_hasPlayedIntro)
        {
            _hasPlayedIntro = true; // Mark as played forever
            dialogueRunner.StartDialogue("Hub_Intro");
            return; // Stop here so we don't play anything else
        }

        // 2. Did we just get back from the Hospital?
        if (ChoiceEngine.Instance != null)
        {
            int hospitalScore = ChoiceEngine.Instance.GetWorldScore("world_hospital");

            // If we finished the world (score is not 0) AND haven't seen the reaction yet
            if (hospitalScore != 0 && !_hasPlayedHospitalReaction)
            {
                _hasPlayedHospitalReaction = true; // Mark as played forever
                
                // FORCE the reaction to play automatically!
                dialogueRunner.StartDialogue("Hub_Child_World1_Reaction");
                return;
            }
        }
    }
}