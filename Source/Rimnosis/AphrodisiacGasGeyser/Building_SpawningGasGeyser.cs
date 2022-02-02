using System;
using Verse;
using Verse.Sound;
using RimWorld;
using HarmonyLib;
using VanillaPowerExpanded;
using GasNetwork;

namespace Rimnosis.Gas
{
    // Fun fact: If your're running this mod, you have at least three functionally identical systems for spraying some visual effect out of a geyser
    public class Building_SpawningGasGeyser : Building_GasGeyser
    {
        public DefModExtension_GasGeyserParams geyserParams;

        public IntermittentGasSprayer_Accessible usableSprayer;

        public Sustainer spraySustainer;

        public int spraySustainerStartTick = -999;

        protected bool spreading;   // I can't imagine someone else ever subclassing and deciding they need to change this, but it's the principle of the thing!

        // Set up the sprayer to spawn some gas after it finishes spraying
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            
            geyserParams = def.GetModExtension<DefModExtension_GasGeyserParams>();
            spreading = typeof(Gas_Spreading).IsAssignableFrom(geyserParams.gasDef.thingClass);

            // ha. hahaha.
            usableSprayer = new IntermittentGasSprayer_Accessible(this, geyserParams.moteDef);
            usableSprayer.startSprayCallback = this.StartSpray;
            usableSprayer.endSprayCallback = this.EndSpray;
            usableSprayer.endSprayCallback += this.SpawnGas;
        }

        public override void Tick()
        {
            if (harvester == null)
            {
                usableSprayer.SteamSprayerTickOverride();
            }
            if (spraySustainer != null && Find.TickManager.TicksGame > this.spraySustainerStartTick + 1000)
            {
                Log.Message("Geyser spray sustainer still playing after 1000 ticks. Force-ending.");
                spraySustainer.End();
                spraySustainer = null;
            }
        }

		public void StartSpray()
		{
			spraySustainer = SoundDefOf.GeyserSpray.TrySpawnSustainer(new TargetInfo(base.Position, base.Map));
			spraySustainerStartTick = Find.TickManager.TicksGame;
		}

		public void EndSpray()
		{
			spraySustainer?.End();
            spraySustainer = null;
		}

        public void SpawnGas()
        {
            // I don't like the way it spawns off centre, so let's try this instead
            var spawnRect = CellRect.FromLimits(this.OccupiedRect().CenterCell, this.OccupiedRect().CenterCell + IntVec3.SouthWest);
            if (spreading)
            {
                foreach (var cell in spawnRect.Cells)
                {
                    GenGas.AddGas(cell, Map, geyserParams.gasDef, geyserParams.amount / 4, spread: true);
                }
            }
            else
            {
                foreach (var cell in spawnRect.Cells)
                {
                    GenSpawn.Spawn(geyserParams.gasDef, cell, Map);
                }
            }
        }
    }
}