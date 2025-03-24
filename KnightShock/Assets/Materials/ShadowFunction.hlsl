// Inputs: 
//   UV (Vector2) - �⺻ UV ��ǥ (0~1 ����)
//   ShadowOffset (Vector2) - �׸��� ������ (UV ����)
//   Border (float) - UI ����� ��� �β� (SDF ����)
//   Smoothness (float) - �ε巯�� ���� ����
// Output:
//   ShadowMask (float) - �׸��� ����(0~1)
float ComputeShadowMask(float2 UV, float2 ShadowOffset, float Border, float Smoothness)
{
    // �׸��ڿ� UV ��ǥ (������ ����)
    float2 offsetUV = UV - ShadowOffset;
    
    // UI ��Ұ� �߾ӿ� ��ġ�� �ܼ� �簢���̶�� �����ϰ�,
    // �簢���� �߽��� (0.5, 0.5), ��ġ���� 0.5 (��ü ũ�� 1)
    // SDF ���: ���� �ȼ��� �簢�� ��迡�� �󸶳� ������ �ִ��� (������ ����)
    float2 d = abs(offsetUV - 0.5) - (0.5 - Border);
    float dist = length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
    
    // smoothstep�� �ε巯�� ��� ����: dist�� 0 �����̸� ������ ����, Smoothness �������� �ε巴�� ��ȯ
    float4 shadowMask = smoothstep(0.0, Smoothness, -dist);
    return shadowMask;
}
