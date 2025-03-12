using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GachaUIController : MonoBehaviour, IWindowController
{
    public GameObject Self { get; set; }

    [Header("배너 및 페이지")]
    [SerializeField] private ScrollRect gacha_scroll;
    [SerializeField] private GameObject content_prefab;

    [SerializeField] private Image backgroundIMG;
    [SerializeField] private TMP_Text bannerText;
    [SerializeField] private RectTransform characterPosition;
    [SerializeField] private List<Image> characterIMG;
    [SerializeField] private Animator bannerAnimator;

    [Header("버튼")]
    [SerializeField] private Button one_roll;
    [SerializeField] private Button ten_roll;
    
    private GachaManager gachaManager;
    private ResultManager resultManager;

    private void Awake()
    {
        Self = this.gameObject;
    }

    private void OnEnable()
    {
        gachaManager = GachaManager.Instance;
        resultManager = ResultManager.Instance;
        Init();
    }

    private void OnDestroy()
    {
        if (ResultManager.Instance != null)
        {
            Destroy(ResultManager.Instance.gameObject);
            ResultManager.Instance = null;
        }

        one_roll.onClick.RemoveAllListeners();
        ten_roll.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //스킵 표시
            if (resultManager.gachaSplash.gameObject.activeSelf || resultManager.gachaSingle.gameObject.activeSelf)
            {
                resultManager.ShowSkip();
            }
        }
    }

    private async void Init()
    {
        ResetUI();
        await gachaManager.InitBannerDatas();
        SetUI();

        GameObject button_first = gacha_scroll.content.GetChild(0).gameObject;
        SelectContent(button_first.GetComponent<BannerContainer>());
    }

    private void ResetUI()
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
            characterIMG[i].sprite = CharacterManager.GetCharacterFromID(_container.Data.SSR_PickupList[i]).characterSprite;
            characterIMG[i].color = Color.white;
        }

        for (int i = count; i < 3; i++)
        {
            characterIMG[i].color = Color.clear;
        }

        one_roll.onClick.RemoveAllListeners();
        one_roll.onClick.AddListener(() => 
        {
            SetResults(_container);
        });

        ten_roll.onClick.RemoveAllListeners();
        ten_roll.onClick.AddListener(() =>
        {
            SetResults(_container, 10);
        });
    }

    //결과값을 적용함
    private async void SetResults(BannerContainer _container, int _count = 1)
    {
        resultManager.gameObject.SetActive(true);

        //가챠 실행 및 데이터 저장
        List<int> result = gachaManager.StartGacha(_container, 10);
        List<int> result_sorted = new List<int>(result);
        result_sorted.Sort();

        //splash
        int count = result.Count;
        if (result.Count == 10)
        {
            for (int i = 0; i < count; i++)
            {
                IReadOnlyCharacter character = CharacterManager.GetCharacterFromID(result[i]);
                Color singleColor = gachaManager.rarityColor[character.baseRare];
            }
        }

        Color targetColor = gachaManager.rarityColor[CharacterManager.GetRareFromID(result_sorted[0])];

        resultManager.gachaSplash.SetColor(targetColor);

        resultManager.gachaSplash.StartSplash();

        while (!resultManager.gachaSplash.FlagEnd)
        {
            await Task.Delay(100);
        }

        resultManager.gachaSplash.FlagEnd = false;

        //single
        List<IReadOnlyCharacter> characters = new List<IReadOnlyCharacter>();
        List<Color> colors = new List<Color>();

        foreach (var id in result)
        {
            IReadOnlyCharacter ch = CharacterManager.GetCharacterFromID(id);
            characters.Add(ch);
            colors.Add(gachaManager.rarityColor[ch.baseRare]);
        }

        resultManager.gachaSingle.InitData(characters, colors);
        resultManager.gachaSingle.StartSingle();

        while (!resultManager.gachaSingle.FlagEnd)
        {
            await Task.Delay(100);
        }

        resultManager.gachaSingle.FlagEnd = false;

        if (_count == 1)
        {
            //가챠씬 종료
            return;
        }

        //total
        resultManager.gachaTotal.InitDatas(characters, colors);
        resultManager.gachaTotal.StartTotal();

        while (!resultManager.gachaTotal.FlagEnd)
        {
            await Task.Delay(100);
        }

        resultManager.gachaTotal.FlagEnd = false;

        resultManager.ShowClose();

        return;
    }
}
