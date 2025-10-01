using RimWorld;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using Verse;
using static Lilly.PlantsPatch2.MyPlant;

namespace Lilly.PlantsPatch2

{
    public class TreeSetupEntry : IExposable
    {
        public string defName="";
        public MyPlant plant = new MyPlant();

        public void ExposeData()
        {
            Scribe_Values.Look(ref defName, "defName");
            Scribe_Deep.Look(ref plant, "plant");
        }
    }

    public enum PlantEnum
    {
        growDays, // 성장일
        fertilityMin,// 최소 비옥도
        fertilitySensitivity, // 토양 민감도
        sowWork,//심는 작업량
        harvestWork, // 수확량
        harvestYield, // 수확량
        //harvestTag, // 수확 종류
        growMinGlow,//최소 광량
        growOptimalGlow, // 최고 광량 0~100
        harvestMinGrowth,
        harvestAfterGrowth, // 수확 가능 성장율
        minGrowthTemperature,
        minOptimalGrowthTemperature,
        maxGrowthTemperature,
        maxOptimalGrowthTemperature,
        minSpacingBetweenSamePlant,// 최소 간격
        lifespanDaysPerGrowDays, // 수명
        plantRespawningCommonalityFactor,// 재생성 확률 0~1
        cavePlantWeight,    // 동굴 확률
        topWindExposure, // 바람에 흔들리는 정도 0~1
        wildClusterWeight,// 군집 확률 0~1
    }

    public class MyPlant : IExposable

    {
        public Dictionary<PlantEnum, float> attributes = new Dictionary<PlantEnum, float>();
        public PlantProperties plantProperties =null;

        public float this[PlantEnum kind]
        {
            get => attributes.ContainsKey(kind) ? attributes[kind] : 0;
            set => attributes[kind] = value;
        }

        //public float growDays=1f;// 나무 성장 시간
        //public float harvestYield=1f;// 수확량

        public MyPlant() { }    // 매개변수 생성자

        public MyPlant(MyPlant plant)  
        {
            this.plantProperties = plant.plantProperties;
            foreach (var kv in plant.attributes)
            {    
                this[kv.Key] = kv.Value;
            }
        }

        // PlantProperties 기반 생성자
        public MyPlant(PlantProperties plant) 
        {
            this.plantProperties = plant;
            foreach (PlantEnum kind in Enum.GetValues(typeof(PlantEnum)))
            {
                var field = typeof(PlantProperties).GetField(kind.ToString());
                if (field == null) continue;

                var value = field.GetValue(plant);
                if (value is float f)
                {
                    this[kind] = f;
                }
            }

        }

        public void ExposeData()
        {
            foreach (var kv in attributes)
            {
                var v=this[kv.Key];
                Scribe_Values.Look(ref v, kv.Key.ToString(), 1f);
                this[kv.Key] = v;
            }
        }

        public void ApplyTo(PlantProperties plant)
        {
            foreach (PlantEnum kind in Enum.GetValues(typeof(PlantEnum)))
            {
                var field = typeof(PlantProperties).GetField(kind.ToString());
                if (field == null) continue;

                field.SetValue(plant,this[kind]);
            }
        }

        public void Apply()
        {
            foreach (PlantEnum kind in Enum.GetValues(typeof(PlantEnum)))
            {
                var field = typeof(PlantProperties).GetField(kind.ToString());
                if (field == null) continue;

                field.SetValue(plantProperties,this[kind]);
            }
        }

        public void MultiplierAll(float multiplier)
        {
            foreach (var kv in attributes)
            {
                this[kv.Key] *= multiplier;
            }
        }

        public void Multiplier(PlantEnum plantEnum, float multiplier)
        {
                this[plantEnum] *= multiplier;            
        }

        public static MyPlant operator /(MyPlant plant, float multiplier)
        {
            var tmp = new MyPlant(plant);
            foreach (var kv in plant.attributes)
            {
                tmp[kv.Key] = kv.Value / multiplier;
            }
            return tmp;
        }

        public static MyPlant operator *(MyPlant plant, float multiplier)
        {
            var tmp = new MyPlant(plant);
            foreach (var kv in plant.attributes)
            {
                tmp[kv.Key] = kv.Value * multiplier;
            }
            return tmp;
        }

        //public override string ToString()
        //{
        //    return $"growDays:{growDays}, harvestYield:{harvestYield}";
        //}

        //public void SetFrom(MyPlant other)
        //{
        //    this.growDays = other.growDays;
        //    this.harvestYield = other.harvestYield;
        //}

        //public MyPlant Copy()
        //{
        //    return new MyPlant(this);
        //}


    }
}
