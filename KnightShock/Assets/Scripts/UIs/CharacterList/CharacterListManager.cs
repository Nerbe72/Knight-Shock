using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterListManager : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private CharacterLister characterListPrefab;

    private List<CharacterLister> contents;

    private Dictionary<Sorting, Func<List<CharacterLister>, List<CharacterLister>>> sortingFunction = new Dictionary<Sorting, Func<List<CharacterLister>, List<CharacterLister>>>
    {
        { Sorting.Level, list => list.OrderBy( c => c.data.level).ToList() },
        { Sorting.Rarity, list => list.OrderBy( c => c.character.baseRare).ToList() },
        { Sorting.Class, list => list.OrderBy( c => c.character.baseClass).ToList() }
    }; 

    public void InitList()
    {
        scrollView = GetComponentInChildren<ScrollRect>(true);
        contents = new List<CharacterLister>();
    }

    public void SortingContents(Sorting _sorting)
    {

    }

    public bool IsAlreadyExist(int _id)
    {
        foreach (var item in contents)
        {
            if (item.character.id == _id) return true;
        }

        return false;
    }

    public void CreateContent(int _id)
    {
        for (int i = 0; i < contents.Count; i++)
        {
            if (!contents[i].gameObject.activeSelf && contents[i].character.id == _id)
            {
                contents[i].InitContent(CharacterManager.GetCharacterFromID(_id));
                return;
            }
        }

        CharacterLister lister = Instantiate(characterListPrefab);
        lister.InitContent(CharacterManager.GetCharacterFromID(_id));
        lister.transform.parent = scrollView.transform;
        contents.Add(lister);
    }

}
