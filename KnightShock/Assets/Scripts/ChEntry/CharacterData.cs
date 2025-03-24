using System;
using System.Collections.Generic;
using UnityEngine;

public interface IReadOnlyCharacterData
{
    public int id { get; }
    public int level { get; }
    public int currentExp { get; }
    public int evolution { get; }
    public Dictionary<int, int> skillLevel { get; }
    public Dictionary<EquipmentPart, int> equipments { get; }
    public string lastUpdated { get; }
}

[Serializable]
public class CharacterData : IReadOnlyCharacterData
{
    public int Id;
    public int Level;
    public int CurrentExp;
    /// <summary>
    /// 캐릭터 돌파
    /// </summary>
    public int Evolution;
    /// <summary>
    /// KEY: 스킬 id / VALUE: 스킬레벨
    /// </summary>
    public Dictionary<int, int> SkillLevels;
    /// <summary>
    /// KEY: 슬롯넘버 / VALUE: 장착된 장비id
    /// </summary>
    public Dictionary<EquipmentPart, int> Equipments;
    public string LastUpdated;


    public int id => Id;
    public int level => Level;

    public int currentExp => CurrentExp;

    public int evolution => Evolution;

    public Dictionary<int, int> skillLevel => SkillLevels;

    public Dictionary<EquipmentPart, int> equipments => Equipments;

    public string lastUpdated => LastUpdated;

    public CharacterData()
    {
        Id = 10000;
        Level = 1;
        CurrentExp = 0;
        Evolution = 0;
        SkillLevels = new Dictionary<int, int>();
        Equipments = new Dictionary<EquipmentPart, int>();
        Equipments.Add(EquipmentPart.Head, -1);
        Equipments.Add(EquipmentPart.UpperBody, -1);
        Equipments.Add(EquipmentPart.LowerBody, -1);
        Equipments.Add(EquipmentPart.Hand, -1);
        LastUpdated = "1970-01-01 00:00:00";
    }
}

[Serializable]
public class Meta
{
    public string LastSync;
    public string LatestUpdatedVersion;
}

[Serializable]
public class CharacterDataWrapper
{
    int uid; //파일 검증용(불러온 파일명의 id와 일치하지 않으면 안됨)
    public Dictionary<int, CharacterData> datas;
    public Meta meta;

    public CharacterDataWrapper()
    {
        datas = new Dictionary<int, CharacterData>();
        meta = new Meta();
    }
}
