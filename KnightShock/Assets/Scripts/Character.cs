using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Character
{
    public int Id;
    public Rare BaseRare;
    public string Name;
    public ClassType BaseClass;
    public int BaseHp;
    public int BaseMelee;
    public int BaseMagic;
    public int BaseMeleeDefense;
    public int BaseMagicDefense;
    public List<int> SkillId;
    public List<int> PassiveId;
    public Sprite CharacterSprite;
    public string SpritePath;

    public Character()
    {
        Id = 10000;
        BaseRare = Rare.R;
        Name = "";
        BaseClass = ClassType.Attacker;
        BaseHp = 1;
        BaseMelee = 0;
        BaseMagic = 0;
        BaseMeleeDefense = 0;
        BaseMagicDefense = 0;
        SkillId = new List<int>();
        PassiveId = new List<int>();
        CharacterSprite = null;
        SpritePath = "";
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;

        if (obj == null) return false;

        Character other = obj as Character;
        return Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

[Serializable]
public class CharacterWrapper
{
    public List<Character> characters;
}
