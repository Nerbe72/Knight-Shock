using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    //resultManager�� ���� �Ŵ����� �����ϰ� invoke�ǵ��� �ϳ��� �Լ��� ������ ������
    public static ResultManager Instance;

    [Header("���â")]
    public TotalManager gachaTotal { get; private set; }
    public SingleManager gachaSingle { get; private set; }
    public SplashManager gachaSplash { get; private set; }

    [Header("��ư")]
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
