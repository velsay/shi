using UnityEngine;

public class TeleportPoint : MonoBehaviour
{
    private SceneChanger sceneChanger;

    void Start()
    {
        sceneChanger = SceneChanger.Instance;  // Singleton'a erişim
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Burada geçiş yapılacak sahnenin ismini verin, örneğin "dungeon2"
            sceneChanger.TriggerSceneTransition("dungeon 2");
        }
    }
}
