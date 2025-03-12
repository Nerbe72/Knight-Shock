using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalManager : MonoBehaviour, IFlag
{
    public bool FlagEnd { get; set; }

    [Header("결과 관리")]
    [SerializeField] private GameObject characterGroup;
    [SerializeField] private List<Image> characterImages; //캐릭터 이미지 변경
    [SerializeField] private List<Image> characterFrames; //등급에 맞춰 색상변경

    [Header("버튼")]
    [SerializeField] private Button returnButton;

    private Animator totalAnimator;
    private int hashSlide;

    private void Awake()
    {
        totalAnimator = GetComponent<Animator>();
        hashSlide = Animator.StringToHash("Slide");

        InitData();
    }

    private void InitData()
    {
        characterGroup = GetComponentInChildren<HorizontalLayoutGroup>(true).gameObject;
    }

    public void TriggerSlide()
    {
        totalAnimator.SetTrigger(hashSlide);
    }

    public void InitDatas(List<IReadOnlyCharacter> _characters, List<Color> _colors)
    {
        InitData();

        int count = _characters.Count;
        for (int i = 0; i < count; i++)
        {
            characterImages[i].sprite = _characters[i].characterSprite;
            characterFrames[i].color = _colors[i];
        }
    }

    public void StartTotal()
    {
        gameObject.SetActive(true);
        TriggerSlide();
    }

    public void CloseTotal()
    {
        FlagEnd = true;
        gameObject.SetActive(false);
    }

    //애니메이션 호출
    private void SetFlagTrue()
    {
        FlagEnd = true;
    }
}
