using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Image profileIMG;
    [SerializeField] private TMP_Text nameTMP;
    [SerializeField] private TMP_Text uidTMP;

    [Header("����ȭ�� ��ư")]
    [SerializeField] private Button goButton;
    [SerializeField] private Button characterButton;
    [SerializeField] private Button gachaButton;
    [SerializeField] private Button settingButton;

    [Header("�κ� ��(â) ����")]
    [SerializeField] private MainUIController mainFrame;
    [SerializeField] private CharacterUIController characterFrame;
    [SerializeField] private GachaUIController gachaFrame;
    [SerializeField] private GameObject settingsFrame;
    [SerializeField] private GameObject battleMapFrame;
    [SerializeField] private GameObject battleListFrame;

    //[Header("������Ʈ �ʱ�ȭ ����")]
    //[SerializeField] private List<GameObject> objects;

    [Header("â ����")]
    [SerializeField] private GameObject buttons;
    [SerializeField] private Button backButton;
    [SerializeField] private Button homeButton;

    private void Start()
    {
        //foreach (var obj in objects)
        //{
        //    obj.SetActive(true);
        //    obj.SetActive(false);
        //}

        InitUI();
        CharacterManager.LoadCharacters();
    }

    private void OnDestroy()
    {
        goButton.onClick.RemoveAllListeners();
        characterButton.onClick.RemoveAllListeners();
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

            if (characterFrame.gameObject.activeSelf || gachaFrame.gameObject.activeSelf)
            {
                SceneStackManager.PopScene();
                if (SceneStackManager.isMain)
                {
                    buttons.SetActive(false);
                }
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

        goButton.onClick.AddListener(ClickGo);
        characterButton.onClick.AddListener(ClickCharacter);
        gachaButton.onClick.AddListener(ClickGacha);
        settingButton.onClick.AddListener(ClickSetting);

        backButton.onClick.AddListener(ClickBack);
        homeButton.onClick.AddListener(ClickHome);

        SceneStackManager.AddScene(mainFrame);
    }

    private void ClickGo()
    {
        //���� ���� ĵ���� Ȱ��ȭ
        
    }

    private void ClickCharacter()
    {
        ((IWindowController)characterFrame).ShowWindow();
        buttons.SetActive(!SceneStackManager.isMain);
    }

    private void ClickGacha()
    {
        ((IWindowController)gachaFrame).ShowWindow();
        buttons.SetActive(!SceneStackManager.isMain);
    }

    private void ClickSetting()
    {
        settingsFrame.gameObject.SetActive(true);
        buttons.SetActive(!SceneStackManager.isMain);
    }
    
    private void ClickBack()
    {
        SceneStackManager.PopScene();
        buttons.SetActive(!SceneStackManager.isMain);
    }

    private void ClickHome()
    {
        SceneStackManager.InitStack();
        buttons.SetActive(!SceneStackManager.isMain);
    }
}
