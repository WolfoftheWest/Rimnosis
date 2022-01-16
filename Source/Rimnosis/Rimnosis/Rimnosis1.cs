using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Rimnosis
{
    public class Proj_GrenadeWN : Projectile_Explosive
    {
        #region Properties
        //
        public ThingDef_WNProjectile Def
        {
            get
            {
                return this.def as ThingDef_WNProjectile;
            }
        }
        #endregion Properties
    }
}
