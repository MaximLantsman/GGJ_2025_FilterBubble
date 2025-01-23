using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bubble : MonoBehaviour
{
    public SpriteRenderer bubbleRenderer;
    
    [SerializeField,Self] private Rigidbody2D rb;
    [SerializeField] private float topSpeed = 20f;
    [SerializeField] private float lowspeed = 0f;
    [SerializeField] private ParticleSystem particlePop;
    [SerializeField] private AudioClip popClip;
    
    private int afterDeathDelay = 2000;
    
    // Update is called once per frame
    private void Start()
    {
        float speed = Random.Range(lowspeed, topSpeed);
        
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
        
        rb.AddForce(randomDirection * speed, ForceMode2D.Impulse);
    }

    /*private void OnCollisionEnter2D(Collision2D other)
    {
        rb.AddForce(other.contacts[Zero].normal, ForceMode2D.Impulse);
    }*/
    


    public async void BubblePop()
    {
        //Growth spurt?
        
        //Color Change
        ParticleSystem.MainModule ps = particlePop.main;
        ps.startColor = bubbleRenderer.color;
        
        ParticleSystem.ColorOverLifetimeModule colorModule = particlePop.colorOverLifetime;
        colorModule.color = bubbleRenderer.color;
        particlePop.Play();
        
        bubbleRenderer.enabled = false;
        
        //Sound Play
        SoundManagerSingleton.PlaySound(popClip);

        await UniTask.Delay(afterDeathDelay, cancellationToken: this.GetCancellationTokenOnDestroy());
        
        //Destroy!!!
        Destroy(this.gameObject);
    }

    
}
