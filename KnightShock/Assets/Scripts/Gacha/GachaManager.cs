using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

public class GachaManager : MonoBehaviour
{
    public static GachaManager Instance;

    public List<BannerData> BannerDatas { get; private set; }

    public List<int> RollHistory { get; private set; }

    [SerializeField] private int srCeil = 10;
    [SerializeField] private int ssrAdventageStart = 50; //Ȯ�� ����
    [SerializeField] private int ssrCeil = 70; //ssrõ���ġ
    [SerializeField] private float ssrChanceIncrease = 1.5f;

    private int totalCount = 0;
    private int currentSSRCount = 0; // ������ SSR ���� ���� ���� �̱� ��
    private int currentSRCount = 0; // ������ SR �Ǵ� SSR ���� ���� ���� �̱� ��
    private bool pickupForce = false;  // ���� SSR�� �Ⱦ��� �ƴϾ��� ��� ���� SSR�� ������ �Ⱦ� ó��
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

        BannerDatas = new List<BannerData>();
        RollHistory = new List<int>();
    }

    /// <summary>
    /// ��í Ȯ�� ���
    /// </summary>
    /// <returns>���(ĳ����id) ��ȯ</returns>
    public List<int> StartGacha(BannerContainer _container, int _count = 1)
    {
        List<int> results = new List<int>();
        List<GachaResult> logs = new List<GachaResult>();

        for (int i = 0; i < _count; i++)
        {
            //todo: �ð��߰�
            string currentTime = "";
            // �� �̱⸶�� pity ī���� ����
            totalCount++;
            currentSSRCount++;
            currentSRCount++;

            // ���� õ�� ����
            bool forcedSSR = (currentSSRCount >= ssrCeil);
            bool forcedSR = (currentSRCount >= srCeil);

            // SRȮ�� �켱 ����
            if (forcedSR)
            {
                int srResult = GetSRResult(_container);
                results.Add(srResult);
                currentSRCount = 0;
                logs.Add(new GachaResult(srResult, totalCount, currentSRCount, currentSSRCount, pickupForce, currentTime));
                continue;
            }

            // SSR Ȯ�� ����
            if (forcedSSR)
            {
                int ssrResult = GetSSRResult(ref pickupForce, _container);
                results.Add(ssrResult);
                currentSSRCount = 0;
                currentSRCount = 0;
                logs.Add(new GachaResult(ssrResult, totalCount, currentSRCount, currentSSRCount, pickupForce, currentTime));
                continue;
            }

            // ssrȮ�� ���� ���
            float SSRChance = _container.Data.SSR_Percent;
            if (currentSSRCount >= ssrAdventageStart)
            {
                SSRChance += (currentSSRCount - ssrAdventageStart - 1) * ssrChanceIncrease;
            }

            // SSR ����
            float roll = Random.Range(0f, 100f);
            if (roll < SSRChance)
            {
                int ssrResult = GetSSRResult(ref pickupForce, _container);
                results.Add(ssrResult);
                currentSSRCount = 0;
                currentSRCount = 0;
                logs.Add(new GachaResult(ssrResult, totalCount, currentSRCount, currentSSRCount, pickupForce, currentTime));
            }
            else
            {
                // SSR ���� �� SR ����
                float srRoll = Random.Range(0f, 100f);
                if (srRoll < _container.Data.SR_Percent)
                {
                    int srResult = GetSRResult(_container);
                    results.Add(srResult);
                    currentSRCount = 0;
                    logs.Add(new GachaResult(srResult, totalCount, currentSRCount, currentSSRCount, pickupForce, currentTime));
                }
                else
                {
                    // �������� R ��� ó�� (�⺻�� 0)
                    int rResult = GetRResult();
                    results.Add(rResult);
                    logs.Add(new GachaResult(rResult, totalCount, currentSRCount, currentSSRCount, pickupForce, currentTime));
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
        Task task = AuthManager.Instance.SetDataAsync(Request.writegachalog, new GachaResultWrapper(logs));
        task.ContinueWith(task =>
        {
            Debug.LogWarning("��í���� ���� ����, 1ȸ ��õ��մϴ�.");
        }, TaskContinuationOptions.OnlyOnFaulted);

        //��� �÷��̾�ĳ���� ������ ���� ������� ������Ʈ

        return results;
    }

    /// <summary>
    /// _pickupForce �÷��װ� Ȱ��ȭ�Ǿ� �ִٸ� ������ �Ⱦ� SSR�� ����,
    /// or 50% Ȯ���� �Ⱦ� SSR, �ƴ� ��� ���� SSR�� ������ �Ⱦ� SSR�� �ǵ��� �÷��� ����
    /// </summary>
    private int GetSSRResult(ref bool _pickupForce, BannerContainer _container)
    {
        if (_pickupForce)
        {
            int result = PickFromList(_container.Data.SSR_PickupList, -1);
            _pickupForce = false;
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
                _pickupForce = true;
                return PickFromList(CharacterManager.GetCharactersFromRare(Rare.SSR), -1);
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
    /// �Ⱦ� ����
    /// </summary>
    private int PickFromList(IReadOnlyList<int> list, int defaultValue)
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
    private int GetRResult()
    {
        int idx = Random.Range(0, CharacterManager.GetRareCharacterCount(Rare.R));

        return CharacterManager.GetCharactersFromRare(Rare.R)[idx];
    }

    // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public async Task InitBannerDatas()
    {
        BannerWrapper wrapper = await AuthManager.Instance.GetDataAsync<BannerWrapper>(Request.banners);
        BannerDatas = (wrapper).banners;
    }

    public async Task InitCount()
    {
        //db�� ���� ������� ������
        GachaResultWrapper result = await AuthManager.Instance.GetDataAsync<GachaResultWrapper>(Request.writegachalog);
        GachaResult latest = result.GachaResultList[result.GachaResultList.Count - 1];
        totalCount = latest.TotalCount;
        currentSRCount = latest.SRCurrentCount;
        currentSSRCount = latest.SSRCurrentCount;
        pickupForce = latest.PickupForce;
        Debug.Log($"�ε�� ���� ���\ntotal:{totalCount}, sr:{currentSRCount}, ssr:{currentSSRCount}, pickupForce:{pickupForce}");
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
