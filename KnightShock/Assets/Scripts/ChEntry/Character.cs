using System;
using System.Collections.Generic;
using UnityEngine;

public interface IReadOnlyCharacter
{
    public int id { get; }
    public Rare baseRare { get; }
    public string name { get; }
    public ClassType baseClass { get; }
    public int baseHp { get; }
    public int baseMelee { get; }
    public int baseMagic { get; }
    public int baseMeleeDefense { get; }
    public int baseMagicDefense { get; }
    public List<int> skillId { get; }
    public List<int> passiveId { get; }
    public Sprite characterSprite { get; }
    public string spritePath { get; }
    public Vector2 spritePivot { get; }
    public Sprite splashSprite { get; }
    public string splashPath { get; }
}

[Serializable]
public class Character : IReadOnlyCharacter
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
    public Vector2 SpritePivot;
    public Sprite SplashSprite;
    public string SplashPath;

    int IReadOnlyCharacter.id => Id;
    Rare IReadOnlyCharacter.baseRare => BaseRare;
    string IReadOnlyCharacter.name => Name;
    ClassType IReadOnlyCharacter.baseClass => BaseClass;
    int IReadOnlyCharacter.baseHp => BaseHp;
    int IReadOnlyCharacter.baseMelee => BaseMelee;
    int IReadOnlyCharacter.baseMagic => BaseMagic;
    int IReadOnlyCharacter.baseMeleeDefense => BaseMeleeDefense;
    int IReadOnlyCharacter.baseMagicDefense => BaseMagicDefense;
    List<int> IReadOnlyCharacter.skillId => SkillId;
    List<int> IReadOnlyCharacter.passiveId => PassiveId;
    Sprite IReadOnlyCharacter.characterSprite => CharacterSprite;
    string IReadOnlyCharacter.spritePath => SpritePath;
    Sprite IReadOnlyCharacter.splashSprite => SplashSprite;
    string IReadOnlyCharacter.splashPath => SplashPath;
    public Vector2 spritePivot => SpritePivot;

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
        SplashSprite = null;
        SplashPath = "";
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;

        if (obj == null) return false;

        Character other = obj as Character;
        return other.Id == this.Id;
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
