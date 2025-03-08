using System;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaUIController : MonoBehaviour
{
    [Header("배너 및 페이지")]
    [SerializeField] private ScrollRect gacha_scroll;
    [SerializeField] private GameObject content_prefab;

    [SerializeField] private Image backgroundIMG;
    [SerializeField] private TMP_Text bannerText;
    [SerializeField] private RectTransform characterPosition;
    [SerializeField] private List<Image> characterIMG;

    [SerializeField] private Button one_roll;
    [SerializeField] private Button ten_roll;

    private GachaManager gachaManager;
    private ResultManager resultManager;

    private void OnEnable()
    {
        gachaManager = GachaManager.Instance;
        resultManager = ResultManager.Instance;
        ShowUI();
    }

    private void OnDestroy()
    {
        if (ResultManager.Instance != null)
        {
            Destroy(ResultManager.Instance.gameObject);
            ResultManager.Instance = null;
        }
    }

    public async void ShowUI()
    {
        InitUI();
        await gachaManager.InitBannerDatas();
        SetUI();

        GameObject button_first = gacha_scroll.content.GetChild(0).gameObject;
        SelectContent(button_first.GetComponent<BannerContainer>());
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
        one_roll.onClick.RemoveAllListeners();
        ten_roll.onClick.RemoveAllListeners();

        backgroundIMG.sprite = await gachaManager.LoadSprite(_container.Data.backgroundPath);
        bannerText.text = _container.Data.bannerName;
        characterPosition.position.Set(_container.Data.characterPosition.x, _container.Data.characterPosition.y, 0);

        //픽업이 3 이상인 경우 3명까지만 표시
        int count = Math.Clamp(_container.Data.SSR_PickupList.Count, 0, 3);
        for (int i = 0; i < count; i++)
        {
            characterIMG[i].sprite = CharacterManager.GetCharacterFromID(_container.Data.SSR_PickupList[i]).CharacterSprite;
            characterIMG[i].color = Color.white;
        }

        for (int i = count; i < 3; i++)
        {
            characterIMG[i].color = Color.clear;
        }

        one_roll.onClick.AddListener(() => 
        {
            SetResults(_container);
        });

        ten_roll.onClick.AddListener(() =>
        {
            SetResults(_container, 10);
        });
    }

    private void SetResults(BannerContainer _container, int _count = 1)
    {
        //가챠 실행 및 데이터 저장
        List<int> result = gachaManager.StartGacha(_container, 10);
        List<int> result_sorted = new List<int>(result);
        result_sorted.Sort();

        int count = result.Count;
        if (result.Count == 10)
        {
            for (int i = 0; i < count; i++)
            {
                Character character = CharacterManager.GetCharacterFromID(result[i]);
                Color singleColor = gachaManager.rarityColor[character.BaseRare];
                resultManager.gachaTotal.SetCharacter(i, character, singleColor);
            }
        }

        Color targetColor = gachaManager.rarityColor[CharacterManager.GetRareFromID(result_sorted[0])];

        resultManager.gachaSplash.SetColor(targetColor);
    }
}
