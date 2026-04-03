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

    // --- ADDED FOR HUB UI ---
    [Header("Terminal UI")]
    [TextArea(3, 6)]
    public string worldDescription;
    public Sprite worldPreviewImage;
    
    [Tooltip("The exact name of the Scene file to load (Case Sensitive!)")]
    public string sceneToLoad; 
}