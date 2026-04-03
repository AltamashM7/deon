using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractiveExit : MonoBehaviour
{
    [Header("Extraction Settings")]
    [Tooltip("The exact Scene name of the 3D Hub to return to")]
    public string hubSceneName = "3D_Main_Hub";

    [Header("Visuals")]
    [Tooltip("Optional: A 'Press Z to Extract' UI prompt")]
    public GameObject interactPrompt;

    private bool _playerInRange;

    private void Start()
    {
        // Ensure this acts as a detection zone
        GetComponent<BoxCollider2D>().isTrigger = true;
        
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    private void Update()
    {
        // If the player isn't close enough, stop running this code
        if (!_playerInRange) return;

        // If they are close, listen for the interact keys!
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.A))
        {
            ExtractPlayer();
        }
    }

    private void ExtractPlayer()
    {
        Debug.Log("Extracting player back to Hub...");
        SceneManager.LoadScene(hubSceneName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerInRange = true;
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerInRange = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }
}