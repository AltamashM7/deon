using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChoiceEngine : MonoBehaviour
{
    public static ChoiceEngine Instance { get; private set; }

    [SerializeField] private DEONConfig config;

    // worldId → score earned in that world
    private Dictionary<string, int> _worldScores = new Dictionary<string, int>();

    // worldId → has this world been completed
    private HashSet<string> _completedWorlds = new HashSet<string>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Called from Yarn Spinner commands when the player makes their final
    /// choice inside a world. score = goodChoiceScore or badChoiceScore
    /// from that world's WorldDefinition.
    /// </summary>
    public void RecordChoice(string worldId, int score)
    {
        _worldScores[worldId] = score;
        _completedWorlds.Add(worldId);

        Debug.Log($"[ChoiceEngine] Recorded: {worldId} → {score} points");
    }

    /// <summary>
    /// Returns true if all worlds defined in config have been completed.
    /// </summary>
    public bool AllWorldsComplete()
    {
        foreach (var world in config.worlds)
        {
            if (!_completedWorlds.Contains(world.worldId))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Adds up scores and loads the correct ending scene.
    /// Call this from the hub child's final dialogue node.
    /// </summary>
    public void EvaluateAndTriggerEnding()
    {
        if (!AllWorldsComplete())
        {
            Debug.LogWarning("[ChoiceEngine] Not all worlds complete yet — cannot evaluate ending.");
            return;
        }

        int total = 0;
        foreach (var score in _worldScores.Values)
            total += score;

        int maxPossible = 0;
        foreach (var world in config.worlds)
            maxPossible += world.goodChoiceScore;

        Debug.Log($"[ChoiceEngine] Total score: {total} / {maxPossible} | Threshold: {config.goodEndingThreshold}");

        if (total >= config.goodEndingThreshold)
            SceneManager.LoadScene(config.goodEndingScene);
        else
            SceneManager.LoadScene(config.badEndingScene);
    }

    /// <summary>
    /// Lets Yarn Spinner check what score the player got in a specific world.
    /// Useful for branching the child's hub reaction dialogue.
    /// </summary>
    public int GetWorldScore(string worldId)
    {
        return _worldScores.TryGetValue(worldId, out int score) ? score : 0;
    }

    /// <summary>
    /// Wipe all data — useful for a New Game button.
    /// </summary>
    public void ResetAll()
    {
        _worldScores.Clear();
        _completedWorlds.Clear();
    }
}