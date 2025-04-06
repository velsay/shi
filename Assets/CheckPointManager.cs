using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Vector3 lastCheckpointPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahneler arası kaybolmasın
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        Debug.Log("Checkpoint kaydedildi: " + position);
    }

    public Vector3 GetLastCheckpoint()
    {
        return lastCheckpointPosition;
    }
}
