using UnityEngine;

public class CharacterUIController : MonoBehaviour, IWindowController, IInitializable
{
    public GameObject Self { get; set; }

    public int InitializationPriority => 1;

    public void Initialize()
    {
        Self = this.gameObject;
    }


}
