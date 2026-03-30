using UnityEngine;
using Yarn.Unity;

public class ChoiceEngineYarnCommands : MonoBehaviour
{
    // Yarn command: <<record_choice "world_hospital" 1>>
    [YarnCommand("record_choice")]
    public static void RecordChoice(string worldId, int score)
    {
        if (ChoiceEngine.Instance == null)
        {
            Debug.LogError("[ChoiceEngine] No ChoiceEngine instance found in scene.");
            return;
        }
        ChoiceEngine.Instance.RecordChoice(worldId, score);
    }

    // Yarn command: <<evaluate_ending>>
    // Call this from the hub child's final dialogue after all worlds are done
    [YarnCommand("evaluate_ending")]
    public static void EvaluateEnding()
    {
        if (ChoiceEngine.Instance == null) return;
        ChoiceEngine.Instance.EvaluateAndTriggerEnding();
    }

    // Yarn function: returns 1 if player made the good choice in a world, 0 otherwise
    // Usage in .yarn: <<if get_world_score("world_hospital") > 0>>
    [YarnFunction("get_world_score")]
    public static int GetWorldScore(string worldId)
    {
        if (ChoiceEngine.Instance == null) return -1;
        return ChoiceEngine.Instance.GetWorldScore(worldId);
    }
}