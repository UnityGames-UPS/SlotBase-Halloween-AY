using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BonusController : MonoBehaviour
{
    [SerializeField]
    private GameObject Bonus_Object;
    [SerializeField]
    private SlotBehaviour slotManager;
    [SerializeField]
    private List<CoffinGame> BonusCases;
    [SerializeField]
    private AudioController _audioManager;

    [SerializeField]
    private List<double> CaseValues;

    [SerializeField] private TMP_Text TotalWin_text;

    internal bool isOpening;
    internal bool isFinished;
    private double totalWin;
    internal double bet { get; private set; }
    internal bool WaitForBonusResult = true;
    internal void StartBonusGame() // List<double> values, double betperline
    {
        //CaseValues.Clear();
        //CaseValues.TrimExcess();
        //CaseValues = values;
        //bet = betperline;


        StartBonus();
    }

    internal void setTotalWin(double amount) {
        totalWin += amount;
        if (TotalWin_text) TotalWin_text.text = totalWin.ToString("f3");
    }

    internal void GameOver()
    {
        slotManager.CheckPopups = false;
        _audioManager.playBgAudio("normal");
        foreach (CoffinGame cases in BonusCases)
        {
            cases.ResetCase();
        }
        isFinished = false;
        TotalWin_text.text = "0.000";
        totalWin = 0;
        if (Bonus_Object) Bonus_Object.SetActive(false);
    }

    //internal double GetValue(int x)
    //{
    //    //slotManager.OnBonusCofinClicked(x);
    //    double value = CaseValues[x];
    //    return value;
    //}

    

    private void StartBonus()
    {
        _audioManager.playBgAudio("bonus");
        if (Bonus_Object) Bonus_Object.SetActive(true);
    }
}
