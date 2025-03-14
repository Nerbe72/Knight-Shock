using UnityEngine;

/// <summary>
/// ���� �ε�Ǵ� ���� ��Ȱ��ȭ�� ������Ʈ�� �����͸� Init�ؾߵǴ� ������Ʈ�� ����
/// </summary>
public interface IInitializable
{
    /// <summary>
    /// �������� �켱������ ����(���� ���ں��� init��)
    /// </summary>
    int InitializationPriority { get; }

    /// <summary>
    /// ���� �ε�� �� ȣ��
    /// </summary>
    void Initialize();
}
