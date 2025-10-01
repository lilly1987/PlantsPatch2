using Verse;

namespace Lilly.PlantsPatch2

{
    public class KindFloatPair : IExposable
    {
        public PlantEnum kind;
        public float value;

        public KindFloatPair() { }

        public KindFloatPair(PlantEnum kind, float value)
        {
            this.kind = kind;
            this.value = value;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref kind, "kind");
            Scribe_Values.Look(ref value, "value");
        }
    }
}
