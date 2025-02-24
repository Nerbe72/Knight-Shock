using UnityEngine;

public class GachaMangaer : MonoBehaviour
{
    /// <summary>
    /// 가챠 확률 계산
    /// </summary>
    /// <returns>결과(캐릭터id) 반환</returns>
    public int SlotGacha()
    {
        //랜덤 년도, 월, 일, 시간, 틱, 밀리초
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
