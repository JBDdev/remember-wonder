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
#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField] bool printCollisionResults;
#endif

    private void OnCollisionEnter(Collision collision)
    {
        //Ignore collisions before the first frame of level load.
        if (Time.timeSinceLevelLoadAsDouble < Time.maximumDeltaTime) return;

#if UNITY_EDITOR
        if (printCollisionResults)
        {
            print($"{name} AudioOnCollide: <color=#F85>( {collision.impulse.magnitude} >= {impulseThreshold} )^2 </color>" +
                $"== <color=#58F>{collision.impulse.sqrMagnitude} >= {impulseThreshold * impulseThreshold} </color>" +
                $"== <color={(collision.impulse.sqrMagnitude >= impulseThreshold * impulseThreshold ? "#FFF" : "#666")}>" +
                $"{collision.impulse.sqrMagnitude >= impulseThreshold * impulseThreshold}</color>");
        }
#endif

        if (collision.impulse.sqrMagnitude >= impulseThreshold * impulseThreshold)
        {
            AudioHub.Inst.Play(audioToPlay, audioSettings, collision.GetContact(0).point);
        }
    }
}