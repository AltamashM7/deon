using UnityEngine;
using Yarn.Unity;
using Yarn.Markup;
using TMPro;
using System.Threading;

public class NextLineIndicator : ActionMarkupHandler
{
    [Header("Visuals")]
    [Tooltip("Drag your Triangle/Arrow GameObject here")]
    public GameObject indicatorObject;

    private void Awake()
    {
        // Start with the arrow hidden when the game boots
        if (indicatorObject != null) indicatorObject.SetActive(false);
    }

    public override void OnPrepareForLine(MarkupParseResult line, TMP_Text text) 
    { 
        // Ensure the arrow hides the moment a new line of text starts preparing
        if (indicatorObject != null) indicatorObject.SetActive(false);
    }

    public override void OnLineDisplayBegin(MarkupParseResult line, TMP_Text text) 
    { 
        // Left empty as it is handled by OnPrepareForLine
    }

    public override void OnLineDisplayComplete() 
    { 
        // The typing is 100% finished, turn the arrow on!
        if (indicatorObject != null) indicatorObject.SetActive(true);
    }

    public override void OnLineWillDismiss() 
    { 
        // The player clicked the hit zone to continue, hide the arrow again!
        if (indicatorObject != null) indicatorObject.SetActive(false);
    }

    // --- The Missing Required Override ---
    public override YarnTask OnCharacterWillAppear(int currentCharacterIndex, MarkupParseResult line, CancellationToken cancellationToken)
    {
        // We don't need the arrow to do anything per-letter, so we just tell Yarn to keep going
        return YarnTask.CompletedTask;
    }
}