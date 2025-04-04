using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLister : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private Button button;

    public IReadOnlyCharacter character;
    public IReadOnlyCharacterData data;

    private void Awake()
    {
        button.onClick.AddListener(OpenDetail);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

    public void InitContent(IReadOnlyCharacter _character, IReadOnlyCharacterData _data)
    {
        character = _character;
        data = _data;
        characterImage.sprite = _character.characterSprite;
        characterImage.rectTransform.pivot = _character.spritePivot;
        characterImage.rectTransform.anchoredPosition = Vector2.zero;
        characterName.text = _character.name;

        gameObject.SetActive(true);
    }

    private void OpenDetail()
    {
        Debug.Log($"{character.id}: {character.name} 캐릭터가 선택됨");
    }
}