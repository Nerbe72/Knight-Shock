using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalManager : MonoBehaviour
{
    [Header("��� ����")]
    [SerializeField] private GameObject characterGroup;

    private List<Image> characterImages; //ĳ���� �̹��� ����
    private Animator totalAnimator;
    private List<Image> characterFrames; //��޿� ���� ���󺯰�
    private int hashSlide;

    private void Awake()
    {
        totalAnimator = GetComponent<Animator>();
        hashSlide = Animator.StringToHash("Slide");

        InitDatas();
    }

    private void InitDatas()
    {
        foreach(Image obj in characterGroup.GetComponentsInChildren<Image>())
        {
            characterImages.Add(obj);
        }

        foreach (var image in characterImages)
        {
            characterFrames.Add(image.GetComponentInChildren<Image>());
        }
    }

    public void TriggerSlide()
    {
        totalAnimator.SetTrigger(hashSlide);
    }

    public void SetCharacter(int _index, Character _character, Color _color)
    {
        characterImages[_index].sprite = _character.CharacterSprite;
        characterFrames[_index].color = _color;
    }
}
