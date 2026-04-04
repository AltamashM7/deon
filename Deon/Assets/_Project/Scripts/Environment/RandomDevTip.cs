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
        "Dev Tip: We didn't build a save system. Don't close the window.",
        "Dev Tip: Please don't try to sequence-break the cutscenes.",
        "Dev Tip: The Hub is safe. The minigames are not.",
        "Dev Tip: If you clip through the floor, it's not a bug. It's a feature.",
        "Dev Tip: We highly recommend wearing headphones. The audio team worked hard.",
        "Dev Tip: The physics in the minigames are... sensitive. Be gentle."
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