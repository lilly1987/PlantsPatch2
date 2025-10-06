using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Verse;

namespace Lilly.PlantsPatch2
{
    public class Settings : ModSettings
    {
        public static bool onDebug = true;
        public static bool onPatch = true;
        //public static int maxFishPopulation = 1000000;

        public static Dictionary<string, MyPlant> plantBackup = new Dictionary<string, MyPlant>();
        public static Dictionary<string, MyPlant> plantSetup = new Dictionary<string, MyPlant>();
        public static Dictionary<string, string> names = new Dictionary<string, string>();
        private List<TreeSetupEntry> plantSetupList = new List<TreeSetupEntry>();


        /// <summary>
        /// 목록 
        /// </summary>
        public static void TreeSetup()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs)
            {
                //if (def.plant != null && def.plant.IsTree)
                if (def.plant != null )
                {
                    MyPlant value = new MyPlant(def);
                    plantBackup.Add(def.defName, value);                    
                    
                    ThingDef tdef = DefDatabase<ThingDef>.GetNamed(def.defName);
                    names.Add(tdef.defName, tdef.label.CapitalizeFirst());
                    
                    if (plantSetup.TryGetValue(def.defName, out var myPlant))
                    {                        
                        myPlant.From(def);
                    }
                    else
                    {
                        plantSetup.Add(def.defName, new MyPlant(value));
                    }

                    //MyLog.Message($"TreeBackup/{def.defName}/{tdef.label.CapitalizeFirst()}/{treeBackup[def.defName][PlantEnum.harvestYield]}/{treeSetup[def.defName][PlantEnum.harvestYield]}", Settings.onDebug);
                }
            }
            MyLog.Message($"treeBackup/{plantBackup.Count}");
            MyLog.Message($"TreeSetup/{plantSetup.Count}");
            foreach (var key in plantSetup.Keys.ToList())
            {
                if (!plantBackup.ContainsKey(key))
                {
                    plantSetup.Remove(key);
                }
            }
            MyLog.Message($"TreeSetup/{plantSetup.Count}");
        }

        /// <summary>
        /// 실제 게임에 변경한 값 적용
        /// </summary>
        public static void TreePatch()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.plant != null && def.plant.IsTree)
                {
                    //MyLog.Message($"DefDatabase/{def.label.CapitalizeFirst()}", Settings.onDebug);
                    // 설정값 얻기
                    if (Settings.plantSetup.TryGetValue(def.defName, out MyPlant myPlant))
                    {
                        //myPlant.ApplyTo(def.plant);
                        myPlant.Apply();
                        //MyLog.Message($"TreePatch/{def.label.CapitalizeFirst()}", Settings.onDebug);
                    }
                }
            }
        }

        // LoadingVars
        // ResolvingCrossRefs
        // PostLoadInit
        public override void ExposeData()
        {
            MyLog.Message($"<color=#00FF00FF>{Scribe.mode}</color>");
            if (Scribe.mode != LoadSaveMode.LoadingVars && Scribe.mode != LoadSaveMode.Saving) return;

            base.ExposeData();

            Scribe_Values.Look(ref onDebug, "onDebug", false);
            Scribe_Values.Look(ref onPatch, "onPatch", true);

            //Scribe_Values.Look<Dictionary<string, MyPlant>>(ref treeSetup, "treeSetup");
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                MyLog.Message($"treeSetup/{plantSetup.Count}");
                plantSetupList= plantSetup.Select(kv => new TreeSetupEntry() { defName = kv.Key, plant = kv.Value }).ToList();
                MyLog.Message($"treeSetupList/{plantSetupList.Count}");
                TreePatch();
            }
            Scribe_Collections.Look(ref plantSetupList, "treeSetupList", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                MyLog.Message($"treeSetupList/{plantSetupList.Count}");
                plantSetup = plantSetupList.ToDictionary(entry => entry.defName, entry => entry.plant);
                MyLog.Message($"treeSetup/{plantSetup.Count}");
                TreePatch();
            }
            Patch.OnPatch(true);
        }

        public static void TreeApply(PlantEnum plantEnum ,float? multiplier=null,float? adder = null)
        {
            if(multiplier!=null)
                foreach (KeyValuePair<string, MyPlant> entry in plantSetup)
                {
                    entry.Value.Multiplier(plantEnum, multiplier.Value);
                }
            if(adder != null)
                foreach (KeyValuePair<string, MyPlant> entry in plantSetup)
                {
                    entry.Value.Adder(plantEnum, adder.Value);
                }
        }

        public static void TreeReset(PlantEnum plantEnum)
        {
            foreach (KeyValuePair<string, MyPlant> entry in plantBackup)
            {
                if (plantSetup.TryGetValue(entry.Key,out var myPlant))
                {
                    myPlant[plantEnum] = entry.Value[plantEnum];            
                }
            }
        }

        public static void TreeReset()
        {
            foreach (KeyValuePair<string, MyPlant> entry in plantBackup)
            {
                if (plantSetup.TryGetValue(entry.Key,out var myPlant))
                {
                    foreach (PlantEnum plantEnum in Enum.GetValues(typeof(PlantEnum)))
                        myPlant[plantEnum] = entry.Value[plantEnum];            
                }
            }
        }

        /// <summary>
        /// 기본값에서 배율 적용
        /// </summary>
        /// <param name="m"></param>
        //public static void TreeApply(MyPlant m=null)
        //{
        //    if (m == null)
        //    {
        //        foreach (var kv in Patch.treeBackup)
        //        {
        //            treeSetup[kv.Key] = Patch.treeBackup[kv.Key];
        //        }
        //    }
        //    else
        //        foreach (var kv in Patch.treeBackup)                
        //        {
        //            treeSetup[kv.Key] = Patch.treeBackup[kv.Key]* m;
        //        }
        //}

    }
}
