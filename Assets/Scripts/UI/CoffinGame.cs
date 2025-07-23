using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoffinGame : MonoBehaviour
{
    [SerializeField] private Button Coffin;
    [SerializeField] private Color32 text_color;
    [SerializeField] private TMP_Text text;
    [SerializeField] private ImageAnimation imageAnimation;
    [SerializeField] private BonusController _bonusManager;
    [SerializeField] private SocketIOManager SocketManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject Skeliton;
    [SerializeField] private AudioController audioController;
    [SerializeField] private double value = 0;

    [SerializeField]
    int index =0;
    //internal bool isOpen;

    void Start()
    {
        if (Coffin) Coffin.onClick.RemoveAllListeners();
        if (Coffin) Coffin.onClick.AddListener(OpenCase);
    }

    internal void ResetCase()
    {
        //isOpen = false;
        text.gameObject.SetActive(false);
        if (Skeliton.gameObject.activeSelf) Skeliton.gameObject.SetActive(false);
        imageAnimation.gameObject.SetActive(true);
        Coffin.interactable = true;
    }

    void OpenCase()
    {
        //if (isOpen) return;
        if (_bonusManager.isOpening) return;
        if (_bonusManager.isFinished) return;

        Coffin.interactable = false;
        //PopulateCase();
        //imageAnimation.StartAnimation();
        StartCoroutine(setCase());
    }

    //void PopulateCase()
    //{
    //    value = _bonusManager.GetValue(index);
    //    if (value == 0)
    //    {
    //        text.text = "game over";
    //    }

    //    else
    //    {
    //        text.text = string.Concat("You Won \n\n", (_bonusManager.bet*value).ToString());
    //    }
    //}

    IEnumerator setCase()
    {
        _bonusManager.isOpening = true;
        audioController.PlaySpinBonusAudio("bonus");
        _bonusManager.WaitForBonusResult = true;
        SocketManager.OnBonusCollect(index);

        Tween tween = transform.DOShakePosition(1f, new Vector3(15, 0, 0), 30, 90, true).SetLoops(-1, LoopType.Incremental);
        yield return new WaitUntil(() => !_bonusManager.WaitForBonusResult);
       
        tween.Kill();
        imageAnimation.StartAnimation();
        yield return new WaitUntil(() => !imageAnimation.isplaying);
        yield return new WaitForSeconds(0.3f);

        if (SocketManager.BonusData.payload.payout > 0)
        {
            audioController.PlayWLAudio("bonuswin");
            text.text = string.Concat("You Won \n\n", SocketManager.BonusData.payload.winAmount.ToString("F2"));
        }
        else
        {
            text.text = "Game Over";
            audioController.PlayWLAudio("bonuslose");
        }

        text.gameObject.SetActive(true);
        text.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, text_color);
        _bonusManager.isOpening = false;

        _bonusManager.setTotalWin(SocketManager.BonusData.payload.winAmount);

        if (SocketManager.BonusData.payload.payout == 0)
        {
            SocketManager.ResultData.payload.winAmount = SocketManager.BonusData.payload.winAmount;
            _bonusManager.isFinished = true;
            Skeliton.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            _bonusManager.GameOver();
        }
        
    }
}
