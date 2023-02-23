using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayVideos : MonoBehaviour
{
    public VideoClip[] videoClips;
    private VideoPlayer videoplayer;
    [SerializeField] private RawImage videoImage;
    private int videoClipIndex;
    public double time;
    public double currentTime;

    private void Awake()
    {
        videoplayer = GetComponent<VideoPlayer>();
    }

    void Start()
    {
        videoplayer.clip = videoClips[0];
        //subscribing to an event
        videoplayer.loopPointReached += playNextVideo;
    }
    private void OnDestroy()
    {
        videoplayer.loopPointReached -= playNextVideo;
    }

    //plays the next video
    void playNextVideo(VideoPlayer vp)
    {
        videoClipIndex++;

        if (videoClipIndex >= videoClips.Length)
        {
            UtilFunctions.SafeSetActive(videoImage, false);
            //unsubscribing to an event
            videoplayer.loopPointReached -= playNextVideo;
            return;
        }

        videoplayer.clip = videoClips[videoClipIndex];
    }
}
