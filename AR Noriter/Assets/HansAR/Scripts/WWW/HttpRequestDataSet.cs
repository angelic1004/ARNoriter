using UnityEngine;

using System;
using System.Collections;

namespace HansAR
{
    public delegate void EventHandlerAfterRequestComplete(HttpRequestDataSet httpRequestDataSet);

    public class HttpRequestDataSet
    {
        // Delegate Variables
        public EventHandlerDrawProgressBar              onDrawProgressBar;
        public EventHandlerRequestComplete              onRequestComplete;
        public EventHandlerAfterRequestComplete         onAfterRequestComplete;
        public EventHandlerFinishWWW                    onFinishWWW;
        public EventHandlerErrorWWW                     onErrorWWW;

        public string                                   requestURL;        

        public string                                   requestFilePath;
        public string                                   requestFileTitle;
        public string                                   requestFileExt;

        public string                                   destinationFilePath;
        public string                                   destinationFileFullPath;

        public float                                    requestFileSize;        

        public int                                      requestFileVersion;

        public GlobalDataManager.CategoryType           categoryType;
        public GlobalDataManager.RequestDownloadStatus  requestDownloadStatus;

        public Coroutine                                coroutine;


        // asset bundle 처리 할때 필요한 변수들
        public string                                   assetBundleName;
        public string[]                                 contentsModelingNames;
        public GameObject[]                             assetBundleCopyObjects;
        public GameObject                               rootObject;
        public GameObject                               arCameraObj;

        // for one modeling     
        public GameObject                               OnceModeling;
        public string                                   OnceModelingName;        

        public HttpRequestDataSet() {
            onDrawProgressBar       = null;
            onRequestComplete       = null;
            onFinishWWW             = null;
            onErrorWWW              = null;

            requestURL              = string.Empty;

            requestFilePath         = string.Empty;
            requestFileTitle        = string.Empty;
            requestFileExt          = string.Empty;

            destinationFilePath     = string.Empty;
            destinationFileFullPath = string.Empty;

            requestFileSize         = 0f;
            requestFileVersion      = 0;

            // other
            assetBundleName         = string.Empty;
            contentsModelingNames   = null;
            assetBundleCopyObjects  = null;
            rootObject              = null;
            arCameraObj             = null;            
        }

        ~HttpRequestDataSet() { }
    }
}
