using System;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;

    private static string token;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
            return;
        }

        token = "";
    }

    public void SetToken(string _token)
    {
        token = _token;
        Debug.LogWarning("<color=orange>��ū�� ����Ǿ����ϴ�</color>");
    }

    public async Task<UnityWebRequest> SendAuthorizedRequest(string url)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // ��ū�� ���� ��� Authorization ����� �߰�
        if (!string.IsNullOrEmpty(token))
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);
        }

        request.downloadHandler = new DownloadHandlerBuffer();

        UnityWebRequestAsyncOperation operation = request.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        return request;
    }

    /// <summary>
    /// ���� ������ �ε�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_type"></param>
    /// <returns></returns>
    public async Task<T> GetDataAsync<T>(Request _type)
    {
        UnityWebRequest request = await SendAuthorizedRequest("http://localhost:3000/" + _type.ToString());

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("���� ���� ����: " + request.error);
            return default;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("���� ������ ��û ����: " + request.error);
            return default;
        }

        string responseText = request.downloadHandler.text;
        T data = JsonUtility.FromJson<T>(responseText);
        return data;
    }

    /// <summary>
    /// ���� ������ ���� �ε�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_fileName"></param>
    /// <returns></returns>
    public async Task<T> GetUserDataAsync<T>(string _fileName) where T : new ()
    {
        UnityWebRequest request = await SendAuthorizedRequest("http://localhost:3000/" + _fileName);

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("���� ���� ����: " + request.error);
            return default;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("���� ������ ��û ����: " + request.error);

            if (request.responseCode == 404)
            {
                Debug.LogWarning("���� �����͸� ã�� �� ���� ���� �ۼ��մϴ�.");
                T newData = new T();
                return newData;
            }

            return default;
        }

        string responseText = request.downloadHandler.text;
        T data = JsonUtility.FromJson<T>(responseText);
        return data;
    }

    public async Task<T> SetUerDataAsync<T>(string _fileName)
    {


        return default;
    }

    //public async Task<UnityWebRequest> SendAuthedWriteRequest(string _url, string jsonData = null)
    //{
    //    UnityWebRequest request = new UnityWebRequest(_url, "POST");

    //    if (!string.IsNullOrEmpty(jsonData))
    //    {
    //        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
    //        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //        request.SetRequestHeader("Content-Type", "application/json");
    //    }

    //    return request;
    //}

    //public async Task<T> SetDataAsync<T>(Request _type, string _jsonData = null)
    //{
    //    UnityWebRequest request = await SendAuthedWriteRequest("http://localhost:3000/" + _type.ToString());

    //    if (request.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.LogError("API ��û ����: " + request.error);
    //        return default;
    //    }

    //    string responseText = request.downloadHandler.text;
    //    T data = JsonUtility.FromJson<T>(responseText);
    //    return data;
    //}
}
