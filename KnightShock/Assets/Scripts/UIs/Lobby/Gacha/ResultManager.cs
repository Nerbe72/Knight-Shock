using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour, IInitializable
{
    //resultManager는 하위 매니저를 관리하고 invoke되도록 하나의 함수로 과정을 묶어줌
    public static ResultManager Instance;
    public int InitializationPriority => 4;

    [Header("결과창")]
    public TotalManager gachaTotal { get; private set; }
    public SingleManager gachaSingle { get; private set; }
    public SplashManager gachaSplash { get; private set; }


    [Header("버튼")]
    [SerializeField] private Button skip;
    [SerializeField] private Button close;

    private void OnDestroy()
    {
        skip.onClick.RemoveAllListeners();
        close.onClick.RemoveAllListeners();
    }

    private void PressSkip()
    {
        if (!gachaSplash.FlagEnd && gachaSplash.gameObject.activeSelf)
        {
            gachaSplash.CloseSplash();
            return;
        }
        else if (!gachaSingle.FlagEnd && gachaSingle.gameObject.activeSelf)
        {
            gachaSingle.CloseSingle();
            return;
        }
        else if (!gachaTotal.FlagEnd && gachaTotal.gameObject.activeSelf)
        {
            gachaTotal.CloseTotal();
            return;
        }
    }

    private void PressClose()
    {
        close.gameObject.SetActive(false);
        gachaTotal.CloseTotal();
    }

    public void ShowSkip()
    {
        skip.gameObject.SetActive(true);
    }

    public void ShowClose()
    {
        close.gameObject.SetActive(true);
    }

    public void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        gachaTotal = GetComponentInChildren<TotalManager>(true);
        gachaSingle = GetComponentInChildren<SingleManager>(true);
        gachaSplash = GetComponentInChildren<SplashManager>(true);

        skip.onClick.AddListener(PressSkip);
        close.onClick.AddListener(PressClose);
    }
}
