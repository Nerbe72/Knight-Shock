using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SplashManager : MonoBehaviour
{
    private Animator splashAnimator;
    private int hashSplash;

    [SerializeField] private Button touchButton;
    [SerializeField] private Image star;
    [SerializeField] private Image shine;

    private float animationTime;
    private Color targetColor = Color.white;

    private void Awake()
    {
        splashAnimator = GetComponent<Animator>();
        hashSplash = Animator.StringToHash("Splash");

        splashAnimator.SetBool(hashSplash, false);
        touchButton.onClick.AddListener(SplashEnd);
    }

    private void OnDestroy()
    {
        touchButton.onClick.RemoveAllListeners();
    }

    private void SplashEnd()
    {
        splashAnimator.SetBool(hashSplash, true);
    }

    public void SetColor(Color _color)
    {

        targetColor = _color;
    }

    //애니메이션 이벤트
    private void ChangeColor()
    {
        StartCoroutine(ChangeColorCo());
    }

    private IEnumerator ChangeColorCo()
    {
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;

            star.color = Color.Lerp(Color.white, targetColor, time - (animationTime * 0.65f));
            shine.color = Color.Lerp(Color.white, targetColor, time - (animationTime  * 0.5f));

            if (time >= animationTime)
            {
                break;
            }
        }

        shine.color = targetColor;
        yield break;
    }
}
