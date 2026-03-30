using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DEONConfig", menuName = "DEON/Game Config")]
public class DEONConfig : ScriptableObject
{
    [Header("Worlds — add or remove freely")]
    public List<WorldDefinition> worlds = new List<WorldDefinition>();

    [Header("Ending Thresholds")]
    [Tooltip("Minimum total score to trigger the good ending")]
    public int goodEndingThreshold = 2;

    [Header("Ending Scene Names")]
    public string goodEndingScene = "Ending_Good";
    public string badEndingScene  = "Ending_Bad";
}