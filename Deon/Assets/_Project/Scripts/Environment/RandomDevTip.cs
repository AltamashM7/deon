using UnityEngine;
using TMPro;

public class RandomDevTip : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI tipText;

    [Header("Developer Tips")]
    // You can add as many tips here as you want!
    private string[] devTips = new string[]
    {
        "Dev Tip: This game is meant to be played in a single seating.",
        "Dev Tip: Wear headphones for the best experience.",
        "Dev Tip: If you clip through the floor, it's not a bug. It's a feature.",
    };

    // OnEnable runs EVERY time the GameObject this is attached to is turned on
    void OnEnable()
    {
        // 1. Pick a random number between 0 and the total number of tips
        int randomIndex = Random.Range(0, devTips.Length);
        
        // 2. Change the TextMeshPro text to match the randomly selected tip
        tipText.text = devTips[randomIndex];
    }
}