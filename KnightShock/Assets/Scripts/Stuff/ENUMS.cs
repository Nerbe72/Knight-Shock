
public enum StatType
{
    None    = 8200,
    Melee,
    Magic,
    MeleeDefense,
    MagicDefense,
    Count
}

public enum EmotionType
{
    None    = 8100,
    Anger,
    Fear,
    Sad,
    Passion,
    Count
}

public enum ClassType
{
    Attacker,
    Defender,
    Healer,
    Supporter,
    Count
}

public enum Rare
{
    R = 3,
    SR = 2,
    SSR = 1,
}

public enum SkillType
{
    None = 0,
    Passive,
    Active,
}

public enum AttackType
{
    None = 0,
    Melee,
    Magic
}

public enum AttackArea
{
    None,
    Single,
    Area,
}

/// <summary>
/// DBȣ�� Ÿ�԰� ��ġ�ϵ��� ���� (server.js ����)
/// </summary>
public enum Request
{
    characters,
    money,
    items,
    banners,
}

public enum BannerType
{
    None,
    Beginner,
    Pickup,
    Limited,
}