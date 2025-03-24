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
    /// ĳ���� ����
    /// </summary>
    public int Evolution;
    /// <summary>
    /// KEY: ��ų id / VALUE: ��ų����
    /// </summary>
    public Dictionary<int, int> SkillLevels;
    /// <summary>
    /// KEY: ���Գѹ� / VALUE: ������ ���id
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
    int uid; //���� ������(�ҷ��� ���ϸ��� id�� ��ġ���� ������ �ȵ�)
    public Dictionary<int, CharacterData> datas;
    public Meta meta;

    public CharacterDataWrapper()
    {
        datas = new Dictionary<int, CharacterData>();
        meta = new Meta();
    }
}
