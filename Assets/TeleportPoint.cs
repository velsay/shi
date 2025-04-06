using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportPoint : MonoBehaviour
{
    private SceneChanger sceneChanger;

    void Start()
    {
        sceneChanger = SceneChanger.Instance; // Singleton'a erişim
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                sceneChanger.TriggerSceneTransition(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("No next scene in build settings!");
            }
        }
    }
}
