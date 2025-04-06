using UnityEngine;

public class PlayerRespawnManager : MonoBehaviour
{
    public static PlayerRespawnManager Instance;

    private Vector3 lastCheckpointPos;
    private GameObject player;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lastCheckpointPos = player.transform.position; // Başlangıç pozisyonu
    }

    public void SetCheckpoint(Vector3 pos)
    {
        lastCheckpointPos = pos;
    }

    public void RespawnPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = lastCheckpointPos;
        Fill.Instance.ResetFill();
        
        IsoCharacterController2D playerScript = player.GetComponent<IsoCharacterController2D>();
        playerScript.RestoreHealth(); // Canı sıfırsa geri doldur
    }
}
