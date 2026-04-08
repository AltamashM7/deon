using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class ChoiceEngineYarnCommands : MonoBehaviour
{
    [YarnCommand("record_choice")]
    public static void RecordChoice(string worldId, int score)
    {
        if (ChoiceEngine.Instance == null) return;
        ChoiceEngine.Instance.RecordChoice(worldId, score);
    }

    [YarnCommand("evaluate_ending")]
    public static void EvaluateEnding()
    {
        if (ChoiceEngine.Instance == null) return;
        ChoiceEngine.Instance.EvaluateAndTriggerEnding();
    }

    [YarnFunction("get_world_score")]
    public static int GetWorldScore(string worldId)
    {
        if (ChoiceEngine.Instance == null) return -1;
        return ChoiceEngine.Instance.GetWorldScore(worldId);
    }

    // --- NEW: The Cinematic Extraction ---
    [YarnCommand("extract_to_hub")]
    public static void ExtractToHub()
    {
        if (ChoiceEngine.Instance != null)
        {
            // We use the persistent ChoiceEngine to run the fading coroutine
            ChoiceEngine.Instance.StartCoroutine(FadeAndLoad("HubOverhaul"));
        }
        else
        {
            // Failsafe
            SceneManager.LoadScene("HubOverhaul");
        }
    }

    private static IEnumerator FadeAndLoad(string sceneName)
    {
        // 1. Generate a black overlay purely via code
        GameObject fadeObj = new GameObject("FadeOverlay");
        Canvas canvas = fadeObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // Ensure it renders on top of EVERYTHING

        Image fadeImage = fadeObj.AddComponent<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 0f); // Start completely transparent

        // 2. Smoothly fade it in over 1.5 seconds
        float fadeDuration = 1.5f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0f, 0f, 0f, timer / fadeDuration);
            yield return null;
        }

        // Lock it to solid black
        fadeImage.color = Color.black;

        // Give the player half a second to sit in the darkness
        yield return new WaitForSeconds(0.5f);

        // 3. Teleport back to the Hub!
        SceneManager.LoadScene(sceneName);
    }
}