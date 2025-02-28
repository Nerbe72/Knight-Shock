
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
    R,
    SR,
    SSR,
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
/// DB호출 타입과 일치하도록 설정 (server.js 참고)
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