using UnityEngine;

public class CharacterUIController : MonoBehaviour, IWindowController
{
    public GameObject Self { get; set; }

    private void Awake()
    {
        Self = this.gameObject;
    }
}
