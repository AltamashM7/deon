using UnityEngine;

[CreateAssetMenu(fileName = "WorldDefinition", menuName = "DEON/World Definition")]
public class WorldDefinition : ScriptableObject
{
    [Header("Identity")]
    public string worldId;          // e.g. "world_hospital"
    public string displayName;      // e.g. "World 1 - Hospital"

    [Header("Scoring")]
    [Tooltip("Score awarded for the morally correct choice")]
    public int goodChoiceScore = 1;
    [Tooltip("Score awarded for the morally wrong choice")]
    public int badChoiceScore = 0;
}