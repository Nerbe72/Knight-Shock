using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static string DefaultImagePath = Path.Combine(Application.dataPath, "Resources", "STANDING");

    public int UID { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        UID = -1;
    }

    public void SetUID(int _id)
    {
        Debug.LogWarning($"플레이어 ID가 수정되었습니다 {UID} -> {_id}");
        UID = _id;
    }
}
