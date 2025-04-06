using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;  // Singleton instance
    public GameObject fadePanel;   // UI Fade panel
    public float fadeDuration = 1.5f;

    private void Awake()
    {
        // Singleton kontrolü
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Sahne değişiminde yok olmasın
        }
        else
        {
            Destroy(gameObject);  // Zaten instance varsa bu objeyi yok et
        }
    }

    public void TriggerSceneTransition(int sceneInt)
    {
        StartCoroutine(FadeAndChangeScene(sceneInt));
    }

    IEnumerator FadeAndChangeScene(int sceneInt)
    {
        if (fadePanel != null)
        {
            CanvasGroup canvasGroup = fadePanel.GetComponent<CanvasGroup>();
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
                yield return null;
            }
        }

        SceneManager.LoadScene(sceneInt);  // Sahne ismini geçer
    }
}
