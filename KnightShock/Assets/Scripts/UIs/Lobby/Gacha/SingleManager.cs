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

public class SingleManager : MonoBehaviour, IFlag, IInitializable
{
    public bool FlagEnd { get; set; }

    public int InitializationPriority => 3;

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

    private void OnDestroy()
    {
        nextButton.onClick.RemoveAllListeners();
    }

    private void MoveNext()
    {
        //animator.Rebind();

        if (characterIndex >= characters.Count)
        {
            CloseSingle();
            return;
        }

        characterImage.sprite = characters[characterIndex].splashSprite;
        characterName.text = characters[characterIndex].name;
        characterRare.text = characters[characterIndex].baseRare.ToString();
        characterRare.color = Color.clear;
        targetColor = colors[characterIndex];
        animator.Play(hashSingle, 0, 0);
        //animator.SetTrigger(hashSingle);

        characterIndex += 1;
    }

    public void Initialize()
    {
        animator = GetComponent<Animator>();
        hashSingle = Animator.StringToHash("Single");
        characterIndex = 0;

        nextButton.onClick.AddListener(MoveNext);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_characters">��í �����</param>
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

    //�ִϸ��̼� �̺�Ʈ
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
