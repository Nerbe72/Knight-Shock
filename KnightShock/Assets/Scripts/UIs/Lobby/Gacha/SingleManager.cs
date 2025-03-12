using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;

public class SingleManager : MonoBehaviour, IFlag
{
    public bool FlagEnd { get; set; }

    private List<IReadOnlyCharacter> characters;
    private List<Color> colors;

    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text characterRare;
    [SerializeField] private Button nextButton;

    private Animator animator;
    [SerializeField] private AnimationClip singleClip;
    private int hashSingle;

    private int characterIndex;

    private Color targetColor;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        hashSingle = Animator.StringToHash("Single");
        characterIndex = 0;

        nextButton.onClick.AddListener(MoveNext);
    }

    private void OnDestroy()
    {
        nextButton.onClick.RemoveAllListeners();
    }

    private void MoveNext()
    {
        if (characterIndex >= characters.Count)
        {
            CloseSingle();
            return;
        }

        animator.Rebind();
        characterImage.sprite = characters[characterIndex].splashSprite;
        characterName.text = characters[characterIndex].name;
        characterRare.text = characters[characterIndex].baseRare.ToString();
        characterRare.color = Color.clear;
        targetColor = colors[characterIndex];
        animator.SetTrigger(hashSingle);

        characterIndex += 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_characters">가챠 결과값</param>
    public void InitData(List<IReadOnlyCharacter> _characters, List<Color> _colors)
    {
        characterIndex = 0;
        characters = _characters;
        colors = _colors;
    }

    public void StartSingle()
    {
        gameObject.SetActive(true);
        MoveNext();
    }

    public void CloseSingle()
    {
        FlagEnd = true;
        gameObject.SetActive(false);
    }

    //애니메이션 이벤트
    private void ChangeRareColor()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeRareColorCo());
    }

    private IEnumerator ChangeRareColorCo()
    {
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;

            characterRare.color = Color.Lerp(Color.clear, targetColor, time);

            if (time >= 0.6f)
            {
                break;
            }

            yield return null;
        }

        characterRare.color = targetColor;

        yield break;
    }
}
