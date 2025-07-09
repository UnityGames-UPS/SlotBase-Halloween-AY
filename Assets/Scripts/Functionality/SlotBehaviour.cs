using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class SlotBehaviour : MonoBehaviour
{
    [SerializeField]
    private RectTransform mainContainer_RT;

    [Header("Sprites")]
    [SerializeField]
    private Sprite[] myImages;

    [Header("Slot Images")]
    [SerializeField]
    private List<SlotImage> images;
    [SerializeField]
    private List<SlotImage> Tempimages;

    [Header("Slots Objects")]
    [SerializeField]
    private GameObject[] Slot_Objects;
    [Header("Slots Elements")]
    [SerializeField]
    private LayoutElement[] Slot_Elements;

    [Header("Slots Transforms")]
    [SerializeField]
    private Transform[] Slot_Transform;

    [Header("Line Button Objects")]
    [SerializeField]
    private List<GameObject> StaticLine_Objects;

    [Header("Buttons")]
    [SerializeField]
    private Button SlotStart_Button;
    [SerializeField]
    private Button MaxBet_Button;
    [SerializeField]
    private Button Lines_Button;
    [SerializeField]
    private Button AutoSpin_Button;
    [SerializeField] private Button AutoSpinStop_Button;
    [SerializeField] private Button BetOne_button;
    [SerializeField] private Button GambleButton;
    [SerializeField] private Button StopSpin_Button;
    [SerializeField] private Button Turbo_Button;
    [SerializeField] private Button Bet_plus;
    [SerializeField] private Button Bet_minus;

    [Header("Animated Sprites")]
    [SerializeField]
    private Sprite[] Symbol1;
    [SerializeField]
    private Sprite[] Symbol3;
    [SerializeField]
    private Sprite[] Symbol4;
    [SerializeField]
    private Sprite[] Symbol5;
    [SerializeField]
    private Sprite[] Symbol6;
    [SerializeField]
    private Sprite[] Symbol7;
    [SerializeField]
    private Sprite[] Symbol8;
    [SerializeField]
    private Sprite[] Symbol9;
    [SerializeField]
    private Sprite[] Symbol10;
    [SerializeField]
    private Sprite[] Symbol2;
    [SerializeField]
    private Sprite[] Symbol11;

    [Header("Miscellaneous UI")]
    [SerializeField]
    private int[] Lines_num;
    [SerializeField]
    private TMP_Text Balance_text;
    [SerializeField]
    private TMP_Text TotalBet_text;
    [SerializeField]
    private Image Lines_Image;
    [SerializeField]
    private TMP_Text TotalWin_text;
    [SerializeField] private TMP_Text BetperLine_text;
    [SerializeField]
    private Sprite AutoSpinHover_Sprite;
    [SerializeField]
    private Sprite AutoSpin_Sprite;
    [SerializeField]
    private Image AutoSpin_Image;
    [SerializeField]
    private TMP_Text Lines_text;

    [SerializeField]
    private Sprite[] Lines_Sprites;
    private int LineCounter = 0;

    [Header("Misc Animations")]
    [SerializeField]
    private GameObject GhostIdle_Object;
    [SerializeField]
    private GameObject GhostLaughing_Object;
    [SerializeField]
    private ImageAnimation GhostIdle_Anim;
    [SerializeField]
    private ImageAnimation GhostLaugh_Anim;

    private Dictionary<int, string> y_string = new Dictionary<int, string>();

    int tweenHeight = 0;

    [SerializeField]
    private GameObject Image_Prefab;

    [SerializeField]
    private PayoutCalculation PayCalculator;

    private List<Tweener> alltweens = new List<Tweener>();

    [SerializeField]
    private List<ImageAnimation> TempList;

    [SerializeField]
    private int IconSizeFactor = 100;
    [SerializeField] private int SpacingFactor;

    private int numberOfSlots = 5;

    [SerializeField]
    int verticalVisibility = 3;

    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private BonusController _bonusManager;
    [SerializeField]
    private SocketIOManager SocketManager;
    [SerializeField] private AudioController audioController;
    private GameObject Gamble;
    [SerializeField] private GambleController gambleController;

    private Tweener WinTween = null;
    private Coroutine AutoSpinRoutine = null;
    private Coroutine tweenroutine = null;
    internal bool IsAutoSpin = false;
    [SerializeField] private bool IsSpinning = false;
    internal int BetCounter = 0;
    internal bool CheckPopups = false;
    private bool CheckSpinAudio = false;
    private double currentBalance = 0;
    private double currentTotalBet = 0;

    private bool StopSpinToggle;
    private bool IsTurboOn;
    internal bool WasAutoSpinOn = false;
    private float SpinDelay = 0.2f;
    private Sprite turboOriginalSprite;
    private Tween  ScoreTween;

    private void Start()
    {

        if (SlotStart_Button) SlotStart_Button.onClick.RemoveAllListeners();
        if (SlotStart_Button) SlotStart_Button.onClick.AddListener(delegate { StartSlots(); });

        if (MaxBet_Button) MaxBet_Button.onClick.RemoveAllListeners();
        if (MaxBet_Button) MaxBet_Button.onClick.AddListener(MaxBet);

        if (Lines_Button) Lines_Button.onClick.RemoveAllListeners();
        if (Lines_Button) Lines_Button.onClick.AddListener(delegate { ToggleLine(); });

        if (Lines_Image) Lines_Image.sprite = Lines_Sprites[8];
        LineCounter = 0;

        if (GhostLaughing_Object) GhostLaughing_Object.SetActive(false);
        if (GhostIdle_Object) GhostIdle_Object.SetActive(true);

        if (AutoSpin_Button) AutoSpin_Button.onClick.RemoveAllListeners();
        if (AutoSpin_Button) AutoSpin_Button.onClick.AddListener(AutoSpin);

        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.RemoveAllListeners();
        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.AddListener(StopAutoSpin);

        if (BetOne_button) BetOne_button.onClick.RemoveAllListeners();
        if (BetOne_button) BetOne_button.onClick.AddListener(OnBetOne);

        if (StopSpin_Button) StopSpin_Button.onClick.RemoveAllListeners();
        if (StopSpin_Button) StopSpin_Button.onClick.AddListener(() => { StopSpinToggle = true; StopSpin_Button.gameObject.SetActive(false); if (audioController) audioController.PlayButtonAudio(); });

        if (Turbo_Button) Turbo_Button.onClick.RemoveAllListeners();
        if (Turbo_Button) Turbo_Button.onClick.AddListener(delegate {TurboToggle(); if (audioController) audioController.PlayButtonAudio();});

        if (Bet_plus) Bet_plus.onClick.RemoveAllListeners();
        if (Bet_plus) Bet_plus.onClick.AddListener(delegate { ChangeBet(true); });

        if (Bet_minus) Bet_minus.onClick.RemoveAllListeners();
        if (Bet_minus) Bet_minus.onClick.AddListener(delegate { ChangeBet(false); });

        tweenHeight = (15 * IconSizeFactor) - 280;
        turboOriginalSprite = Turbo_Button.GetComponent<Image>().sprite;
    }

    internal void AutoSpin()
    {
        if (!IsAutoSpin)
        {

            IsAutoSpin = true;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(true);
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(false);

            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                AutoSpinRoutine = null;
            }
            AutoSpinRoutine = StartCoroutine(AutoSpinCoroutine());

        }



    }

    internal void SetInitialUI()
    {
        BetCounter = 0;
        LineCounter = SocketManager.InitialData.lines.Count - 1;
        if (Lines_text) Lines_text.text = SocketManager.InitialData.lines[LineCounter].ToString();
       // PayCalculator.SetButtonActive(SocketManager.InitialData.lines[LineCounter]);         //hh
        if (TotalBet_text) TotalBet_text.text = (SocketManager.InitialData.bets[BetCounter] * SocketManager.InitialData.lines.Count).ToString();
        if (TotalWin_text) TotalWin_text.text = 0.ToString("f2");
        if (Balance_text) Balance_text.text = SocketManager.PlayerData.balance.ToString();
        if (BetperLine_text) BetperLine_text.text = (SocketManager.InitialData.bets[BetCounter]).ToString();
        currentBalance = SocketManager.PlayerData.balance;
        currentTotalBet = SocketManager.InitialData.bets[BetCounter] * SocketManager.InitialData.lines.Count;
        CompareBalance();

        uiManager.InitialiseUIData( SocketManager.UIData.paylines);
    }

    internal void StopAutoSpin()
    {
        if (IsAutoSpin)
        {
            IsAutoSpin = false;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(false);
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(true);
            StartCoroutine(StopAutoSpinCoroutine());
        }

    }
    internal void FetchLines(string LineVal, int count)
    {
        y_string.Add(count, LineVal);
    }

    private IEnumerator AutoSpinCoroutine()
    {

        while (IsAutoSpin)
        {
            StartSlots(IsAutoSpin);
            yield return tweenroutine;
            yield return new WaitForSeconds(SpinDelay);

        }
    }

    private IEnumerator StopAutoSpinCoroutine()
    {
        yield return new WaitUntil(() => !IsSpinning);
        ToggleButtonGrp(true);
        if (AutoSpinRoutine != null || tweenroutine != null)
        {
            StopCoroutine(AutoSpinRoutine);
            StopCoroutine(tweenroutine);
            tweenroutine = null;
            AutoSpinRoutine = null;
            StopCoroutine(StopAutoSpinCoroutine());
        }
    }

    void TurboToggle()
    {
        if (IsTurboOn)
        {
            IsTurboOn = false;
            Turbo_Button.GetComponent<ImageAnimation>().StopAnimation();
            Turbo_Button.image.sprite = turboOriginalSprite;
            //Turbo_Button.image.sprite = TurboToggleSprites[0];
            //Turbo_Button.image.color = new Color(0.86f, 0.86f, 0.86f, 1);
        }
        else
        {
            IsTurboOn = true;
            Turbo_Button.GetComponent<ImageAnimation>().StartAnimation();
            //Turbo_Button.image.color = new Color(1, 1, 1, 1);
        }
    }

    void OnBetOne()
    {
        if (audioController) audioController.PlayButtonAudio();

        if (BetCounter < SocketManager.InitialData.bets.Count - 1)
        {
            BetCounter++;
        }
        else
        {
            BetCounter = 0;
        }
        Debug.Log("Index:" + BetCounter);

        currentTotalBet = SocketManager.InitialData.bets[BetCounter] * SocketManager.InitialData.lines.Count;
        if (BetperLine_text) BetperLine_text.text = (SocketManager.InitialData.bets[BetCounter]).ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.InitialData.bets[BetCounter] * SocketManager.InitialData.lines.Count).ToString();
       // CompareBalance();

    }
    private void ChangeBet(bool IncDec)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (IncDec)
        {
            BetCounter++;
            if (BetCounter >= SocketManager.InitialData.bets.Count)
            {
                BetCounter = 0; // Loop back to the first bet
            }
        }
        else
        {
            BetCounter--;
            if (BetCounter < 0)
            {
                BetCounter = SocketManager.InitialData.bets.Count - 1; // Loop to the last bet
            }
        }

        Debug.Log("run this");
        // if (LineBet_text) LineBet_text.text = SocketManager.InitialData.bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.InitialData.bets[BetCounter] * SocketManager.InitialData.lines.Count).ToString();
        currentTotalBet = SocketManager.InitialData.bets[BetCounter] * SocketManager.InitialData.lines.Count;
        //CompareBalance();
    }
    
    private void MaxBet()
    {
        if (audioController) audioController.PlayButtonAudio();
        BetCounter = SocketManager.InitialData.bets.Count - 1;
        currentTotalBet = SocketManager.InitialData.bets[BetCounter] * SocketManager.InitialData.lines.Count;

        if (TotalBet_text) TotalBet_text.text = currentTotalBet.ToString();
        if (BetperLine_text) BetperLine_text.text = SocketManager.InitialData.bets[BetCounter].ToString();

        //if (currentTotalBet < currentBalance)
       // CompareBalance();
    }

    private void ToggleLine()
    {
        if (audioController) audioController.PlayButtonAudio();
        LineCounter++;
        if (LineCounter == SocketManager.InitialData.lines.Count)
        {
            LineCounter = 0;
        }
        if (Lines_text) Lines_text.text = SocketManager.InitialData.lines[LineCounter].ToString();
        if (Lines_Image) Lines_Image.sprite = Lines_Sprites[LineCounter];
        PayCalculator.DontDestroy.Clear();
        PayCalculator.ResetLines();
      //  PayCalculator.GeneratePayoutLinesBackend(-1, SocketManager.InitialData.lines[LineCounter]);           /hh
      //  PayCalculator.SetButtonActive(SocketManager.InitialData.lines[LineCounter]);                           //hh
    }

    

    internal void PopulateInitalSlots(int number, List<int> myvalues)
    {
        PopulateSlot(myvalues, number);
    }

    internal void LayoutReset(int number)
    {
        if (Slot_Elements[number]) Slot_Elements[number].ignoreLayout = true;
        if (SlotStart_Button) SlotStart_Button.interactable = true;
    }

    private void PopulateSlot(List<int> values, int number)
    {
        

        GenerateMatrix(number);
    }

    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        animScript.AnimationSpeed = 5;
        switch (val)
        {
            case 0:
                for (int i = 0; i < Symbol1.Length; i++)
                {
                    animScript.textureArray.Add(Symbol1[i]);
                }
                break;
            case 1:
                for (int i = 0; i < Symbol3.Length; i++)
                {
                    animScript.textureArray.Add(Symbol3[i]);
                }
                break;
            case 2:
                for (int i = 0; i < Symbol4.Length; i++)
                {
                    animScript.textureArray.Add(Symbol4[i]);
                }
                break;
            case 3:
                for (int i = 0; i < Symbol5.Length; i++)
                {
                    animScript.textureArray.Add(Symbol5[i]);
                }
                break;
            case 4:
                for (int i = 0; i < Symbol6.Length; i++)
                {
                    animScript.textureArray.Add(Symbol6[i]);
                }
                break;
            case 5:
                for (int i = 0; i < Symbol7.Length; i++)
                {
                    animScript.textureArray.Add(Symbol7[i]);
                }
                break;
            case 6:
                for (int i = 0; i < Symbol8.Length; i++)
                {
                    animScript.textureArray.Add(Symbol8[i]);
                }
                break;
            case 7:
                for (int i = 0; i < Symbol9.Length; i++)
                {
                    animScript.textureArray.Add(Symbol9[i]);
                }
                break;
            case 8:
                for (int i = 0; i < Symbol10.Length; i++)
                {
                    animScript.textureArray.Add(Symbol10[i]);
                }
                break;
            case 9:
                for (int i = 0; i < Symbol2.Length; i++)
                {
                    animScript.textureArray.Add(Symbol2[i]);
                }
                break;
            case 10:
                for (int i = 0; i < Symbol11.Length; i++)
                {
                    animScript.textureArray.Add(Symbol11[i]);
                    animScript.AnimationSpeed = 2;
                }
                break;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        audioController.CheckFocusFunction(focus, CheckSpinAudio);
    }

    private void StartSlots(bool autoSpin = false)
    {
        if (!autoSpin)
        {
            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                StopCoroutine(tweenroutine);
                tweenroutine = null;
                AutoSpinRoutine = null;
            }

        }
       
        StartCoroutine(GhostRoutine(true));

        if (GhostIdle_Anim) GhostIdle_Anim.StartAnimation();

        if (audioController) audioController.PlayButtonAudio("spin");
        WinningsAnim(false);
        if (SlotStart_Button) SlotStart_Button.interactable = false;
        if (TempList.Count > 0)
        {
            StopGameAnimation();
        }
        PayCalculator.ResetLines();
        tweenroutine = StartCoroutine(TweenRoutine());
    }


    internal void shuffleInitialMatrix()
    {
        for (int i = 0; i < Tempimages.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, 11);
                Tempimages[i].slotImages[j].sprite = myImages[randomIndex];
            }
        }
    }

    private IEnumerator TweenRoutine()
    {

        if (currentBalance < currentTotalBet)
        {
            CompareBalance();
            if (IsAutoSpin)
            {
                StopAutoSpin();
                yield return new WaitForSeconds(1f);
            }
            ToggleButtonGrp(true);
            yield break;
        }
        IsSpinning = true;
        ToggleButtonGrp(false);

        if (!IsTurboOn  && !IsAutoSpin)
        {
            StopSpin_Button.gameObject.SetActive(true);
        }

        CheckSpinAudio = true;
        gambleController.toggleDoubleButton(false);
        gambleController.GambleTweeningAnim(false);
        if (audioController) audioController.PlaySpinBonusAudio();
        for (int i = 0; i < numberOfSlots; i++)
        {
            InitializeTweening(Slot_Transform[i]);
            yield return new WaitForSeconds(0.1f);
        }

        double bet = 0;
        double balance = 0;
        try
        {
            bet = double.Parse(TotalBet_text.text);
        }

        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        try
        {
            balance = double.Parse(Balance_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        double initAmount = balance;
        balance = balance - bet;

        ScoreTween = DOTween.To(() => initAmount, (val) => initAmount = val, balance, 0.8f).OnUpdate(() =>
        {
            if (Balance_text) Balance_text.text = initAmount.ToString("f3");
        });

       

        SocketManager.AccumulateResult(BetCounter);

        yield return new WaitUntil(() => SocketManager.isResultdone);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int resultNum = int.Parse(SocketManager.ResultData.matrix[i][j]);
                
                PopulateAnimationSprites(Tempimages[j].slotImages[i].GetComponent<ImageAnimation>(), resultNum);
                Tempimages[j].slotImages[i].GetComponent<Image>().sprite = myImages[resultNum];
            }
        }
        CheckForFeaturesAnimation();
        if (IsTurboOn )                                                      // changes
        {

            yield return new WaitForSeconds(0.1f);
            StopSpinToggle = true;
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(0.1f);
                if (StopSpinToggle)
                {
                    break;
                }
            }
            StopSpin_Button.gameObject.SetActive(false);
        }

        for (int i = 0; i < numberOfSlots; i++)
        {
            yield return StopTweening(5, Slot_Transform[i], i, StopSpinToggle);
        }

        StopSpinToggle = false;

        //yield return new WaitForSeconds(0.3f);

        yield return alltweens[^1].WaitForCompletion();
        // yield return new WaitForSeconds(0.3f);
        if (SocketManager.ResultData.payload.winAmount > 0)
        {
            SpinDelay = 2f;
        }
        else
        {
            SpinDelay = 0.2f;
        }

        if (audioController) audioController.StopApinBonusAudio();
        if (GhostIdle_Anim) GhostIdle_Anim.StopAnimation();
        if (SocketManager.ResultData.payload.winAmount > 0)
        {
            List<int> winLine = new();
            foreach (var item in SocketManager.ResultData.payload.wins)
            {
                winLine.Add(item.line);
            }
            CheckPayoutLineBackend(winLine);
            //  if (m_Gamble_Button) m_Gamble_Button.interactable = true;
        }
        KillAllTweens();

        //yield return new WaitForSeconds(2.5f);
        CheckPopups = true;

        //audioController.StopGhostAudio();
        currentBalance = SocketManager.PlayerData.balance;

        if (SocketManager.ResultData.bonus.istriggered)
        {
            //_bonusManager.GetCaseList(SocketManager.ResultData.bonus.result, SocketManager.InitialData.bets[BetCounter]);
            _bonusManager.StartBonusGame();
        }
        else if (SocketManager.ResultData.payload.winAmount >= bet * 10 && SocketManager.ResultData.payload.winAmount < bet * 15)
        {
            uiManager.PopulateWin(1, SocketManager.ResultData.payload.winAmount);
        }
        else if (SocketManager.ResultData.payload.winAmount >= bet * 15 && SocketManager.ResultData.payload.winAmount < bet * 20)
        {
            uiManager.PopulateWin(2, SocketManager.ResultData.payload.winAmount);
        }
        else if (SocketManager.ResultData.payload.winAmount >= bet * 20)
        {
            uiManager.PopulateWin(3, SocketManager.ResultData.payload.winAmount);
        }
        else if (SocketManager.ResultData.payload.winAmount > 0 && SocketManager.ResultData.payload.winAmount < bet * 10)
        {
            WinningsAnim(true);
            GhostIdle_Anim.gameObject.SetActive(false);
            if (GhostLaugh_Anim) GhostLaugh_Anim.gameObject.SetActive(true);
            GhostLaugh_Anim.StartAnimation();
            audioController.PlayWLAudio("win", 1.25f);
            audioController.PlayGhostAudio(1.3f);
            StartCoroutine(GhostRoutine(false));
            CheckPopups = false;
        }
        else
        {
            CheckPopups = false;
        }

        yield return new WaitUntil(() => !CheckPopups);

        ScoreTween?.Kill();
        if (TotalWin_text) TotalWin_text.text = SocketManager.ResultData.payload.winAmount.ToString("f3");
        if (Balance_text) Balance_text.text = SocketManager.PlayerData.balance.ToString("f3");
        CheckAndActivateGamble();
        if (!IsAutoSpin)
        {
            IsSpinning = false;
            ToggleButtonGrp(true);
        }
        else
        {
            if (IsTurboOn) yield return new WaitForSeconds(1f);
            else yield return new WaitForSeconds(2f);                                   // changes
            IsSpinning = false;
        }
    }

    IEnumerator GhostRoutine(bool immediate){
            if(!immediate)
            yield return new WaitForSeconds(3f);
            audioController.StopWLAaudio();
            GhostLaugh_Anim.StopAnimation();
            GhostLaugh_Anim.gameObject.SetActive(false);
            GhostIdle_Anim.gameObject.SetActive(true);
            audioController.StopGhostAudio();
    }




    void CheckAndActivateGamble()
    {
        if (SocketManager.ResultData.payload.winAmount > 0)
        {
            gambleController.gambleAmount = SocketManager.ResultData.payload.winAmount;
            gambleController.toggleDoubleButton(true);
            gambleController.GambleTweeningAnim(true);
        }

    }
    private void CheckForFeaturesAnimation()
    {
        bool playJackpot = false;
        bool playScatter = false;
        bool playBonus = false;
        bool playFreespin = false;
       
        
        if (SocketManager.ResultData.bonus.istriggered)
        {
            playBonus = true;
        }
        
        PlayFeatureAnimation(playJackpot, playScatter, playBonus, playFreespin);
    }
    private void PlayFeatureAnimation(bool jackpot = false, bool scatter = false, bool bonus = false, bool freeSpin = false)
    {
        for (int i = 0; i < SocketManager.ResultData.matrix.Count; i++)
        {
            for (int j = 0; j < SocketManager.ResultData.matrix[i].Count; j++)
            {

                if (int.TryParse(SocketManager.ResultData.matrix[i][j], out int parsedNumber))
                {
                                  
                    if (bonus && parsedNumber == 9)
                    {
                        StartGameAnimation(Tempimages[j].slotImages[i].gameObject);
                    }
                  
                }

            }
        }
    }
    internal void DeactivateGamble()
    {
        StopAutoSpin();
        ToggleButtonGrp(true);
    }

    internal void CallCloseSocket()
    {
        SocketManager.CloseSocket();
    }


    internal void updateBalance()
    {
        // if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString();
        if (Balance_text) Balance_text.text = SocketManager.PlayerData.balance.ToString("f3");
        if (TotalWin_text) TotalWin_text.text = SocketManager.ResultData.payload.winAmount.ToString("f3");
    }

    private void CompareBalance()
    {
        if (currentBalance < currentTotalBet)
        {
            uiManager.LowBalPopup();
            //if (AutoSpin_Button) AutoSpin_Button.interactable = false;
            //if (SlotStart_Button) SlotStart_Button.interactable = false;
        }
        //else
        //{
        //    if (AutoSpin_Button) AutoSpin_Button.interactable = true;
        //    if (SlotStart_Button) SlotStart_Button.interactable = true;
        //}
    }

    private void WinningsAnim(bool IsStart)
    {
        if (IsStart)
        {
            WinTween = TotalWin_text.gameObject.GetComponent<RectTransform>().DOScale(new Vector2(1.5f, 1.5f), 1f).SetLoops(-1, LoopType.Yoyo).SetDelay(0);
        }
        else
        {
            WinTween.Kill();
            TotalWin_text.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }
    void ToggleButtonGrp(bool toggle)
    {
        if (SlotStart_Button) SlotStart_Button.interactable = toggle;
        if (Lines_Button) Lines_Button.interactable = toggle;
        if (AutoSpin_Button) AutoSpin_Button.interactable = toggle;
        if (MaxBet_Button) MaxBet_Button.interactable = toggle;
        if (BetOne_button) BetOne_button.interactable = toggle;

        if (Bet_plus) Bet_plus.interactable = toggle;
        if (Bet_minus) Bet_minus.interactable = toggle;
        //if (!IsSpinning || !IsAutoSpin)
        //{
        //    if (Bet_plus) Bet_plus.interactable = true;
        //    if (Bet_minus) Bet_minus.interactable = true;
        //}
        //else
        //{
        //    if (MaxBet_Button) MaxBet_Button.interactable = toggle;
        //    if (BetOne_button) BetOne_button.interactable = toggle;
        //    if (Bet_plus) Bet_plus.interactable = toggle;
        //    if (Bet_minus) Bet_minus.interactable = toggle;

        //}
    }


    private void StartGameAnimation(GameObject animObjects)
    {
        int i = animObjects.transform.childCount;

        if (i > 0)
        {
            ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();
            animObjects.transform.GetChild(0).gameObject.SetActive(true);

            temp.StartAnimation();

            TempList.Add(temp);
        }
        else
        {
            animObjects.GetComponent<ImageAnimation>().StartAnimation();

        }
    }
    private void StopGameAnimation()
    {
        for (int i = 0; i < TempList.Count; i++)
        {
            TempList[i].StopAnimation();
            if (TempList[i].transform.childCount > 0)
                TempList[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void CheckPayoutLineBackend(List<int> LineId, double jackpot = 0)
    {
        List<int> y_points = null;
        if (LineId.Count > 0)
        {
            if (jackpot <= 0)
            {
                if (audioController) audioController.PlayWLAudio("win");
            }

            for (int i = 0; i < LineId.Count; i++)
            {
                y_points = y_string[LineId[i]]?.Split(',')?.Select(Int32.Parse)?.ToList();
                PayCalculator.GeneratePayoutLinesBackend(LineId[i]);
            }

            if (jackpot > 0)
            {
                if (audioController) audioController.PlayWLAudio("megaWin");
                for (int i = 0; i < Tempimages.Count; i++)
                {
                    for (int k = 0; k < Tempimages[i].slotImages.Count; k++)
                    {
                        StartGameAnimation(Tempimages[i].slotImages[k].gameObject);
                    }
                }
            }
            else
            {
                List<KeyValuePair<int, int>> coords = new();
                for (int j = 0; j < LineId.Count; j++)
                {
                    for (int k = 0; k < SocketManager.ResultData.payload.wins[j].positions.Count; k++)
                    {
                        int rowIndex = SocketManager.InitialData.lines[LineId[j]][k];
                        int columnIndex = k;
                        coords.Add(new KeyValuePair<int, int>(rowIndex, columnIndex));
                    }
                }

                foreach (var coord in coords)
                {
                    int rowIndex = coord.Key;
                    int columnIndex = coord.Value;
                    StartGameAnimation(Tempimages[columnIndex].slotImages[rowIndex].gameObject);
                }
            }
            WinningsAnim(true);
        }
        else
        {

            //if (audioController) audioController.PlayWLAudio("lose");
            if (audioController) audioController.StopWLAaudio();
        }
        CheckSpinAudio = false;
    }

    private void GenerateMatrix(int value)
    {
        for (int j = 0; j < 3; j++)
        {
            Tempimages[value].slotImages.Add(images[value].slotImages[images[value].slotImages.Count - 5 + j]);
        }
    }

    #region TweeningCode
    private void InitializeTweening(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        Tweener tweener = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener.Play();
        alltweens.Add(tweener);
    }

    private IEnumerator StopTweening(int reqpos, Transform slotTransform, int index, bool isStop)
    {
        alltweens[index].Pause();
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        alltweens[index] = slotTransform.DOLocalMoveY(-tweenpos - 8 + 100, 0.5f).SetEase(Ease.OutElastic);
        if (!isStop)
        {
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            yield return null;
        }
    }


    private void KillAllTweens()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();

    }
    #endregion
    //internal void OnBonusCofinClicked(int index)
    //{
    //    SocketManager.OnBonusCollect(index);
    //}

    internal void GambleCollect()
    {
        SocketManager.OnCollect();           //hh
    }
}

[Serializable]
public class SlotImage
{
    public List<Image> slotImages = new List<Image>(10);
}

