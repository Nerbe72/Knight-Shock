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
        Debug.LogWarning($"�÷��̾� ID�� �����Ǿ����ϴ� {UID} -> {_id}");
        UID = _id;
    }
}
