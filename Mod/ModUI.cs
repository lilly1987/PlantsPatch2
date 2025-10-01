using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static RimWorld.ColonistBar;

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

            MyLog.Message($"ED");
        }

        Vector2 scrollPosition;
        string tmp;

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


        //float colWidtCnt = 5f;
        //int cnt = 5;

        // 매 프레임마다 호출됨
        public override void DoSettingsWindowContents(Rect inRect)
        {
            //MyLog.Message($"ST {Settings.treeSetup.Count}");
            base.DoSettingsWindowContents(inRect);

            var rect = new Rect(0, 0, inRect.width - 16, 1000);

            Widgets.BeginScrollView(inRect, ref scrollPosition, rect);

            Listing_Standard listing = new Listing_Standard();

            listing.Begin(rect);

            listing.GapLine();

            // ---------

            listing.CheckboxLabeled($"Debug", ref Settings.onDebug);
            //listing.CheckboxLabeled($"on Patch", ref Settings.onPatch);

            listing.GapLine();

            // whdfb rngus vlfdy

            listing.GapLine();

            Rect rowRect = listing.GetRect(30f);
            float colWidth = rowRect.width / 3f;
            float colWidth2 = colWidth / buttons2.Length;

            Widgets.Label(new Rect(rowRect.x, rowRect.y, colWidth, rowRect.height), "일괄 적용".Translate());

            int i = 0;
            float x = rowRect.x + colWidth + colWidth2 * i;
            Rect buttonRect = new Rect(x, rowRect.y, colWidth2, rowRect.height);

            if (Widgets.ButtonText(buttonRect, "reset"))
            {
                Settings.TreeReset(1f);
                //if (buttons[i].multiplier.HasValue)
                //Settings.TreeApply(new MyPlant(buttons[i].multiplier));
                //else
                //Settings.TreeApply();
            }

            // 버튼 출력
            for (i = 0; i < buttons2.Length; i++)
            {
                x = rowRect.x + colWidth + colWidth2 * i;
                buttonRect = new Rect(x, rowRect.y, colWidth2, rowRect.height);

                if (Widgets.ButtonText(buttonRect, buttons2[i].v1))
                {
                    Settings.TreeApply(buttons2[i].multiplier);
                    //if (buttons[i].multiplier.HasValue)
                    //Settings.TreeApply(new MyPlant(buttons[i].multiplier));
                    //else
                    //Settings.TreeApply();
                }
            }

            i = 5;
            x = rowRect.x + colWidth + colWidth2 * i;
            buttonRect = new Rect(x, rowRect.y, colWidth2, rowRect.height);

            if (Widgets.ButtonText(buttonRect, "reset"))
            {
                Settings.TreeReset(yieldMultiplier: 1f);
                //if (buttons[i].multiplier.HasValue)
                //Settings.TreeApply(new MyPlant(buttons[i].multiplier));
                //else
                //Settings.TreeApply();
            }

            for (i = buttons2.Length; i < buttons2.Length * 2; i++)
            {
                x = rowRect.x + colWidth + colWidth2 * i;
                buttonRect = new Rect(x, rowRect.y, colWidth2, rowRect.height);

                if (Widgets.ButtonText(buttonRect, buttons2[i].v2))
                {
                    Settings.TreeApply(yieldMultiplier: 1f/buttons2[i].multiplier);
                    //if (buttons[i].multiplier.HasValue)
                    //Settings.TreeApply(new MyPlant(harvestYield: buttons[i].multiplier));
                    //else
                    //Settings.TreeApply();
                }
            }

            listing.GapLine();

            // 한 줄 높이의 Rect 확보
            rowRect = listing.GetRect(30f);

            // 열 너비 계산 (3등분)

            Widgets.Label(new Rect(rowRect.x, rowRect.y, colWidth, rowRect.height), "이름".Translate());
            Widgets.Label(new Rect(rowRect.x + colWidth, rowRect.y, colWidth, rowRect.height), "성장일".Translate());
            Widgets.Label(new Rect(rowRect.x + colWidth * 2, rowRect.y, colWidth, rowRect.height), "수급량".Translate());

            // ---------

            foreach (KeyValuePair<string, MyPlant> item in Settings.treeSetup)
            {
                TextFieldNumeric(listing, item, colWidth, colWidth2);
            }

            // ---------

            listing.GapLine();

            listing.End();

            Widgets.EndScrollView();

            //MyLog.Message($"ED");
        }

        public override string SettingsCategory()
        {
            return "Plants Patch".Translate();
        }

        public void TextFieldNumeric(Listing_Standard listing, KeyValuePair<string, MyPlant> num, float colWidth, float colWidth2)
        {
            // 한 줄 높이의 Rect 확보
            Rect rowRect = listing.GetRect(30f);

            if (Patch.names.TryGetValue(num.Key, out var value))
            {
                Widgets.Label(new Rect(rowRect.x, rowRect.y, colWidth, rowRect.height), $"{value} , {Settings.treeSetup[num.Key].harvestYield / Settings.treeSetup[num.Key].growDays}");
            }
            else
            {
                Widgets.Label(new Rect(rowRect.x, rowRect.y, colWidth, rowRect.height), $"{num.Key.Translate()} , {Settings.treeSetup[num.Key].harvestYield / Settings.treeSetup[num.Key].growDays}");
            }


            if (Widgets.ButtonText(new Rect(rowRect.x + colWidth, rowRect.y, colWidth2, rowRect.height), "reset"))
            {
                Settings.treeSetup[num.Key].growDays = Patch.treeBackup[num.Key].growDays;
            }
            // 두 번째 열: growDays 입력 필드
            string growStr = num.Value.growDays.ToString();
            Widgets.TextFieldNumeric(
                new Rect(rowRect.x + colWidth + colWidth2, rowRect.y, colWidth - colWidth2, rowRect.height),
                ref num.Value.growDays,
                ref growStr,
                0.01f
            );
            if (Widgets.ButtonText(new Rect(rowRect.x + colWidth * 2, rowRect.y, colWidth2, rowRect.height), "reset"))
            {
                Settings.treeSetup[num.Key].harvestYield = Patch.treeBackup[num.Key].harvestYield ;                
            }
            // 세 번째 열: harvestYield 입력 필드
            string yieldStr = num.Value.harvestYield.ToString();
            Widgets.TextFieldNumeric(
                new Rect(rowRect.x + colWidth * 2 + colWidth2, rowRect.y, colWidth - colWidth2, rowRect.height),
                ref num.Value.harvestYield,
                ref yieldStr,
                0f
            );

            //Widgets.Label(listing.GetRect(30f), num.Key.Translate());
            //Widgets.TextFieldNumericLabeled(listing.GetRect(30f),"성장일", ref num.Value.growDays, ref tmp);
            //Widgets.TextFieldNumericLabeled(listing.GetRect(30f),"수확량", ref num.Value.harvestYield, ref tmp);

            /*            listing.Label(num.Key.Translate());
                        tmp = num.Value.growDays.ToString();
                        //listing.TextFieldNumeric(ref num.Value.growDays, ref tmp);
                        listing.TextFieldNumericLabeled("성장일", ref num.Value.growDays, ref tmp);
                        tmp = num.Value.harvestYield.ToString();
                        //listing.TextFieldNumeric(ref num.Value.harvestYield, ref tmp);
                        listing.TextFieldNumericLabeled("수급량", ref num.Value.harvestYield, ref tmp);*/
        }

        public void TextFieldNumeric<T>(Listing_Standard listing, ref T num, string label = "", string tipSignal = "") where T : struct
        {
            listing.Label(label.Translate(), tipSignal: tipSignal.Translate());
            tmp = num.ToString();
            listing.TextFieldNumeric<T>(ref num, ref tmp);
        }
    }
}
