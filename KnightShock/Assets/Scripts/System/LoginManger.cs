using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static UnityEngine.Networking.UnityWebRequest;

public class LoginManger : MonoBehaviour
{
    private const string loginUrl = "http://localhost:3000/login";

    public async static Task LoginAsync(string _username, string _password)
    {
        LoginRequestData data = new LoginRequestData { username = _username, password = _password };
        string jsonData = JsonUtility.ToJson(data);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        //POST
        UnityWebRequest request = new UnityWebRequest(loginUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        Debug.Log("�α������Դϴ�...");

        while (!operation.isDone)
            await Task.Yield();

        if (request.result != Result.Success)
        {
            Debug.Log("�α��� ��û ���� " + request.error);
        }
        else
        {
            Debug.Log("�α��� ��û ���� " + request.downloadHandler.text);
            string loginData = request.downloadHandler.text;
            LoginAnswerData answerData = JsonUtility.FromJson<LoginAnswerData>(loginData);

            if (!answerData.success)
            {
                Debug.Log("�α��� ����");
            }
            else
            {
                Debug.Log("<color=green>�α��� �Ϸ�!</color>");
                GameManager.Instance.SetUID(answerData.userId);
                AuthManager.Instance.SetToken(answerData.token);
                SceneManager.LoadScene("Lobby");
            }
        }
    }

    [Serializable]
    public class LoginRequestData
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class LoginAnswerData
    {
        public bool success;
        public string token;
        public int userId;
    }
}
