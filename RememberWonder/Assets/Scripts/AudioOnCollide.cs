using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOnCollide : MonoBehaviour
{
    [SerializeField] AudioList audioToPlay;
    [SerializeField] SourceSettings audioSettings;
    [Space(10)]
    [Tooltip("How much change in momentum the collision has to have before playing any audio." +
        "See https://wikipedia.org/wiki/Impulse_%28physics%29.")]
    [SerializeField][Min(0)] float impulseThreshold = 1;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.sqrMagnitude >= impulseThreshold * impulseThreshold)
        {
            AudioHub.Inst.Play(audioToPlay, audioSettings, collision.GetContact(0).point);
        }
    }
}