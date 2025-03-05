using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static string DefaultImagePath = Path.Combine(Application.dataPath, "Resources", "STANDING");

    public int UID { get; private set; }
    public string UserName { get; private set; }

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
        UserName = "";
    }

    public void SetUID(int _id)
    {
        Debug.LogWarning($"�÷��̾� ID�� �����Ǿ����ϴ� <color=green>{UID} -> {_id}</color>");
        UID = _id;
    }

    public void SetUserName(string _name)
    {
        Debug.LogWarning($"�÷��̾� �̸��� �����Ǿ����ϴ� <color=green>{UserName} -> {_name}</color>");
        UserName = _name;
    }
}
