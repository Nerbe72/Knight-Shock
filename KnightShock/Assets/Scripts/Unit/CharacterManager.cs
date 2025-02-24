using NUnit.Framework;
using System.Collections.Generic;
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
        if (File.Exists(characterPath))
        {
            string json = File.ReadAllText(characterPath);

            CharacterWrapper wrapper = JsonUtility.FromJson<CharacterWrapper>(json);

            if (wrapper != null && wrapper.characters != null)
            {
                if (!rareCharacters.ContainsKey(Rare.R))
                    rareCharacters.Add(Rare.R, new List<int>());

                if (!rareCharacters.ContainsKey(Rare.SR))
                    rareCharacters.Add(Rare.SR, new List<int>());

                if (!rareCharacters.ContainsKey(Rare.SSR))
                    rareCharacters.Add(Rare.SSR, new List<int>());

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

            Debug.Log("캐릭터 json이 비어있음");
        }
    }

    public static Character GetCharacter(int _id)
    {
        if (characters.TryGetValue(new Character(){ Id = _id }, out Character found))
        {
            return found;
        }

        return null;
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
