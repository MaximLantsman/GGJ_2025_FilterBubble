using KBCore.Refs;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManagerSingleton : MonoBehaviour
{
    [SerializeField]private AudioClip[] soundList;
    
    public static SoundManagerSingleton instance { get; private set; }

    private static AudioSource _audioSource;
    
    private void Awake()
    {
        if (instance != null)
        {
            
            Destroy(gameObject);
            Debug.LogError("More than one SoundManagerSingleton");
        }

        instance = this;
    }


    public static void PlaySound(AudioClip clip)
    {
        if (clip == null)
            return;
        
        GameObject sfxInstance = new GameObject(clip.name);

        AudioSource source = sfxInstance.AddComponent<AudioSource>();
        source.transform.parent = instance.transform;
        source.clip = clip;
        source.Play();
        
        // destroy after clip length
        Destroy(sfxInstance, clip.length);
    }
}
