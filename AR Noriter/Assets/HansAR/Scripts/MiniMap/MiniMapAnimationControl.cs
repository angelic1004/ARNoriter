using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapAnimationControl : MonoBehaviour
{
    /// <summary>
    /// 공주가 들어온 순서대로 배열번호를 담을 리스트
    /// </summary>
    public List<int> index;

    /// <summary>
    /// 공주의 배열번호가 담긴 리스트에서 실행할 공주 애니메이션의 리스트의 배열번호를 가져옴.
    /// </summary>
    private int playingIndex;

    /// <summary>
    /// 미니맵 오브젝트를 담을 빈오브젝트
    /// </summary>
    public GameObject minimapModel;

    /// <summary>
    /// 미니맵 인포 스크립트를 가져옴
    /// </summary>
    private MiniMapInfo minimapInfo;

    /// <summary>
    /// 실행할 애니메이션을 담을 애니
    /// </summary>
    private Animation ani;

    public static MiniMapAnimationControl instance;

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 공주미니맵 애니메이션 컨트롤
    /// </summary>
    public void InitControl()
    {
        minimapModel    = MiniMapManager.instance.miniMapModeling;
        minimapInfo     = minimapModel.GetComponent<MiniMapInfo>();
        ani             = minimapModel.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>();

        index           = new List<int>();
    }

    /// <summary>
    /// 리스트 내에 공주가 들어온 순서대로 배열을 저장함.
    /// </summary>
    /// <param name="modelName"></param>
    public void SaveContentsIndex(string modelName)
    {
        for (int i = 0; i < minimapInfo.miniMapResourceInfo.Length; i++)
        {
            //현재들어온 모델의 이름과, 미니맵인포에 저장된 모델의 이름이 같을때..
            if (modelName.Equals(minimapInfo.miniMapResourceInfo[i].miniMapCompareModelName))
            {
                for (int j = 0; j < index.Count; j++)
                {
                    //인덱스 내부에 동일한 공주가 있다면
                    if (index[j] == i)
                    {
                        //저장은 하지않고, 그 공주의 애니메이션을 실행함.
                        PlaySelectAni(index[j]);
                        playingIndex = j;
                        return;
                    }
                }

                //인덱스 내부에 동일한 공주가 없다면, 인덱스를 저장하고, 공주 애니메이션을 실행함.
                index.Add(i);
                PlaySelectAni(index[index.Count - 1]);
                playingIndex = index.Count - 1;
                break;
            }
        }
    }

    /// <summary>
    /// 선택된 리스트배열에 있는 애니를 실행함.
    /// </summary>
    /// <param name="num"></param>
    private void PlaySelectAni(int num)
    {
        //null 체크와 플레이중인지를 체크
        if (ani != null && ani.isPlaying)
        {
            //애니 실행중 체크 코루틴을 종료
            StopCoroutine(GetIsPlaying());

            //애니를 중지.
            ani.Stop();
        }

        //클립에 실행할 애니를 저장 후, 플레이
        ani.clip = minimapInfo.miniMapResourceInfo[num].miniMapAnimations[0];
        ani.wrapMode = WrapMode.Once;
        ani.Play();

        //애니가 종료됐는지를 체크하기 위해 코루틴을 실행
        StartCoroutine(GetIsPlaying());
    }

    /// <summary>
    /// 애니 플레이 체크 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetIsPlaying()
    {
        while (true)
        {
            //애니가 실행중이 아니라면..
            if (ani.isPlaying == false)
            {
                //인덱스 마지막 배열일 경우
                if (playingIndex >= index.Count - 1)
                {
                    playingIndex = 0;
                }
                //인덱스가 처음 또는 중간배열일 경우
                else
                {
                    playingIndex += 1;
                }

                //다음 애니메이션을 실행
                PlaySelectAni(index[playingIndex]);
                yield break;
            }
            yield return null;
        }
    }
}