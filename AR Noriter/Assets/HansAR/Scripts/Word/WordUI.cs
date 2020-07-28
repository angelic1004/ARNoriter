using UnityEngine;
using System.Collections;

public class WordUI : MonoBehaviour
{
    public GameObject resultUI;
    public GameObject gameUI;
    public GameObject pauseUI;

    public GameObject pauseBtn;
    public GameObject nextLevelBtn;

    public GameObject loadingUI;

    public static WordUI instance;

    public GameObject alphabetCard_Original;

    public GameObject parentAlphabetCard;

    [HideInInspector]
    public GameObject[] alphabetCard;

    private GameObject[] preAlphabetCard;

    [HideInInspector]
    public bool isRecog;

    void Start()
    {
        instance = this;

        UICheck(resultUI, false);
        UICheck(gameUI, false);
        UICheck(pauseUI, false);
        UICheck(pauseBtn, false);

        InitWordUI();
    }

    private void UICheck(GameObject uiObj, bool active)
    {
        if (uiObj != null)
        {
            uiObj.SetActive(active);
        }
        else
        {
            Debug.LogError("UI is NULL <NULL Obj = " + uiObj + ">");
        }
    }

    public void InitWordUI()
    {

        TargetManager.타깃메니저.HideAllModelingContents();

        MainUI.메인.애니메이션동작_UI숨기기();

        MainUI.메인.uiEventLinkManager.SetActive(false);
    }

    public void WordGameStart(string trackableTargetName)
    {
        if (isRecog == false)
        {
            isRecog = true;
            Debug.Log("단어 게임 시작");
            TargetManager.EnableTracking = false;
            //임시
            MainUI.메인.인식글자UI.SetActive(false);

            loadingUI.SetActive(true);

            pauseBtn.SetActive(true);

            WordGameManager.instance.isPause = false;

            WordGameManager.instance.GameStart(trackableTargetName);
        }
    }

    public void CreateAlphabetCard(string currentQuestion, int index)
    {
        for (int i = 0; i < index; i++)
        {
            WordGameManager.instance.spelling[i] = currentQuestion[i];

            alphabetCard[i] = Instantiate(alphabetCard_Original);
            alphabetCard[i].transform.parent = parentAlphabetCard.transform;
            alphabetCard[i].transform.localScale = Vector3.one * 100;

            alphabetCard[i].transform.GetChild(0).GetComponent<UILabel>().text = WordGameManager.instance.spelling[i].ToString();

            AlphabetCardAlphaColorSet(alphabetCard[i].transform.GetChild(0).GetComponent<UILabel>(), 80f);

            WordGameManager.instance.answerCheck[i] = false;
        }

        parentAlphabetCard.GetComponent<UIGrid>().repositionNow = true;

    }

    public void AlphabetCardColorSet(GameObject card)
    {
        int colorNum = -1;

        switch (card.transform.GetChild(0).GetComponent<UILabel>().text[0])
        {
            case 'A':
                colorNum = 1;
                break;
            case 'B':
                colorNum = 2;
                break;
            case 'C':
                colorNum = 3;
                break;
            case 'D':
                colorNum = 4;
                break;
            case 'E':
                colorNum = 5;
                break;
            case 'F':
                colorNum = 6;
                break;
            case 'G':
                colorNum = 1;
                break;
            case 'H':
                colorNum = 2;
                break;
            case 'I':
                colorNum = 3;
                break;
            case 'J':
                colorNum = 4;
                break;
            case 'K':
                colorNum = 7;
                break;
            case 'L':
                colorNum = 6;
                break;
            case 'M':
                colorNum = 1;
                break;
            case 'N':
                colorNum = 2;
                break;
            case 'O':
                colorNum = 3;
                break;
            case 'P':
                colorNum = 5;
                break;
            case 'Q':
                colorNum = 7;
                break;
            case 'R':
                colorNum = 6;
                break;
            case 'S':
                colorNum = 1;
                break;
            case 'T':
                colorNum = 2;
                break;
            case 'U':
                colorNum = 4;
                break;
            case 'V':
                colorNum = 5;
                break;
            case 'W':
                colorNum = 7;
                break;
            case 'X':
                colorNum = 6;
                break;
            case 'Y':
                colorNum = 1;
                break;
            case 'Z':
                colorNum = 8;
                break;
        }

        switch (colorNum)
        {
            case 1:
                card.GetComponent<UISprite>().spriteName = "Wordgame_dice_red";
                break;
            case 2:
                card.GetComponent<UISprite>().spriteName = "Wordgame_dice_yellow";
                break;
            case 3:
                card.GetComponent<UISprite>().spriteName = "Wordgame_dice_orange";
                break;
            case 4:
                card.GetComponent<UISprite>().spriteName = "Wordgame_dice_green";
                break;
            case 5:
                card.GetComponent<UISprite>().spriteName = "Wordgame_dice_sky";
                break;
            case 6:
                card.GetComponent<UISprite>().spriteName = "Wordgame_dice_purple";
                break;
            case 7:
                card.GetComponent<UISprite>().spriteName = "Wordgame_dice_blue";
                break;
            case 8:
                card.GetComponent<UISprite>().spriteName = "Wordgame_dice_gray";
                break;
        }
    }

    public void AlphabetCardAlphaColorSet(UILabel label, float alpha)
    {
        label.alpha = alpha / 255f;
    }

    public void RetryBtnClick()
    {
        resultUI.SetActive(false);
        pauseBtn.GetComponent<BoxCollider2D>().enabled = true;

        WordGameManager.instance.sideMenuBtn.GetComponent<BoxCollider2D>().enabled = true;

        WordGameManager.instance.RetrySet();
    }

    public void NextLevelBtnClick()
    {
        resultUI.SetActive(false);
        pauseBtn.GetComponent<BoxCollider2D>().enabled = true;
        
        WordGameManager.instance.sideMenuBtn.GetComponent<BoxCollider2D>().enabled = true;

        WordGameManager.instance.NextLevelSet();
    }

    public void QuitBtnClick()
    {
        resultUI.SetActive(false);
        pauseBtn.SetActive(false);
        pauseUI.SetActive(false);
        pauseBtn.GetComponent<BoxCollider2D>().enabled = true;
        isRecog = false;

        TargetManager.EnableTracking = true;

        WordGameManager.instance.sideMenuBtn.GetComponent<BoxCollider2D>().enabled = true;

        WordGameManager.instance.GameQuit();
    }

    public void PauseBtnClick()
    {
        pauseBtn.GetComponent<BoxCollider2D>().enabled = false;
        WordGameManager.instance.isPause = true;
        pauseUI.SetActive(true);
        WordGameManager.instance.sideMenuBtn.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void PauseUIClose()
    {
        pauseBtn.GetComponent<BoxCollider2D>().enabled = true;
        WordGameManager.instance.isPause = false;
        pauseUI.SetActive(false);
        WordGameManager.instance.sideMenuBtn.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void AlphabetCardLevelSet(int level)
    {
        if (level == 1)
        {
            return;
        }
        else if (level == 2)
        {
            for (int i = 0; i < alphabetCard.Length / 2; i++)
            {
                int num = 0;
                while (true)
                {
                    num = Random.Range(0, alphabetCard.Length);

                    if (alphabetCard[num].transform.GetChild(0).GetComponent<UILabel>().alpha == 0f)
                    {
                        continue;
                    }
                    else
                    {
                        alphabetCard[num].transform.GetChild(0).GetComponent<UILabel>().alpha = 0f;
                        break;
                    }
                }
            }
        }
        else if (level == 3)
        {
            for (int i = 0; i < alphabetCard.Length; i++)
            {
                alphabetCard[i].transform.GetChild(0).GetComponent<UILabel>().alpha = 0f;
            }
        }
        else
        {
            Debug.LogError("지정된 난이도 범위를 벗어남 : " + level);
        }
    }

    public void AlphabetCardDestry()
    {
        WordGameManager.instance.TrashDestroyStop();

        preAlphabetCard = alphabetCard;
        alphabetCard = null;

        for (int i = 0; i < preAlphabetCard.Length; i++)
        {
            if (preAlphabetCard[i] != null)
            {
                preAlphabetCard[i].transform.parent = WordGameManager.instance.trashCan.transform;
                preAlphabetCard[i].transform.localScale = Vector3.zero;
                preAlphabetCard[i].SetActive(false);
                //WordGameManager.instance.trashObj.Add(preAlphabetCard[i]);
            }

        }

        WordGameManager.instance.TrashDestroyStart();
        preAlphabetCard = null;
    }

}