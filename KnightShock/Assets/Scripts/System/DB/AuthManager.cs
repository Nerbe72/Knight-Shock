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
        Debug.LogWarning("<color=orange>토큰이 변경되었습니다</color>");
    }

    public async Task<UnityWebRequest> SendAuthorizedRequest(string url)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // 토큰이 있을 경우 Authorization 헤더에 추가
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
    /// 공통 데이터 로드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_type"></param>
    /// <returns></returns>
    public async Task<T> GetDataAsync<T>(Request _type)
    {
        UnityWebRequest request = await SendAuthorizedRequest("http://localhost:3000/" + _type.ToString());

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("서버 연결 실패: " + request.error);
            return default;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("공용 데이터 요청 실패: " + request.error);
            return default;
        }

        string responseText = request.downloadHandler.text;
        T data = JsonUtility.FromJson<T>(responseText);
        return data;
    }

    /// <summary>
    /// 유저 데이터 관련 로드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_fileName"></param>
    /// <returns></returns>
    public async Task<T> GetUserDataAsync<T>(string _fileName) where T : new ()
    {
        UnityWebRequest request = await SendAuthorizedRequest("http://localhost:3000/" + _fileName);

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("서버 연결 실패: " + request.error);
            return default;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("유저 데이터 요청 실패: " + request.error);

            if (request.responseCode == 404)
            {
                Debug.LogWarning("유저 데이터를 찾을 수 없어 새로 작성합니다.");
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
    //        Debug.LogError("API 요청 실패: " + request.error);
    //        return default;
    //    }

    //    string responseText = request.downloadHandler.text;
    //    T data = JsonUtility.FromJson<T>(responseText);
    //    return data;
    //}
}
