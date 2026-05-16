using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb.Maneuver {
	/// <summary>
	/// Create a maneuver to transfer to another planet
	/// </summary>
	[KRPCClass(Service = "MechJeb")]
	public class OperationInterplanetaryTransfer : Operation {
		internal new const string MechJebType = "MuMech.OperationInterplanetaryTransfer";

		// Fields and methods
		private static FieldInfo waitForPhaseAngle;

		internal static new void InitType(Type type) {
			// 2.15: `public bool WaitForPhaseAngle` — public, no need for
			// NonPublic flag (which would *exclude* it).
			waitForPhaseAngle = type.GetCheckedField("WaitForPhaseAngle");
		}

		[KRPCProperty]
		public bool WaitForPhaseAngle {
			get => (bool)waitForPhaseAngle.GetValue(this.instance);
			set => waitForPhaseAngle.SetValue(this.instance, value);
		}
	}
}
