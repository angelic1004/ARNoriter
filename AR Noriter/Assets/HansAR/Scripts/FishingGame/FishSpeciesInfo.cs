using UnityEngine;
using System.Collections;
using System;

public static class FishSpeciesInfo
{
    /// <summary>
    /// 물고기 종류
    /// </summary>
    public enum FishSpecies
    {
        Blowfish,               // 복어            레벨1
        Lionfish,               // 쏠배감펭        레벨1
        Raccoon_Butterfly,      // 라쿤버터플라이  레벨1
        Clownfish,              // 흰동가리        레벨1
        Blue_Tang,              // 블루탱          레벨1
        Beluga,                 // 벨루가          레벨2
        Napoleon_Fish,          // 나폴레옹피쉬    레벨2
        Grey_Nurse_Shark,       // 청새리상어      레벨2
        Porcupine_Fish,         // 가시복          레벨2
        Giant_Grouper,          // 자이언트 그루퍼 레벨3
        Manta_Ray,              // 만타 가오리     레벨3
        Blacktip_Shark,         // 블랙팁 샤크     레벨3
        none
        //SMALL_FISH,         // 작은 물고기
        //MIDDLE_FISH,        // 중간 물고기
        //BIG_FISH            // 큰 물고기
    }

    public enum TrapSpecies
    {
        shoes,
        starFish,
        can,
        clam,
        none
    }

    [Serializable]
    public class Fish
    {
        public FishSpecies species;

        [HideInInspector]
        public int strength;            // 물고기 힘

        [HideInInspector]
        public int upPoint;             // 화면 터치할때마다 게이지 올라가는 포인트

        [HideInInspector]
        public float length;

        [HideInInspector]
        public string textValueName;
    }

    [Serializable]
    public class Trap
    {
        public TrapSpecies species;

        [HideInInspector]
        public int strength;            // 물고기 힘

        [HideInInspector]
        public int upPoint;             // 화면 터치할때마다 게이지 올라가는 포인트

        [HideInInspector]
        public float length;

        [HideInInspector]
        public string textValueName;
    }


    public static Fish SetFishInfo(Fish fishInfo)
    {
        switch (fishInfo.species)
        {
            //1레벨
            case FishSpecies.Blowfish:
                fishInfo.strength = 8;
                fishInfo.upPoint = 35;
                fishInfo.length = UnityEngine.Random.Range(0.4f, 1.0f);
                fishInfo.textValueName = "BlowFish";
                break;

            case FishSpecies.Lionfish:
                fishInfo.strength = 8;
                fishInfo.upPoint = 35;
                fishInfo.length = UnityEngine.Random.Range(0.4f, 1.0f);
                fishInfo.textValueName = "LionFish";
                break;

            case FishSpecies.Raccoon_Butterfly:
                fishInfo.strength = 8;
                fishInfo.upPoint = 35;
                fishInfo.length = UnityEngine.Random.Range(0.4f, 1.0f);
                fishInfo.textValueName = "Raccoon_ButterFly";
                break;

            case FishSpecies.Clownfish:
                fishInfo.strength = 8;
                fishInfo.upPoint = 35;
                fishInfo.length = UnityEngine.Random.Range(0.4f, 1.0f);
                fishInfo.textValueName = "ClowFish";
                break;

            case FishSpecies.Blue_Tang:
                fishInfo.strength = 8;
                fishInfo.upPoint = 35;
                fishInfo.length = UnityEngine.Random.Range(0.4f, 1.0f);
                fishInfo.textValueName = "BlueTang";
                break;
            //2레벨
            case FishSpecies.Beluga:
                fishInfo.strength = 12;
                fishInfo.upPoint = 25;
                fishInfo.length = UnityEngine.Random.Range(1.4f, 2.0f);
                fishInfo.textValueName = "Beluga";
                break;

            case FishSpecies.Napoleon_Fish:
                fishInfo.strength = 12;
                fishInfo.upPoint = 25;
                fishInfo.length = UnityEngine.Random.Range(1.4f, 2.0f); ;
                fishInfo.textValueName = "NapoleonFish";
                break;

            case FishSpecies.Grey_Nurse_Shark:
                fishInfo.strength = 12;
                fishInfo.upPoint = 25;
                fishInfo.length = UnityEngine.Random.Range(1.4f, 2.0f); ;
                fishInfo.textValueName = "Grey_Nursh_Shark";
                break;

            case FishSpecies.Porcupine_Fish:
                fishInfo.strength = 12;
                fishInfo.upPoint = 25;
                fishInfo.length = UnityEngine.Random.Range(1.4f, 2.0f); ;
                fishInfo.textValueName = "Porcupine_Fish";
                break;
            //3레벨
            case FishSpecies.Giant_Grouper:
                fishInfo.strength = 14;
                fishInfo.upPoint = 15;
                fishInfo.length = UnityEngine.Random.Range(1.4f, 2.0f); ;
                fishInfo.textValueName = "Giant_Grouper";
                break;

            case FishSpecies.Manta_Ray:
                fishInfo.strength = 14;
                fishInfo.upPoint = 15;
                fishInfo.length = UnityEngine.Random.Range(5.0f, 10.0f);
                fishInfo.textValueName = "Manta_Ray";
                break;

            case FishSpecies.Blacktip_Shark:
                fishInfo.strength = 14;
                fishInfo.upPoint = 15;
                fishInfo.length = UnityEngine.Random.Range(5.0f, 10.0f); ;
                fishInfo.textValueName = "BlackTip_Shark";
                break;

            default:
                Debug.Log("물고기 설정 확인 필요");
                break;
        }

      //  fishInfo.strength = (int)(fishInfo.length / 2);

        return fishInfo;
    }


    public static Trap SetTrapInfo(Trap trapInfo)
    {
        switch (trapInfo.species)
        {
            case TrapSpecies.can:
                trapInfo.strength = 5;
                trapInfo.upPoint = 45;
                trapInfo.length = UnityEngine.Random.Range(0.05f, 0.1f);
                trapInfo.textValueName = "Can";
                break;

            case TrapSpecies.clam:
                trapInfo.strength = 8;
                trapInfo.upPoint = 45;
                trapInfo.length = UnityEngine.Random.Range(0.05f, 0.15f); ;
                trapInfo.textValueName = "Clam";
                break;

            case TrapSpecies.shoes:
                trapInfo.strength = 10;
                trapInfo.upPoint = 45;
                trapInfo.length = UnityEngine.Random.Range(0.22f, 0.26f); ;
                trapInfo.textValueName = "Shoes";
                break;

            case TrapSpecies.starFish:
                trapInfo.strength = 12;
                trapInfo.upPoint = 45;
                trapInfo.length = UnityEngine.Random.Range(0.05f, 0.1f); ;
                trapInfo.textValueName = "StarFish";
                break;

            default:
                Debug.Log("물고기 설정 확인 필요");
                break;
        }

       // trapInfo.strength = (int)(trapInfo.length / 2);

        return trapInfo;
    }

}
