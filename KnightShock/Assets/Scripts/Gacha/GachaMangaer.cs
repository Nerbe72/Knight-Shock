using NUnit.Framework;
using System;
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

    public List<int> roll_history { get; private set; }
    public List<int> roll_result { get; private set; }

    [Serializable]
    public class GachaCharacter
    {
        public string Name;
        public Rare Rarity;
        public bool IsPickup;
        public float BaseProbability;
    }

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
        roll_history = new List<int>();
    }


    [SerializeField] private List<GachaCharacter> gacha_SSR_Pool;
    [SerializeField] private List<GachaCharacter> gacha_SR_Pool;
    [SerializeField] private List<GachaCharacter> gacha_R_Pool;
    [SerializeField] private int srPity = 10;
    [SerializeField] private int ssrHalfAdventageStartCut = 40; //Ȯ�� ����
    [SerializeField] private int ssrHalfPity = 60; //ssrõ���ġ
    [SerializeField] private bool isSSRFullPity = false; //�Ⱦ� õ������
    [SerializeField] private float ssrProbabilityIncrease = 1.5f;

    /// <summary>
    /// ��í Ȯ�� ���
    /// </summary>
    /// <returns>���(ĳ����id) ��ȯ</returns>
    public void StartGacha(BannerContainer _container, int _count = 1)
    {
        // ��í ����� ������ ����Ʈ�Դϴ�.
        List<int> results = new List<int>();

        // �̱� ���´� �ش� ȣ�� �������� �����մϴ�.
        int currentSSRCount = 0;   // ������ SSR ���� ���� ���� �̱� ��
        int currentSRCount = 0;    // ������ SR �Ǵ� SSR ���� ���� ���� �̱� ��
        bool forcePickupSSR = false;  // ���� SSR�� �Ⱦ��� �ƴϾ��� ��� ���� SSR�� ������ �Ⱦ� ó��

        for (int i = 0; i < _count; i++)
        {
            // �� �̱⸶�� pity ī���� ����
            currentSSRCount++;
            currentSRCount++;

            // ���� õ�� ����
            bool forcedSSR = (currentSSRCount >= 60);
            bool forcedSR = (currentSRCount >= 10);

            // ���� �� õ���� ���ÿ� �ߵ��ϸ� SR�� �켱 ����˴ϴ�.
            if (forcedSSR && forcedSR)
            {
                int srResult = DrawSR(_container);
                results.Add(srResult);
                currentSRCount = 0;  // SR pity�� �ʱ�ȭ
                                     // SSR pity�� �״�� �����Ǿ� ���� �̱⿡ �ݿ��˴ϴ�.
                continue;
            }

            // SR õ�� �켱 ó��
            if (forcedSR)
            {
                int srResult = DrawSR(_container);
                results.Add(srResult);
                currentSRCount = 0;
                continue;
            }

            // SSR õ�� ó��
            if (forcedSSR)
            {
                int ssrResult = GetSSRResult(ref forcePickupSSR, _container);
                results.Add(ssrResult);
                currentSSRCount = 0;
                currentSRCount = 0;
                continue;
            }

            // �Ϲ� �̱�: �⺻ SSR Ȯ���� 40ȸ �̻���� �߰� Ȯ���� �����˴ϴ�.
            float effectiveSSRChance = _container.Data.SSR_Percent;
            if (currentSSRCount >= 40)
            {
                effectiveSSRChance += (currentSSRCount - 39) * 1.5f;
            }

            // SSR ����
            float roll = UnityEngine.Random.Range(0f, 100f);
            if (roll < effectiveSSRChance)
            {
                int ssrResult = GetSSRResult(ref forcePickupSSR, _container);
                results.Add(ssrResult);
                currentSSRCount = 0;
                currentSRCount = 0;
            }
            else
            {
                // SSR ���� �� SR ����
                float srRoll = UnityEngine.Random.Range(0f, 100f);
                if (srRoll < _container.Data.SR_Percent)
                {
                    int srResult = DrawSR(_container);
                    results.Add(srResult);
                    currentSRCount = 0;
                }
                else
                {
                    // �������� R ��� ó�� (���⼭�� �⺻�� 0 ���)
                    results.Add(0);
                }
            }
        }

        // ��� ��� (����� �α�)
        for (int i = 0; i < results.Count; i++)
        {
            Debug.Log(string.Format("�̱� ��� {0}: {1}", i + 1, results[i]));
        }
    }

    /// <summary>
    /// SSR ����� �����մϴ�.
    /// forcePickupSSR �÷��װ� Ȱ��ȭ�Ǿ� �ִٸ� ������ �Ⱦ� SSR�� �����ϸ�,
    /// �׷��� ���� ��� 50% Ȯ���� �Ⱦ� SSR, �ƴ� ��� ���� SSR�� ������ �Ⱦ� SSR�� �ǵ��� �մϴ�.
    /// </summary>
    private int GetSSRResult(ref bool forcePickupSSR, BannerContainer _container)
    {
        if (forcePickupSSR)
        {
            int result = PickFromList(_container.Data.SSR_PickupList, -1);
            forcePickupSSR = false;
            return result;
        }
        else
        {
            float pickupRoll = UnityEngine.Random.Range(0f, 100f);
            if (pickupRoll < 50f)
            {
                return PickFromList(_container.Data.SSR_PickupList, -1);
            }
            else
            {
                // �Ⱦ��� �ƴ� ��� ���� SSR�� ������ �Ⱦ� SSR�� �ǵ��� �÷��� ����
                forcePickupSSR = true;
                return PickFromList(_container.Data.SSR_PickupList, -1);
            }
        }
    }

    /// <summary>
    /// SR ����� �����մϴ�.
    /// SR �Ⱦ� ����Ʈ�� �ִٸ� ������ ����, ������ �⺻��(-2) ��ȯ.
    /// </summary>
    private int DrawSR(BannerContainer _container)
    {
        return PickFromList(_container.Data.SR_PickupList, -2);
    }

    /// <summary>
    /// �־��� ����Ʈ���� �������� �������� �����մϴ�.
    /// ����Ʈ�� ������� ��� defaultValue�� ��ȯ�մϴ�.
    /// </summary>
    private int PickFromList(List<int> list, int defaultValue)
    {
        if (list != null && list.Count > 0)
        {
            int idx = UnityEngine.Random.Range(0, list.Count);
            return list[idx];
        }
        return defaultValue;
    }


    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


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
