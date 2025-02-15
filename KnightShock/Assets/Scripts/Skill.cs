using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Skill
{
    public int Id;
    public SkillType Type;
    public string Description;
    public List<ChangeEmotion> ChangeEmotions;
    public List<ChangeStat> ChangeStats;
    public List<AttackSkill> Attacks;

    public Skill()
    {
        Id = 1000;
        Type = SkillType.None;
        Description = "";
        ChangeEmotions = new List<ChangeEmotion>();
        ChangeStats = new List<ChangeStat>();
    }
}

[Serializable]
public class ChangeEmotion
{
    public EmotionType EmotionType;
    public bool IsPercent;
    public float Amount;

    public ChangeEmotion()
    {
        EmotionType = EmotionType.None;
        IsPercent = false;
        Amount = 0;
    }
}

[Serializable]
public class ChangeStat
{
    public StatType StatType;
    public bool IsPercent;
    public float Amount;

    public ChangeStat()
    {
        StatType = StatType.None;
        IsPercent = false;
        Amount = 0;
    }
}

[Serializable]
public class AttackSkill
{
    /// <summary>
    /// 공격 보정 타입
    /// </summary>
    public AttackFixType FixType;
    public bool IsPercent;
    public bool IsFixedDamage;
    public float Amount;

    public AttackSkill()
    {
        FixType = AttackFixType.None;
        IsPercent = false;
        IsFixedDamage = false;
        Amount = 0;
    }
}

[Serializable]
public class SkillWrapper
{
    public List<Skill> skills;
}
