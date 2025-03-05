using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Image profileIMG;
    [SerializeField] private TMP_Text nameTMP;
    [SerializeField] private TMP_Text uidTMP;

    [SerializeField] private Button goBTN;
    [SerializeField] private Button characterBTN;
    [SerializeField] private Button gachaBTN;

    [SerializeField] private GameObject characterFrame;
    [SerializeField] private GameObject gachaFrame;
    [SerializeField] private GameObject settingsFrame;
    [SerializeField] private GameObject battleMapFrame;
    [SerializeField] private GameObject battleListFrame;

    private void Start()
    {
        InitUI();
        CharacterManager.LoadCharacters();
    }

    private void OnDestroy()
    {
        goBTN.onClick.RemoveAllListeners();
        characterBTN.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (settingsFrame.activeSelf)
            {
                settingsFrame.SetActive(false);
                return;
            }

            if (characterFrame.activeSelf)
            {
                characterFrame.SetActive(false);
                return;
            }

            if (gachaFrame.activeSelf)
            {
                gachaFrame.SetActive(false);
                return;
            }

            //if (battleListFrame.activeSelf)
            //{
            //    battleListFrame.SetActive(false);
            //    return;
            //}

            //if (battleMapFrame.activeSelf)
            //{
            //    battleMapFrame.SetActive(false);
            //    return;
            //}

            settingsFrame.SetActive(true);
        }
    }

    private void InitUI()
    {
        nameTMP.text = GameManager.Instance.UserName;
        uidTMP.text = GameManager.Instance.UID.ToString();

        goBTN.onClick.AddListener(ClickGo);
        characterBTN.onClick.AddListener(ClickCharacter);
        gachaBTN.onClick.AddListener(ClickGacha);
    }

    private void ClickGo()
    {
        //전투 지도 캔버스 활성화
        
    }

    private void ClickCharacter()
    {
        characterFrame.SetActive(true);
    }

    private void ClickGacha()
    {
        gachaFrame.SetActive(true);
    }
    
}
