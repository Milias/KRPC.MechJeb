using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb.Maneuver {
	/// <summary>
	/// Plan a maneuver to change the longitude of ascending node (LAN).
	///
	/// In MechJeb 2.15 the target LAN is read from
	/// <see cref="TargetController"/> (<c>targetLongitude</c>) at MakeNodes
	/// time — there's no longer a per-operation field. Set the target's
	/// longitude through the TargetController before calling
	/// <see cref="Operation.MakeNodes"/>.
	/// </summary>
	[KRPCClass(Service = "MechJeb")]
	public class OperationLan : TimedOperation {
		internal new const string MechJebType = "MuMech.OperationLan";

		private static FieldInfo timeSelector;

		internal static new void InitType(Type type) {
			timeSelector = GetTimeSelectorField(type);
		}

		protected internal override void InitInstance(object instance) {
			base.InitInstance(instance);
			this.InitTimeSelector(timeSelector);
		}
	}
}
