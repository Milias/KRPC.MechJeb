// AscentPSG — Powered Soft-landing Guidance profile (RSS/RO).
//
// 2.15.0's MechJebModuleAscentSettings field set for PSG includes:
//   * Pitch ramp:   PitchStartVelocity, PitchRate
//   * Target orbit: DesiredApoapsis, DesiredAttachAlt, AttachAltFlag,
//                   DesiredFPA
//   * Optimiser:    MinDeltaV, MinCoast, MaxCoast, LastStage,
//                   PreStageTime, OptimizerPauseTime, OptimizeStageFlag,
//                   OptimizeStageInternal
//   * Coast stage:  CoastStageFlag, CoastStageInternal, CoastStage(prop),
//                   CoastBeforeFlag
//   * Spinup stage: SpinupStageFlag, SpinupStageInternal, SpinupStage(prop),
//                   SpinupLeadTime, SpinupAngularVelocity
//   * Unguided:     UnguidedStagesFlag, UnguidedStagesInternal
//   * PVG-era hold: DynamicPressureTrigger, FixedCoast, FixedCoastLength,
//                   StagingTrigger, StagingTriggerFlag
//
// Master added Cd / Aref / DesiredArgP / DesiredArgPFlag / CoastLocation /
// FixedStagesFlag / FixedStagesInternal — none of those exist in 2.15.0
// and aren't exposed here.

using System.Collections.Generic;

using KRPC.MechJeb.ExtensionMethods;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb {
	/// <summary>
	/// PSG (Powered Soft-landing Guidance, RSS/RO) ascent profile.
	/// Settings live on MechJebModuleAscentSettings; this class shares
	/// the same instance as <see cref="AscentAutopilot"/>.
	/// </summary>
	[KRPCClass(Service = "MechJeb")]
	public class AscentPSG : AscentBase {

		private object pitchStartVelocity;
		private object pitchRate;
		private object desiredApoapsis;
		private object desiredAttachAlt;
		private object desiredFPA;
		private object minDeltaV;
		private object maxCoast;
		private object minCoast;
		private object coastStageInternal;
		private object spinupStageInternal;
		private object spinupLeadTime;
		private object spinupAngularVelocity;
		private object unguidedStagesInternal;
		private object optimizeStageInternal;
		private object lastStage;
		private object preStageTime;
		private object optimizerPauseTime;
		private object dynamicPressureTrigger;
		private object fixedCoastLength;
		private object stagingTrigger;

		protected internal override void InitInstance(object instance) {
			base.InitInstance(instance);

			this.pitchStartVelocity     = AscentSettingsBinding.pitchStartVelocity.GetInstanceValue(instance);
			this.pitchRate              = AscentSettingsBinding.pitchRate.GetInstanceValue(instance);
			this.desiredApoapsis        = AscentSettingsBinding.desiredApoapsis.GetInstanceValue(instance);
			this.desiredAttachAlt       = AscentSettingsBinding.desiredAttachAlt.GetInstanceValue(instance);
			this.desiredFPA             = AscentSettingsBinding.desiredFPA.GetInstanceValue(instance);
			this.minDeltaV              = AscentSettingsBinding.minDeltaV.GetInstanceValue(instance);
			this.maxCoast               = AscentSettingsBinding.maxCoast.GetInstanceValue(instance);
			this.minCoast               = AscentSettingsBinding.minCoast.GetInstanceValue(instance);
			this.coastStageInternal     = AscentSettingsBinding.coastStageInternal.GetInstanceValue(instance);
			this.spinupStageInternal    = AscentSettingsBinding.spinupStageInternal.GetInstanceValue(instance);
			this.spinupLeadTime         = AscentSettingsBinding.spinupLeadTime.GetInstanceValue(instance);
			this.spinupAngularVelocity  = AscentSettingsBinding.spinupAngularVelocity.GetInstanceValue(instance);
			this.unguidedStagesInternal = AscentSettingsBinding.unguidedStagesInternal.GetInstanceValue(instance);
			this.optimizeStageInternal  = AscentSettingsBinding.optimizeStageInternal.GetInstanceValue(instance);
			this.lastStage              = AscentSettingsBinding.lastStage.GetInstanceValue(instance);
			this.preStageTime           = AscentSettingsBinding.preStageTime.GetInstanceValue(instance);
			this.optimizerPauseTime     = AscentSettingsBinding.optimizerPauseTime.GetInstanceValue(instance);
			this.dynamicPressureTrigger = AscentSettingsBinding.dynamicPressureTrigger.GetInstanceValue(instance);
			this.fixedCoastLength       = AscentSettingsBinding.fixedCoastLength.GetInstanceValue(instance);
			this.stagingTrigger         = AscentSettingsBinding.stagingTrigger.GetInstanceValue(instance);
		}

		// -- Pitch ramp -------------------------------------------------------

		/// <summary>Velocity (m/s) at which the pitch program begins.</summary>
		[KRPCProperty]
		public double PitchStartVelocity {
			get => EditableDouble.Get(this.pitchStartVelocity);
			set => EditableDouble.Set(this.pitchStartVelocity, value);
		}

		/// <summary>Pitch-program rate.</summary>
		[KRPCProperty]
		public double PitchRate {
			get => EditableDouble.Get(this.pitchRate);
			set => EditableDouble.Set(this.pitchRate, value);
		}

		// -- Target orbit -----------------------------------------------------

		[KRPCProperty]
		public double DesiredApoapsis {
			get => EditableDouble.Get(this.desiredApoapsis);
			set => EditableDouble.Set(this.desiredApoapsis, value);
		}

		[KRPCProperty]
		public bool AttachAltFlag {
			get => (bool)AscentSettingsBinding.attachAltFlag.GetValue(this.instance);
			set => AscentSettingsBinding.attachAltFlag.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public double DesiredAttachAlt {
			get => EditableDouble.Get(this.desiredAttachAlt);
			set => EditableDouble.Set(this.desiredAttachAlt, value);
		}

		[KRPCProperty]
		public double DesiredFPA {
			get => EditableDouble.Get(this.desiredFPA);
			set => EditableDouble.Set(this.desiredFPA, value);
		}

		// -- Optimiser knobs --------------------------------------------------

		[KRPCProperty]
		public double MinDeltaV {
			get => EditableDouble.Get(this.minDeltaV);
			set => EditableDouble.Set(this.minDeltaV, value);
		}

		[KRPCProperty]
		public double MaxCoast {
			get => EditableDouble.Get(this.maxCoast);
			set => EditableDouble.Set(this.maxCoast, value);
		}

		[KRPCProperty]
		public double MinCoast {
			get => EditableDouble.Get(this.minCoast);
			set => EditableDouble.Set(this.minCoast, value);
		}

		[KRPCProperty]
		public int LastStage {
			get => EditableInt.Get(this.lastStage);
			set => EditableInt.Set(this.lastStage, value);
		}

		[KRPCProperty]
		public double PreStageTime {
			get => EditableDouble.Get(this.preStageTime);
			set => EditableDouble.Set(this.preStageTime, value);
		}

		[KRPCProperty]
		public double OptimizerPauseTime {
			get => EditableDouble.Get(this.optimizerPauseTime);
			set => EditableDouble.Set(this.optimizerPauseTime, value);
		}

		[KRPCProperty]
		public bool OptimizeStageEnabled {
			get => (bool)AscentSettingsBinding.optimizeStageFlag.GetValue(this.instance);
			set => AscentSettingsBinding.optimizeStageFlag.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public int OptimizeStageIndex {
			get => EditableInt.Get(this.optimizeStageInternal);
			set => EditableInt.Set(this.optimizeStageInternal, value);
		}

		// -- Coast stage ------------------------------------------------------

		[KRPCProperty]
		public bool CoastStageEnabled {
			get => (bool)AscentSettingsBinding.coastStageFlag.GetValue(this.instance);
			set => AscentSettingsBinding.coastStageFlag.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public int CoastStageIndex {
			get => EditableInt.Get(this.coastStageInternal);
			set => EditableInt.Set(this.coastStageInternal, value);
		}

		[KRPCProperty]
		public int CoastStage =>
			(int)AscentSettingsBinding.coastStageProp.GetValue(this.instance, null);

		/// <summary>Coast before staging (vs after).</summary>
		[KRPCProperty]
		public bool CoastBeforeFlag {
			get => (bool)AscentSettingsBinding.coastBeforeFlag.GetValue(this.instance);
			set => AscentSettingsBinding.coastBeforeFlag.SetValue(this.instance, value);
		}

		// -- Spinup stage -----------------------------------------------------

		[KRPCProperty]
		public bool SpinupStageEnabled {
			get => (bool)AscentSettingsBinding.spinupStageFlag.GetValue(this.instance);
			set => AscentSettingsBinding.spinupStageFlag.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public int SpinupStageIndex {
			get => EditableInt.Get(this.spinupStageInternal);
			set => EditableInt.Set(this.spinupStageInternal, value);
		}

		[KRPCProperty]
		public int SpinupStage =>
			(int)AscentSettingsBinding.spinupStageProp.GetValue(this.instance, null);

		[KRPCProperty]
		public double SpinupLeadTime {
			get => EditableDouble.Get(this.spinupLeadTime);
			set => EditableDouble.Set(this.spinupLeadTime, value);
		}

		[KRPCProperty]
		public double SpinupAngularVelocity {
			get => EditableDouble.Get(this.spinupAngularVelocity);
			set => EditableDouble.Set(this.spinupAngularVelocity, value);
		}

		// -- Unguided stages list ---------------------------------------------

		[KRPCProperty]
		public bool UnguidedStagesEnabled {
			get => (bool)AscentSettingsBinding.unguidedStagesFlag.GetValue(this.instance);
			set => AscentSettingsBinding.unguidedStagesFlag.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public IList<int> UnguidedStages {
			get => EditableIntList.Get(this.unguidedStagesInternal);
			set => EditableIntList.Set(this.unguidedStagesInternal, value);
		}

		// -- PVG-era staging-trigger knobs (still in 2.15.0) ------------------

		/// <summary>
		/// Dynamic pressure (kPa) below which a staging event is allowed.
		/// PSG-style alternative to <see cref="StagingTrigger"/>.
		/// </summary>
		[KRPCProperty]
		public double DynamicPressureTrigger {
			get => EditableDouble.Get(this.dynamicPressureTrigger);
			set => EditableDouble.Set(this.dynamicPressureTrigger, value);
		}

		/// <summary>Trigger PSG to stage at a specific stage index.</summary>
		[KRPCProperty]
		public int StagingTrigger {
			get => EditableInt.Get(this.stagingTrigger);
			set => EditableInt.Set(this.stagingTrigger, value);
		}

		/// <summary>Whether <see cref="StagingTrigger"/> is honoured.</summary>
		[KRPCProperty]
		public bool StagingTriggerFlag {
			get => (bool)AscentSettingsBinding.stagingTriggerFlag.GetValue(this.instance);
			set => AscentSettingsBinding.stagingTriggerFlag.SetValue(this.instance, value);
		}

		/// <summary>Whether PSG holds a fixed coast of <see cref="FixedCoastLength"/> seconds.</summary>
		[KRPCProperty]
		public bool FixedCoast {
			get => (bool)AscentSettingsBinding.fixedCoast.GetValue(this.instance);
			set => AscentSettingsBinding.fixedCoast.SetValue(this.instance, value);
		}

		/// <summary>Fixed coast duration (s).</summary>
		[KRPCProperty]
		public double FixedCoastLength {
			get => EditableDouble.Get(this.fixedCoastLength);
			set => EditableDouble.Set(this.fixedCoastLength, value);
		}
	}
}
