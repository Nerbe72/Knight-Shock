using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Title : MonoBehaviour
{
    private UIDocument uiDocument;

    private VisualElement frame;
    private Button pressAny;
    private Label blink;

    private void Awake()
    {
        InitUI();
    }

    private void InitUI()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        frame = root.Q<VisualElement>("background");
        pressAny = root.Q<Button>("press");
        blink = root.Q<Label>("blink");

        pressAny.clicked += Press;
        StartCoroutine(blinkCo());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private async void Press()
    {
        await LoginManger.LoginAsync("user1", "1234aa!");
    }

    private IEnumerator blinkCo()
    {
        float time = 0f;
        bool reverse = false;
        while (true)
        {
            blink.style.color = 
                !reverse ? Color.Lerp(Color.white, Color.clear, time) : Color.Lerp(Color.clear, Color.white, time);
            blink.style.unityTextOutlineColor =
                !reverse ? Color.Lerp(Color.black, Color.clear, time) : Color.Lerp(Color.clear, Color.black, time);

            time += Time.deltaTime;

            if (time >= 1f)
            {
                reverse = !reverse;
                time = -0.1f;
            }

            yield return null;
        }
    }
}
