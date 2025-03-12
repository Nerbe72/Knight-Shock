using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkipController : MonoBehaviour
{
    private Image skipImage;
    private TMP_Text skipText;

    private float time = -0.2f;

    private void Awake()
    {
        skipImage = GetComponentInChildren<Image>();
        skipText = GetComponentInChildren<TMP_Text>();
    }

    void LateUpdate()
    {
        if (gameObject.activeSelf)
        {
            time += Time.deltaTime * 0.85f;

            skipImage.color = Color.Lerp(Color.white, Color.clear, time);
            skipText.color = Color.Lerp(Color.white, Color.clear, time);

            if (time > 1f)
            {
                time = -0.2f;
                gameObject.SetActive(false);
            }
        }
    }
}
