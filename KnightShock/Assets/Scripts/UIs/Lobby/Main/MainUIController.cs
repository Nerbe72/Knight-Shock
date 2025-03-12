using UnityEngine;

public class MainUIController : MonoBehaviour, IWindowController
{
    public GameObject Self { get; set; }

    private void Awake()
    {
        Self = this.gameObject;
    }

}
