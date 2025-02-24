using UnityEngine;

public class GachaMangaer : MonoBehaviour
{
    /// <summary>
    /// ��í Ȯ�� ���
    /// </summary>
    /// <returns>���(ĳ����id) ��ȯ</returns>
    public int SlotGacha()
    {
        //���� �⵵, ��, ��, �ð�, ƽ, �и���
        int seed = (int)System.DateTime.Now.Ticks + Random.Range(0, System.DateTime.Now.Year) + System.DateTime.Now.Month + System.DateTime.Now.Day + System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Millisecond;
        Random.InitState(seed);

        int random = Random.Range(0, 101);
        //R
        Rare selectedRare = Rare.R;
        if (random > 80 && random <= 99) selectedRare = Rare.SR;
        if (random > 99) selectedRare = Rare.SSR;

        int inner = Random.Range(0, CharacterManager.GetRareCharacterCount(selectedRare));

        int selectedId = CharacterManager.GetRandomCharacter(selectedRare, inner);

        return selectedId;
    }
}
