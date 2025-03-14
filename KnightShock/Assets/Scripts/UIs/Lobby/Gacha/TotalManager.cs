using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalManager : MonoBehaviour, IFlag, IInitializable
{
    public bool FlagEnd { get; set; }

    public int InitializationPriority => 3;

    [Header("��� ����")]
    [SerializeField] private GameObject characterGroup;
    [SerializeField] private List<Image> characterImages; //ĳ���� �̹��� ����
    [SerializeField] private List<Image> characterFrames; //��޿� ���� ���󺯰�

    [Header("��ư")]
    [SerializeField] private Button returnButton;

    private Animator totalAnimator;
    private int hashSlide;

    private void InitData()
    {
        characterGroup = GetComponentInChildren<HorizontalLayoutGroup>(true).gameObject;
    }

    public void Initialize()
    {
        totalAnimator = GetComponent<Animator>();
        hashSlide = Animator.StringToHash("Slide");

        InitData();
    }

    public void PlaySlide()
    {
        totalAnimator.Play(hashSlide, 0, 0);
        //totalAnimator.SetTrigger(hashSlide);
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
        PlaySlide();
    }

    public void CloseTotal()
    {
        FlagEnd = true;
        gameObject.SetActive(false);
    }

    //�ִϸ��̼� ȣ��
    private void SetFlagTrue()
    {
        FlagEnd = true;
    }
}
