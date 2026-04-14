using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedGame : MonoBehaviour
{
    [SerializeField] float critChance = 0.2f;
    [SerializeField] float meanDamage = 20f;
    [SerializeField] float stdDevDamage = 5f;
    [SerializeField] float enemyHP = 100f;
    [SerializeField] float poissonLambda = 2f;
    [SerializeField] float hitRate = 0.6f;
    [SerializeField] float critDamageRate = 2f;
    [SerializeField] int maxHitsPerTurn = 5;

    int turn = 0;
    bool rareItemObtained = false;

    string[] rewards = { "Gold", "Weapon", "Armor", "Potion" };

    public void StartSimulation()
    {
        rareItemObtained = false;
        turn = 0;
        float currentBonus = 0f; // 누적될 추가 확률 (5%씩 증가)

        while (!rareItemObtained)
        {
            // 현재 확률 = 기본 20%(0.2f) + 누적 보너스
            float currentRareChance = 0.2f + currentBonus;

            // SimulateTurn에 현재 확률을 전달하거나 내부에서 참조하도록 설계
            // (SimulateTurn 내부 판정 로직에서 currentRareChance를 사용하게 변경해야 함)
            SimulateTurn(currentRareChance);

            if (rareItemObtained)
            {
                currentBonus = 0f; // 아이템 획득 시 확률 초기화
            }
            else
            {
                currentBonus += 0.05f; // 획득 실패 시 다음 턴 확률 5% 상승
            }

            turn++;
        }
        void SimulateTurn(float chance)
        {
            Debug.Log($"--- Turn {turn + 1} (현재 확률: {chance * 100f}%) ---");
            // ... (이하 로직에서 기존 0.2f 대신 전달받은 chance 사용)
            // 예: if (reward == "Weapon" && Random.value < chance)
        }

        Debug.Log($"레어 아이템 {turn} 턴에 획득");
    }

    void SimulateTurn()
    {
        Debug.Log($"--- Turn {turn + 1} ---");

        // 푸아송 샘플링: 적 등장 수
        int enemyCount = SamplePoisson(poissonLambda);
        Debug.Log($"적 등장 : {enemyCount}");

        for (int i = 0; i < enemyCount; i++)
        {
            // 이항 샘플링: 명중 횟수
            int hits = SampleBinomial(maxHitsPerTurn, hitRate);
            float totalDamage = 0f;

            for (int j = 0; j < hits; j++)
            {
                float damage = SampleNormal(meanDamage, stdDevDamage);

                // 베르누이 분포 샘플링: 확률 기반 치명타 발생
                if (Random.value < critChance)
                {
                    damage *= critDamageRate;
                    Debug.Log($" 크리티컬 hit! {damage:F1}");
                }
                else
                    Debug.Log($" 일반 hit! {damage:F1}");

                totalDamage += damage;
            }

            if (totalDamage >= enemyHP)
            {
                Debug.Log($"적 {i + 1} 처치! (데미지: {totalDamage:F1})");

                // 균등 분포 샘플링: 보상 결정
                string reward = rewards[UnityEngine.Random.Range(0, rewards.Length)];
                Debug.Log($"보상: {reward}");

                if (reward == "Weapon" && Random.value < 0.2f)
                {
                    rareItemObtained = true;
                    Debug.Log("레어 무기 획득!");
                }
                else if (reward == "Armor" && Random.value < 0.2f)
                {
                    rareItemObtained = true;
                    Debug.Log("레어 방어구 획득");
                }
            }
        }
    }

    // --- 분포 샘플 함수들 ---
    int SamplePoisson(float lambda)
    {
        int k = 0;
        float p = 1f;
        float L = Mathf.Exp(-lambda);
        while (p > L)
        {
            k++;
            p *= Random.value;
        }
        return k - 1;
    }

    int SampleBinomial(int n, float p)
    {
        int success = 0;
        for (int i = 0; i < n; i++)
            if (Random.value < p) success++;
        return success;
    }

    float SampleNormal(float mean, float stdDev)
    {
        float u1 = Random.value;
        float u2 = Random.value;
        float z = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Cos(2.0f * Mathf.PI * u2);
        return mean + stdDev * z;
    }
}