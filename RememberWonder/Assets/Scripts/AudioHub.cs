using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHub : MonoBehaviour
{
    private static AudioHub instance;
    public static AudioHub Inst
    {
        get
        {
            Debug.Assert(instance, "AudioHub instance was accessed, but there is no instance in the scene.");
            return instance;
        }
    }

    [SerializeField] AudioSource sourcePrefab;
    [SerializeField][Min(1)] int sourcePoolSize = 10;
    [Space(10)]
    [SerializeField] Bewildered.UDictionary<AudioList, SoundContainer> soundLibrary;

    private Stack<AudioSource> idleSources;

    private void Awake()
    {
        if (instance)
        {
            Debug.LogWarning($"More than one AudioHub in the scene; " +
                $"removing AudioHub component from {name}.");
            Destroy(this);
        }
        else instance = this;
    }

    private void Start()
    {
        idleSources = new Stack<AudioSource>();

        for (int i = 0; i < sourcePoolSize; i++)
        {
            var source = Instantiate(sourcePrefab, transform);
            source.name = source.name.Replace("(Clone)", $" [{i + 1}]");
            idleSources.Push(source);
        }
    }

    public AudioSource Play(AudioList listItem, Vector3 playPos) => Play(listItem, null, playPos);
    public AudioSource Play(AudioList listItem, SourceSettings settings = null, Vector3 playPos = default)
    {
        if (!soundLibrary.TryGetValue(listItem, out var sound))
        {
            Debug.LogWarning($"No clip with name \"{listItem}\" found in AudioHub's sound library.");
            return null;
        }

        return Play(sound.Clip, settings, playPos);
    }
    public AudioSource Play(AudioClip clip, Vector3 playPos) => Play(clip, null, playPos);
    public AudioSource Play(AudioClip clip, SourceSettings settings = null, Vector3 playPos = default)
    {
        //If all the sources in our pool are busy, just give up.
        if (!idleSources.TryPop(out var source))
        {
            Debug.LogWarning($"Clip \"{clip}\" not played; all audio sources are busy. " +
                $"Consider increasing the source pool size, or playing less sounds all at once.");
            return null;
        }

        source.transform.position = playPos;
        source.clip = clip;
        settings?.ApplyToSource(ref source);
        source.Play();

        //Wait until the clip's done, then return this source to the pool.
        Coroutilities.DoWhen(this, () => idleSources.Push(source), () => !source.isPlaying);
        return source;
    }
}

[System.Serializable]
public class SoundContainer
{
    public bool randomOrder;
    [SerializeField] private AudioClip[] clips;

    private int clipIndex;
    public AudioClip Clip
    {
        get
        {
            //Make sure index isn't out of range
            clipIndex %= clips.Length;

            //If not random, return current index, then increment index.
            return clips[randomOrder
                ? Random.Range(0, clips.Length)
                : clipIndex++];
        }
    }
}