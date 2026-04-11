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

    [YarnFunction("get_world_score")]
    public static int GetWorldScore(string worldId)
    {
        if (ChoiceEngine.Instance == null) return -1;
        return ChoiceEngine.Instance.GetWorldScore(worldId);
    }

    // --- RESTORED: Static Method with Hardcoded Scene Names ---
    [YarnCommand("evaluate_ending")]
    public static void EvaluateEnding()
    {
        if (ChoiceEngine.Instance == null)
        {
            Debug.LogError("ChoiceEngine is missing! Cannot evaluate ending.");
            return;
        }

        // 1. Fetch the scores from the Choice Engine
        int hospitalScore = ChoiceEngine.Instance.GetWorldScore("world_hospital");
        int utopiaScore = ChoiceEngine.Instance.GetWorldScore("world_utopia");

        // 2. Print the math to the Console so you can verify it is working
        Debug.Log("=== ENDING EVALUATION TRIGGERED ===");
        Debug.Log($"Hospital Score: {hospitalScore}");
        Debug.Log($"Utopia Score: {utopiaScore}");

        // 3. Evaluate the 3 possible scenarios and load the exact hardcoded scene
        if (hospitalScore == 1 && utopiaScore == 1)
        {
            Debug.Log("Result: Loading GOOD Ending -> ReturnEnd");
            SceneManager.LoadScene("ReturnEnd");
        }
        else if (hospitalScore != utopiaScore)
        {
            Debug.Log("Result: Loading BAD Ending (Return to Hub) -> TrappedEnd");
            SceneManager.LoadScene("TrappedEnd");
        }
        else
        {
            Debug.Log("Result: Loading WORST Ending (Banished) -> CycleEnd");
            SceneManager.LoadScene("CycleEnd");
        }
    }

    // --- RESTORED: Static Method with Hardcoded Hub Name ---
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

        // 3. Teleport back!
        SceneManager.LoadScene(sceneName);
    }
}