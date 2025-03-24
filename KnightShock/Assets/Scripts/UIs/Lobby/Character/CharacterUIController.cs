using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIController : MonoBehaviour, IWindowController, IInitializable
{
    public GameObject Self { get; set; }

    public int InitializationPriority => 1;

    private CharacterListManager listManager;

    public void Initialize()
    {
        Self = this.gameObject;

        listManager = GetComponent<CharacterListManager>();
        listManager.InitList();
    }

    private async void OnEnable()
    {
        if (Self == null) return;
        
        IReadOnlyDictionary<int, CharacterData> datas = UserDataManager.Instance.GetAllCharacterDatas();

        foreach (var data in datas)
        {
            listManager.CreateContent(data.Value.id);
        }
    }
}
