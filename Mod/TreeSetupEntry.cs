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
}
