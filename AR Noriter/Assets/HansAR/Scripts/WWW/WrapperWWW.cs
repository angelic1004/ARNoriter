using UnityEngine;

using System;
using System.Collections;
using System.Net;

/***
 * author   : N.C Park
 * date     : 2017.08.31
 * comment  : WWW 클래스의 Wrapper 클래스를 구성 합니다. 
 *            Deletegate 를 구성하여 프로그래스, 다운로드 완료, 에러 처리를 외부에서 할 수 있도록 구성 합니다. (Callback)
 *            WWW 형식과 WWW.LoadFromCacheOrDownload 형식을 구성합니다. 
 *            namespace는 HansAR 로 사용하는 곳에서 'using HansAR' 을 명시하여 참조 해야 합니다.
***/

namespace HansAR {   
    public delegate void EventHandlerDrawProgressBar(HttpRequestDataSet requestDataSet, float progressValue);
    public delegate void EventHandlerRequestComplete(HttpRequestDataSet requestDataSet, WWW www);
    public delegate void EventHandlerFinishWWW(HttpRequestDataSet requestDataSet);
    public delegate void EventHandlerErrorWWW(HttpRequestDataSet requestDataSet, string errorMsg);

    public static class WrapperWWW
    {
        private static float durationValue  = 0f;

        private static bool isCheckTimeout  = true;
        private static bool isApplyTimeout  = false;

        private const float timeoutValue    = 5f;

        public static int count             = 0;

        /// <summary>
        /// WW 요청을 완료 후 처리하는 함수 (delegate로 연결되어 있는 함수 실행)
        /// </summary>
        /// <param name="requestData">다운로드 요청 정보 개체</param>
        /// <param name="www">WWW object</param>
        private static void DoCallback(HttpRequestDataSet requestData, WWW www)        
        {
            if (!string.IsNullOrEmpty(www.error) || isApplyTimeout)
            {
                if (requestData.onErrorWWW != null)
                {
                    requestData.onErrorWWW(requestData, www.error);
                    Debug.LogErrorFormat("error : {0}", www.error);
                }
            }
            else
            {
                isCheckTimeout  = false;                

                if (requestData.onRequestComplete != null)
                {
                    requestData.onRequestComplete(requestData, www);
                }

                if (requestData.onFinishWWW != null)
                {
                    requestData.onFinishWWW(requestData);
                }
            }

            --count;
        }

        /// <summary>
        /// WWW 클래스 기반으로 파일 다운로드를 요청하는 Coroutine
        /// </summary>
        /// <param name="requestData">다운로드 요청 정보 개체</param>
        /// <returns></returns>
        public static IEnumerator FileDownloadWWW(HttpRequestDataSet requestData)
        {
            //Debug.LogWarning(string.Format("request addr = {0}", requestData.requestURL));

            using (WWW www = new WWW(requestData.requestURL))
            {
                count++;
                durationValue   = 0f;
                isApplyTimeout  = false;

                while (!www.isDone)                
                {
                    if (isCheckTimeout)
                    {
                        // www 처음 시작 시 서버에서 응답이 없을 경우 자체 타임아웃 처리 하기 위함
                        // DoCallback() 안에 isCheckTimeOut 값을 false 로 변경 한 후에는 사용 되지 않음
                        if (durationValue > timeoutValue)
                        {
                            isApplyTimeout = true;
                            break;
                        }

                        durationValue += Time.deltaTime;
                    }

                    if (requestData.onDrawProgressBar != null)
                    {
                        requestData.onDrawProgressBar(requestData, www.progress);
                    }

                    yield return new WaitForEndOfFrame();                                     
                }

                DoCallback(requestData, www);                
            }
        }

        /// <summary>
        /// WWW 클래스 기반으로 파일 다운로드를 요청하는 Coroutine
        /// LoadFromCacheOrDownload() 사용으로 Cache 를 사용함
        /// </summary>
        /// <param name="requestData">다운로드 요청 정보 개체</param>
        /// <returns></returns>
        public static IEnumerator FileDownloadWWWCache(HttpRequestDataSet requestData)
        {
            while (!Caching.ready)
            {
                yield return null;
            }
            
            using (WWW www = WWW.LoadFromCacheOrDownload(requestData.requestURL, requestData.requestFileVersion))
            {
                count++;
                while (!www.isDone)
                {
                    if (requestData.onDrawProgressBar != null)
                    {
                        requestData.onDrawProgressBar(requestData, www.progress);
                    }

                    yield return new WaitForEndOfFrame();
                }

                DoCallback(requestData, www);                            
            }            
        }        
    }
}