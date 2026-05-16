using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb.Maneuver {
	/// <summary>
	/// Hohmann-style transfer to target. MechJeb 2.15 replaced the
	/// 2.14 PVG-era trio (intercept_only / periodOffset / simpleTransfer)
	/// with a richer transfer model: capture-burn toggles, plane-matching,
	/// configurable departure window, and lag time.
	/// </summary>
	[KRPCClass(Service = "MechJeb")]
	public class OperationTransfer : TimedOperation {
		internal new const string MechJebType = "MuMech.OperationGeneric";

		// Fields and methods (all on MuMech.OperationGeneric in 2.15.0).
		private static FieldInfo captureField;
		private static FieldInfo planCaptureField;
		private static FieldInfo rendezvousField;    // was `MatchOrbit` on master, `Rendezvous` in 2.15.0
		private static FieldInfo coplanarField;
		private static FieldInfo lagTimeField;
		private static FieldInfo minDepartureUTField;
		private static FieldInfo maxDepartureUTField;
		private static FieldInfo timeSelector;

		// Instance objects — cached EditableDouble/EditableTime holders.
		private object lagTime;
		private object minDepartureUT;
		private object maxDepartureUT;

		internal static new void InitType(Type type) {
			captureField        = type.GetCheckedField("Capture");
			planCaptureField    = type.GetCheckedField("PlanCapture");
			rendezvousField     = type.GetCheckedField("Rendezvous");
			coplanarField       = type.GetCheckedField("Coplanar");
			lagTimeField        = type.GetCheckedField("LagTime");
			minDepartureUTField = type.GetCheckedField("MinDepartureUT");
			maxDepartureUTField = type.GetCheckedField("MaxDepartureUT");
			timeSelector        = GetTimeSelectorField(type);
		}

		protected internal override void InitInstance(object instance) {
			base.InitInstance(instance);

			this.lagTime        = lagTimeField.GetInstanceValue(instance);
			this.minDepartureUT = minDepartureUTField.GetInstanceValue(instance);
			this.maxDepartureUT = maxDepartureUTField.GetInstanceValue(instance);
			this.InitTimeSelector(timeSelector);
		}

		/// <summary>Plan a capture burn on arrival (otherwise an intercept-only burn).</summary>
		[KRPCProperty]
		public bool Capture {
			get => (bool)captureField.GetValue(this.instance);
			set => captureField.SetValue(this.instance, value);
		}

		/// <summary>Whether to actually place the capture node (vs only previewing).</summary>
		[KRPCProperty]
		public bool PlanCapture {
			get => (bool)planCaptureField.GetValue(this.instance);
			set => planCaptureField.SetValue(this.instance, value);
		}

		/// <summary>Match the target's orbit at arrival (vs an intercept-only burn).</summary>
		[KRPCProperty]
		public bool Rendezvous {
			get => (bool)rendezvousField.GetValue(this.instance);
			set => rendezvousField.SetValue(this.instance, value);
		}

		/// <summary>Restrict the transfer plane to the source plane (coplanar).</summary>
		[KRPCProperty]
		public bool Coplanar {
			get => (bool)coplanarField.GetValue(this.instance);
			set => coplanarField.SetValue(this.instance, value);
		}

		/// <summary>Lag time added to the computed transfer (seconds).</summary>
		[KRPCProperty]
		public double LagTime {
			get => EditableDouble.Get(this.lagTime);
			set => EditableDouble.Set(this.lagTime, value);
		}

		/// <summary>Earliest departure UT considered by the planner.</summary>
		[KRPCProperty]
		public double MinDepartureUT {
			get => EditableDouble.Get(this.minDepartureUT);
			set => EditableDouble.Set(this.minDepartureUT, value);
		}

		/// <summary>Latest departure UT considered by the planner.</summary>
		[KRPCProperty]
		public double MaxDepartureUT {
			get => EditableDouble.Get(this.maxDepartureUT);
			set => EditableDouble.Set(this.maxDepartureUT, value);
		}
	}
}
