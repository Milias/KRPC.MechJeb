// AttitudeController bridge — read-only observability for
// MechJebModuleAttitudeController, the underlying attitude-control
// module that AscentBaseAutopilot, NodeExecutor, RendezvousAutopilot,
// LandingAutopilot etc. all register themselves as users of when
// engaged.
//
// Distinct from `SmartASS` (MechJebModuleSmartASS), which is the
// in-game GUI attitude-target wrapper. SmartASS adds itself as a
// USER of AttitudeController when active — so AttitudeController is
// the lower-level mechanism, SmartASS is one of its clients.
//
// For now this is a minimal surface: `Enabled` (inherited) and
// `UsersCount` (inherited via KRPCComputerModule). Deeper fields
// (target pitch/heading, axis enables, attitudeKILLROT) are Wave 2.

using KRPC.Service.Attributes;

namespace KRPC.MechJeb {
	/// <summary>
	/// MechJeb's underlying attitude controller. Autopilots register
	/// themselves as users when engaged. The Enabled / UsersCount
	/// fields are inherited from KRPCComputerModule.
	/// </summary>
	[KRPCClass(Service = "MechJeb")]
	public class AttitudeController : KRPCComputerModule {
		internal new const string MechJebType = "MuMech.MechJebModuleAttitudeController";

		internal static new void InitType(System.Type type) {
			// No extra reflection beyond the base ComputerModule cache.
		}
	}
}
