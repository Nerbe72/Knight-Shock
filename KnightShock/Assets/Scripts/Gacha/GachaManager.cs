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
    [SerializeField] private int ssrHalfAdventageStart = 40; //확업 시작
    [SerializeField] private int ssrHalfCeil = 60; //ssr천장수치
    [SerializeField] private float ssrProbabilityIncrease = 1.5f;

    private int currentSSRCount = 0; // 마지막 SSR 등장 이후 누적 뽑기 수
    private int currentSRCount = 0; // 마지막 SR 또는 SSR 등장 이후 누적 뽑기 수
    private bool forcePickupSSR = false;  // 이전 SSR이 픽업이 아니었을 경우 다음 SSR은 무조건 픽업 처리

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
    /// 가챠 확률 계산
    /// </summary>
    /// <returns>결과(캐릭터id) 반환</returns>
    public List<int> StartGacha(BannerContainer _container, int _count = 1)
    {
        // 가챠 결과를 저장할 리스트입니다.
        List<int> results = new List<int>();

        for (int i = 0; i < _count; i++)
        {
            // 각 뽑기마다 pity 카운터 증가
            currentSSRCount++;
            currentSRCount++;

            // 강제 천장 조건
            bool forcedSSR = (currentSSRCount >= ssrHalfCeil);
            bool forcedSR = (currentSRCount >= srCeil);

            // 만약 두 천장이 동시에 발동하면 SR이 우선 적용됩니다.
            if (forcedSSR && forcedSR)
            {
                int srResult = GetSRResult(_container);
                results.Add(srResult);
                fullResult.Add(srResult);
                currentSRCount = 0;

                continue;
            }

            // SR 천장 우선 처리
            if (forcedSR)
            {
                int srResult = GetSRResult(_container);
                results.Add(srResult);
                fullResult.Add(srResult);
                currentSRCount = 0;
                continue;
            }

            // SSR 천장 처리
            if (forcedSSR)
            {
                int ssrResult = GetSSRResult(ref forcePickupSSR, _container);
                results.Add(ssrResult);
                fullResult.Add(ssrResult);
                currentSSRCount = 0;
                currentSRCount = 0;
                continue;
            }

            // 일반 뽑기: 기본 SSR 확률에 40회 이상부터 추가 확률이 누적됩니다.
            float effectiveSSRChance = _container.Data.SSR_Percent;
            if (currentSSRCount >= ssrHalfAdventageStart)
            {
                effectiveSSRChance += (currentSSRCount - 39) * ssrProbabilityIncrease;
            }

            // SSR 판정
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
                // SSR 실패 시 SR 판정
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
                    // 나머지는 R 등급 처리 (기본값 0)
                    int rResult = GetRResult();
                    results.Add(rResult);
                    fullResult.Add(rResult);
                }
            }
        }

        // 결과 출력 (디버그 로그)
        for (int i = 0; i < results.Count; i++)
        {
            Debug.Log(string.Format("뽑기 결과 {0}: {1}", i + 1, results[i]));
        }
        Debug.Log("SSR 스택: " + currentSSRCount);

        //결과 저장

        return results;
    }

    /// <summary>
    /// forcePickupSSR 플래그가 활성화되어 있다면 무조건 픽업 SSR을 선택하며,
    /// 그렇지 않을 경우 50% 확률로 픽업 SSR, 아닐 경우 다음 SSR은 무조건 픽업 SSR이 되도록 함.
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
                // 픽업이 아닌 경우 다음 SSR은 무조건 픽업 SSR이 되도록 플래그 설정
                forcePickupSSR = true;
                return PickFromList(_container.Data.SSR_PickupList, -1);
            }
        }
    }

    /// <summary>
    /// SR 결과 반환
    /// SR 픽업 리스트가 있다면 무작위 선택, 없으면 기본값(-2) 반환.
    /// </summary>
    private int GetSRResult(BannerContainer _container)
    {
        return PickFromList(_container.Data.SR_PickupList, -2);
    }

    /// <summary>
    /// 주어진 리스트에서 무작위로 아이템을 선택합니다.
    /// 리스트가 비어있을 경우 defaultValue를 반환합니다.
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
    /// R 결과 반환
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
        await handle.Task;  // Addressables 작업이 완료될 때까지 대기

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return handle.Result;
        }
        else
        {
            Debug.LogError($"Sprite 로딩 실패: {_path}");
            return null;
        }
    }

}
