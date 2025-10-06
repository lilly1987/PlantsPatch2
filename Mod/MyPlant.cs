using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Verse;
using static Lilly.PlantsPatch2.MyPlant;

namespace Lilly.PlantsPatch2
{
    public class MyPlant : IExposable
    {
        private List<KindFloatPair> attributeList = new List<KindFloatPair>();

        public Dictionary<PlantEnum, float> attributes = new Dictionary<PlantEnum, float>();
        public PlantProperties plantProperties =null;
        public ThingDef def = null;

        public bool isGround = false;
        public bool isTree = false;

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
            this.def = plant.def;
            foreach (KeyValuePair<PlantEnum, float> kv in plant.attributes)
            {    
                this[kv.Key] = kv.Value;
            }
        }

        // PlantProperties 기반 생성자
        public MyPlant(ThingDef def) 
        {
            this.plantProperties = def.plant;
            this.def = def;
            isTree = def.plant.IsTree;
            isGround = def.plant.sowTags.Contains("Ground");
            foreach (PlantEnum kind in Enum.GetValues(typeof(PlantEnum)))
            {
                var field = typeof(PlantProperties).GetField(kind.ToString());
                if (field == null)
                {
                    MyLog.Warning($"{def.label.CapitalizeFirst()} no {kind}");
                    continue;
                }

                var value = field.GetValue(def.plant);
                if (value is float f)
                {
                    this[kind] = f;                    
                }
                else
                {
                    MyLog.Warning($"{def.label.CapitalizeFirst()}/{kind} no float : {value}");
                }
            }

        }

        public void From(ThingDef def)
        {
            this.plantProperties = def.plant;
            this.def = def;
            isTree = def.plant.IsTree;
            isGround = def.plant.sowTags.Contains("Ground");
            foreach (PlantEnum kind in Enum.GetValues(typeof(PlantEnum)))
            {
                if (attributes.TryGetValue(kind,out var p))
                {
                    continue;
                }
                var field = typeof(PlantProperties).GetField(kind.ToString());
                if (field == null)
                {
                    MyLog.Warning($"{def.label.CapitalizeFirst()} no {kind}");
                    continue;
                }

                var value = field.GetValue(def.plant);
                if (value is float f)
                {
                    this[kind] = f;
                }
                else
                {
                    MyLog.Warning($"{def.label.CapitalizeFirst()}/{kind} no float : {value}");
                }
            }
        }

        public void ExposeData()
        {
/*
            foreach (var kv in attributes.ToList())
            {
                var v = kv.Value;
                Scribe_Values.Look(ref v, kv.Key.ToString());
                //MyLog.Message($"{kv.Key.ToString()}/{kv.Value}");
                this[kv.Key] = v;
            }
*/
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                attributeList.Clear();
                foreach (var kv in attributes)
                    attributeList.Add(new KindFloatPair(kv.Key, kv.Value));
            }

            Scribe_Collections.Look(ref attributeList, "attributeList", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                attributes.Clear();
                foreach (var pair in attributeList)
                    attributes[pair.kind] = pair.value;
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

        public void AdderAll(float adder)
        {
            foreach (var kv in attributes)
            {
                this[kv.Key] += adder;
            }
        }

        public void Adder(PlantEnum plantEnum, float adder)
        {
                this[plantEnum] += adder;            
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

        public static MyPlant operator +(MyPlant plant, float adder)
        {
            var tmp = new MyPlant(plant);
            foreach (var kv in plant.attributes)
            {
                tmp[kv.Key] = kv.Value + adder;
            }
            return tmp;
        }

        public static MyPlant operator -(MyPlant plant, float adder)
        {
            var tmp = new MyPlant(plant);
            foreach (var kv in plant.attributes)
            {
                tmp[kv.Key] = kv.Value - adder;
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
