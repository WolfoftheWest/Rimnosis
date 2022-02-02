using System.Collections.Generic;
using Verse;
using GasNetwork;

namespace Rimnosis.Gas
{
    // DefExtension to make it possible to control what gas gets sprayed in which quantities with which visual effect.
    public class DefModExtension_GasGeyserParams : DefModExtension
    {
        public ThingDef gasDef;

        // TODO: Switch to fleck system
        public ThingDef moteDef;

        public float amount;

        public override IEnumerable<string> ConfigErrors()
        {
            if (gasDef == null)
            {
                yield return "thingDef cannot be null";
            }

            if (gasDef.gas == null)
            {
                yield return "thingDef must be a gas";
            }

            if (!typeof(Gas_Spreading).IsAssignableFrom(gasDef.thingClass))
            {
                if(amount != 0)
                {
                    yield return "amount was specified but thingDef is not an instance of or assignable to Gas_Spreading";
                }
            }
            else if (amount == 0)
            {
                yield return "thingDef is a spreading gas but no amount was given";
            }
        }
    }
}