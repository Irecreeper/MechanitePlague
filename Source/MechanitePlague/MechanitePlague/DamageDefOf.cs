using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MP_MechanitePlague
{
	[DefOf]
	public static class PlagueDamageDefOf
	{
		static PlagueDamageDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(DamageDefOf));
		}

		public static DamageDef MP_StabInfect;

		public static DamageDef MP_BlastInfectMortar;
	}

	[DefOf]
	public static class PlagueThingDefOf
	{ 
		static PlagueThingDefOf()
        {
			DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
        }

		public static ThingDef MP_MechaniteCanister;
	}
}
