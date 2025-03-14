using UnityEngine;

/// <summary>
/// 씬이 로드되는 순간 비활성화된 오브젝트의 데이터를 Init해야되는 오브젝트에 부착
/// </summary>
public interface IInitializable
{
    /// <summary>
    /// 낮을수록 우선순위가 높음(낮은 숫자부터 init됨)
    /// </summary>
    int InitializationPriority { get; }

    /// <summary>
    /// 씬이 로드될 때 호출
    /// </summary>
    void Initialize();
}
