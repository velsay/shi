using UnityEngine;

public class PlayerRespawnManager : MonoBehaviour
{
    public static PlayerRespawnManager Instance;
    private GameObject player;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        player = IsoCharacterController2D.instance.gameObject;
    }

    public void RespawnPlayer()
    {
        Vector3 respawnPosition = CheckpointManager.Instance.GetLastCheckpoint();
        player.transform.position = respawnPosition;

        player.GetComponent<IsoCharacterController2D>().RestoreHealth();
    }
}
