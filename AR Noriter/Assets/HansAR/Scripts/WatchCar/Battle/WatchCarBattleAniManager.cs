using UnityEngine;
using System;
using System.Collections;

public class WatchCarBattleAniManager : MonoBehaviour
{

    [Serializable]
    public class BattleAnInfo
    {
        public string m_idleTriggerName;
        public string m_wakeUpTriggerName;
        public string[] m_AttackTriggerName;
        public string m_SkillTriggerName;
        public string m_underAttackTriggerName;
        public string m_victoryTriggerName;
        public string m_realizedTriggerName;
    }

    [SerializeField]
    public BattleAnInfo m_battelAniInfo;

    public static WatchCarBattleAniManager watchBattleAniManager;

    void Awake()
    {
        watchBattleAniManager = this;
    }

    public void TargetModelInit()
    {

    }

    /// <summary>
    /// 차량모델 모두 idle 애니메이션 실행
    /// </summary>
    public void BattleModelInit()
    {
        for(int i=0; i< WatchCarBattleManager.watchCarBattleManager.m_copyContent.Length; i++)
        {
            BattleModelIdle(WatchCarBattleManager.watchCarBattleManager.m_copyContent[i]);
        }
    }

    /// <summary>
    /// 차량 모델 idle 애니메이션 실행
    /// </summary>
    /// <param name="obj"></param>
    public void BattleModelIdle(GameObject obj)
    {
        Animator anim = obj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>();
        anim.SetTrigger(m_battelAniInfo.m_idleTriggerName);
    }

    /// <summary>
    /// 차량 모델 wakup 애니메이션 실행
    /// </summary>
    /// <param name="obj"></param>
    public void BattleModelWakeUp(GameObject obj)
    {
        Animator anim = obj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>();
        anim.SetTrigger(m_battelAniInfo.m_wakeUpTriggerName);
    }

    /// <summary>
    /// 차량 모델 기본 attack 애니메이션 실행
    /// </summary>
    /// <param name="obj"></param>
    public void BattleModelAttack(GameObject obj)
    {
        Animator anim = obj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>();
        anim.SetTrigger(m_battelAniInfo.m_AttackTriggerName[0]);
    }

    /// <summary>
    /// 차량 모델 doubleAttack 애니메이션 실행
    /// </summary>
    /// <param name="obj"></param>
    public void BattleModelDoubleAttack(GameObject obj)
    {
        Animator anim = obj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>();
        anim.SetTrigger(m_battelAniInfo.m_AttackTriggerName[1]);
    }

    /// <summary>
    /// 차량 모델 skill 애니메이션 실행
    /// </summary>
    /// <param name="obj"></param>
    public void BattleModelSkill(GameObject obj)
    {
        Animator anim = obj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>();
        anim.SetTrigger(m_battelAniInfo.m_SkillTriggerName);
    }

    /// <summary>
    /// 차량 모델 underAttack 애니메이션 실행
    /// </summary>
    /// <param name="obj"></param>
    public void BattleModelUnderAttack(GameObject obj)
    {
        Debug.Log("UnderAttack");
        Debug.Log("obj : " + obj);
        Animator anim = obj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>();
        anim.SetTrigger(m_battelAniInfo.m_underAttackTriggerName);
    }

    /// <summary>
    /// 차량 모델 victory 애니메이션 실행
    /// </summary>
    /// <param name="obj"></param>
    public void BattleModelVictory(GameObject obj)
    {
        Animator anim = obj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>();
        anim.SetTrigger(m_battelAniInfo.m_victoryTriggerName);
    }

    /// <summary>
    /// 차량 모델 realized 애니메이션 실행
    /// </summary>
    /// <param name="obj"></param>
    public void BattleModelRealized(GameObject obj)
    {
        Animator anim = obj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>();
        anim.SetTrigger(m_battelAniInfo.m_realizedTriggerName);
    }

}
