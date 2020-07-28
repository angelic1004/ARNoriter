using UnityEngine;
using System.Collections;

public class WatchCarBattleSoundManager : MonoBehaviour
{
    public AudioSource versusAudioSource;
    public AudioSource attackAudioSource;
    public AudioSource defendAudioSource;

    public static WatchCarBattleSoundManager watchcarSoundManager;

    void Awake()
    {
        watchcarSoundManager = this;
    }

    public void AttackSoundCall()
    {
        if (WatchCarBattleManager.watchCarBattleManager.m_nowCard
            == WatchCarBattleManager.CardState.success)
        {
            if (WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character == WatchCarBattleManager.Character.porty)
            {
                AttackMissileSound();
            }
            else
            {
                AttackNormalSound();
            }
        }
        else if (WatchCarBattleManager.watchCarBattleManager.m_nowCard
            == WatchCarBattleManager.CardState.fail)
        {
            if (WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_character == WatchCarBattleManager.Character.porty)
            {
                AttackMissileSound();
            }
            else
            {
                AttackNormalSound();
            }
        }
        else
        {
            if (WatchCarBattleManager.watchCarBattleManager.doubleAttack)
            {
                if (WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character == WatchCarBattleManager.Character.porty)
                {
                    AttackDoubleMissileSound();
                }
                else
                {
                    AttackDoubleSound();
                }
            }
            else
            {
                Debug.Log("버프입니다.");
            }
        }
    }

    public void DefendSoundCall()
    {
        if (WatchCarBattleManager.watchCarBattleManager.m_nowCard
           == WatchCarBattleManager.CardState.success)
        {
            if (WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character == WatchCarBattleManager.Character.porty)
            {
                DefendMissileSound();
            }
            else
            {
                DefendNormalSound();
            }
        }
        else if (WatchCarBattleManager.watchCarBattleManager.m_nowCard
           == WatchCarBattleManager.CardState.fail)
        {
            if (WatchCarBattleManager.watchCarBattleManager.shield)
            {
                DefendShieldSound();
            }
            else
            {
                if (WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_character == WatchCarBattleManager.Character.porty)
                {
                    DefendMissileSound();
                }
                else
                {
                    DefendNormalSound();
                }
            }
        }
        else
        {
            if (WatchCarBattleManager.watchCarBattleManager.doubleAttack)
            {
                if (WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character == WatchCarBattleManager.Character.porty)
                {
                    DefendDoubleMissileSound();
                }
                else
                {
                    DefendDoubleSound();
                }
            }
            else
            {
                Debug.Log("버프입니다.");
            }
        }
    }

    public void SkillSoundCall()
    {
        switch (WatchCarBattleManager.watchCarBattleManager.m_nowBuffCard)
        {
            case WatchCarBattleManager.CardBuffState.doubleAttack:
                Debug.Log("더블은 사운드 재생 안함");
                //SkillDoubleSound();
                break;

            case WatchCarBattleManager.CardBuffState.shield:
                SkillShieldSound();
                break;

            case WatchCarBattleManager.CardBuffState.heal:
                SkillRecoverySound();
                break;

            default:
                Debug.Log("스킬이없습니다.");
                return;
        }
    }

    public void SkillAttackSoundCall()
    {
        if (WatchCarBattleManager.watchCarBattleManager.m_nowCard == WatchCarBattleManager.CardState.success
            || WatchCarBattleManager.watchCarBattleManager.m_nowCard == WatchCarBattleManager.CardState.fail)
        {
            SkillAttackSound();
        }
    }

    public void SkillDoubleSoundCall()
    {
        if(WatchCarBattleManager.watchCarBattleManager.m_nowBuffCard 
            == WatchCarBattleManager.CardBuffState.doubleAttack)
        {
            SkillDoubleSound();
        }
    }

    public void VersusSoundCall()
    {
        VersusBgmSound();
    }

    public void WinLoseSoundCall()
    {
        if(WatchCarBattleManager.watchCarBattleManager.m_nowGame == WatchCarBattleManager.GameState.win)
        {
            WinSound();
        }
        else
        {
            LoseSound();
        }
    }


    private void AttackNormalSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_attackAudio.m_attackNormal;

        if(attackAudioSource.clip != null)
        {
            attackAudioSource.Stop();
            attackAudioSource.clip = null;
        }

        attackAudioSource.clip = audioClip;
        attackAudioSource.Play();
    }

    private void AttackDoubleSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_attackAudio.m_attackDouble;

        if (attackAudioSource.clip != null)
        {
            attackAudioSource.Stop();
            attackAudioSource.clip = null;
        }

        attackAudioSource.clip = audioClip;
        attackAudioSource.Play();
    }

    private void AttackMissileSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_attackAudio.m_attackMissile;

        if (attackAudioSource.clip != null)
        {
            attackAudioSource.Stop();
            attackAudioSource.clip = null;
        }

        attackAudioSource.clip = audioClip;
        attackAudioSource.Play();
    }

    private void AttackDoubleMissileSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_attackAudio.m_attackDoubleMissile;

        if (attackAudioSource.clip != null)
        {
            attackAudioSource.Stop();
            attackAudioSource.clip = null;
        }

        attackAudioSource.clip = audioClip;
        attackAudioSource.Play();
    }

    private void DefendNormalSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_defendAudio.m_defendNormal;

        if (defendAudioSource.clip != null)
        {
            defendAudioSource.Stop();
            defendAudioSource.clip = null;
        }

        defendAudioSource.clip = audioClip;
        defendAudioSource.Play();
    }

    private void DefendDoubleSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_defendAudio.m_defendDouble;

        if (defendAudioSource.clip != null)
        {
            defendAudioSource.Stop();
            defendAudioSource.clip = null;
        }

        defendAudioSource.clip = audioClip;
        defendAudioSource.Play();
    }

    private void DefendMissileSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_defendAudio.m_defendMissile;

        if (defendAudioSource.clip != null)
        {
            defendAudioSource.Stop();
            defendAudioSource.clip = null;
        }

        defendAudioSource.clip = audioClip;
        defendAudioSource.Play();
    }

    private void DefendDoubleMissileSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_defendAudio.m_defendDoubleMissile;

        if (defendAudioSource.clip != null)
        {
            defendAudioSource.Stop();
            defendAudioSource.clip = null;
        }

        defendAudioSource.clip = audioClip;
        defendAudioSource.Play();
    }


    private void DefendShieldSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_defendAudio.m_defendShield;

        if (defendAudioSource.clip != null)
        {
            defendAudioSource.Stop();
            defendAudioSource.clip = null;
        }

        defendAudioSource.clip = audioClip;
        defendAudioSource.Play();
    }

    private void SkillAttackSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_skillAudio.m_skillAttack;

        if (attackAudioSource.clip != null)
        {
            attackAudioSource.Stop();
            attackAudioSource.clip = null;
        }

        attackAudioSource.clip = audioClip;
        attackAudioSource.Play();
    }

    private void SkillDoubleSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_skillAudio.m_skillDouble;

        if (attackAudioSource.clip != null)
        {
            attackAudioSource.Stop();
            attackAudioSource.clip = null;
        }

        attackAudioSource.clip = audioClip;
        attackAudioSource.Play();
    }

    private void SkillRecoverySound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_skillAudio.m_skillRecovery;

        if (attackAudioSource.clip != null)
        {
            attackAudioSource.Stop();
            attackAudioSource.clip = null;
        }

        attackAudioSource.clip = audioClip;
        attackAudioSource.Play();
    }

    private void SkillShieldSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_skillAudio.m_skillShield;

        if (attackAudioSource.clip != null)
        {
            attackAudioSource.Stop();
            attackAudioSource.clip = null;
        }

        attackAudioSource.clip = audioClip;
        attackAudioSource.Play();
    }

    private void VersusBgmSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_versusBgm;

        if (versusAudioSource.clip != null)
        {
            versusAudioSource.Stop();
            versusAudioSource.clip = null;
        }

        versusAudioSource.clip = audioClip;
        versusAudioSource.Play();
    }

    private void WinSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_winAudio;

        if (versusAudioSource.clip != null)
        {
            versusAudioSource.Stop();
            versusAudioSource.clip = null;
        }

        versusAudioSource.clip = audioClip;
        versusAudioSource.Play();
    }

    private void LoseSound()
    {
        WatchCarBattleSoundInfo info = WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<WatchCarBattleSoundInfo>();
        AudioClip audioClip = info.m_loseAudio;

        if (versusAudioSource.clip != null)
        {
            versusAudioSource.Stop();
            versusAudioSource.clip = null;
        }

        versusAudioSource.clip = audioClip;
        versusAudioSource.Play();
    }

}
