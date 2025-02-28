using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Windows;
using static UnityEditor.Progress;

public class GachaMangaer : MonoBehaviour
{
    public static GachaMangaer Instance;
    public List<BannerData> bannerDatas { get; private set; }

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

        bannerDatas = new List<BannerData>();
    }

    /// <summary>
    /// ��í Ȯ�� ���
    /// </summary>
    /// <returns>���(ĳ����id) ��ȯ</returns>
    public int StartGacha()
    {
        //���� �⵵, ��, ��, �ð�, ƽ, �и���
        int seed = (int)System.DateTime.Now.Ticks + Random.Range(0, System.DateTime.Now.Year) + System.DateTime.Now.Month + System.DateTime.Now.Day + System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Millisecond;
        Random.InitState(seed);

        int random = Random.Range(0, 101);
        //R
        Rare selectedRare = Rare.R;
        if (random > 80 && random <= 99) selectedRare = Rare.SR;
        if (random > 99) selectedRare = Rare.SSR;

        int inner = Random.Range(0, CharacterManager.GetRareCharacterCount(selectedRare));

        int selectedId = CharacterManager.GetRandomCharacter(selectedRare, inner);

        return selectedId;
    }

    public async Task InitBannerDatas()
    {
        BannerWrapper wrapper = await AuthManager.Instance.GetDataAsync<BannerWrapper>(Request.banners);
        bannerDatas = (wrapper).banners;
        Debug.Log(bannerDatas.Count);
    }

    public async Task<Sprite> LoadSprite(string _path)
    {
        var handle = Addressables.LoadAssetAsync<Sprite>(_path);
        await handle.Task;  // Addressables �۾��� �Ϸ�� ������ ���

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return handle.Result;
        }
        else
        {
            Debug.LogError($"Sprite �ε� ����: {_path}");
            return null;
        }
    }

}
