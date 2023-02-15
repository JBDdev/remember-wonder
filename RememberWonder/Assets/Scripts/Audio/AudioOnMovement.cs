using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOnMovement : MonoBehaviour
{
    [SerializeField] AudioList audioToPlay;
    [Tooltip("If loop is false, how often to play audio when moving (once every X seconds)")]
    [SerializeField][Min(0.01f)] float playInterval = 1;
    [SerializeField] SourceSettings audioSettings;
    [Space(10)]
    [Tooltip("The body to read velocity from. If null, movement will be determined with position delta instead.")]
    [SerializeField] Rigidbody body;
    [Tooltip("If body isn't null, how fast this object needs to go before playing any audio.")]
    [SerializeField][Min(0.01f)] float velocityThreshold = 0.1f;
    [Tooltip("If body is null, much position needs to change per fixed update before playing any audio.")]
    [SerializeField][Min(0.001f)] float posDeltaThreshold = 0.01f;

    bool moving;
    Vector3 previousPosition;
    AudioSource currentSource;

    Coroutine intervalTimerCorout;
    float intervalTimer;

    private void OnEnable()
    {
        intervalTimerCorout = StartCoroutine(TryPlayIfMoving());
    }
    private void OnDisable()
    {
        Coroutilities.TryStopCoroutine(this, ref intervalTimerCorout);
    }

    private void FixedUpdate()
    {
        if (body)
        {
            moving = body.velocity.sqrMagnitude >= velocityThreshold * velocityThreshold;
        }
        else
        {
            var posDelta = transform.position - previousPosition;
            moving = posDelta.sqrMagnitude >= posDeltaThreshold * posDeltaThreshold;
            previousPosition = transform.position;

            /*if (posDelta != Vector3.zero)
            {
                print($"{posDelta.x}, {posDelta.y}, {posDelta.z}");
            }*/
        }

        ManageLoopingAudio();
    }

    private IEnumerator TryPlayIfMoving()
    {
        while (Application.isPlaying)
        {
            //No need to be running interval stuff at all if the audio loops.
            if (audioSettings.useLoop && audioSettings.loop) yield break;

            if (!moving)
            {
                intervalTimer = 0;
                yield return null;
            }

            //Increment interval timer, and when it reaches playInterval, play + reset timer to 0
            intervalTimer += Time.deltaTime;
            if (intervalTimer >= playInterval)
            {
                intervalTimer = 0;
                AudioHub.Inst.Play(audioToPlay, audioSettings, transform.position);
            }

            yield return null;
        }
    }

    private void ManageLoopingAudio()
    {
        if (!audioSettings.useLoop || !audioSettings.loop) return;

        if (moving)
        {
            if (!currentSource)
            {
                currentSource = AudioHub.Inst.Play(audioToPlay, audioSettings, transform.position);
                print($"Playing {currentSource.clip} from {currentSource.name} @ {Time.timeAsDouble}");
            }
        }
        else if (currentSource)
        {
            print($"Stopping {currentSource.clip} on {currentSource.name} @ {Time.timeAsDouble}");
            currentSource.Stop();
            currentSource = null;
        }
    }
}
