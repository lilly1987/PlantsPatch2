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




    }
}
