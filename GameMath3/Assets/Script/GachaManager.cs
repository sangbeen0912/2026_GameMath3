using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GachaManager : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Text gachaInfoText;
    public TMP_Text inventoryText;

    private float nProb = 0.5f, rProb = 0.3f, eProb = 0.15f, lProb = 0.05f;
    private List<string> droppedItems = new List<string>();

    void Start()
    {
        UpdateUI();
    }

    // 이 메서드가 BattleSystem에서 호출되어야 합니다!
    public void SimulateGachaSingle()
    {
        string result = Simulate();

        // 아이템 드롭 리스트에 추가
        string coloredResult = GetColoredRank(result);
        droppedItems.Insert(0, coloredResult);

        // 확률 변동 후 즉시 UI 업데이트
        UpdateUI();
        Debug.Log($"아이템 획득: {result} / 현재 전설 확률: {lProb * 100}%");
    }

    string Simulate()
    {
        float r = Random.value;
        string result = "";

        // 확률 누적 계산 (안전한 방식)
        if (r < nProb) result = "Normal";
        else if (r < nProb + rProb) result = "Rare";
        else if (r < nProb + rProb + eProb) result = "Epic";
        else result = "Legendary";

        if (result == "Legendary") ResetProbabilities();
        else UpdateProbabilities();

        return result;
    }

    void UpdateProbabilities()
    {
        lProb += 0.015f; // 전설 +1.5%
        nProb -= 0.005f;
        rProb -= 0.005f;
        eProb -= 0.005f;

        // 확률이 0 이하로 내려가지 않게 방지
        nProb = Mathf.Max(0, nProb);
        rProb = Mathf.Max(0, rProb);
        eProb = Mathf.Max(0, eProb);
    }

    void ResetProbabilities()
    {
        nProb = 0.5f; rProb = 0.3f; eProb = 0.15f; lProb = 0.05f;
    }

    void UpdateUI()
    {
        if (gachaInfoText != null)
        {
            gachaInfoText.text = $"<b>[실시간 확률]</b>\n" +
                $"Normal: {nProb * 100:F1}%\n" +
                $"Rare: {rProb * 100:F1}%\n" +
                $"Epic: {eProb * 100:F1}%\n" +
                $"<color=#FFD700>Legendary: {lProb * 100:F1}%</color>";
        }

        if (inventoryText != null)
        {
            string invStr = "<b>[획득한 아이템]</b>\n";
            for (int i = 0; i < Mathf.Min(droppedItems.Count, 10); i++)
            {
                invStr += $"- {droppedItems[i]}\n";
            }
            inventoryText.text = invStr;
        }
    }

    string GetColoredRank(string rank)
    {
        switch (rank)
        {
            case "Normal": return "<color=#FFFFFF>Normal</color>"; // 흰색
            case "Rare": return "<color=#0000FF>Rare</color>";     // 파랑색
            case "Epic": return "<color=#800080>Epic</color>";     // 보라색
            case "Legendary": return "<color=#FFD700>Legendary</color>"; // 금색
            default: return rank;
        }
    }
}