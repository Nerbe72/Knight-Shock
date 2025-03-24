using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

public class GachaManager : MonoBehaviour
{
    public static GachaManager Instance;

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

    [SerializeField] private int srCeil = 10;
    [SerializeField] private int ssrHalfAdventageStart = 40; //Ȯ�� ����
    [SerializeField] private int ssrHalfCeil = 60; //ssrõ���ġ
    [SerializeField] private float ssrProbabilityIncrease = 1.5f;

    private int currentSSRCount = 0; // ������ SSR ���� ���� ���� �̱� ��
    private int currentSRCount = 0; // ������ SR �Ǵ� SSR ���� ���� ���� �̱� ��
    private bool forcePickupSSR = false;  // ���� SSR�� �Ⱦ��� �ƴϾ��� ��� ���� SSR�� ������ �Ⱦ� ó��

    private List<int> fullResult = new List<int>();


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

        bannerDatas = new List<BannerData>();
        roll_history = new List<int>();
    }

    /// <summary>
    /// ��í Ȯ�� ���
    /// </summary>
    /// <returns>���(ĳ����id) ��ȯ</returns>
    public List<int> StartGacha(BannerContainer _container, int _count = 1)
    {
        // ��í ����� ������ ����Ʈ�Դϴ�.
        List<int> results = new List<int>();

        for (int i = 0; i < _count; i++)
        {
            // �� �̱⸶�� pity ī���� ����
            currentSSRCount++;
            currentSRCount++;

            // ���� õ�� ����
            bool forcedSSR = (currentSSRCount >= ssrHalfCeil);
            bool forcedSR = (currentSRCount >= srCeil);

            // ���� �� õ���� ���ÿ� �ߵ��ϸ� SR�� �켱 ����˴ϴ�.
            if (forcedSSR && forcedSR)
            {
                int srResult = GetSRResult(_container);
                results.Add(srResult);
                fullResult.Add(srResult);
                currentSRCount = 0;

                continue;
            }

            // SR õ�� �켱 ó��
            if (forcedSR)
            {
                int srResult = GetSRResult(_container);
                results.Add(srResult);
                fullResult.Add(srResult);
                currentSRCount = 0;
                continue;
            }

            // SSR õ�� ó��
            if (forcedSSR)
            {
                int ssrResult = GetSSRResult(ref forcePickupSSR, _container);
                results.Add(ssrResult);
                fullResult.Add(ssrResult);
                currentSSRCount = 0;
                currentSRCount = 0;
                continue;
            }

            // �Ϲ� �̱�: �⺻ SSR Ȯ���� 40ȸ �̻���� �߰� Ȯ���� �����˴ϴ�.
            float effectiveSSRChance = _container.Data.SSR_Percent;
            if (currentSSRCount >= ssrHalfAdventageStart)
            {
                effectiveSSRChance += (currentSSRCount - 39) * ssrProbabilityIncrease;
            }

            // SSR ����
            float roll = UnityEngine.Random.Range(0f, 100f);
            if (roll < effectiveSSRChance)
            {
                int ssrResult = GetSSRResult(ref forcePickupSSR, _container);
                results.Add(ssrResult);
                fullResult.Add(ssrResult);
                currentSSRCount = 0;
                currentSRCount = 0;
            }
            else
            {
                // SSR ���� �� SR ����
                float srRoll = Random.Range(0f, 100f);
                if (srRoll < _container.Data.SR_Percent)
                {
                    int srResult = GetSRResult(_container);
                    results.Add(srResult);
                    fullResult.Add(srResult);
                    currentSRCount = 0;
                }
                else
                {
                    // �������� R ��� ó�� (�⺻�� 0)
                    int rResult = GetRResult();
                    results.Add(rResult);
                    fullResult.Add(rResult);
                }
            }
        }

        // ��� ��� (����� �α�)
        for (int i = 0; i < results.Count; i++)
        {
            Debug.Log(string.Format("�̱� ��� {0}: {1}", i + 1, results[i]));
        }
        Debug.Log("SSR ����: " + currentSSRCount);

        //��� ����

        return results;
    }

    /// <summary>
    /// forcePickupSSR �÷��װ� Ȱ��ȭ�Ǿ� �ִٸ� ������ �Ⱦ� SSR�� �����ϸ�,
    /// �׷��� ���� ��� 50% Ȯ���� �Ⱦ� SSR, �ƴ� ��� ���� SSR�� ������ �Ⱦ� SSR�� �ǵ��� ��.
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
            float pickupRoll = Random.Range(0f, 100f);
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
    /// SR ��� ��ȯ
    /// SR �Ⱦ� ����Ʈ�� �ִٸ� ������ ����, ������ �⺻��(-2) ��ȯ.
    /// </summary>
    private int GetSRResult(BannerContainer _container)
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
            int idx = Random.Range(0, list.Count);
            return list[idx];
        }
        return defaultValue;
    }

    /// <summary>
    /// R ��� ��ȯ
    /// </summary>
    /// <returns></returns>
    private int GetRResult()
    {
        int idx = Random.Range(0, CharacterManager.GetRareCharacterCount(Rare.R));

        return CharacterManager.GetCharactersFromRare(Rare.R)[idx];
    }

    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public async Task InitBannerDatas()
    {
        BannerWrapper wrapper = await AuthManager.Instance.GetDataAsync<BannerWrapper>(Request.banners);
        bannerDatas = (wrapper).banners;
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
