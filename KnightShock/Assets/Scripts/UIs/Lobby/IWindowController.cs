using UnityEngine;

public interface IWindowController
{
    public GameObject Self { get; set; }

    public void ShowWindow()
    {
        SceneStackTracer.AddScene(this);
        Self.SetActive(true);
    }
}
