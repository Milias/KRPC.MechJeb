// AscentClassic — Classic Ascent Profile settings.
//
// In MechJeb 2.15 the per-path settings were folded into the shared
// MechJebModuleAscentSettings ComputerModule (the actual autopilot logic
// lives in MechJebModuleAscentClassicAutopilot, but that class holds no
// public config state — config is on AscentSettings). Our bridge class
// thus shares the same backing instance as AscentAutopilot and just
// exposes the Classic-specific subset.

using KRPC.MechJeb.ExtensionMethods;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb {
	/// <summary>
	/// Classic Ascent Profile settings on MechJebModuleAscentSettings.
	/// Shares its instance with <see cref="AscentAutopilot"/>; both
	/// classes mutate the same MechJeb module.
	/// </summary>
	[KRPCClass(Service = "MechJeb")]
	public class AscentClassic : AscentBase {

		// EditableDouble / EditableDoubleMult instance holders, cached
		// at InitInstance time off the AscentSettings module instance.
		private object turnStartAltitude;
		private object turnStartVelocity;
		private object turnEndAltitude;
		private object turnEndAngle;
		private object turnShapeExponent;

		protected internal override void InitInstance(object instance) {
			base.InitInstance(instance);

			this.turnStartAltitude  = AscentSettingsBinding.turnStartAltitude.GetInstanceValue(instance);
			this.turnStartVelocity  = AscentSettingsBinding.turnStartVelocity.GetInstanceValue(instance);
			this.turnEndAltitude    = AscentSettingsBinding.turnEndAltitude.GetInstanceValue(instance);
			this.turnEndAngle       = AscentSettingsBinding.turnEndAngle.GetInstanceValue(instance);
			this.turnShapeExponent  = AscentSettingsBinding.turnShapeExponent.GetInstanceValue(instance);
		}

		/// <summary>The turn starts when this altitude is reached.</summary>
		[KRPCProperty]
		public double TurnStartAltitude {
			get => EditableDouble.Get(this.turnStartAltitude);
			set => EditableDouble.Set(this.turnStartAltitude, value);
		}

		/// <summary>The turn starts when this velocity is reached.</summary>
		[KRPCProperty]
		public double TurnStartVelocity {
			get => EditableDouble.Get(this.turnStartVelocity);
			set => EditableDouble.Set(this.turnStartVelocity, value);
		}

		/// <summary>The turn ends when this altitude is reached.</summary>
		[KRPCProperty]
		public double TurnEndAltitude {
			get => EditableDouble.Get(this.turnEndAltitude);
			set => EditableDouble.Set(this.turnEndAltitude, value);
		}

		/// <summary>The final flight path angle.</summary>
		[KRPCProperty]
		public double TurnEndAngle {
			get => EditableDouble.Get(this.turnEndAngle);
			set => EditableDouble.Set(this.turnEndAngle, value);
		}

		/// <summary>A value between 0 and 1 describing how steep the turn is.</summary>
		[KRPCProperty]
		public double TurnShapeExponent {
			get => EditableDouble.Get(this.turnShapeExponent);
			set => EditableDouble.Set(this.turnShapeExponent, value);
		}

		/// <summary>Whether to enable automatic altitude turn.</summary>
		[KRPCProperty]
		public bool AutoPath {
			get => (bool)AscentSettingsBinding.autoPath.GetValue(this.instance);
			set => AscentSettingsBinding.autoPath.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public float AutoTurnPercent {
			get => (float)AscentSettingsBinding.autoTurnPerc.GetValue(this.instance);
			set => AscentSettingsBinding.autoTurnPerc.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public float AutoTurnSpeedFactor {
			get => (float)AscentSettingsBinding.autoTurnSpdFactor.GetValue(this.instance);
			set => AscentSettingsBinding.autoTurnSpdFactor.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public double AutoTurnStartAltitude =>
			(double)AscentSettingsBinding.autoTurnStartAltitude.GetValue(this.instance, null);

		[KRPCProperty]
		public double AutoTurnStartVelocity =>
			(double)AscentSettingsBinding.autoTurnStartVelocity.GetValue(this.instance, null);

		[KRPCProperty]
		public double AutoTurnEndAltitude =>
			(double)AscentSettingsBinding.autoTurnEndAltitude.GetValue(this.instance, null);
	}
}
