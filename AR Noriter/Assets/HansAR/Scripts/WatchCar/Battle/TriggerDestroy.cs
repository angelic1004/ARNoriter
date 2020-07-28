using UnityEngine;
using System.Collections;

public class TriggerDestroy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Destroy(gameObject);
        Debug.Log("여기 부딪힘" + other.transform.parent.gameObject.name);
        if (WatchCarBattleManager.watchCarBattleManager.m_nowCard == WatchCarBattleManager.CardState.fail)
        {
            if (other.transform.parent.gameObject == WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj)
            {
                Debug.Log("fail 공격당함");
                //Destroy(gameObject);
                if (WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_character == WatchCarBattleManager.Character.porty)
                {
                    WatchCarEffectManager.watchCarEffectManager.BeAttackedEffect();
                    Destroy(gameObject);
                }
                else
                {
                    WatchCarEffectManager.watchCarEffectManager.BeAttackedEffect();
                    Destroy(gameObject);
                    //TweenManager.tween_Manager.AddTweenScale(gameObject, gameObject.transform.localScale, Vector3.zero, 0.3f);
                    //TweenManager.tween_Manager.TweenScale(gameObject);
                }
            }
        }
        else
        {
            if (other.transform.parent.gameObject == WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_modelObj)
            {
                Debug.Log("success 공격성공");
                //Destroy(gameObject);
                if (WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character == WatchCarBattleManager.Character.porty)
                {
                    WatchCarEffectManager.watchCarEffectManager.BeAttackedEffect();
                    Destroy(gameObject);
                }
                else
                {
                    WatchCarEffectManager.watchCarEffectManager.BeAttackedEffect();
                    Destroy(gameObject);
                    //TweenManager.tween_Manager.AddTweenScale(gameObject, gameObject.transform.localScale, Vector3.zero, 0.3f);
                    //TweenManager.tween_Manager.TweenScale(gameObject);
                }
            }
        }
    }
}
