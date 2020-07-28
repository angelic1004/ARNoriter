using UnityEngine;
using System.Collections;
using System;

public class KartPlayerManager : Singleton<KartPlayerManager>
{
    private float distance;
    public float moveSpeed;                         // 방향전환시 이동 속도
    public Transform movePoint;
    public Transform itemParent;
    private float rotateTime;
    private Vector3 dir;
    private int rotRadius;                           // 방향전환시 회전 반경


    private void Awake()
    {
        moveSpeed = 16f;
        distance = 0;
        StartCoroutine(SwitchCarDirection());
        StartCoroutine(ChangeCarRotation());
    }

    private void OnTriggerEnter(Collider col)
    {
        try
        {
            switch (col.gameObject.tag)
            {
                case "kartgame_fuel":
                    KartGameManager.getInstance.GetFuelItem();
                    col.gameObject.SetActive(false);
                    col.gameObject.transform.parent = itemParent;
                    col.gameObject.transform.position = Vector3.zero;
                    break;

                case "kartgame_item":
                    col.gameObject.SetActive(false);
                    KartItemManager.getInstance.GetCollectItem(col.gameObject.name.ToLower());
                    col.gameObject.transform.parent = itemParent;
                    col.gameObject.transform.position = Vector3.zero;
                    break;

                case "kartgame_obstacle":
                    KartGameManager.getInstance.CrashCar();
                    col.gameObject.SetActive(false);
                    //StartCoroutine(FadeObstacle(col.gameObject));
                    break;

                case "kartgame_booster":
                    //col.gameObject.SetActive(false);
                    KartGameManager.getInstance.PlayBooster();
                    break;

                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(string.Format("Error Message : {0}, Function Name : OnTriggerEngter - KartPlayerManager", ex.Message));
        }
    }

    /// <summary>
    /// 자동차 방향 전환
    /// </summary>
    /// <param name="movePoint">움직일 대상 지점</param>
    /// <returns></returns>
    private IEnumerator SwitchCarDirection()
    {
        //float moveSpeed = 20f;

        //KartGameManager.getInstance.fixedCarPos = true;

        while (true)
        {
            if (distance > 0.1)
            {
                // 방향 이동
                //carObj.transform.localPosition = Vector3.Lerp(carObj.transform.localPosition, movePoint.localPosition, moveSpeed * Time.deltaTime);
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, movePoint.localPosition, moveSpeed * Time.deltaTime);
                distance = Mathf.Abs(movePoint.localPosition.x - transform.localPosition.x);

            }
            yield return new WaitForFixedUpdate();
        }

        //KartGameManager.getInstance.fixedCarPos = false;
    }

    /// <summary>
    /// 자동차 회전 변경 (이동시 꺾는 효과)
    /// </summary>
    private IEnumerator ChangeCarRotation()
    {
        rotateTime = 0;
        rotRadius = 6;

        while (true)
        {
            if (rotateTime > 0)
            {
                dir = movePoint.GetChild(0).position - transform.position;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotRadius * Time.deltaTime);

                rotateTime -= Time.deltaTime;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public void SwitchCar(Transform movePos)
    {
        //StopAllCoroutines();
        //StartCoroutine(SwitchCarDirection(movePos));
        movePoint = movePos;
        rotateTime = 2f;
        distance = Mathf.Abs(movePoint.localPosition.x - transform.localPosition.x);
    }

    /// <summary>
    /// 장애물 충돌시 사라지는 효과
    /// </summary>
    private IEnumerator FadeObstacle(GameObject item)
    {
        float speed = 1;

        Material mat = item.GetComponent<Renderer>().material;

        while (mat.color.a >= 0)
        {
            yield return new WaitForEndOfFrame();

            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, mat.color.a - Time.deltaTime * speed);
        }

        yield return new WaitForEndOfFrame();
        item.SetActive(false);

        item.transform.parent = itemParent;
        item.transform.localPosition = Vector3.zero;
        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 255);
    }
}
