using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoHandler : MonoBehaviour
{
    public RawImage mScreen;
    public VideoPlayer mVideoPlayer;
    // Canvas 컴포넌트에 접근하기 위한 변수
    public Canvas canvas;
    void Start()
    {
        if (canvas == null)
        {
            canvas = mScreen.GetComponent<Canvas>();
        }
        ChangeOrder(1);
    }
    // UI 요소의 우선 순위를 변경하는 함수
    public void ChangeOrder(int newSortingOrder)
    {
        // Canvas가 있을 경우 sortingOrder를 변경합니다.
        if (canvas != null)
        {
            Debug.Log("222");
            canvas.sortingOrder = newSortingOrder;
        }
    }
    public IEnumerator PrepareVideo()
    {
        // 비디오 준비
        mVideoPlayer.Prepare();

        // 비디오가 준비되는 것을 기다림
        while (!mVideoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // VideoPlayer의 출력 texture를 RawImage의 texture로 설정한다
        mScreen.texture = mVideoPlayer.texture;
    }

    public void PlayVideo()
    {
        if (mVideoPlayer != null && mVideoPlayer.isPrepared)
        {
            // 비디오 재생
            mVideoPlayer.Play();
        }
    }

    public void StopVideo()
    {
        if (mVideoPlayer != null && mVideoPlayer.isPrepared)
        {
            // 비디오 멈춤
            mVideoPlayer.Stop();
        }
    }
}
