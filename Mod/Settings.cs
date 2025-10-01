using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Verse;

namespace Lilly.PlantsPatch2
{
    public class Settings : ModSettings
    {
        public static bool onDebug = true;
        //public static bool onPatch = true;
        //public static int maxFishPopulation = 1000000;

        public static Dictionary<string, MyPlant> treeSetup = new Dictionary<string, MyPlant>();
        private List<TreeSetupEntry> treeSetupList = new List<TreeSetupEntry>();

        // LoadingVars
        // ResolvingCrossRefs
        // PostLoadInit
        public override void ExposeData()
        {
            MyLog.Message($"<color=#00FF00FF>{Scribe.mode}</color>");
            if (Scribe.mode != LoadSaveMode.LoadingVars && Scribe.mode != LoadSaveMode.Saving) return;

            base.ExposeData();

            Scribe_Values.Look(ref onDebug, "onDebug", false);
            //Scribe_Values.Look(ref onPatch, "onPatch", true);

            //Scribe_Values.Look<Dictionary<string, MyPlant>>(ref treeSetup, "treeSetup");
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                MyLog.Message($"treeSetup {treeSetup.Count}");
                treeSetupList= treeSetup.Select(kv => new TreeSetupEntry() { defName = kv.Key, plant = kv.Value }).ToList();
                MyLog.Message($"treeSetupList {treeSetupList.Count}");
                Patch.TreePatch();
            }
            Scribe_Collections.Look(ref treeSetupList, "treeSetup", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                MyLog.Message($"treeSetupList {treeSetupList.Count}");
                treeSetup = treeSetupList.ToDictionary(entry => entry.defName, entry => entry.plant);
                MyLog.Message($"treeSetup {treeSetup.Count}");
                Patch.TreePatch();
            }

        }

        public static void TreeApply(float? growMultiplier = null, float? yieldMultiplier = null)
        {
            foreach (var entry in treeSetup)
            {
                if (growMultiplier.HasValue)
                    entry.Value.growDays *= growMultiplier.Value;

                if (yieldMultiplier.HasValue)
                    entry.Value.harvestYield *= yieldMultiplier.Value;
            }
        }

        public static void TreeReset(float? growMultiplier = null, float? yieldMultiplier = null)
        {
            
            if (growMultiplier.HasValue)
                foreach (var entry in Patch.treeBackup)
                    treeSetup[entry.Key].growDays = entry.Value.growDays * growMultiplier.Value;

            if (yieldMultiplier.HasValue)
                foreach (var entry in Patch.treeBackup)
                    treeSetup[entry.Key].harvestYield = entry.Value.harvestYield * yieldMultiplier.Value;
            
        }


        /// <summary>
        /// 기본값에서 배율 적용
        /// </summary>
        /// <param name="m"></param>
        public static void TreeApply(MyPlant m=null)
        {
            if (m == null)
            {
                foreach (var kv in Patch.treeBackup)
                {
                    treeSetup[kv.Key] = Patch.treeBackup[kv.Key];
                }
            }
            else
                foreach (var kv in Patch.treeBackup)                
                {
                    treeSetup[kv.Key] = Patch.treeBackup[kv.Key]* m;
                }
        }

        /// <summary>
        /// UI 목록 표시용
        /// </summary>
        public static void TreeSetup()
        {
            MyLog.Message($"treeBackup {Patch.treeBackup.Count}");             
            foreach (var kv in Patch.treeBackup)
            {
                // def.plant.growDays 
                // def.plant.harvestYield
                if (!treeSetup.ContainsKey(kv.Key))
                    treeSetup.Add(kv.Key, new MyPlant(kv.Value));
                MyLog.Message($"TreeSetup {kv.Key} {kv.Value}", Settings.onDebug);                
            }
            MyLog.Message($"TreeSetup {treeSetup.Count}");             

        }
    }
}
