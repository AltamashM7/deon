using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity; 

public class PortraitManager : MonoBehaviour
{
    [Header("Yarn Brain")]
    [Tooltip("Drag your Dialogue System object here")]
    [SerializeField] private DialogueRunner dialogueRunner;

    [Header("Animators")]
    [SerializeField] private Animator leftAnimator;
    [SerializeField] private Animator rightAnimator;

    [Header("UI References")]
    [SerializeField] private Image leftPortrait;
    [SerializeField] private Image rightPortrait;

    [Header("Sprite Library")]
    [SerializeField] private CharacterSprite[] spriteLibrary;

    [System.Serializable]
    public struct CharacterSprite
    {
        public string spriteName; 
        public Sprite spriteImage;
    }

    private void Awake()
    {
        // Hard-wire the commands to the brain so we don't need GameObject names in the script
        if (dialogueRunner != null)
        {
            dialogueRunner.AddCommandHandler<string>("set_left", SetLeftPortrait);
            dialogueRunner.AddCommandHandler<string>("set_right", SetRightPortrait);
            dialogueRunner.AddCommandHandler<string>("highlight", HighlightSpeaker);
            
            // Animation Commands
            dialogueRunner.AddCommandHandler("enter_left", EnterLeft);
            dialogueRunner.AddCommandHandler("exit_left", ExitLeft);
            dialogueRunner.AddCommandHandler("enter_right", EnterRight);
            dialogueRunner.AddCommandHandler("exit_right", ExitRight);
        }
        else
        {
            Debug.LogError("PortraitManager needs the DialogueRunner assigned in the Inspector!");
        }
    }

    private void Start()
    {
        // Hide portraits by default when the game starts
        leftPortrait.color = new Color(1, 1, 1, 0); 
        rightPortrait.color = new Color(1, 1, 1, 0);
    }

    private Sprite GetSprite(string requestedName)
    {
        foreach (var character in spriteLibrary)
        {
            if (character.spriteName == requestedName) 
                return character.spriteImage;
        }
        Debug.LogWarning("PortraitManager couldn't find a sprite named: " + requestedName);
        return null;
    }

    public void SetLeftPortrait(string spriteName)
    {
        Sprite newSprite = GetSprite(spriteName);
        if (newSprite != null)
        {
            leftPortrait.sprite = newSprite;
            leftPortrait.color = Color.white; 
        }
    }

    public void SetRightPortrait(string spriteName)
    {
        Sprite newSprite = GetSprite(spriteName);
        if (newSprite != null)
        {
            rightPortrait.sprite = newSprite;
            rightPortrait.color = Color.white; 
        }
    }

    public void HighlightSpeaker(string side)
    {
        Color darkened = new Color(0.4f, 0.4f, 0.4f, 1f); 

        if (side.ToLower() == "left")
        {
            leftPortrait.color = Color.white;
            rightPortrait.color = darkened;
        }
        else if (side.ToLower() == "right")
        {
            rightPortrait.color = Color.white;
            leftPortrait.color = darkened;
        }
        else if (side.ToLower() == "both")
        {
            leftPortrait.color = Color.white;
            rightPortrait.color = Color.white;
        }
    }

    // --- Animation Triggers ---
    public void EnterLeft() { if (leftAnimator != null) leftAnimator.SetTrigger("SlideIn"); }
    public void ExitLeft() { if (leftAnimator != null) leftAnimator.SetTrigger("SlideOut"); }
    public void EnterRight() { if (rightAnimator != null) rightAnimator.SetTrigger("SlideIn"); }
    public void ExitRight() { if (rightAnimator != null) rightAnimator.SetTrigger("SlideOut"); }
}