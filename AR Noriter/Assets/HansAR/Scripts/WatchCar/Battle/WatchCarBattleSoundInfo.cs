using UnityEngine;
using System;
using System.Collections;

public class WatchCarBattleSoundInfo : MonoBehaviour
{
    public AudioClip m_versusBgm;
    public AudioClip m_winAudio;
    public AudioClip m_loseAudio;

    [Serializable]
    public class AttackAudioClip
    {
        public AudioClip m_attackNormal;
        public AudioClip m_attackDouble;
        public AudioClip m_attackMissile;
        public AudioClip m_attackDoubleMissile;
    }

    [Serializable]
    public class DefendAudioClip
    {
        public AudioClip m_defendNormal;
        public AudioClip m_defendDouble;
        public AudioClip m_defendMissile;
        public AudioClip m_defendDoubleMissile;
        public AudioClip m_defendShield;
    }

    [Serializable]
    public class SkillAudioClip
    {
        public AudioClip m_skillAttack;
        public AudioClip m_skillDouble;
        public AudioClip m_skillRecovery;
        public AudioClip m_skillShield;
    }

    [SerializeField]
    public AttackAudioClip m_attackAudio;

    [SerializeField]
    public DefendAudioClip m_defendAudio;

    [SerializeField]
    public SkillAudioClip m_skillAudio;
}
