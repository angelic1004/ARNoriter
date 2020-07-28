using UnityEngine;
using System;
using System.Collections;

public class ParticleInfo : MonoBehaviour
{
    [Serializable]
    public class WatchCarBattle
    {
        public GameObject m_mycarCheckPar;
        public GameObject m_turnCheckPar;

        public GameObject m_attackParticle;
        public GameObject m_missileAttackPar;

        public GameObject m_missileUnderAttackPar;
        public GameObject m_underAttackParticle;
        public GameObject m_DoubleunderAttackParticle;

        public GameObject m_doubleAttackParticle;
        public GameObject m_shieldParticle;
        public GameObject m_recoveryParticle;

        public GameObject m_daBuffParticle;
        public GameObject m_shBuffParticle;

        public GameObject m_missile;
        public GameObject m_railGun;
    }

    [SerializeField]
    public WatchCarBattle m_watchCarBattle;
}
