using System;
using UnityEngine;
using Verse;
using RimWorld;
using GasNetwork;
using VanillaPowerExpanded;

namespace Rimnosis.Gas
{
    /* Yet another object used to spawn puffs of gas. Mostly a copy of VE's IntermittentGasSprayer due to an overabundance of 
	 * private and non-virtual members. IntermittentGasSprayer is itself practically a copy of the vanilla class
	 * IntermittentSteamSprayer for exactly the same reason. */
    public class IntermittentGasSprayer_Accessible : IntermittentGasSprayer
    {
		public Thing parent;

        public ThingDef moteDef;

		public int ticksUntilSpray = 500;

		public int sprayTicksLeft;

		protected const int MinTicksBetweenSprays = 500;

		protected const int MaxTicksBetweenSprays = 2000;

		protected const int MinSprayDuration = 200;

		protected const int MaxSprayDuration = 500;

		public IntermittentGasSprayer_Accessible(Thing parent, ThingDef moteDef) : base(parent)
		{
			this.parent = parent;
            this.moteDef = moteDef;
		}

        // A lot of trouble could have been avoided if either "parent" class had made its tick method virtual.
        // It probably won't actually metter here, but it's the principle of the thing, damn it!
		public virtual void SteamSprayerTickOverride()
		{
			if (sprayTicksLeft > 0)
			{
				sprayTicksLeft--;
				if (moteDef != null && Rand.Chance(0.6f))
				{
					SpawnMote(parent.TrueCenter(), parent.Map);
				}
				if (sprayTicksLeft <= 0)
				{
					if (endSprayCallback != null)
					{
						endSprayCallback();
					}
					ticksUntilSpray = Rand.RangeInclusive(500, 2000);
				}
				return;
			}
			ticksUntilSpray--;
			if (ticksUntilSpray <= 0)
			{
				if (startSprayCallback != null)
				{
					startSprayCallback();
				}
				sprayTicksLeft = Rand.RangeInclusive(200, 500);
			}
		}

        protected virtual MoteThrown MakeMote()
        {
            const int maxRotSpeed = 240;    // Tweak one value instead of two!
            var mote = (MoteThrown) ThingMaker.MakeThing(moteDef);
            mote.Scale = 1.5f;
            mote.rotationRate = Rand.RangeInclusive(-maxRotSpeed, maxRotSpeed);
            return mote;
        }

        public virtual void SpawnMote(Vector3 loc, Map map)
        {
            const float maxHorDisplacement = 0.2f;  // ... or four!

            if (!loc.ToIntVec3().ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }

            var mote = MakeMote();
            mote.exactPosition = loc + new Vector3(Rand.Range(-maxHorDisplacement, maxHorDisplacement), 0, Rand.Range(-maxHorDisplacement, maxHorDisplacement));
            mote.SetVelocity(Rand.Range(-45, 45), Rand.Range(1.2f, 1.5f));
            GenSpawn.Spawn(mote, loc.ToIntVec3(), map);
        }
    }
}