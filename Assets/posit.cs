using UnityEngine;

public class Posit : MonoBehaviour
{
    // Sahne geçişi sonrası pozisyonu değiştirme
    void Start()
    {
        if (IsoCharacterController2D.instance != null)
        {
            // Player'ın pozisyonunu değiştirme
            IsoCharacterController2D.instance.transform.position = new Vector3(-16, 4, 0);
        }
        else
        {
            Debug.LogError("IsoCharacterController2D instance bulunamadı!");
        }
    }
}
