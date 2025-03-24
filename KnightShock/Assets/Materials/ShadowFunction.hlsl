// Inputs: 
//   UV (Vector2) - 기본 UV 좌표 (0~1 범위)
//   ShadowOffset (Vector2) - 그림자 오프셋 (UV 단위)
//   Border (float) - UI 요소의 경계 두께 (SDF 계산용)
//   Smoothness (float) - 부드러운 에지 정도
// Output:
//   ShadowMask (float) - 그림자 영역(0~1)
float ComputeShadowMask(float2 UV, float2 ShadowOffset, float Border, float Smoothness)
{
    // 그림자용 UV 좌표 (오프셋 적용)
    float2 offsetUV = UV - ShadowOffset;
    
    // UI 요소가 중앙에 위치한 단순 사각형이라고 가정하고,
    // 사각형의 중심은 (0.5, 0.5), 반치수는 0.5 (전체 크기 1)
    // SDF 계산: 현재 픽셀이 사각형 경계에서 얼마나 떨어져 있는지 (음수면 내부)
    float2 d = abs(offsetUV - 0.5) - (0.5 - Border);
    float dist = length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
    
    // smoothstep로 부드러운 경계 생성: dist가 0 이하이면 완전히 내부, Smoothness 범위에서 부드럽게 전환
    float4 shadowMask = smoothstep(0.0, Smoothness, -dist);
    return shadowMask;
}
