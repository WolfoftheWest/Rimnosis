using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WhiteNoiseGrenade
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
