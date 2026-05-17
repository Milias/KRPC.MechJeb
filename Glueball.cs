// Glueball bridge — read-only observability for
// MechJebModulePVGGlueBall, the PVG-style trajectory solver. PSG
// (MechJebModuleAscentPVGAutopilot) registers as a user and calls
// SetTarget(...) every tick to feed Glueball the target orbit.
//
// Wave 1: bare Enabled / UsersCount.
// Wave 2: Status enum, target readback, last-solve timestamp.

using KRPC.Service.Attributes;

namespace KRPC.MechJeb {
	/// <summary>
	/// MechJeb's PVG trajectory solver. PSG registers as a user when
	/// engaged and feeds the solver via `SetTarget` every tick.
	/// Enabled / UsersCount inherited from KRPCComputerModule.
	/// </summary>
	[KRPCClass(Service = "MechJeb")]
	public class Glueball : KRPCComputerModule {
		internal new const string MechJebType = "MuMech.MechJebModulePVGGlueBall";

		internal static new void InitType(System.Type type) {
			// No extra reflection beyond the base ComputerModule cache.
		}
	}
}
