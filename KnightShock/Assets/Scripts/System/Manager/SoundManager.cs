using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;   
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
