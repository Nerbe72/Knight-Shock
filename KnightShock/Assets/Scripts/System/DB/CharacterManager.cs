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

    public static Dictionary<Rare, Color> rarityColor = new Dictionary<Rare, Color>
    {
        { Rare.SSR, new Color(1f, 0.72f, 0f, 1f) },
        { Rare.SR, new Color(0.8f, 0f, 1f, 1f) },
        { Rare.R, new Color(0f, 0.8f, 1f, 1f) }
    };

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
                        ch.SplashSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ch.SplashPath);
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

    /// <summary>
    /// 암시적 ReadOnly 캐스팅
    /// </summary>
    /// <param name="_id"></param>
    /// <returns>수정 불가능한 데이터 반환</returns>
    public static IReadOnlyCharacter GetCharacterFromID(int _id)
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
        if (!Enum.IsDefined(typeof(Rare), (Rare)fid)) return Rare.R;
        return (Rare)fid;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_rarity"></param>
    /// <returns>레어등급에 맞는 캐릭터 id 리스트</returns>
    public static IReadOnlyList<int> GetCharactersFromRare(Rare _rarity)
    {
        return rareCharacters[_rarity].AsReadOnly();
    }

    public static int GetRandomRareCharacter(Rare _rare, int _position)
    {
        return rareCharacters[_rare][_position];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_rare"></param>
    /// <returns>레어등급에 맞는 캐릭터 Count</returns>
    public static int GetRareCharacterCount(Rare _rare)
    {
        return rareCharacters[_rare].Count;
    }
}
