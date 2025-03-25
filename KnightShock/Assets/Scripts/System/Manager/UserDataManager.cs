using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager Instance;

    public int TrialCount = 3;
    Dictionary<int, CharacterData> characterDatas;

    private async void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        await Init();
    }

    public async Task Init()
    {
        characterDatas = null;
        int trial = 0;

        while (characterDatas == null && trial < TrialCount)
        {
            CharacterDataWrapper wrapper = await AuthManager.Instance.GetUserDataAsync<CharacterDataWrapper>($"user_{GameManager.Instance.UID}");
            characterDatas = wrapper.datas;
            trial++;
            Debug.Log($"로드 시도 횟수: {trial}");
        }
        
        Debug.Log("보유 캐릭터 정보 로드됨");
    }

    public async Task SaveCharacterData()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>ID, 캐릭터 쌍</returns>
    public IReadOnlyDictionary<int, CharacterData> GetAllCharacterDatas()
    {
        if (characterDatas == null) return new Dictionary<int, CharacterData>();

        return characterDatas;
    }

    public IReadOnlyCharacterData GetCharacterData(int _id)
    {
        if (!characterDatas.ContainsKey(_id)) return null;

        return characterDatas[_id];
    }

    public void SetCharacterData(int _id, CharacterData _data)
    {
        characterDatas[_id] = _data;

        SetLastUpdateTime(_id);
    }

    public void SetExp(int _id, int _after)
    {
        characterDatas[_id].CurrentExp = _after;
        SetLastUpdateTime(_id);
    }

    public void SetLevel(int _id, int _after)
    {
        characterDatas[_id].Level = _after;
        SetLastUpdateTime(_id);
    }

    public void SetEvolution(int _id, int _after)
    {
        characterDatas[_id].Evolution= _after;
        SetLastUpdateTime(_id);
    }

    public void SetSkillLevels(int _id, int _skillid, int _after)
    {
        characterDatas[_id].SkillLevels[_skillid] = _after;
        SetLastUpdateTime(_id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_id">변경 선택된 캐릭터</param>
    /// <param name="_slot">슬롯 타입</param>
    /// <param name="_after">해제하는 경우 -1</param>
    public void SetEquipments(int _id, EquipmentPart _slot, int _after)
    {
        characterDatas[_id].Equipments[_slot] = _after;
        SetLastUpdateTime(_id);
    }

    private void SetLastUpdateTime(int _id)
    {
        characterDatas[_id].LastUpdated = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day} {DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}";
    }
}
