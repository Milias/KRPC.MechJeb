// GuidanceController bridge — read-only observability for
// MechJebModuleGuidanceController, the closed-loop guidance module
// that PSG (formerly PVG) registers with for trajectory output.
//
// PVG's OnModuleEnabled does `Core.Guidance.Users.Add(this);` so
// during a PSG ascent this module should report UsersCount > 0.
// If it reports 0 while PSG is engaged, the cascade didn't fully
// wire and PSG won't get guidance pitch/heading commands.
//
// Wave 1: bare Enabled / UsersCount.
// Wave 2: IsStable, Pitch, Heading, Inertial, Status enum.

using KRPC.Service.Attributes;

namespace KRPC.MechJeb {
	/// <summary>
	/// MechJeb's PVG-style closed-loop guidance module. PSG registers
	/// as a user when engaged; the module then runs Glueball-fed
	/// trajectory output. Enabled / UsersCount inherited from
	/// KRPCComputerModule.
	/// </summary>
	[KRPCClass(Service = "MechJeb")]
	public class GuidanceController : KRPCComputerModule {
		internal new const string MechJebType = "MuMech.MechJebModuleGuidanceController";

		internal static new void InitType(System.Type type) {
			// No extra reflection beyond the base ComputerModule cache.
		}
	}
}
