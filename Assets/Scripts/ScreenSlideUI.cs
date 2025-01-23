using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScreenSlideUI : MonoBehaviour
{
    [SerializeField]private RectTransform rectTransform;
    [SerializeField] private AudioClip swipeClip;

    [SerializeField]private float animationDistance=700;
    [SerializeField]private float animationDuration=1f;

    [SerializeField]private Image mainArticlePanel, articlePicked, articlePassed;
    
    private Vector2 startPosition;
    private Sprite currrentArticleSprite;

    private void Start()
    {
        startPosition.x = rectTransform.rect.x;
    }
    
    public void MoveUI(bool isRightSwipe, Sprite newArticleSprite)
    {
        Vector2 currMove ;
        currrentArticleSprite = newArticleSprite;
        
        if (isRightSwipe)
        {
            currMove.x = animationDistance;
            articlePicked.sprite = currrentArticleSprite;
        }
        else
        {
            currMove.x = -animationDistance;
            articlePassed.sprite = currrentArticleSprite;
        }

        rectTransform.DOAnchorPosX(currMove.x, animationDuration, false).OnComplete(ReturnToStart);
        
        SoundManagerSingleton.PlaySound(swipeClip);
    }

    private void ReturnToStart()
    {
        mainArticlePanel.sprite = currrentArticleSprite;
        rectTransform.DOAnchorPosX(startPosition.x, 0, false);
        
    }
}
