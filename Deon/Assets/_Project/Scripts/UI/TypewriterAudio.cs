using UnityEngine;
using Yarn.Unity;
using Yarn.Markup;
using TMPro;
using System.Threading;

[RequireComponent(typeof(AudioSource))]
public class TypewriterAudio : ActionMarkupHandler
{
    private AudioSource audioSource;
    
    [Tooltip("Drag your blip sound effect here")]
    public AudioClip blipSound;

    [Tooltip("Time in seconds between blips. Lower = faster blipping. Try 0.03 to 0.07.")]
    public float blipCooldown = 0.05f;

    [Tooltip("Check this to stop blips from playing on spaces.")]
    public bool skipSpaces = true;

    private float lastBlipTime = 0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // --- Required Lifecycle Overrides ---
    public override void OnPrepareForLine(MarkupParseResult line, TMP_Text text) { }
    public override void OnLineDisplayBegin(MarkupParseResult line, TMP_Text text) { }
    public override void OnLineDisplayComplete() { }
    public override void OnLineWillDismiss() { }

    // --- Our Actual Audio Logic ---
    public override YarnTask OnCharacterWillAppear(int currentCharacterIndex, MarkupParseResult line, CancellationToken cancellationToken)
    {
        if (blipSound != null)
        {
            // 1. Skip spaces so the rhythm breathes naturally
            if (skipSpaces && currentCharacterIndex < line.Text.Length)
            {
                if (char.IsWhiteSpace(line.Text[currentCharacterIndex]))
                {
                    return YarnTask.CompletedTask;
                }
            }

            // 2. The Cooldown Check
            if (Time.time - lastBlipTime >= blipCooldown)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                
                // 3. Stop the current clip immediately. This cuts off any audio tails 
                // and prevents the "machine gun" overlapping distortion.
                audioSource.Stop(); 
                
                audioSource.clip = blipSound;
                audioSource.volume = 0.5f;
                audioSource.Play();

                // 4. Reset the timer
                lastBlipTime = Time.time;
            }
        }
        
        return YarnTask.CompletedTask;
    }
}