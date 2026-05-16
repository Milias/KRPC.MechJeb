using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb.Maneuver {
	/// <summary>
	/// Plan a maneuver to set the surface longitude of an orbital apsis.
	///
	/// In MechJeb 2.15 the target longitude is read from
	/// <see cref="TargetController"/> (<c>targetLongitude</c>) at
	/// MakeNodes time. Set it through the TargetController before
	/// calling <see cref="Operation.MakeNodes"/>.
	/// </summary>
	[KRPCClass(Service = "MechJeb")]
	public class OperationLongitude : TimedOperation {
		internal new const string MechJebType = "MuMech.OperationLongitude";

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
