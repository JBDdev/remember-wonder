using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayVideos : MonoBehaviour
{
    public VideoClip[] videoClips;
    private VideoPlayer videoplayer;
    private int videoClipIndex;
    public double time;
    public double currentTime;

    private void Awake()
    {
        videoplayer = GetComponent<VideoPlayer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        videoplayer.clip = videoClips[0];
        videoplayer.loopPointReached += playNextVideo;
    }


    //play the next video in the tutorial
    void playNextVideo(VideoPlayer vp)
    {
        //Debug.Log("Video over!");
        videoClipIndex++;
        if (videoClipIndex >= videoClips.Length)
        {
            vp.gameObject.SetActive(false);
            vp.Stop();
            vp.gameObject.transform.position += new Vector3(10000.0f, 0, 0);
            Debug.Log("not active!");
            return;
        }
        //Debug.Log("video clip: " + ( videoClipIndex + 1));
        videoplayer.clip = videoClips[videoClipIndex];
    }
}
