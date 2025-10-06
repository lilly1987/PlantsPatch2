using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static RimWorld.ColonistBar;
using static Verse.Widgets;

namespace Lilly.PlantsPatch2
{

    public class ModUI : Mod
    {
        public static ModUI self;
        public static Settings settings;

        public ModUI(ModContentPack content) : base(content)
        {
            self = this;
            MyLog.Message($"ST");

            //Patch.OnPatch();//이미 defs가 로드된 후라 너무 늦음

            //Patch.TreeBackup();//아직 로드 안됨
            //Settings.TreeSetup();//아직 로드 안됨

            settings = GetSettings<Settings>();// StaticConstructorOnStartup 보다 먼저 실행됨         
            ModUI.plantSetup = Settings.plantSetup;

            MyLog.Message($"ED");
        }

        Vector2 scrollPosition;
        string tmp;
        float viewHeight = 3000f;


        // 버튼 정의: 텍스트와 multiplier (null은 기본값 적용)
        /*        (string label, float multiplier)[] buttons = new[]
                {
                    ("reset", 1f),
                    ("x2", 2f),
                    ("x1.25", 1.25f),
                    ("x0.75", 0.75f),
                    ("x0.25", 0.25f),

                    ("reset", 1f),
                    ("x2", 2f),
                    ("x1.25", 1.25f),
                    ("x0.75", 0.75f),
                    ("x0.25", 0.25f),
                };*/
        (float multiplier,string v1,string v2)[] buttons2 = new []
        {
            (2f,"x2","/2"),
            (3f,"x3","/3"),
            (5f,"x5","/5"),
            (7f,"x7","/7"),
            //(11f,"x2","/2")
        };
        (float multiplier,string v1,string v2)[] buttons3 = new []
        {
            (1f ,"+1","-1"),
            (5f ,"+5","-5"),
            (10f,"+10","-10"),
            (50f,"+50","-50"),
            //(11f,"x2","/2")
        };

        string[] buttons = Enum.GetNames(typeof(PlantEnum));

        static PlantEnum plantEnum = PlantEnum.harvestYield;

        private static IEnumerable<Widgets.DropdownMenuElement<PlantEnum>> GeneratePageMenu(PlantEnum p)
        {
            return from PlantEnum page in Enum.GetValues(typeof(PlantEnum))
                   where page != p
                   select new Widgets.DropdownMenuElement<PlantEnum>
                   {
                       option = new FloatMenuOption(
                           page.ToString().Translate(), 
                           () =>
                           {
                               plantEnum = page;
                               //scrollPosition = Vector2.zero;
                               //MyLog.Message($"{plantSetup.Count}");
                           }
                       )
                       {
                           tooltip =  null
                       },
                       payload = page,
                   };
        }

        private static IEnumerable<Widgets.DropdownMenuElement<PlantFilterType>> GeneratePageMenu2(PlantFilterType p)
        {
            return from PlantFilterType page in Enum.GetValues(typeof(PlantFilterType))
                   where page != p
                   select new Widgets.DropdownMenuElement<PlantFilterType>
                   {
                       option = new FloatMenuOption(
                           $"{page.ToString().Translate()} / {GetFilteredPlants(page).Count}", 
                           () =>
                           {
                               selectedFilter = page;
                               //scrollPosition = Vector2.zero;
                               //MyLog.Message($"{selectedFilter.ToString()}");
                               plantSetup=GetFilteredPlants(selectedFilter);
                               //MyLog.Message($"{plantSetup.Count}");
                           }
                       )
                       {
                           tooltip =  null
                       },
                       payload = page,
                   };
        }

        public static PlantFilterType selectedFilter = PlantFilterType.All;

        public static Dictionary<string, MyPlant> plantSetup=new Dictionary<string, MyPlant>();

        // 매 프레임마다 호출됨
        public override void DoSettingsWindowContents(Rect inRect)
        {
            //MyLog.Message($"ST {Settings.treeSetup.Count}");
            base.DoSettingsWindowContents(inRect);
            float h = 0f;
            var rect = new Rect(0, 0, inRect.width - 16, viewHeight);

            Widgets.BeginScrollView(inRect, ref scrollPosition, rect);

            Listing_Standard listing = new Listing_Standard();

            listing.Begin(rect);

            listing.GapLine();
            h += 12;
            // ---------

            listing.CheckboxLabeled($"Debug", ref Settings.onDebug);
            listing.CheckboxLabeled($"PlantRestingAndSchedulePatch".Translate(), ref Settings.onPatch, "PlantRestingAndSchedulePatchDec".Translate());
            h += 60;

            listing.GapLine();
            h += 12;

            Rect rowRect = listing.GetRect(30f);
            float colWidth = rowRect.width /2;
            h += 30;

            Widgets.Dropdown(
                new Rect(rowRect.x, rowRect.y, colWidth, rowRect.height),
                selectedFilter,
                b=> b,
                GeneratePageMenu2,
                $"{selectedFilter.ToString().Translate()} / {plantSetup.Count}"                
            );

            Widgets.Dropdown(
                new Rect(rowRect.x+ colWidth, rowRect.y, colWidth, rowRect.height),
                plantEnum,
                b=> b,
                GeneratePageMenu,
                plantEnum.ToString().Translate()
            );
            h += 60;
            listing.GapLine();
            h += 12;
            // === 일괄 적용 ===
            rowRect = listing.GetRect(30f);
            h += 30;
            float colWidth2 = rowRect.width / 2;
            float colWidthT = rowRect.width / (buttons2.Length*2 + 2);

            Widgets.Label(new Rect(rowRect.x, rowRect.y, colWidthT, rowRect.height), "일괄 적용".Translate()); // 1칸

            if (Widgets.ButtonText(new Rect(rowRect.x + colWidthT, rowRect.y, colWidthT, rowRect.height), "reset"))
            {
                Settings.TreeReset(plantEnum, plantSetup);
            }

            for (int i = buttons2.Length - 1; i >= 0; i--)
            {
                int reverseIndex = buttons2.Length - 1 - i;
                if (Widgets.ButtonText(new Rect(rowRect.x + colWidthT * (reverseIndex + 2), rowRect.y, colWidthT, rowRect.height), buttons2[i].v1))
                {
                    Settings.TreeApply(plantEnum, plantSetup, buttons2[i].multiplier);
                }
            }

            for (int i = 0; i < buttons2.Length; i++)
            {
                if (Widgets.ButtonText(new Rect(rowRect.x + colWidthT *(buttons2.Length+i + 2), rowRect.y, colWidthT, rowRect.height), buttons2[i].v2))
                {
                    Settings.TreeApply(plantEnum, plantSetup, 1f /buttons2[i].multiplier);
                }
            }

            rowRect = listing.GetRect(30f);
            h += 30;
            if (Widgets.ButtonText(new Rect(rowRect.x + colWidthT, rowRect.y, colWidthT, rowRect.height), "set 0"))
            {
                Settings.TreeApply(plantEnum, plantSetup, 0f);
            }

            for (int i = buttons3.Length - 1; i >= 0; i--)
            {
                int reverseIndex = buttons3.Length - 1 - i;
                if (Widgets.ButtonText(new Rect(rowRect.x + colWidthT * (reverseIndex + 2), rowRect.y, colWidthT, rowRect.height), buttons3[i].v1))
                {
                    Settings.TreeApply(plantEnum, plantSetup, null, buttons3[i].multiplier);
                }
            }

            for (int i = 0; i < buttons3.Length; i++)
            {
                if (Widgets.ButtonText(new Rect(rowRect.x + colWidthT * (buttons3.Length + i + 2), rowRect.y, colWidthT, rowRect.height), buttons3[i].v2))
                {
                    Settings.TreeApply(plantEnum, plantSetup, null, - buttons3[i].multiplier);
                }
            }

            // === 일괄 적용 ===

            listing.GapLine();
            h += 12;
            // ---------

            foreach (KeyValuePair<string, MyPlant> item in plantSetup)
            {
                //MyLog.Message($"{item.Key}");
                TextFieldNumeric(listing, item, colWidth2, colWidthT);
            }
            h += plantSetup.Count*30f;
            // ---------
            listing.GapLine(24f);

            if (Widgets.ButtonText(listing.GetRect(30f), "reset All"))
            {
                Settings.TreeResetAll();
            }
            h += 30;
            //viewHeight = listing.CurHeight;
            viewHeight = h;
            //MyLog.Message($"{listing.CurHeight} / {h}");

            Widgets.EndScrollView();

            listing.End();
        }


        public override string SettingsCategory()
        {
            return "Plants Patch".Translate();
        }

        public void TextFieldNumeric(Listing_Standard listing, KeyValuePair<string, MyPlant> num, float colWidth, float colWidthR)
        {

            //Widgets.Label(new Rect(rowRect.x, rowRect.y, colWidth, rowRect.height), "이름".Translate());
            //Widgets.Label(new Rect(rowRect.x + colWidth, rowRect.y, colWidth, rowRect.height), "성장일".Translate());

            // 한 줄 높이의 Rect 확보
            Rect rowRect = listing.GetRect(30f);

            // 이름 부분
            if (Settings.names.TryGetValue(num.Key, out var value))
            {
                Widgets.Label(new Rect(rowRect.x, rowRect.y, colWidth - colWidthR * 2, rowRect.height), $"{value}");
            }
            else
            {
                Widgets.Label(new Rect(rowRect.x, rowRect.y, colWidth- colWidthR*2, rowRect.height), $"{num.Key.Translate()}");
            }

            //
            Widgets.Label(new Rect(rowRect.x + colWidth- colWidthR, rowRect.y, colWidthR, rowRect.height), $"{Settings.plantBackup[num.Key][plantEnum]}");

            // 리셋 부분
            if (Widgets.ButtonText(new Rect(rowRect.x + colWidth, rowRect.y, colWidthR, rowRect.height), "reset"))
            {
                plantSetup[num.Key][plantEnum] = Settings.plantBackup[num.Key][plantEnum];
            }
            
            // 입력 부분
            float n=num.Value[plantEnum];
            string growStr = n.ToString();
            Widgets.TextFieldNumeric(
                new Rect(rowRect.x + colWidth + colWidthR, rowRect.y, colWidth - colWidthR, rowRect.height),
                ref n,
                ref growStr,
                0.01f
            );
            num.Value[plantEnum] = n;
        }

        public void TextFieldNumeric<T>(Listing_Standard listing, ref T num, string label = "", string tipSignal = "") where T : struct
        {
            listing.Label(label.Translate(), tipSignal: tipSignal.Translate());
            tmp = num.ToString();
            listing.TextFieldNumeric<T>(ref num, ref tmp);
        }

        public static Dictionary<string, MyPlant> GetFilteredPlants(PlantFilterType filter)
        {
            switch (filter)
            {
                case PlantFilterType.TreeOnly:
                    return Settings.fillterTrees;
                case PlantFilterType.GroundOnly:
                    return Settings.fillterGrounds;
                case PlantFilterType.TreeAndGround:
                    return Settings.fillterAll;
                case PlantFilterType.Neither:
                    return Settings.fillterNoAll;
                case PlantFilterType.All:
                default:
                    return Settings.plantSetup;
            }
        }
    }
}
