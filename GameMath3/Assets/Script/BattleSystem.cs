using System.Collections; // 코루틴 사용을 위해 필요
using UnityEngine;
using TMPro;

public class BattleSystem : MonoBehaviour
{
    public GachaManager gachaManager;

    [Header("UI Reference")]
    public TMP_Text logText;
    public TMP_Text hpText;
    public GameObject enemyVisual;

    [Header("Settings")]
    public float playerAtk = 30f;
    public float critChance = 0.3f;
    public float critMultiplier = 2f;
    public float enemyMaxHp = 300f;

    private float currentEnemyHp;
    private bool isEnemyAlive = true;

    void Start()
    {
        ResetEnemy();
    }

    public void OnAttackButtonClick()
    {
        // 적이 죽었거나 부활 중이면 클릭 무시
        if (!isEnemyAlive) return;

        Attack();
    }

    void Attack()
    {
        bool isCrit = Random.value < critChance;
        float finalDamage = isCrit ? playerAtk * critMultiplier : playerAtk;

        currentEnemyHp -= finalDamage;

        if (isCrit)
            logText.text = $"<color=red><b>CRITICAL! -{finalDamage}</b></color>";
        else
            logText.text = $"-{finalDamage} HP";

        UpdateHPUI();

        if (currentEnemyHp <= 0)
        {
            StartCoroutine(DieAndRespawn()); // 코루틴 시작
        }
    }

    // 적이 죽고 다시 태어나는 과정을 관리하는 코루틴
    IEnumerator DieAndRespawn()
    {
        isEnemyAlive = false;
        currentEnemyHp = 0;
        UpdateHPUI();

        logText.text = "<color=yellow>적 처치! 보상 획득!</color>";

        // 가챠 실행 (여기서 에러가 나도 부활은 되도록 시도)
        if (gachaManager != null)
        {
            gachaManager.SimulateGachaSingle();
        }

        // 적 이미지 끄기
        if (enemyVisual != null) enemyVisual.SetActive(false);

        // --- 여기서 기다립니다 ---
        Debug.Log("부활 대기 시작...");
        yield return new WaitForSeconds(1.5f); // 1.5초 대기

        // --- 부활 로직 ---
        ResetEnemy();
        Debug.Log("적 부활 완료!");
    }

    void ResetEnemy()
    {
        currentEnemyHp = enemyMaxHp;
        isEnemyAlive = true;

        if (enemyVisual != null) enemyVisual.SetActive(true);

        logText.text = "새로운 적이 나타났습니다!";
        UpdateHPUI();
    }

    void UpdateHPUI()
    {
        if (hpText != null)
            hpText.text = $"Enemy HP: {Mathf.Max(0, currentEnemyHp)} / {enemyMaxHp}";
    }
}