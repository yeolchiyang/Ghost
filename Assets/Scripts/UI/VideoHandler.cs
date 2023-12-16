using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoHandler : MonoBehaviour
{
    public RawImage mScreen;
    public VideoPlayer mVideoPlayer;
    // Canvas ������Ʈ�� �����ϱ� ���� ����
    public Canvas canvas;
    void Start()
    {
        if (canvas == null)
        {
            canvas = mScreen.GetComponent<Canvas>();
        }
        ChangeOrder(1);
    }
    // UI ����� �켱 ������ �����ϴ� �Լ�
    public void ChangeOrder(int newSortingOrder)
    {
        // Canvas�� ���� ��� sortingOrder�� �����մϴ�.
        if (canvas != null)
        {
            Debug.Log("222");
            canvas.sortingOrder = newSortingOrder;
        }
    }
    public IEnumerator PrepareVideo()
    {
        // ���� �غ�
        mVideoPlayer.Prepare();

        // ������ �غ�Ǵ� ���� ��ٸ�
        while (!mVideoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // VideoPlayer�� ��� texture�� RawImage�� texture�� �����Ѵ�
        mScreen.texture = mVideoPlayer.texture;
    }

    public void PlayVideo()
    {
        if (mVideoPlayer != null && mVideoPlayer.isPrepared)
        {
            // ���� ���
            mVideoPlayer.Play();
        }
    }

    public void StopVideo()
    {
        if (mVideoPlayer != null && mVideoPlayer.isPrepared)
        {
            // ���� ����
            mVideoPlayer.Stop();
        }
    }
}
