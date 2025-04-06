using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IsoCharacterController2D playerController = other.GetComponent<IsoCharacterController2D>();
            if (playerController != null)
            {
                playerController.SetRespawnPoint(transform.position);
            }
        }
    }
}
