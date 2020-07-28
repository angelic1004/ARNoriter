using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MazeSelectUI : MonoBehaviour
{
    public GameObject mazeSelectUI;

    public GameObject[] mapSelectObj;

    public static MazeSelectUI instance;

    private int targetIndex = -1;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        TargetManager.타깃메니저.StartVuforia();
        //TargetManager.DelEventMarkerFound += GetTargetIndex;

        TargetManager.타깃메니저.ApplySceneUI(null);

        mazeSelectUI.SetActive(false);
    }

    public void SelectMap(GameObject obj)
    {
        for (int i = 0; i < mapSelectObj.Length; i++)
        {
            if (mapSelectObj[i] == obj)
            {
                MainUI.메인.딜레이팝업UI.SetActive(true);
                StartMazeGame(i);
                break;
            }
        }
    }

    private void StartMazeGame(int mazeIndex)
    {
        MazeGameManager.level = 1;
        MazeGameManager.sceneName = string.Format("{0}_mazegame_maze{1}", GlobalDataManager.m_ResourceFolderEnum, mazeIndex + 1).ToLower();

        SceneManager.LoadSceneAsync(string.Format("{0}_1", MazeGameManager.sceneName));
    }

    public void MazeSelectStart(string trackableTargetName)
    {
        MainUI.메인.인식글자UI.SetActive(false);
        
        string markerNameOfList = string.Empty;

        for (int i = 0; i < TargetManager.타깃메니저.타깃정보.Length; i++)
        {
            markerNameOfList = TargetManager.타깃메니저.타깃정보[i].마커타깃오브젝트.GetComponent<Vuforia.ImageTargetBehaviour>().TrackableName;

            // 뷰포리아에 등록된 마커 이름이 같다면..
            if (string.Compare(trackableTargetName, markerNameOfList, true) == 0)
            {
                MazeGameManager.targetIndex = i;
                mazeSelectUI.SetActive(true);
                return;
            }
        }
        
        Debug.LogError("타겟을 찾지 못하였습니다. 타겟이름을 확인해주세요");
    }

    public void CloseMazeSelectUI()
    {
        mazeSelectUI.SetActive(false);
    }
}
