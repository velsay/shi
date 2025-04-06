using UnityEngine;
using UnityEngine.UI;

public class Fill : MonoBehaviour
{
    public static Fill Instance { get; private set; }

    public Image fillImage;

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); // Optional: destroy duplicate
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

    }

    void Start()
    {
        // Cache the Image component from the nested child
        
    }


    public void ResetFill()
    {

            fillImage.fillAmount = 1f;
        

    }
}
