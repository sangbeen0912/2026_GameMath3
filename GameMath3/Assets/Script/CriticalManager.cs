using UnityEngine;

public class CriticalManager : MonoBehaviour
{
    public int totalHits = 0;
    public int critHits = 0;
    public float targetRate = 0.3f; // 30% critical hit rate

    public bool RollCrit()
    {
        totalHits++;
        float currentRate = 0f;

        // [수정 1] totalHits가 이미 1 증가했으므로, 
        // 이전까지의 확률을 계산하려면 (totalHits - 1)로 나누거나 
        // 비교 로직을 조정해야 합니다. 여기선 안전하게 0 체크를 추가했습니다.
        if (totalHits > 1)
        {
            currentRate = (float)critHits / (totalHits - 1);
        }

        // [수정 2] 강제 성공 조건문
        // 기존 조건은 너무 엄격해서 초반에 확률이 한 번만 튀어도 작동하지 않습니다.
        if (currentRate < targetRate && (float)(critHits + 1) / totalHits <= targetRate)
        {
            Debug.Log("Critical Hit! (forced)");
            critHits++;
            return true;
        }

        // [수정 3] 강제 실패 조건문
        if (currentRate > targetRate && (float)critHits / totalHits >= targetRate)
        {
            Debug.Log("Normal Hit! (Forced)");
            return false;
        }

        // [수정 4] 기본 확률 로직 및 반환 값 추가
        if (Random.value < targetRate)
        {
            Debug.Log("Critical Hit!,Base");
            critHits++;
            return true;
        }

        Debug.Log("Normal Hit!,Base");
        return false; // <-- 이 부분(반환)이 없어서 에러가 났던 것입니다.
    }
}
