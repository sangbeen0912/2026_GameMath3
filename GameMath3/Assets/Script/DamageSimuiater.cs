using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


public class DamageSimuiater : MonoBehaviour
{
    public TextMeshProUGUI statusDisplay;
    public TextMeshProUGUI logDisplay;
    public TextMeshProUGUI resultDisplay;
    public TextMeshProUGUI rangeDisplay;

    private int level = 1;
    private float totalDamage = 0, baseDamage = 20f;
    private int attackCount = 0;

    private string weaponName;
    private float stdDevMult, critRate, critMult;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetWeapon(0); //시작시 단검 장착
    }
    private void ResetData()
    {
        totalDamage = 0;
        attackCount = 0;
        level = 1;
        baseDamage = 20f;
    }
    public void SetWeapon(int id)
    {
        ResetData();
        if(id == 0)
        {
            SetStats("단검", 0.1f, 0.4f, 1.5f);
        }
        else if (id == 1)
        {
            SetStats("장검", 02f, 0.3f, 2.0f);
        }
        else
        {
            SetStats("도끼", 0.3f, 0.2f, 3.0f);
        }

        logDisplay.text = string.Format("{0} 장착!", weaponName);
        UpdateUI();
    }

    private void SetStats(string _name, float _stdDev, float _critRate, float _critMult)
    {
        weaponName = _name;
        stdDevMult = _stdDev;
        critRate = _critRate;
        critMult = _critMult;

    }

    public void LevelUp()
    {
        totalDamage = 0;
        attackCount = 0;
        level++;
        logDisplay.text = string.Format("레벨업! 현재 레벨:{0}", level);
        UpdateUI();

    }
    public void OnAttack()
    {
        // 정규 분포 데미지 계산
        float sd = baseDamage * stdDevMult;
        float normalDamage = GetNormalStdDevDamage(baseDamage, sd);

        // 치명타 판정
        bool isCrit = Random.value < critRate;
        float finalDamage = isCrit ? normalDamage * critMult : normalDamage;

        //통계 누적
        attackCount++;
        totalDamage += finalDamage;

        //로그 및 UI 업데이트
        string critMark = isCrit ? "<color = red>[치명타!]</color>" : "";
        logDisplay.text = string.Format("{0} 데미지: {1:F1}", critMark, finalDamage);
        UpdateUI();
    }

    public float GetDamage()
    {   
        float sd = baseDamage * stdDevMult;
        float normalDamage = GetNormalStdDevDamage(baseDamage, sd);
        bool isCrit = Random.value < critRate;
        return isCrit ? normalDamage * critMult : normalDamage;
    }

    private void UpdateUI()
    {
        statusDisplay.text = string.Format("Level: {0} / 무기: {1}\n기본 데미지:{2} / 치명타: {3}%(x{4})",
            level, weaponName, baseDamage, critRate * 100, critMult);
        rangeDisplay.text = string.Format("예상 일반 데미지 범위: [{0:F1} ~ {1:F1}]",
            baseDamage - (3 * baseDamage * stdDevMult),
            baseDamage + (3 * baseDamage * stdDevMult));
        float dpa = attackCount > 0 ? totalDamage / attackCount : 0;
        resultDisplay.text = string.Format("누적 데미지 :{0:F1}\n공격 횟수: {1}\n평균DPA :{2:F2}",
            totalDamage, attackCount, dpa);
    }

    private float GetNormalStdDevDamage(float mean, float stdDev)
    {
        float u1 = 1.0f - Random.value;
        float u2 = 1.0f - Random.value;
        float randstdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1))
                            * Mathf.Sin(2.0f * Mathf.PI * u2);

        float raw = mean + stdDev * randstdNormal;

        // -2σ 미만 → MISS
        if (raw < mean - 2 * stdDev)
        {
            logDisplay.text = "<color=red> [빗나감!] </color>";
            return 0f;
        }

        // +2σ 초과 → 약점 공격 (2배)
        if (raw > mean + 2 * stdDev)
        {
            logDisplay.text = "<color=yellow>[약점 공격!]</color>";
            raw *= 2f;
        }

        return raw;
    }
    public void OnAttack1000()
    {
        float sd = baseDamage * stdDevMult;

        int critCount = 0;
        int missCount = 0;

        for (int i = 0; i < 1000; i++)
        {
            float normalDamage = GetNormalStdDevDamage(baseDamage, sd);

            // MISS 체크 (0이면 MISS로 간주)
            if (normalDamage <= 0f)
            {
                missCount++;
            }

            bool isCrit = Random.value < critRate;
            if (isCrit) critCount++;

            float finalDamage = isCrit ? normalDamage * critMult : normalDamage;

            totalDamage += finalDamage;
            attackCount++;
        }

        float dpa = attackCount > 0 ? totalDamage / attackCount : 0;

        logDisplay.text = string.Format(
            "<color=yellow>[1000회 공격 완료]</color>\n치명타: {0}회 / MISS: {1}회\n평균 DPA: {2:F2}",
            critCount, missCount, dpa);

        UpdateUI();
    }
}
