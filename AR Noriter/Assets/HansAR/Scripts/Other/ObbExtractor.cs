using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

/***
 * AUTHOR   : N.C Park
 * DATE     : 2018.04.12
 * COMMENT  : OBB 분할 패키지 생성 시 Vuforia 타겟이 인식되지 않는 현상으로 인하여
 *            Application.streamingAssetsPath + vuforiaFolerName 경로의 Vuforia 데이터셋 파일을 
 *            Application.persistentDataPath + vuforiaFolerName 경로로 복사하면 타겟이 인식됨.
 *            적용은 Slpash 씬에서 '스플래쉬 매니저' 의 컴포넌트로 추가한 후 데이터셋 파일이름들을 추가
***/

public class ObbExtractor : MonoBehaviour
{
    public string           vuforiaFolerName;
    public string[]         targetFileNames;

    public static ObbExtractor instance;

    void Awake()
    {
        vuforiaFolerName    = "QCAR";
        instance            = this;
    }

    void Start()
    {
        
    }
 
    public void CopyDatasetFiles()
    {
        StartCoroutine(ExtractObbDatasets());
    }

    private IEnumerator ExtractObbDatasets()
    {
        WWW www                     = null;

        string assetPath            = string.Empty;
        string copyPath             = string.Empty;

        string uri                  = string.Empty;
        string outputFilePath       = string.Empty;

        List<string> loadFileList   = new List<string>();
        
        if (targetFileNames.Length > 0)
        {
            // assetPath : 원본 파일 경로, copyPath : 복사 될 파일 경로
            assetPath   = string.Format("{0}/{1}", Application.streamingAssetsPath, vuforiaFolerName);
            copyPath    = string.Format("{0}/{1}", Application.persistentDataPath, vuforiaFolerName);

            //Debug.LogFormat("assetPath ==> {0}", assetPath);
            //Debug.LogFormat("copyPath ==> {0}", copyPath);

            // 복사 될 폴더에 이미 'vuforiaFolerName' 이름을 갖는 폴더가 존재하면 복사를 진행 하지 않음
            if (Directory.Exists(copyPath) == false)
            {
                Directory.CreateDirectory(copyPath);

                foreach (string fileName in targetFileNames)
                {
                    loadFileList.Add(fileName + ".dat");
                    loadFileList.Add(fileName + ".xml");

                    yield return null;
                }

                foreach (string filename in loadFileList)
                {
                    uri = string.Format("{0}/{1}", assetPath, filename);
                    outputFilePath = string.Format("{0}/{1}", copyPath, filename);

                    using (www = new WWW(uri))
                    {
                        yield return www;

                        if (string.IsNullOrEmpty(www.error))
                        {
                            SaveFile(www, outputFilePath);
                            yield return new WaitForEndOfFrame();
                        }
                        else
                        {
                            Debug.LogError(www.error);
                        }
                    }

                    yield return null;
                }
            }            
        }

        loadFileList.RemoveRange(0, loadFileList.Count);        
        loadFileList = null;

        SceneManager.LoadSceneAsync("01. HansMain");        
    }
    
    private void SaveFile(WWW www, string outputPath)
    {
        try
        {
            File.WriteAllBytes(outputPath, www.bytes);

            // Verify that the File has been actually stored
            if (File.Exists(outputPath))
            {
                //Debug.Log("File successfully saved at: " + outputPath);
            }
            else
            {
                //Debug.Log("Failure!! - File does not exist at: " + outputPath);
            }
        }
        catch
        {
            if (www != null)
            {
                www.Dispose();
            }
        }        
    }
}