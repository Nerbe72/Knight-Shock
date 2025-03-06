using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public static ResultManager Instance;

    [Header("결과창")]
    [SerializeField] private GameObject gachaTotal;
    [SerializeField] private GameObject gachaSingle;
    [SerializeField] private GameObject splash;

    [Header("버튼")]
    [SerializeField] private Button skip;
    [SerializeField] private Button close;

    [Header("결과 관리")]
    [SerializeField] private GameObject characterGroup;
    [SerializeField] private Dictionary<Rare, Color> frameColor;

    private List<Image> characterImages; //캐릭터 이미지 변경
    private Animator animator;
    private List<Image> characterFrames; //등급에 맞춰 색상변경
    private int hashSlide;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
            return;
        }

        animator = gachaTotal.gameObject.GetComponent<Animator>();
        hashSlide = Animator.StringToHash("Slide");

        foreach (Image obj in characterGroup.GetComponentsInChildren<Image>())
        {
            characterImages.Add(obj);
        }

        foreach (var image in characterImages)
        {
            characterFrames.Add(image.GetComponentInChildren<Image>());
        }

        frameColor = new Dictionary<Rare, Color>();
        frameColor.Add(Rare.SSR, new Color(1f, 0.72f, 0f, 1f));
        frameColor.Add(Rare.SR, new Color(0.8f, 0f, 1f, 1f));
        frameColor.Add(Rare.R, new Color(0f, 0.75f, 1f, 1f));
    }

    public void TriggerSlide()
    {
        animator.SetTrigger(hashSlide);
    }

    public void SetCharacter(int _index, int _id)
    {
        characterImages[_index].sprite = CharacterManager.GetCharacterFromID(_id).CharacterSprite;
        characterFrames[_index].color = frameColor[CharacterManager.GetRareFromID(_id)];
    }
}
