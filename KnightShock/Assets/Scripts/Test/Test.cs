using System;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    CharacterList chs;
    private async void Start()
    {
        chs = await AuthManager.Instance.GetDataAsync<CharacterList>(Request.characters);
    }

    private void Update()
    {
        if (chs != null)
            Debug.Log(chs.characters.Count);
    }

    [Serializable]
    public class CharacterList
    {
        public int uid;
        public List<Character> characters;

        public CharacterList()
        {
            uid = -1;
            characters = new List<Character>();
        }
    }
}
