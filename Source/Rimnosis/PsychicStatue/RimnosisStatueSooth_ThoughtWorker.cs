using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PsychicStatue
{
    public class ThoughtWorker_RimnosisStatueSoothe : ThoughtWorker
    {
        // Token: 0x06003F32 RID: 16178 RVA: 0x0015BD04 File Offset: 0x00159F04
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.Spawned)
            {
                return false;
            }
            List<ThingDef> listDefs = RetrieveStatueDefs();
            List<Thing> statuesOnMap = new List<Thing>();
            foreach(ThingDef statueDef in listDefs)
            {
                statuesOnMap.AddRange(p.Map.listerThings.ThingsOfDef(statueDef));
            }
            for (int i = 0; i < statuesOnMap.Count; i++)
            {
                if (p.Position.InHorDistOf(statuesOnMap[i].Position, Radius))
                {
                    return true;
                }
            }
            return false;
        }

        List<ThingDef> RetrieveStatueDefs()
        {
            return DefDatabase<SoothingStatuesDef>.GetNamed("default").Statues;
        }

        // Token: 0x04002179 RID: 8569
        private const float Radius = 5f;
    }
}