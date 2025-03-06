using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class CharacterManager
{
    public static string characterPath = Path.Combine(Application.dataPath, "Datas", "CHARACTER.json");

    private static HashSet<Character> characters = new HashSet<Character>();
    private static Dictionary<Rare, List<int>> rareCharacters = new Dictionary<Rare, List<int>>();

    public static void LoadCharacters()
    {
        foreach (var rarity in Enum.GetValues(typeof(Rare)).Cast<Rare>())
        {
            if (!rareCharacters.ContainsKey(rarity))
            {
                rareCharacters.Add(rarity, new List<int>());
            }
        }

        if (File.Exists(characterPath))
        {
            string json = File.ReadAllText(characterPath);

            CharacterWrapper wrapper = JsonUtility.FromJson<CharacterWrapper>(json);

            if (wrapper != null && wrapper.characters != null)
            {
                foreach (var ch in wrapper.characters)
                {
                    if (!string.IsNullOrEmpty(ch.SpritePath))
                    {
                        ch.CharacterSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ch.SpritePath);
                    }

                    rareCharacters[ch.BaseRare].Add(ch.Id);
                }

                Debug.Log("캐릭터 데이터 로드됨");
                characters = wrapper.characters.ToHashSet();
            }
            else
                Debug.Log("캐릭터 json이 비어있음");

        }
    }

    public static Character GetCharacterFromID(int _id)
    {
        if (characters.TryGetValue(new Character(){ Id = _id }, out Character found))
        {
            return found;
        }

        return null;
    }

    public static Rare GetRareFromID(int _id)
    {
        int fid = Mathf.FloorToInt(_id / 10000);
        if (!Enum.IsDefined(typeof(Rare), (Rare)_id)) return Rare.R;

        return (Rare)fid;
    }

    public static IReadOnlyList<int> GetCharactersFromRare(Rare _rarity)
    {
        return rareCharacters[_rarity].AsReadOnly();
    }

    public static int GetRandomCharacter(Rare _rare, int _position)
    {
        return rareCharacters[_rare][_position];
    }

    public static int GetRareCharacterCount(Rare _rare)
    {
        return rareCharacters[_rare].Count;
    }
}
