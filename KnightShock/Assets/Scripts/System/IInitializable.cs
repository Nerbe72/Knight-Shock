using UnityEngine;

public interface IInitializable
{
    /// <summary>
    /// �������� �켱������ ����(���� ���ں��� init��)
    /// </summary>
    int InitializationPriority { get; }
    void Initialize();
}
