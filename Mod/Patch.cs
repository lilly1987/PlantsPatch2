using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Lilly.PlantsPatch2

{
    public enum Kind
    {

    }

    public static class Patch
    {
        public static HarmonyX harmony = null;
        public static string harmonyId = "Lilly.";
/*
        public static void OnPatch(bool repatch = false)
        {
            if (repatch)
            {
                Unpatch();
            }
            if (harmony != null || !Settings.onPatch) return;
            harmony = new HarmonyX(harmonyId);
            try
            {
                harmony.PatchAll();
                MyLog.Message($"Patch <color=#00FF00FF>Succ</color>");
            }
            catch (System.Exception e)
            {
                MyLog.Error($"Patch Fail");
                MyLog.Error(e.ToString());
                MyLog.Error($"Patch Fail");
            }
            
        }

        public static void Unpatch()
        {
            MyLog.Message($"UnPatch");
            if (harmony == null) return;
            harmony.UnpatchSelf();
            harmony = null;
        }
*/
        // PlayDataLoader.DoPlayLoad() 메서드가 def 로딩 후 호출되므로 효과 없음
        //[HarmonyPatch(typeof(PlayDataLoader), "DoPlayLoad")]
        //[HarmonyPostfix]

        public static Dictionary<string, MyPlant> treeBackup = new Dictionary<string, MyPlant>();
        public static Dictionary<string, string> names= new Dictionary<string, string>();

        /// <summary>
        /// 변경하면 안되는 백업본
        /// </summary>
        public static void TreeBackup()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.plant != null && def.plant.IsTree)
                {                    
                    // def.plant.growDays 
                    // def.plant.harvestYield
                    MyPlant value = new MyPlant(def.plant);
                    treeBackup.Add(def.defName, value);
                    MyLog.Message($"TreeBackup {def.defName} {value}", Settings.onDebug);
                    ThingDef tdef = DefDatabase<ThingDef>.GetNamed(def.defName);
                    names.Add(tdef.defName, tdef.label.CapitalizeFirst());                   
                    MyLog.Message($"TreeBackup {def.defName} {tdef.label.CapitalizeFirst()}", Settings.onDebug);
                }
            }
            MyLog.Message($"TreeBackup {treeBackup.Count}");

        }

        /// <summary>
        /// 실제 게임에 변경한 값 적용
        /// </summary>
        public static void TreePatch(MyPlant m=null )
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.plant != null && def.plant.IsTree)
                {
                    // def.plant.growDays 
                    // def.plant.harvestYield
                    MyLog.Message($"TreePatch 1 {def.defName}", Settings.onDebug);
                    // 설정값 얻기
                    if (Settings.treeSetup.TryGetValue(def.defName, out MyPlant myPlant))
                    {
                        myPlant.ApplyTo(def.plant);
                        MyLog.Message($"TreePatch 2 {def.defName} {myPlant}", Settings.onDebug);
                    }
                    //else
                    //{
                    //    MyLog.Message($"TreePatch {def.defName} not found");
                    //}
                }
            }
        }

    }
}
