using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;

namespace KRPC.MechJeb.Util {
	// LaunchTiming (MuMech.LaunchTiming) was removed in MechJeb 2.15. Its
	// only entry point, `TimeToPhaseAngle`, is now a *private static*
	// method on MechJebModuleAscentMenu. We reflect into it via
	// AscentMenuBinding (set up at type-load time in AscentBindings.cs);
	// no MechJebType const here means InitTypes won't try to match this
	// helper against a now-nonexistent MuMech type.

	internal static class LaunchTiming {
		public static double TimeToPhaseAngle(double launchPhaseAngle) {
			return (double)AscentMenuBinding.timeToPhaseAngle.Invoke(
				null,
				new object[] {
					launchPhaseAngle,
					FlightGlobals.ActiveVessel.mainBody,
					MechJeb.vesselState.Longitude,
					MechJeb.TargetController.TargetOrbit.InternalOrbit
				});
		}
	}

	internal static class MathFunctions {
		internal const string MechJebType = "MechJebLib.Functions.Astro";

		// Fields and methods
		private static MethodInfo timeToPlane;

		internal static void InitType(Type type) {
			timeToPlane = type.GetCheckedMethod("TimeToPlane");
		}

		public static Tuple<double, double> MinimumTimeToPlane(double lan, double inclination) {
			double normal = TimeToPlane(lan, inclination);
			double inverted = TimeToPlane(lan, -inclination);
			return normal < inverted ? Tuple.Create(normal, inclination) : Tuple.Create(inverted, -inclination);
		}

		public static double TimeToPlane(double lan, double inclination) {
			return (double)timeToPlane.Invoke(null, new object[] { FlightGlobals.ActiveVessel.mainBody.rotationPeriod, MechJeb.vesselState.Latitude, MechJeb.vesselState.CelestialLongitude, lan, inclination });
		}
	}
}
