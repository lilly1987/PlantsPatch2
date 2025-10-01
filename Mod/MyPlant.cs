using RimWorld;
using System;
using Verse;

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


    public class MyPlant : IExposable

    {
        public enum Kind
        {
            growDays,
            harvestYield
        }

        public float growDays=1f;// 나무 성장 시간
        public float harvestYield=1f;// 수확량

        public MyPlant() { }    // 매개변수 생성자

        public MyPlant(float growDays = 1f, float harvestYield = 1f)
        {
            this.growDays = growDays;
            this.harvestYield = harvestYield;
        }

        public MyPlant(MyPlant plant)  
        { 
        
        }

        // PlantProperties 기반 생성자
        public MyPlant(PlantProperties plant) 
        { 
        
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref growDays, "growDays", 1f);
            Scribe_Values.Look(ref harvestYield, "harvestYield", 25f);
        }

        public void ApplyTo(PlantProperties plant)
        {
            plant.growDays = growDays;
            plant.harvestYield = harvestYield;
        }

        public override string ToString()
        {
            return $"growDays:{growDays}, harvestYield:{harvestYield}";
        }

        //public void SetFrom(MyPlant other)
        //{
        //    this.growDays = other.growDays;
        //    this.harvestYield = other.harvestYield;
        //}

        //public MyPlant Copy()
        //{
        //    return new MyPlant(this);
        //}

        public static MyPlant operator *(MyPlant plant, MyPlant multiplier)
        {
            var tmp=new MyPlant(plant);
            foreach (var nm in Enum.GetNames(typeof(MyPlant)))
            {
                //
            }
            return tmp;
        }
    }
}
