using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;


public class FillUpBrain : MonoBehaviour
{
    public UnityEvent notifyOnGameWin;
    
    [SerializeField] private Bubble ideaBubblePrefab;
    [SerializeField] private GameObject bubbleParent;

    [SerializeField] private int bubbleStartAmount;
    
    [SerializeField] private Color blueBubbleColor;
    [SerializeField] private Color redBubbleColor;
    [SerializeField] private Color greenBubbleColor;
    [SerializeField] private Color yellowBubbleColor;
    
    private List<Bubble> tutorialListBubbles = new List<Bubble>();

    
    private List<Bubble> blueBubbles = new List<Bubble>();
    private List<Bubble> redBubbles = new List<Bubble>();
    private List<Bubble> greenBubbles = new List<Bubble>();
    private List<Bubble> yellowBubbles = new List<Bubble>();
    
    private bool isCountingTime = false;
    private List<GameObject> objectsInside = new List<GameObject>();
    private CancellationTokenSource cts;

    
    public void FillBrainWithThemeBubble(NewsScriptable bubbleColor)
    {
        Bubble newBubble = CreateBubbles();
        
        newBubble.bubbleRenderer.color = AddToColorList(bubbleColor.type, newBubble);
    }

    private Bubble CreateBubbles()
    {
        Bubble newBubble = Instantiate(ideaBubblePrefab, bubbleParent.transform.position, Quaternion.identity, bubbleParent.transform);

        return newBubble;
    }

    public void TutorialBubbleCreation(int bubbleAmount)
    {
        for (int i=0; i<bubbleAmount-1; i++)
        {
            Bubble tutorialBubble = CreateBubbles();
            tutorialListBubbles.Add(tutorialBubble);
            tutorialBubble.bubbleRenderer.color = blueBubbleColor;
            
            tutorialBubble = CreateBubbles();
            tutorialListBubbles.Add(tutorialBubble);
            tutorialBubble.bubbleRenderer.color = redBubbleColor;
            
            tutorialBubble = CreateBubbles();
            tutorialListBubbles.Add(tutorialBubble);
            tutorialBubble.bubbleRenderer.color = yellowBubbleColor;
        }
    }

    public void TutorialBubbleDestroy(int bubbleAmount)
    {
        for (int i = 0; i < bubbleAmount-1; i++)
        {
            tutorialListBubbles[tutorialListBubbles.Count - 1].BubblePop();
            tutorialListBubbles.RemoveAt(tutorialListBubbles.Count - 1);
        }
    }
    
    private Color AddToColorList(newsType bubbleColor,Bubble bubbleToList)
    {
        switch (bubbleColor)
        {
            case newsType.Blue:
                blueBubbles.Add(bubbleToList);
                return blueBubbleColor;
                
            case newsType.Red:
                redBubbles.Add(bubbleToList);
                return redBubbleColor;
               
            case newsType.Green:
                greenBubbles.Add(bubbleToList);
                return greenBubbleColor;
                
            case newsType.Yellow:
                yellowBubbles.Add(bubbleToList);
                return yellowBubbleColor;
        }

        Debug.LogError(bubbleColor + " is not a valid color");
        return Color.white; 
    }

    public void RemoveThemeBubbleFromBrain(newsType bubbleColor)
    {
        switch (bubbleColor)
        {
            case newsType.Blue:
                blueBubbles[blueBubbles.Count-1].BubblePop();
                blueBubbles.RemoveAt(blueBubbles.Count-1);
                break;
                
            case newsType.Red:
                redBubbles[redBubbles.Count-1].BubblePop();
                redBubbles.RemoveAt(redBubbles.Count-1);
                break;
               
            case newsType.Green:
                greenBubbles[greenBubbles.Count-1].BubblePop();
                greenBubbles.RemoveAt(greenBubbles.Count-1);
                break;
                
            case newsType.Yellow:
                if (yellowBubbles[0] != null)
                {
                    yellowBubbles[yellowBubbles.Count-1].BubblePop();
                    yellowBubbles.RemoveAt(yellowBubbles.Count-1);
                }
                break;
        }
    }
    
    public void EndGameBubbles(newsType bubbleColor)
    {
        switch (bubbleColor)
        {
            case newsType.Blue:
                foreach (Bubble bubble in blueBubbles)
                {
                    bubble.BubblePop();
                }
                break;
                
            case newsType.Red:
                foreach (Bubble bubble in redBubbles)
                {
                    bubble.BubblePop();
                }
                break;
               
            case newsType.Green:
                foreach (Bubble bubble in greenBubbles)
                {
                    bubble.BubblePop();
                }
                break;
                
            case newsType.Yellow:
                foreach (Bubble bubble in greenBubbles)
                {
                    bubble.BubblePop();
                }
                foreach (Bubble bubble in redBubbles)
                {
                    bubble.BubblePop();
                }
                foreach (Bubble bubble in blueBubbles)
                {
                    bubble.BubblePop();
                }
                break;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        objectsInside.Add(other.gameObject);
        
        // Start counting only if this is the first object
        if (objectsInside.Count == 1 && !isCountingTime)
        {
            cts = new CancellationTokenSource();
            CountTimeInsideCollider();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        objectsInside.Remove(other.gameObject);
        
        // Stop counting if no objects remain
        if (objectsInside.Count == 0 && cts != null)
        {
            cts.Cancel();
        }
    }


    private async void CountTimeInsideCollider()
    {
        await UniTask.Delay(3000, cancellationToken: cts.Token);

        WinLevel();
    }

    
    private void WinLevel()
    {
        Debug.Log("You Win!");
        notifyOnGameWin?.Invoke();
    }

    public void ResetAllBrainBubbles()
    {
        foreach (Bubble bubble in blueBubbles)
        {
            bubble.BubblePop();
        }
        blueBubbles.Clear();
        
        foreach (Bubble bubble in redBubbles)
        {
            bubble.BubblePop();
        }
        redBubbles.Clear();
        
        foreach (Bubble bubble in blueBubbles)
        {
            bubble.BubblePop();
        }
        yellowBubbles.Clear();
    }
}
