using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteNoiseGrenade
{
        public class ThingDef_WNProjectile : ThingDef
        {
            public float AddHediffChance = 0.8f; //The default chance of adding a hediff.
            public HediffDef HediffToAdd = HediffDefOf.Anesthetic;
        }

}
