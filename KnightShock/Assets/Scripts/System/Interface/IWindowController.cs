using UnityEngine;

public interface IWindowController
{
    public GameObject Self { get; set; }

    public void ShowWindow()
    {
        SceneStackManager.AddScene(this);
        Self.SetActive(true);
    }
}
