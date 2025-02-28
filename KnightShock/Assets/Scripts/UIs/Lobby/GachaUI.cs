using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaUI : MonoBehaviour
{
    [SerializeField] private ScrollRect gacha_scroll;
    [SerializeField] private GameObject content_prefab;

    [SerializeField] private Image backgroundIMG;
    [SerializeField] private TMP_Text bannerText;
    [SerializeField] private RectTransform characterPosition;
    [SerializeField] private List<Image> characterIMG;

    private GachaMangaer gachaManager;

    private void OnEnable()
    {
        gachaManager = GachaMangaer.Instance;
        ShowUI();
    }

    public async void ShowUI()
    {
        InitUI();
        await gachaManager.InitBannerDatas();
        SetUI();
    }

    private void InitUI()
    {
        int count = gacha_scroll.content.childCount;
        for (int i = 0; i < count; i++)
        {
            gacha_scroll.content.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void SetUI()
    {
        //데이터보다 스크롤 크기가 더 작으면 추가 생성
        int scrollCount = gacha_scroll.content.childCount;
        int bannerCount = gachaManager.bannerDatas.Count;

        for (int i = 0; i < scrollCount; i++)
        {
            AddContent(gachaManager.bannerDatas[i], i);
        }

        for (int i = scrollCount; i < bannerCount; i++)
        {
            AddContent(gachaManager.bannerDatas[i]);
        }
    }

    private async void AddContent(BannerData _data, int _index = -1)
    {
        GameObject currentContent;
        if (_index == -1)
            currentContent = Instantiate(content_prefab, gacha_scroll.content);
        else
            currentContent = gacha_scroll.content.GetChild(_index).gameObject;

        currentContent.SetActive(true);

        var container = currentContent.GetComponent<BannerContainer>();
        container.Data = _data;
        currentContent.GetComponent<Image>().sprite = await gachaManager.LoadSprite(container.Data.bannerPath);
        currentContent.GetComponent<Button>().onClick.AddListener(() => { SelectContent(container); });
    }

    private async void SelectContent(BannerContainer _container)
    {
        backgroundIMG.sprite = await gachaManager.LoadSprite(_container.Data.backgroundPath);
        bannerText.text = _container.Data.bannerName;
        characterPosition.position.Set(_container.Data.characterPosition.x, _container.Data.characterPosition.y, 0);

        int count = characterIMG.Count;
        for (int i = 0; i < count; i++)
        {
            try
            {
                characterIMG[i].sprite = CharacterManager.GetCharacter(_container.Data.SSR_PickupList[i]).CharacterSprite;
                Debug.Log(CharacterManager.GetCharacter(_container.Data.SSR_PickupList[i]).Id);
            }
            catch (IndexOutOfRangeException) { break; }
        }
    }
}
