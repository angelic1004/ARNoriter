using UnityEngine;
using System.Collections;

public class WatchCarEffectManager : MonoBehaviour
{
    public string m_effectInfoName;
    public GameObject m_effectInfoObj;
    public Color m_mycarCheckMinColor;
    public Color m_mycarCheckMaxColor;

    public static WatchCarEffectManager watchCarEffectManager;

    void Awake()
    {
        watchCarEffectManager = this;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void WatchCarEffectInit()
    {
        ShieldEffectDel();
        DoubleAttackEffectDel();
    }

    public void MyCarCheckEffectCall()
    {
        MyCarCheckEffectSet();
    }

    public void PlayerNormalEffectSet()
    {
        switch(WatchCarBattleManager.watchCarBattleManager.m_nowCard)
        {
            case WatchCarBattleManager.CardState.success:
                if(WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character == WatchCarBattleManager.Character.porty)
                {
                  //  MissileAttackEffectCall(WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj);
                }
                else
                {

                }
                //DoubleAttackEffectDel();
                //BuffDoubleAttackEffectDel();
                break;

            case WatchCarBattleManager.CardState.fail:
                //ShieldEffectCall();
                if (WatchCarBattleManager.watchCarBattleManager.shield)
                {
                    ShieldEffectCall();
                }
               
                break;
            case WatchCarBattleManager.CardState.buff:
                if (WatchCarBattleManager.watchCarBattleManager.m_nowBuffCard == WatchCarBattleManager.CardBuffState.doubleAttack)
                {
                    if (WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character == WatchCarBattleManager.Character.porty)
                    {
                      //  MissileAttackEffectCall(WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj);
                    }
                    else
                    {

                    }
                    //DoubleAttackEffectDel();
                    // BuffDoubleAttackEffectDel();
                }
                else
                {
                    Debug.Log("힐 또는 방어막 카드입니다.");
                }
                break;
            default:
                Debug.Log("카드상태 확인 부탁드립니다.");
                return;
        }
    }

    public void EnemyNormalEffectSet()
    {
        switch (WatchCarBattleManager.watchCarBattleManager.m_nowCard)
        {
            case WatchCarBattleManager.CardState.fail:
                if (WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_character == WatchCarBattleManager.Character.porty)
                {
                  //  MissileAttackEffectCall(WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_modelObj);
                }
                else
                {

                }

                break;
            default:
                Debug.Log("enemy 공격 상태 아님");
                return;
        }
    }

    public void BuffEffectSet()
    {
        switch(WatchCarBattleManager.watchCarBattleManager.m_nowBuffCard)
        {
            case WatchCarBattleManager.CardBuffState.doubleAttack:
                DoubleAttackEffectCall();
                break;

            case WatchCarBattleManager.CardBuffState.shield:
                ShieldEffectCall();
                break;

            case WatchCarBattleManager.CardBuffState.heal:
                RecoveryEffectCall();
                break;

            default:
                Debug.Log("버프 설정이 잘못되었습니다.");
                return;
        }
    }

    public void BeAttackedEffect()
    {
        switch (WatchCarBattleManager.watchCarBattleManager.m_nowCard)
        {
            case WatchCarBattleManager.CardState.success:
                if(WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character == WatchCarBattleManager.Character.porty)
                {
                    BeAttackedMissileEffectCall(WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_modelObj);
                }
                else
                {
                    BeAttackedNormalEffectCall(WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_modelObj);
                }
                break;

            case WatchCarBattleManager.CardState.fail:
                if (WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_character == WatchCarBattleManager.Character.porty)
                {
                    BeAttackedMissileEffectCall(WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj);
                }
                else
                {
                    BeAttackedNormalEffectCall(WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj);
                }
                break;
            case WatchCarBattleManager.CardState.buff:
                if (WatchCarBattleManager.watchCarBattleManager.m_nowBuffCard == WatchCarBattleManager.CardBuffState.doubleAttack)
                {
                    if (WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character == WatchCarBattleManager.Character.porty)
                    {
                        BeAttackedMissileEffectCall(WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_modelObj);
                    }
                    else
                    {
                        BeAttackedDoubleEffectCall(WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_modelObj);
                    }
                }
                else
                {
                    Debug.Log("힐 또는 방어막 카드입니다.");
                }
                break;
            default:
                Debug.Log("카드상태 확인 부탁드립니다.");
                return;
        }
    }

    public void UiBuffDoubleAttackEffect(GameObject obj)
    {
        UiBuffDoubleAttackEffectCall(obj);
    }


    public void UiBuffSheildEffect(GameObject obj)
    {
        UiBuffShieldEffectCall(obj);
    }

    private void MyCarCheckEffectSet()
    {
        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_mycarCheckPar == null)
        {
            Debug.Log("m_mycarCheckPar 파티클이 없습니다.");
            return;
        } 

        GameObject par = null;

        Transform[] tempTransforms = WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj.GetComponentsInChildren<Transform>();

        foreach (Transform child in tempTransforms)
        {
            if (child.name.Contains(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_mycarCheckPar.name))
            {
                par = child.gameObject;
                Debug.Log("child pos : " + child.name);
                break;
            }
        }

        if (par == null)
        {
            par = Instantiate(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_mycarCheckPar, Vector3.zero, Quaternion.identity) as GameObject;
            par.transform.parent = WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj.transform;
            par.transform.localPosition = new Vector3(0, 0.1f, 0);
            par.name = par.name.Split('(')[0];

            MyCarCheckEffectColorSet(par);

            par.SetActive(true);
        }
        else
        {
            MyCarCheckEffectColorSet(par);
        }

    }

    private void AttackEffectCall()
    {
        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_attackParticle == null)
        {
            Debug.Log("어택 파티클이 없습니다.");
            return;
        }

        GameObject par;

        par = Instantiate(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_attackParticle, Vector3.zero, Quaternion.identity) as GameObject;
        par.transform.parent = WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj.transform;
        par.transform.localPosition = new Vector3(0, 0.1f, 0);
        par.name = par.name.Split('(')[0];
        par.SetActive(true);
    }

    private void MissileAttackEffectCall(GameObject obj)
    {
        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_missileAttackPar == null)
        {
            Debug.Log("어택 파티클이 없습니다.");
            return;
        }

        GameObject par;

        par = Instantiate(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_missileAttackPar, Vector3.zero, Quaternion.identity) as GameObject;
        par.transform.parent = obj.transform;
        par.transform.localPosition = new Vector3(0, 0.1f, 0);
        par.name = par.name.Split('(')[0];
        par.SetActive(true);
    }


    private void DoubleAttackEffectCall()
    {
        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_doubleAttackParticle == null)
        {
            Debug.Log("더블어택 파티클이 없습니다.");
            return;
        }

        GameObject par = null;
        Transform[] tempTransforms = WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj.GetComponentsInChildren<Transform>();

        foreach (Transform child in tempTransforms)
        {
            if (child.name.Contains(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_doubleAttackParticle.name))
            {
                par = child.gameObject;
                Debug.Log("child pos : " + child.name);
                break;
            }
        }

        if (par == null)
        {
            par = Instantiate(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_doubleAttackParticle, Vector3.zero, Quaternion.identity) as GameObject;
            par.transform.parent = WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj.transform;
            par.transform.localPosition = new Vector3(0, 0.2f, 0);
            par.name = par.name.Split('(')[0];
            par.SetActive(true);
        }
    }

    private void ShieldEffectCall()
    {
        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_shieldParticle == null)
        {
            Debug.Log("쉴드 파티클이 없습니다.");
            return;
        }

        GameObject par = null;
        Transform[] tempTransforms = WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj.GetComponentsInChildren<Transform>();

        foreach (Transform child in tempTransforms)
        {
            if (child.name.Contains(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_shieldParticle.name))
            {
                par = child.gameObject;
                Debug.Log("child pos : " + child.name);
                break;
            }
        }

        if (par == null)
        {
            par = Instantiate(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_shieldParticle, Vector3.zero, Quaternion.identity) as GameObject;
            par.transform.parent = WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj.transform;
            par.transform.localPosition = new Vector3(0, 0, 0);
            par.transform.localEulerAngles = new Vector3(-90, 0, 0);
            par.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            par.name = par.name.Split('(')[0];
            par.SetActive(true);
        }
    }

    private void RecoveryEffectCall()
    {
        if(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_recoveryParticle == null)
        {
            Debug.Log("리커버리 파티클이 없습니다.");
            return;
        }

        GameObject par;
        par = Instantiate(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_recoveryParticle, Vector3.zero, Quaternion.identity) as GameObject;
        par.transform.parent = WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj.transform;
        par.transform.localPosition = new Vector3(0, 0.2f, 0);
        par.name = par.name.Split('(')[0];
        par.SetActive(true);
    }


    private void BeAttackedNormalEffectCall(GameObject obj)
    {
        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_underAttackParticle == null)
        {
            Debug.Log("underAttack 파티클이 없습니다.");
            return;
        }

        GameObject par;
        par = Instantiate(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_underAttackParticle, Vector3.zero, Quaternion.identity) as GameObject;
        par.transform.parent = obj.transform;
        par.transform.localPosition = new Vector3(0, 0, 0);
        par.name = par.name.Split('(')[0];
        par.SetActive(true);
    }

    private void BeAttackedDoubleEffectCall(GameObject obj)
    {
        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_DoubleunderAttackParticle == null)
        {
            Debug.Log("underAttack 파티클이 없습니다.");
            return;
        }

        GameObject par;
        par = Instantiate(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_DoubleunderAttackParticle, Vector3.zero, Quaternion.identity) as GameObject;
        par.transform.parent = obj.transform;
        par.transform.localPosition = new Vector3(0, 0, 0);
        par.name = par.name.Split('(')[0];
        par.SetActive(true);
    }

    private void BeAttackedMissileEffectCall(GameObject obj)
    {
        int objCount = 0;

        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_missileUnderAttackPar == null)
        {
            Debug.Log("underAttackMissile 파티클이 없습니다.");
            return;
        }

        Transform[] tempTransforms = obj.GetComponentsInChildren<Transform>();

        foreach (Transform child in tempTransforms)
        {
            if (child.name.Contains(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_missileUnderAttackPar.name))
            {
                objCount++;
            }
        }

        if (objCount < 2)
        {
            GameObject par;
            par = Instantiate(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_missileUnderAttackPar, Vector3.zero, Quaternion.identity) as GameObject;
            par.transform.parent = obj.transform;
            
            par.transform.localPosition = new Vector3(Random.Range(0.1f, 0.2f), Random.Range(-0.03f, 0.03f), -0.2f);
            par.name = par.name.Split('(')[0];
            par.SetActive(true);
        }
    }

    private void MyCarCheckEffectDel()
    {
        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_mycarCheckPar == null)
        {
            Debug.Log("m_mycarCheckPar 파티클이 없습니다.");
            return;
        }

        GameObject obj = null;

        Transform[] tempTransforms = WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj.GetComponentsInChildren<Transform>();

        foreach (Transform child in tempTransforms)
        {
            if (child.name.Contains(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_mycarCheckPar.name))
            {
                obj = child.gameObject;
                Debug.Log("child pos : " + child.name);
                break;
            }
        }

        if (obj != null)
        {
            Destroy(obj);
        }
    }

    private void UiBuffDoubleAttackEffectCall(GameObject checkObj)
    {
        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_daBuffParticle == null)
        {
            Debug.Log("더블어택 버프 파티클이 없습니다.");
            return;
        }

        GameObject par = null;
        Transform[] tempTransforms = checkObj.GetComponentsInChildren<Transform>();

        foreach (Transform child in tempTransforms)
        {
            if (child.name.Contains(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_daBuffParticle.name))
            {
                par = child.gameObject;
                Debug.Log("child pos : " + child.name);
                break;
            }
        }
        if (par == null)
        {
            par = Instantiate(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_daBuffParticle, Vector3.zero, Quaternion.identity) as GameObject;
            par.layer = 9;
            par.transform.parent = checkObj.transform;
            par.transform.localPosition = Vector3.zero;
            par.name = par.name.Split('(')[0];
            par.SetActive(true);
        }
    }

    private void UiBuffShieldEffectCall(GameObject checkObj)
    {
        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_shBuffParticle == null)
        {
            Debug.Log("쉴드 버프 파티클이 없습니다.");
            return;
        }

        GameObject par = null;
        Transform[] tempTransforms = checkObj.GetComponentsInChildren<Transform>();

        foreach (Transform child in tempTransforms)
        {
            if (child.name.Contains(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_shBuffParticle.name))
            {
                par = child.gameObject;
                Debug.Log("child pos : " + child.name);
                break;
            }
        }

        if (par == null)
        {
            par = Instantiate(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_shBuffParticle, Vector3.zero, Quaternion.identity) as GameObject;
            par.layer = 9;
            par.transform.parent = checkObj.transform;
            par.transform.localPosition = Vector3.zero;
            par.name = par.name.Split('(')[0];
            par.SetActive(true);
        }
    }

    private void DoubleAttackEffectDel()
    {
        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_doubleAttackParticle == null)
        {
            Debug.Log("더블어택 파티클이 없습니다.");
            return;
        }

        GameObject obj = null;// = WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj.transform.FindChild(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_doubleAttackParticle.name).gameObject;

        Transform[] tempTransforms = WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj.GetComponentsInChildren<Transform>();

        foreach (Transform child in tempTransforms)
        {
            if (child.name.Contains(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_doubleAttackParticle.name))
            {
                obj = child.gameObject;
                Debug.Log("child pos : " + child.name);
                break;
            }
        }

        if (obj != null)
        {
            Destroy(obj);
        }
    }

    private void ShieldEffectDel()
    {
        if(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_shieldParticle == null)
        {
            Debug.Log("쉴드 파티클이 없습니다.");
            return;
        }

        GameObject obj = null;// WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj.transform.FindChild(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_shieldParticle.name).gameObject;

        Transform[] tempTransforms = WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj.GetComponentsInChildren<Transform>();

        foreach (Transform child in tempTransforms)
        {
            if (child.name.Contains(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_shieldParticle.name))
            {
                obj = child.gameObject;
                Debug.Log("child pos : " + child.name);
                break;
            }
        }

        if (obj != null)
        {
            Destroy(obj);
        }
    }

    private void BuffDoubleAttackEffectDel()
    {
        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_daBuffParticle == null)
        {
            Debug.Log("더블어택 버프 파티클이 없습니다.");
            return;
        }

        GameObject obj = null;

        for (int i = 0; i < WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffObjList.Length; i++)
        {
            Transform[] tempTransforms = WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffObjList[i].GetComponentsInChildren<Transform>();

            foreach (Transform child in tempTransforms)
            {
                if (child.name.Contains(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_daBuffParticle.name))
                {
                    obj = child.gameObject;
                    Debug.Log("child pos : " + child.name);
                    break;
                }
            }
        }

        if (obj != null)
        {
            Destroy(obj);
        }
    }

    private void BuffShieldEffectDel()
    {
        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_shBuffParticle == null)
        {
            Debug.Log("쉴드 버프 파티클이 없습니다.");
            return;
        }

        GameObject obj = null;

        for (int i = 0; i < WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffObjList.Length; i++)
        {
            Transform[] tempTransforms = WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffObjList[i].GetComponentsInChildren<Transform>();

            foreach (Transform child in tempTransforms)
            {
                if (child.name.Contains(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_shBuffParticle.name))
                {
                    obj = child.gameObject;
                    Debug.Log("child pos : " + child.name);
                    break;
                }
            }
        }

        if (obj != null)
        {
            Destroy(obj);
        }
    }

    private void MyCarCheckEffectColorSet(GameObject par)
    {
        switch(WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character)
        {
            case WatchCarBattleManager.Character.bluewill:
                m_mycarCheckMinColor = new Color32(0, 192, 255, 255);
                m_mycarCheckMaxColor = new Color32(0, 192, 255, 255);
                break;

            case WatchCarBattleManager.Character.avan:
                m_mycarCheckMinColor = new Color32(255, 0, 69, 255);
                m_mycarCheckMaxColor = new Color32(255, 0, 69, 255);
                break;

            case WatchCarBattleManager.Character.sona:
                m_mycarCheckMinColor = new Color32(255, 111, 239, 255);
                m_mycarCheckMaxColor = new Color32(255, 111, 239, 255);
                break;

            case WatchCarBattleManager.Character.porty:
                m_mycarCheckMinColor = new Color32(255, 248, 0, 255);
                m_mycarCheckMaxColor = new Color32(255, 248, 0, 255);
                break;

            case WatchCarBattleManager.Character.blood:
                m_mycarCheckMinColor = new Color32(95, 43, 255, 255);
                m_mycarCheckMaxColor = new Color32(95, 43, 255, 255);
                break;

            default:
                Debug.Log("WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character 부분 선택 안됨");
                return;

        }

        if (par.transform.childCount > 0)
        {
            par.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = m_mycarCheckMinColor;
        }

        ParticleSystem ps = par.GetComponent<ParticleSystem>();
        ParticleSystem.ColorOverLifetimeModule col = ps.colorOverLifetime;
        col.enabled = true;

        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(m_mycarCheckMinColor, 0.0f), new GradientColorKey(m_mycarCheckMaxColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 0.125f), new GradientAlphaKey(0.0f, 1.0f) });

        col.color = new ParticleSystem.MinMaxGradient(grad);

        if(WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character == WatchCarBattleManager.Character.porty)
        {
            par.transform.localPosition = new Vector3(0, -0.14f, 0);
        }
        else
        {
            par.transform.localPosition = new Vector3(0, -0.075f, 0);
        }

        par.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        
        par.transform.localEulerAngles = new Vector3(84.5f, 90, 90);
        
    }

    /// <summary>
    /// 턴에 따른 이펙트의 색상 변경
    /// </summary>
    /// <param name="par"></param>
    private void TurnCheckEffectColorSet(GameObject par)
    {
        WatchCarBattleManager.Character selectCha = WatchCarBattleManager.Character.none;
         
        if(WatchCarBattleManager.watchCarBattleManager.m_nowCard == WatchCarBattleManager.CardState.fail)
        {
            selectCha = WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_character;
        }
        else
        {
            selectCha = WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character;
        }

        switch (selectCha)
        {
            case WatchCarBattleManager.Character.bluewill:
                m_mycarCheckMinColor = new Color32(0, 192, 255, 255);
                m_mycarCheckMaxColor = new Color32(0, 192, 255, 255);
                break;

            case WatchCarBattleManager.Character.avan:
                m_mycarCheckMinColor = new Color32(255, 0, 69, 255);
                m_mycarCheckMaxColor = new Color32(255, 0, 69, 255);
                break;

            case WatchCarBattleManager.Character.sona:
                m_mycarCheckMinColor = new Color32(255, 111, 239, 255);
                m_mycarCheckMaxColor = new Color32(255, 111, 239, 255);
                break;

            case WatchCarBattleManager.Character.porty:
                m_mycarCheckMinColor = new Color32(255, 248, 0, 255);
                m_mycarCheckMaxColor = new Color32(255, 248, 0, 255);
                break;

            case WatchCarBattleManager.Character.blood:
                m_mycarCheckMinColor = new Color32(95, 43, 255, 255);
                m_mycarCheckMaxColor = new Color32(95, 43, 255, 255);
                break;

            default:
                Debug.Log("WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character 부분 선택 안됨");
                return;

        }


        ParticleSystem ps; //= null;
        ParticleSystem.ColorOverLifetimeModule col;// = ps.colorOverLifetime;
        Gradient grad;// = null;

        Transform[] tempTransforms = par.GetComponentsInChildren<Transform>();

        foreach (Transform child in tempTransforms)
        {
            if (child.GetComponent<ParticleSystem>() != null)
            {
                if (child.GetComponent<ParticleSystem>().colorOverLifetime.enabled)
                {
                    ps = child.GetComponent<ParticleSystem>();
                    col = ps.colorOverLifetime;
                    col.enabled = true;

                    grad = new Gradient();
                    grad.SetKeys(new GradientColorKey[] { new GradientColorKey(m_mycarCheckMinColor, 0.0f), new GradientColorKey(m_mycarCheckMaxColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 0.125f), new GradientAlphaKey(0.0f, 1.0f) });

                    col.color = new ParticleSystem.MinMaxGradient(grad);
                }
            }
        }

        if (selectCha == WatchCarBattleManager.Character.porty)
        {
            par.transform.localPosition = new Vector3(0, -0.14f, 0);
        }
        else
        {
            par.transform.localPosition = new Vector3(0, -0.075f, 0);
        }

        par.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

        par.transform.localEulerAngles = new Vector3(-90.0f, 0, 0);

    }

    /// <summary>
    /// 턴에 따른 이펙트 발생
    /// </summary>
    public void CallTurnEffect()
    {

        if (m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_turnCheckPar == null)
        {
            Debug.Log("m_turnCheckPar 파티클이 없습니다.");
            return;
        }

        GameObject par = null;
        GameObject turnObj = null;

        //상대방 턴
        if (WatchCarBattleManager.watchCarBattleManager.m_nowCard == WatchCarBattleManager.CardState.fail)
        {
            turnObj = WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_modelObj;
        }
        //플레이어 턴
        else
        {
            turnObj = WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_modelObj;
        }

        par = Instantiate(m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_turnCheckPar, Vector3.zero, Quaternion.identity) as GameObject;
        par.transform.parent = turnObj.transform;
        par.transform.localPosition = new Vector3(0, 0.1f, 0);
        par.name = par.name.Split('(')[0];

        TurnCheckEffectColorSet(par);

        par.SetActive(true);
    }
}
