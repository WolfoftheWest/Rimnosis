using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Rimnosis
{
        public class ThingDef_WNProjectile : ThingDef
        {
            public float AddHediffChance = 0.8f; //The default chance of adding a hediff.
            public HediffDef HediffToAdd = HediffDefOf.Anesthetic;
        }

}
