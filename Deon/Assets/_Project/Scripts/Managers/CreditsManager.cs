using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    [Header("Scroll Settings")]
    [Tooltip("The UI container holding all your credits text and images.")]
    public RectTransform creditsContent;
    
    [Tooltip("How fast the credits scroll upwards.")]
    public float scrollSpeed = 75f;

    [Header("End Settings")]
    [Tooltip("The exact Y position where the credits should stop scrolling.")]
    public float endPositionY = 2500f; 

    private bool _isScrolling = true;

    // --- NEW: Force the mouse to appear when the scene loads! ---
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (!_isScrolling) return;

        // Smoothly move the credits content upwards
        creditsContent.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // Stop scrolling once it hits the target position
        if (creditsContent.anchoredPosition.y >= endPositionY)
        {
            _isScrolling = false;
        }
    }

    // This method will be triggered by your always-visible Exit Button
    public void QuitGame()
    {
        Debug.Log("DEON is closing...");
        Application.Quit();
    }
}