using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayVideos : MonoBehaviour
{
    public VideoClip[] videoClips;
    private VideoPlayer videoplayer;
    private int videoClipIndex;

    private void Awake()
    {
        videoplayer = GetComponent<VideoPlayer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        videoplayer.clip = videoClips[0];
        Debug.Log("video clip: " + (videoClipIndex + 1));
    }

    // Update is called once per frame
    void Update()
    {
        if (videoClipIndex >= 0 && videoClipIndex < videoClips.Length)
        {
            Debug.Log("update index: " + videoClipIndex);
            playNextVideo();
        }
    }

    public void delayVideoTime()
    {
        Invoke(nameof(playNextVideo), 2.0f);
    }

    //play the next video in the tutorial
    public void playNextVideo()
    {
        videoClipIndex++;
        if (videoClipIndex >= videoClips.Length)
        {
            //videoplayer.GameObject.setActive(false); 
            Debug.Log("not active!");
            return;
        }
        Debug.Log("video clip: " + ( videoClipIndex + 1));
        videoplayer.clip = videoClips[videoClipIndex];
    }
}
