using UnityEngine;

public interface IInitializable
{
    /// <summary>
    /// 낮을수록 우선순위가 높음(낮은 숫자부터 init됨)
    /// </summary>
    int InitializationPriority { get; }
    void Initialize();
}
