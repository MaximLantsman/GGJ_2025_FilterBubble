using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

// ReSharper disable InconsistentNaming

public class NewsManagerScript : MonoBehaviour
{
    [SerializeField, Anywhere] private NewsScriptable passedArticle;
    [SerializeField, Anywhere] private Animator handAnimator;
    [SerializeField, Anywhere] private Image articlePhoneSpace;
    [SerializeField, Anywhere] private FillUpBrain bubbleFill;
    [SerializeField, Anywhere] private EndGameScreen endGameScreen;
    [SerializeField, Anywhere] private WinGameScreen winGameScreen;
    [SerializeField, Anywhere] private ScreenSlideUI screenUIMove;
    [SerializeField] private InputReader input;
    
    #region News Article Scriptable Lists
    [SerializeField] private List<NewsScriptable> newsListRed = new List<NewsScriptable>();
    [SerializeField] private List<NewsScriptable> newsListBlue = new List<NewsScriptable>();
    [SerializeField] private List<NewsScriptable> newsListGreen = new List<NewsScriptable>();
    [SerializeField] private List<NewsScriptable> newsListYellow = new List<NewsScriptable>();
    #endregion
    
    [SerializeField] private float directionThreshold = 300f;
    [SerializeField] private float changePercentileValue = 0.02f;
    [SerializeField] private float changePercentileValueOnPass = 0.01f;
    
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip winSound;

    #region Lose Conditions
    [SerializeField] private float fakeThreshold = 0.5f;
    [SerializeField] private float filterThemeThreshold = 0.2f;
    #endregion
    
    #region Bubble Spawn Numbers
    [SerializeField] private int beginSpawn = 5;
    #endregion
    
    #region Tutorial Images
    [SerializeField] private Sprite tutorialImageLeft, tutorialImageRight, tutorialImageFun, tutorialImageFake, tutorialImageFakePass;
    #endregion
    
    private float percentRedArticles = 0.25f;
    private float percentBlueArticles = 0.25f;
    //private float percentGreenArticles = 0.25f;
    private float percentYellowArticles = 0.50f;

    private Dictionary<List<NewsScriptable>, float> newsDictionary = new Dictionary<List<NewsScriptable>, float>();
    private int articleCategories = 3;
    private int currentChosenCategory;

    private int timeBetweenPosts = 1000;
    private bool isWaiting = true;
    private bool isEndGame = false;
    private NewsScriptable currentArticle;

    private Vector2 startPos;
    private Vector2 endPos;

    private readonly int handAnimationSwipe = Animator.StringToHash("Swipe");
    private readonly int handAnimationPass = Animator.StringToHash("Pass");

    private const int Zero = 0;
    private const int OneHundred = 100;
    private const int threeHundred = 300;
    private const int thuasandMiliseconds = 1000;
    private const int waitBetweenTutorials = 3000;
    private const int handAnimationDelay = 500;
    private const int lastTutorialLinger = 6000;

    private void Start()
    {
        BuildNewsDictionary();
        
        bubbleFill.notifyOnGameWin.AddListener(WonGame);

        bubbleFill.TutorialBubbleCreation(articleCategories);
        
        input.EnablePlayerActions();
    }
    
    public void StartTutorial()
    {
        SetUpFilterBubble();
    }
    
    private void StartGame()
    {
        PullNewsToPhone();
        
        screenUIMove.MoveUI(false, currentArticle.newsImage);

        isWaiting = false;
    }

    private void OnEnable()
    {
        input.ClickStart += FirstClickToStartGame;
        input.ClickEnd += OnClickEnd;
    }

    private void OnDisable()
    {
        input.ClickStart -= OnClickStart;
        input.ClickEnd -= OnClickEnd;
        input.ClickEnd -= FirstClickToStartGame;
    }

    private void FirstClickToStartGame()
    {
        StartTutorial();
        
        input.ClickStart -= FirstClickToStartGame;
        input.ClickStart += OnClickStart;
    }

    private void OnClickStart()
    {
        startPos = input.Position;
    }

    private void OnClickEnd()
    {
        endPos = input.Position;
        if (!isWaiting && !isEndGame)
        {
            SwipedDirection();
        }
    }

    private void SwipedDirection()
    {
        Vector2 direction = endPos - startPos;

       

        if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            ChosenArticle(true);
            handAnimator.Play(handAnimationSwipe);
        }

        if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            ChosenArticle(false);
            handAnimator.Play(handAnimationPass);
        }
        
        /*if (Vector2.Dot(Vector2.up, direction) > directionThreshold)
       {
           Debug.Log("up");
       }*/

        /*if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            Debug.Log("down");
        }*/
    }
    
    //Fillter with bubbles and percents together
    private async void SetUpFilterBubble() 
    {
        handAnimator.Play(handAnimationSwipe);
        screenUIMove.MoveUI(true, tutorialImageLeft);
        await UniTask.Delay(waitBetweenTutorials, cancellationToken: this.GetCancellationTokenOnDestroy());
        
        //First Category - Left wing blue
        ExplanationAnimation(tutorialImageRight,newsListBlue);
        await UniTask.Delay(waitBetweenTutorials, cancellationToken: this.GetCancellationTokenOnDestroy());
        
        //Second Category - right wing red
        //ExplanationAnimation(tutorialImageFun,newsListRed); ----needed for green
        ExplanationAnimation(tutorialImageFake,newsListRed);
        await UniTask.Delay(waitBetweenTutorials, cancellationToken: this.GetCancellationTokenOnDestroy());
        
        //fourth - fix fake
        ExplanationAnimation(tutorialImageFakePass,newsListYellow);
        await UniTask.Delay(lastTutorialLinger, cancellationToken: this.GetCancellationTokenOnDestroy());
        
        //third category - fake
        handAnimator.Play(handAnimationPass);
        screenUIMove.MoveUI(false, tutorialImageFake);
        await UniTask.Delay(waitBetweenTutorials, cancellationToken: this.GetCancellationTokenOnDestroy());
        
        handAnimator.Play(handAnimationPass);
        StartGame();
    }
   
    private async void ExplanationAnimation(Sprite tutorialImage, List<NewsScriptable> newsList)
    {
        handAnimator.Play(handAnimationSwipe);
        await UniTask.Delay(handAnimationDelay, cancellationToken: this.GetCancellationTokenOnDestroy());
        
        screenUIMove.MoveUI(true, tutorialImage);

        for (int i = Zero; i < beginSpawn; i++)
        {
            bubbleFill.FillBrainWithThemeBubble(newsList[Zero]);
            if (newsList == newsListYellow)
            {
                bubbleFill.FillBrainWithThemeBubble(newsList[Zero]);
            }
        }
        
        bubbleFill.TutorialBubbleDestroy(articleCategories);
    }

    public async void RestartGameNoTutorial()
    {
        //close winning losing screen
        winGameScreen.gameObject.SetActive(false);
        endGameScreen.gameObject.SetActive(false);
        //Reset percentile
        percentRedArticles = 0.25f;
        percentBlueArticles = 0.25f;
        percentYellowArticles = 0.50f;
        
        newsDictionary[newsListRed] = percentRedArticles;
        newsDictionary[newsListBlue] = percentBlueArticles;
        newsDictionary[newsListYellow] = percentYellowArticles;

        //blow up all bubbles!
        bubbleFill.ResetAllBrainBubbles();
        await UniTask.Delay(thuasandMiliseconds, cancellationToken: this.GetCancellationTokenOnDestroy());
        
        for (int i = Zero; i < beginSpawn; i++)
        {
            bubbleFill.FillBrainWithThemeBubble(newsListRed[Zero]);
            bubbleFill.FillBrainWithThemeBubble(newsListBlue[Zero]);
            bubbleFill.FillBrainWithThemeBubble(newsListYellow[Zero]);
            bubbleFill.FillBrainWithThemeBubble(newsListYellow[Zero]);
            
            await UniTask.Delay(threeHundred, cancellationToken: this.GetCancellationTokenOnDestroy());
        }
        
        isEndGame = false;
        
        

        StartGame();
    }

    private void BuildNewsDictionary()
    {
        newsDictionary.Add(newsListRed, percentRedArticles);
        newsDictionary.Add(newsListBlue, percentBlueArticles);
        //newsDictionary.Add(newsListGreen, percentGreenArticles);
        newsDictionary.Add(newsListYellow, percentYellowArticles);
    }

    private void PullNewsToPhone()
    {
        RandomizeArticle(RandomizeArticleTheme());
    }

    private List<NewsScriptable> RandomizeArticleTheme()
    {
        float randomizedPercentile = Random.value;
        float currentSum = Zero;

        foreach (KeyValuePair<List<NewsScriptable>, float> newsTheme in newsDictionary)
        {
            currentSum += newsTheme.Value;
            if (randomizedPercentile <= currentSum)
            {
                return newsTheme.Key;
            }
        }

        return newsDictionary.Keys.GetEnumerator().Current; //Fallback
    }

    private void RandomizeArticle(List<NewsScriptable> chosenCategory)
    {
        int randomArticle = Random.Range(Zero, chosenCategory.Count);
        currentArticle = chosenCategory[randomArticle];
    }

    public void ChosenArticle(bool isArticleChosen)
    {
        NewsScriptable chosenArticle = currentArticle;
        
        PullNewsToPhone();
        WaitAfterChoice();
        
        if (isArticleChosen)
        {
            screenUIMove.MoveUI(true, currentArticle.newsImage);
            
            ChangeCatagoriesPercentile(chosenArticle,changePercentileValue);
        }
        else
        {
            screenUIMove.MoveUI(false, currentArticle.newsImage);

            
            if (chosenArticle.type == newsType.Blue)
            {
                ChangeCatagoriesPercentile(newsListRed[0], changePercentileValueOnPass);
            }
            
            if (chosenArticle.type == newsType.Red)
            {
                ChangeCatagoriesPercentile(newsListBlue[0], changePercentileValueOnPass);
            }
            
        }
        
        CheckLoseConditions();
    }

    private async void ChangeCatagoriesPercentile(NewsScriptable newsArticlePercentileChange, float percentChange)
    {
        await UniTask.Delay(thuasandMiliseconds/2, cancellationToken: this.GetCancellationTokenOnDestroy());
        
        List<List<NewsScriptable>> keys = new List<List<NewsScriptable>>(newsDictionary.Keys);

        foreach (List<NewsScriptable> key in keys)
        {
            if (key.Contains(newsArticlePercentileChange))
            {
                float addedAmount = percentChange * (articleCategories - 1);
                newsDictionary[key] += addedAmount;

                for (int i = Zero; i < addedAmount * OneHundred; i++)
                {
                    bubbleFill.FillBrainWithThemeBubble(key[Zero]);
                }
                
            }
            else
            {
                    newsDictionary[key] -= percentChange;
                
                    bubbleFill.RemoveThemeBubbleFromBrain(key[Zero].type);
            }
            
            Debug.Log(key[Zero].type + "  " + newsDictionary[key]);
        }
    }

    private async void WaitAfterChoice()
    {
        isWaiting = true;
        await UniTask.Delay(timeBetweenPosts, cancellationToken: this.GetCancellationTokenOnDestroy());

        isWaiting = false;
        
    }

    public void CheckLoseConditions()
    {

        foreach (KeyValuePair<List<NewsScriptable>, float> newsTheme in newsDictionary)
        {
            
            if (newsTheme.Key == newsListYellow )
            {
                if(newsTheme.Value >= fakeThreshold)
                {
                    bubbleFill.EndGameBubbles(newsTheme.Key[0].type);
                    
                    EnterFilterBubble(newsTheme.Key[0].type);
                    
                    return;
                } 
            }
            else
            {
                if(newsTheme.Value <= filterThemeThreshold)
                {
                    bubbleFill.EndGameBubbles(newsTheme.Key[0].type);
                    
                    EnterFilterBubble(newsTheme.Key[0].type);
                    
                    return;
                } 
            }
        }
    }

    public void EnterFilterBubble(newsType loseReason)
    {
        //You lose?
        isEndGame = true;
        endGameScreen.gameObject.SetActive(true);
        endGameScreen.GameEnded(loseReason);
        SoundManagerSingleton.PlaySound(loseSound);
    }
    
    public void WonGame()
    {
        //You lose?
        isEndGame = true;
        winGameScreen.gameObject.SetActive(true);
        winGameScreen.GameEnded();
        SoundManagerSingleton.PlaySound(winSound);
    }
}