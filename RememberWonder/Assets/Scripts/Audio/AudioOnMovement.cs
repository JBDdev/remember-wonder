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
    [SerializeField][Min(0.01f)] float posDeltaThreshold = 0.1f;

    bool moving;
    Vector3 previousPosition;

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
            //If moving and the sound isn't already playing (or something), start playing
        }
        else
        {
            moving = (transform.position - previousPosition).sqrMagnitude >= velocityThreshold * velocityThreshold;
            previousPosition = transform.position;
        }
    }

    private IEnumerator TryPlayIfMoving()
    {
        while (Application.isPlaying)
        {
            //No need to be running interval stuff at all if the audio loops.
            if (audioSettings.useLoop && audioSettings.loop) yield break;

            //Otherwise, if not moving, we're not playing audio anyway.
            if (!moving)
            {
                intervalTimer = 0;
                yield return null;
            }

            //Increment interval timer, and when it reaches playInterval, play + reset timer to 0

            yield return null;
        }
    }
}
