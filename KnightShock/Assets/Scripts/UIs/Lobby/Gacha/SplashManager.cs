using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SplashManager : MonoBehaviour, IFlag
{
    public bool FlagEnd { get; set; }

    private Animator splashAnimator;
    private int hashSplash;

    [SerializeField] private Button touchButton;
    [SerializeField] private Image star;
    [SerializeField] private Image shine;

    private float animationTime = 0.8f;
    private float splashTime = 3.3f; //임시 지정
    private Color targetColor = Color.white;

    private void Awake()
    {
        splashAnimator = GetComponent<Animator>();
        hashSplash = Animator.StringToHash("Splash");

        foreach (var clip in splashAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "Splash")
            {
                splashTime = clip.length;
            }
        }

        touchButton.onClick.AddListener(RollEffect);

        FlagEnd = false;
    }

    private void OnDestroy()
    {
        touchButton.onClick.RemoveAllListeners();
    }

    private void RollEffect()
    {
        splashAnimator.SetTrigger(hashSplash);
    }

    public void SetColor(Color _color)
    {
        targetColor = _color;
    }

    public void StartSplash()
    {
        FlagEnd = false;
        gameObject.SetActive(true);
    }

    //애니메이션 이벤트
    public void CloseSplash()
    {
        FlagEnd = true;
        gameObject.SetActive(false);
    }

    //애니메이션 이벤트
    private void ChangeColor()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeColorCo());
    }

    private void ChangeAlpha()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeAlphaCo());
    }

    private IEnumerator ChangeColorCo()
    {
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;

            star.color = Color.Lerp(Color.white, targetColor, time - (animationTime * 0.2f));
            shine.color = Color.Lerp(Color.white, targetColor, time - (animationTime * 0.2f));

            if (time >= animationTime)
            {
                break;
            }

            yield return null;
        }

        shine.color = targetColor;
        
        yield break;
    }

    private IEnumerator ChangeAlphaCo()
    {
        float time = 0f;
        Color alpha02 = new Color(1f, 1f, 1f, 0.2f);

        star.color = Color.white;
        while (true)
        {
            time += Time.deltaTime;


            if (time < splashTime / 2)
            {
                shine.color = Color.Lerp(Color.white, alpha02, time);
            }
            else if (time >= splashTime / 2)
            {
                shine.color = Color.Lerp(alpha02, Color.white, time - (splashTime / 2));
            }

            if (time >= splashTime)
            {
                break;
            }

            yield return null;
        }

        shine.color = Color.white;

        yield break;
    }
}
