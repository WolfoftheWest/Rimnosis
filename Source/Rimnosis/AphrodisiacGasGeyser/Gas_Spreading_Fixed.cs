using System;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using GasNetwork;

namespace Rimnosis.Gas
{
    public class Gas_Spreading_Fixed : Gas_Spreading
    {

        public override void Tick()
        {
            if (this.IsHashIntervalTick(GenTicks.TicksPerRealSecond))
            {
                DissipateAndSpread();
                if (Density <= 0)
                {
                    Destroy();
                    return;
                }

                UpdateGraphic();

                if (Flammable || Toxic)
                {
                    if (this.IsHashIntervalTick(GenTicks.TicksPerRealSecond * 4))
                    {
                        // NOTE: make sure this inner interval is a multiple of the outer interval.
                        UpdateDanger(Map);
                    }

                    var things = Position.GetThingList(Map);
                    if (Flammable
                     && (Position.GetRoom(Map).Temperature > 100
                      || things.Any(t => t.def == ThingDefOf.Fire || t.IsBurning())))
                    {
                        DoExplode();
                    }

                    if (Toxic)
                    {
                        try
                        {
                            foreach (var pawn in things.OfType<Pawn>().Where(p => p.RaceProps.IsFlesh))
                            {
                                HealthUtility.AdjustSeverity(pawn, GasProps.exposureHediff,
                                                             (float)GenTicks.TicksPerRealSecond /
                                                             GenDate.TicksPerHour *
                                                             // wind effects can cause a density > 1, we don't want pawn 
                                                             // instagibs or weird buggy healing, so clamp to 0-1.
                                                             Mathf.Clamp01(Density) * GasProps.severityPerHourExposed);
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            // death causes foreach collection to be modified.
                            // catching and ignoring the error may cause gassing of very large groups to take slightly longer,
                            // as each death will stop iteration of any other victims in this cell. This seems like a minor
                            // inconvenience.
                        }
                    }
                }
            }

            graphicRotation += graphicRotationSpeed;
        }

        private void DissipateAndSpread()
        {
            var roofed = Position.Roofed(Map);

            // dissipate
            Density -= (float)GasProps.staticDissipation / GasProps.maxDensity;
            if (Density <= 0)
            {
                return;
            }

            // spread
            var room = Position.GetRoom(Map);
            if (roofed)
            {
                foreach (var neighbour in GenAdjFast.AdjacentCellsCardinal(Position))
                {
                    if (!neighbour.Impassable(Map) && neighbour.InBounds(Map) && neighbour.GetRoom(Map) == room)
                    {
                        var amountDissipated = GenGas.AddGas(neighbour, Map, def, GasProps.staticDissipation, false);
                        Density -= amountDissipated / GasProps.maxDensity;
                    }
                }
            }
            else
            {
                // not roofed, so lets assume wind influence.
                var wind = Map.windVector() * Density;

                // TODO: wind dissipation can probably use some balancing passes
                // have a look at fluid dynamics for a simplified but proper algorithm?
                var windNorth = Mathf.Clamp((Vector2.up * wind).y, .1f, float.MaxValue) * GasProps.windDissipation;
                var windEast = Mathf.Clamp((Vector2.right * wind).x, .1f, float.MaxValue) * GasProps.windDissipation;
                var windSouth = Mathf.Clamp((Vector2.down * wind).y, .1f, float.MaxValue) * GasProps.windDissipation;
                var windWest = Mathf.Clamp((Vector2.left * wind).x, .1f, float.MaxValue) * GasProps.windDissipation;

                // normalize dissipation so that no gas is created out of nothing
                var sum = windNorth + windEast + windSouth + windWest;
                if (sum > Density * GasProps.maxDensity)
                {
                    var factor = Density * GasProps.maxDensity / sum;
                    windNorth *= factor;
                    windEast *= factor;
                    windSouth *= factor;
                    windWest *= factor;
                }

                // north
                var neighbour = Position + IntVec3.North;
                if (!neighbour.Impassable(Map) && neighbour.InBounds(Map) && neighbour.GetRoom(Map) == room)
                {
                    var amountDissipated = GenGas.AddGas(
                        neighbour, Map, def, windNorth, false, true);
                    Density -= amountDissipated / GasProps.maxDensity;
                }

                // east
                neighbour = Position + IntVec3.East;
                if (!neighbour.Impassable(Map) && neighbour.InBounds(Map) && neighbour.GetRoom(Map) == room)
                {
                    var amountDissipated = GenGas.AddGas(
                        neighbour, Map, def, windEast, false, true);
                    Density -= amountDissipated / GasProps.maxDensity;
                }

                // south
                neighbour = Position + IntVec3.South;
                if (!neighbour.Impassable(Map) && neighbour.InBounds(Map) && neighbour.GetRoom(Map) == room)
                {
                    var amountDissipated = GenGas.AddGas(
                        neighbour, Map, def, windSouth, false, true);
                    Density -= amountDissipated / GasProps.maxDensity;
                }

                // west
                neighbour = Position + IntVec3.West;
                if (!neighbour.Impassable(Map) && neighbour.InBounds(Map) && neighbour.GetRoom(Map) == room)
                {
                    var amountDissipated = GenGas.AddGas(
                        neighbour, Map, def, windWest, false, true);
                    Density -= amountDissipated / GasProps.maxDensity;
                }
            }
        }
    }
}