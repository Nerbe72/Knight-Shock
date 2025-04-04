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
    [SerializeField] private int ssrAdventageStart = 50; //확업 시작
    [SerializeField] private int ssrCeil = 70; //ssr천장수치
    [SerializeField] private float ssrChanceIncrease = 1.5f;

    private int totalCount = 0;
    private int currentSSRCount = 0; // 마지막 SSR 등장 이후 누적 뽑기 수
    private int currentSRCount = 0; // 마지막 SR 또는 SSR 등장 이후 누적 뽑기 수
    private bool pickupForce = false;  // 이전 SSR이 픽업이 아니었을 경우 다음 SSR은 무조건 픽업 처리
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
    /// 가챠 확률 계산
    /// </summary>
    /// <returns>결과(캐릭터id) 반환</returns>
    public List<int> StartGacha(BannerContainer _container, int _count = 1)
    {
        List<int> results = new List<int>();
        List<GachaResult> logs = new List<GachaResult>();

        for (int i = 0; i < _count; i++)
        {
            //todo: 시간추가
            string currentTime = "";
            // 각 뽑기마다 pity 카운터 증가
            totalCount++;
            currentSSRCount++;
            currentSRCount++;

            // 강제 천장 조건
            bool forcedSSR = (currentSSRCount >= ssrCeil);
            bool forcedSR = (currentSRCount >= srCeil);

            // SR확정 우선 적용
            if (forcedSR)
            {
                int srResult = GetSRResult(_container);
                results.Add(srResult);
                currentSRCount = 0;
                logs.Add(new GachaResult(srResult, totalCount, currentSRCount, currentSSRCount, pickupForce, currentTime));
                continue;
            }

            // SSR 확정 적용
            if (forcedSSR)
            {
                int ssrResult = GetSSRResult(ref pickupForce, _container);
                results.Add(ssrResult);
                currentSSRCount = 0;
                currentSRCount = 0;
                logs.Add(new GachaResult(ssrResult, totalCount, currentSRCount, currentSSRCount, pickupForce, currentTime));
                continue;
            }

            // ssr확률 증가 계산
            float SSRChance = _container.Data.SSR_Percent;
            if (currentSSRCount >= ssrAdventageStart)
            {
                SSRChance += (currentSSRCount - ssrAdventageStart - 1) * ssrChanceIncrease;
            }

            // SSR 판정
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
                // SSR 실패 시 SR 판정
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
                    // 나머지는 R 등급 처리 (기본값 0)
                    int rResult = GetRResult();
                    results.Add(rResult);
                    logs.Add(new GachaResult(rResult, totalCount, currentSRCount, currentSSRCount, pickupForce, currentTime));
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
        Task task = AuthManager.Instance.SetDataAsync(Request.writegachalog, new GachaResultWrapper(logs));
        task.ContinueWith(task =>
        {
            Debug.LogWarning("가챠정보 저장 실패, 1회 재시도합니다.");
        }, TaskContinuationOptions.OnlyOnFaulted);

        //결과 플레이어캐릭터 정보로 전달 보유목록 업데이트

        return results;
    }

    /// <summary>
    /// _pickupForce 플래그가 활성화되어 있다면 무조건 픽업 SSR을 선택,
    /// or 50% 확률로 픽업 SSR, 아닐 경우 다음 SSR은 무조건 픽업 SSR이 되도록 플래그 설정
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
                // 픽업이 아닌 경우 다음 SSR은 무조건 픽업 SSR이 되도록 플래그 설정
                _pickupForce = true;
                return PickFromList(CharacterManager.GetCharactersFromRare(Rare.SSR), -1);
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
    /// 픽업 선택
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
    /// R 결과 반환
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
        //db로 부터 결과값을 가져옴
        GachaResultWrapper result = await AuthManager.Instance.GetDataAsync<GachaResultWrapper>(Request.writegachalog);
        GachaResult latest = result.GachaResultList[result.GachaResultList.Count - 1];
        totalCount = latest.TotalCount;
        currentSRCount = latest.SRCurrentCount;
        currentSSRCount = latest.SSRCurrentCount;
        pickupForce = latest.PickupForce;
        Debug.Log($"로드된 최종 결과\ntotal:{totalCount}, sr:{currentSRCount}, ssr:{currentSSRCount}, pickupForce:{pickupForce}");
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
