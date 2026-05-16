using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb.Maneuver {
	[KRPCEnum(Service = "MechJeb")]
	public enum TimeReference {
		/// <summary>
		/// At the optimum time.
		/// </summary>
		Computed,

		/// <summary>
		/// After a fixed <see cref="TimeSelector.LeadTime" />.
		/// </summary>
		XFromNow,

		/// <summary>
		/// At the next apoapsis.
		/// </summary>
		Apoapsis,

		/// <summary>
		/// At the next periapsis.
		/// </summary>
		Periapsis,

		/// <summary>
		/// At the selected <see cref="TimeSelector.CircularizeAltitude" />.
		/// </summary>
		Altitude,

		/// <summary>
		/// At the equatorial ascending node.
		/// </summary>
		EqAscending,

		/// <summary>
		/// At the equatorial descending node.
		/// </summary>
		EqDescending,

		/// <summary>
		/// At the next ascending node with the target.
		/// </summary>
		RelAscending,

		/// <summary>
		/// At the next descending node with the target.
		/// </summary>
		RelDescending,

		/// <summary>
		/// At the closest approach to the target.
		/// </summary>
		ClosestApproach,

		/// <summary>
		/// At the cheapest equatorial AN/DN.
		/// </summary>
		EqHighestAd,

		/// <summary>
		/// At the nearest equatorial AN/DN.
		/// </summary>
		EqNearestAd,

		/// <summary>
		/// At the cheapest AN/DN with the target.
		/// </summary>
		RelHighestAd,

		/// <summary>
		/// At the nearest AN/DN with the target.
		/// </summary>
		RelNearestAd
	}

	[KRPCClass(Service = "MechJeb")]
	public class TimeSelector {
		internal const string MechJebType = "MuMech.TimeSelector";

		// Fields and methods
		private static FieldInfo allowedTimeRefField;
		private static FieldInfo currentTimeRef;
		private static FieldInfo leadTimeField;
		private static FieldInfo circularizeAltitudeField;

		// Instance objects
		internal object instance;

		private int[] allowedTimeRef; //MuMech.TimeReference enum
		private object leadTime;
		private object circularizeAltitude;

		internal static void InitType(Type type) {
			// MechJeb 2.15: both fields gained a leading underscore;
			// `_allowedTimeRef` is private, `_currentTimeRef` became public
			// (still bound via NonPublic|Public for forward compat).
			allowedTimeRefField = type.GetCheckedField("_allowedTimeRef",
				BindingFlags.NonPublic | BindingFlags.Instance);
			currentTimeRef = type.GetCheckedField("_currentTimeRef",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			leadTimeField = type.GetCheckedField("LeadTime");
			circularizeAltitudeField = type.GetCheckedField("CircularizeAltitude");
		}

		protected internal void InitInstance(object instance) {
			this.instance = instance;

			// In 2.15 the field is TimeReference[] (an enum array) — cast
			// element-wise to int rather than relying on Array variance,
			// which doesn't allow `(int[]) (TimeReference[])` at runtime.
			Array timeRefs = (Array)allowedTimeRefField.GetInstanceValue(instance);
			this.allowedTimeRef = new int[timeRefs.Length];
			for (int i = 0; i < timeRefs.Length; i++)
				this.allowedTimeRef[i] = (int)timeRefs.GetValue(i);

			this.leadTime = leadTimeField.GetInstanceValue(instance);
			this.circularizeAltitude = circularizeAltitudeField.GetInstanceValue(instance);
		}

		[KRPCProperty]
		public TimeReference TimeReference {
			get => (TimeReference)this.allowedTimeRef[(int)currentTimeRef.GetValue(this.instance)];
			set => currentTimeRef.SetValue(this.instance, this.GetTimeRefIndex(value));
		}

		private int GetTimeRefIndex(TimeReference timeRef) {
			for(int i = 0; i < this.allowedTimeRef.Length; i++)
				if(this.allowedTimeRef[i] == (int)timeRef)
					return i;
			throw new OperationException("This TimeReference is not allowed: " + timeRef);
		}

		/// <summary>
		/// To be used with <see cref="TimeReference.XFromNow" />.
		/// </summary>
		[KRPCProperty]
		public double LeadTime {
			get => EditableDouble.Get(this.leadTime);
			set => EditableDouble.Set(this.leadTime, value);
		}

		/// <summary>
		/// To be used with <see cref="TimeReference.Altitude" />.
		/// </summary>
		[KRPCProperty]
		public double CircularizeAltitude {
			get => EditableDouble.Get(this.circularizeAltitude);
			set => EditableDouble.Set(this.circularizeAltitude, value);
		}
	}
}
