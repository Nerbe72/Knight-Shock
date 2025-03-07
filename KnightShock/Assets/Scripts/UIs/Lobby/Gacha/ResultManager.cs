using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    //resultManager는 하위 매니저를 관리하고 invoke되도록 하나의 함수로 과정을 묶어줌
    public static ResultManager Instance;

    [Header("결과창")]
    public TotalManager gachaTotal { get; private set; }
    public SingleManager gachaSingle { get; private set; }
    public SplashManager gachaSplash { get; private set; }

    [Header("버튼")]
    [SerializeField] private Button skip;
    [SerializeField] private Button close;

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
    }

    public void ShowResult()
    {

    }
}
