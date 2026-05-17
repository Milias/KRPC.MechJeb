// AscentAutopilot — ported to MechJeb 2.15.
//
// In MechJeb 2.15 the old MechJebModuleAscentAutopilot was retired. The
// public configuration surface now lives on MechJebModuleAscentSettings;
// the runtime autopilot state (Status, TimedLaunch, StartCountdown) lives
// on the abstract MechJebModuleAscentBaseAutopilot, with the concrete
// per-path instances exposed as AscentSettings.AscentAutopilot (a
// property that returns whichever of MechJebModuleAscentClassicAutopilot
// / MechJebModuleAscentPSGAutopilot matches the current AscentType).
//
// This kRPC class binds to the AscentSettings instance directly and
// dispatches runtime calls through AscentSettings.AscentAutopilot. The
// path-specific bridge classes (AscentClassic / AscentPSG) also share
// the same AscentSettings instance — they expose subsets of its fields.
//
// AscentPathGT is gone (MechJeb 2.15 removed the gravity-turn variant).
// AscentPathIndex is now a 2-value enum: 0 = CLASSIC, 1 = PSG.

using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;
using KRPC.MechJeb.Util;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb {
	/// <summary>
	/// Drives MechJeb 2.15's Ascent Guidance. Config (target orbit, force
	/// roll, AoA limits, etc.) lives on MechJebModuleAscentSettings; runtime
	/// status comes from the currently-active concrete autopilot
	/// (Classic or PSG) selected by <see cref="AscentPathIndex"/>.
	/// </summary>
	[KRPCClass(Service = "MechJeb")]
	public class AscentAutopilot : KRPCComputerModule {
		// No `MechJebType` const here: reflection for the underlying
		// MechJebModuleAscentSettings type happens once in
		// AscentSettingsBinding (and only one bridge type can claim each
		// MuMech.* key — duplicates collide in MechJeb.InitTypes()).
		// InitInstance is called manually from MechJeb.cs's modules dict.

		// EditableDouble / EditableInt holders on AscentSettings.
		private object desiredOrbitAltitude;
		private object correctiveSteeringGain;
		private object verticalRoll;
		private object turnRoll;
		private object maxAoA;
		private object aoALimitFadeoutPressure;
		private object launchPhaseAngle;
		private object launchLANDifference;
		private object warpCountDown;
		// EditableDouble for inclination lives on AscentSettings too in 2.15
		// (used to live on AscentGuidance.desiredInclination).
		private object desiredInclination;

		// AscentSettings.AscentAutopilot is a property — we re-resolve each
		// read because the user may change AscentType at runtime.
		private object ActiveAutopilot =>
			AscentSettingsBinding.ascentAutopilotProp.GetValue(this.instance, null);

		protected internal override void InitInstance(object instance) {
			base.InitInstance(instance);

			this.desiredOrbitAltitude    = AscentSettingsBinding.desiredOrbitAltitude.GetInstanceValue(instance);
			this.desiredInclination      = AscentSettingsBinding.desiredInclination.GetInstanceValue(instance);
			this.correctiveSteeringGain  = AscentSettingsBinding.correctiveSteeringGain.GetInstanceValue(instance);
			this.verticalRoll            = AscentSettingsBinding.verticalRoll.GetInstanceValue(instance);
			this.turnRoll                = AscentSettingsBinding.turnRoll.GetInstanceValue(instance);
			this.maxAoA                  = AscentSettingsBinding.maxAoA.GetInstanceValue(instance);
			this.aoALimitFadeoutPressure = AscentSettingsBinding.aoALimitFadeoutPressure.GetInstanceValue(instance);
			this.launchPhaseAngle        = AscentSettingsBinding.launchPhaseAngle.GetInstanceValue(instance);
			this.launchLANDifference     = AscentSettingsBinding.launchLANDifference.GetInstanceValue(instance);
			this.warpCountDown           = AscentSettingsBinding.warpCountDown.GetInstanceValue(instance);

			// AscentSettings is the shared backing store for the per-path
			// settings classes below; they all bind to the same instance.
			this.AscentPathClassic.InitInstance(instance);
			this.AscentPathPSG.InitInstance(instance);

			// Re-apply current ascent type so the right concrete autopilot
			// is enabled / disabled consistently with what the user has set.
			if (instance != null)
				this.AscentPathIndex = this.AscentPathIndex;
		}

		public AscentAutopilot() {
			this.AscentPathClassic = new AscentClassic();
			this.AscentPathPSG     = new AscentPSG();
		}

		/// <summary>
		/// Enable / disable the autopilot. Delegates to whichever concrete
		/// autopilot is currently active (Classic or PSG) — both Classic
		/// and PSG instances are always loaded; only the one matching the
		/// current AscentType actually runs.
		/// </summary>
		[KRPCProperty]
		public override bool Enabled {
			get {
				object active = this.ActiveAutopilot;
				return active != null && (bool)enabled.GetValue(active, null);
			}
			set {
				object active = this.ActiveAutopilot;
				if (active == null) return;
				object activeUsers = usersField.GetValue(active);
				MethodInfo m = value ? UserPool.usersAdd : UserPool.usersRemove;
				m.Invoke(activeUsers, new object[] { active });
			}
		}

		/// <summary>
		/// The autopilot status string (depends on which ascent path is active).
		/// </summary>
		[KRPCProperty]
		public string Status {
			get {
				object active = this.ActiveAutopilot;
				return active != null
					? (string)AscentBaseAutopilotBinding.status.GetValue(active)
					: "";
			}
		}

		/// <summary>
		/// Top-level ascent state machine: `PRELAUNCH` / `ASCEND` /
		/// `CIRCULARIZE` (the private `_mode` field on the abstract
		/// `MechJebModuleAscentBaseAutopilot`). `"Off"` when no autopilot
		/// is active. Useful for telling whether the autopilot got past
		/// pre-launch into the actual ascent dispatch.
		/// </summary>
		[KRPCProperty]
		public string AscentStage {
			get {
				object active = this.ActiveAutopilot;
				if (active == null || AscentBaseAutopilotBinding.mode == null)
					return "Off";
				return AscentBaseAutopilotBinding.mode.GetValue(active).ToString();
			}
		}

		/// <summary>
		/// Per-path inner state machine. For Classic: VERTICAL_ASCENT /
		/// GRAVITY_TURN / COAST_TO_APOAPSIS / EXIT. For PSG:
		/// VERTICAL_ASCENT / PITCHPROGRAM / ZEROLIFT / GUIDANCE / EXIT.
		/// Both paths declare their own private `_mode` field; we
		/// dispatch on `AscentPathIndex` to pick the right binding.
		/// </summary>
		[KRPCProperty]
		public string PathMode {
			get {
				object active = this.ActiveAutopilot;
				if (active == null) return "Off";
				int idx = this.AscentPathIndex;
				FieldInfo fi = idx == 0
					? AscentClassicAutopilotBinding.mode
					: AscentPVGAutopilotBinding.mode;
				if (fi == null) return "?";
				return fi.GetValue(active).ToString();
			}
		}

		/// <summary>
		/// The selected ascent path.
		///
		/// 0 = <see cref="AscentClassic" /> (Classic Ascent Profile)
		///
		/// 1 = <see cref="AscentPSG" /> (Powered Soft-landing Guidance — formerly PVG)
		/// </summary>
		[KRPCProperty]
		public int AscentPathIndex {
			get => (int)AscentSettingsBinding.ascentTypeInteger.GetValue(this.instance);
			set {
				if (value < 0 || value > 1)
					return;
				if (value == this.AscentPathIndex)
					return;

				// Migrate the Users-pool registration when the active path
				// changes. The Enabled setter binds the bridge into
				// ActiveAutopilot.users at the moment it's called; if we
				// switch path mid-flight without migrating, the entry stays
				// in the OLD path's pool and the new path's pool stays empty
				// — neither autopilot ends up steering. Capture wasEnabled +
				// the old ActiveAutopilot BEFORE writing the new index;
				// re-resolve ActiveAutopilot afterwards to hit the new path.
				bool wasEnabled = this.Enabled;
				object oldActive = wasEnabled ? this.ActiveAutopilot : null;

				AscentSettingsBinding.ascentTypeInteger.SetValue(this.instance, value);

				if (wasEnabled) {
					if (oldActive != null) {
						object oldUsers = usersField.GetValue(oldActive);
						UserPool.usersRemove.Invoke(oldUsers, new object[] { oldActive });
					}
					object newActive = this.ActiveAutopilot;
					if (newActive != null) {
						object newUsers = usersField.GetValue(newActive);
						UserPool.usersAdd.Invoke(newUsers, new object[] { newActive });
					}
				}
			}
		}

		/// <summary>Classic Ascent Profile settings.</summary>
		[KRPCProperty]
		public AscentClassic AscentPathClassic { get; }

		/// <summary>PSG (RSS/RO) Ascent Profile settings.</summary>
		[KRPCProperty]
		public AscentPSG AscentPathPSG { get; }

		[KRPCProperty]
		public double DesiredOrbitAltitude {
			get => EditableDouble.Get(this.desiredOrbitAltitude);
			set => EditableDouble.Set(this.desiredOrbitAltitude, value);
		}

		[KRPCProperty]
		public double DesiredInclination {
			get => EditableDouble.Get(this.desiredInclination);
			set => EditableDouble.Set(this.desiredInclination, value);
		}

		/// <remarks>Equivalent to <see cref="MechJeb.ThrustController" />.</remarks>
		[KRPCProperty]
		public ThrustController ThrustController => MechJeb.ThrustController;

		[KRPCProperty]
		public bool CorrectiveSteering {
			get => (bool)AscentSettingsBinding.correctiveSteering.GetValue(this.instance);
			set => AscentSettingsBinding.correctiveSteering.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public double CorrectiveSteeringGain {
			get => EditableDouble.Get(this.correctiveSteeringGain);
			set => EditableDouble.Set(this.correctiveSteeringGain, value);
		}

		[KRPCProperty]
		public bool ForceRoll {
			get => (bool)AscentSettingsBinding.forceRoll.GetValue(this.instance);
			set => AscentSettingsBinding.forceRoll.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public double VerticalRoll {
			get => EditableDouble.Get(this.verticalRoll);
			set => EditableDouble.Set(this.verticalRoll, value);
		}

		[KRPCProperty]
		public double TurnRoll {
			get => EditableDouble.Get(this.turnRoll);
			set => EditableDouble.Set(this.turnRoll, value);
		}

		[KRPCProperty]
		public bool AutodeploySolarPanels {
			get => (bool)AscentSettingsBinding.autoDeploySolarPanels.GetValue(this.instance);
			set => AscentSettingsBinding.autoDeploySolarPanels.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public bool AutoDeployAntennas {
			get => (bool)AscentSettingsBinding.autoDeployAntennas.GetValue(this.instance);
			set => AscentSettingsBinding.autoDeployAntennas.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public bool SkipCircularization {
			get => (bool)AscentSettingsBinding.skipCircularization.GetValue(this.instance);
			set => AscentSettingsBinding.skipCircularization.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public bool Autostage {
			get => (bool)AscentSettingsBinding.autostage.GetValue(this.instance, null);
			set => AscentSettingsBinding.autostage.SetValue(this.instance, value, null);
		}

		/// <remarks>Equivalent to <see cref="MechJeb.StagingController" />.</remarks>
		[KRPCProperty]
		public StagingController StagingController => MechJeb.StagingController;

		[KRPCProperty]
		public bool LimitAoA {
			get => (bool)AscentSettingsBinding.limitAoA.GetValue(this.instance);
			set => AscentSettingsBinding.limitAoA.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public double MaxAoA {
			get => EditableDouble.Get(this.maxAoA);
			set => EditableDouble.Set(this.maxAoA, value);
		}

		[KRPCProperty]
		public double AoALimitFadeoutPressure {
			get => EditableDouble.Get(this.aoALimitFadeoutPressure);
			set => EditableDouble.Set(this.aoALimitFadeoutPressure, value);
		}

		[KRPCProperty]
		public double LaunchPhaseAngle {
			get => EditableDouble.Get(this.launchPhaseAngle);
			set => EditableDouble.Set(this.launchPhaseAngle, value);
		}

		[KRPCProperty]
		public double LaunchLANDifference {
			get => EditableDouble.Get(this.launchLANDifference);
			set => EditableDouble.Set(this.launchLANDifference, value);
		}

		[KRPCProperty]
		public int WarpCountDown {
			get => EditableInt.Get(this.warpCountDown);
			set => EditableInt.Set(this.warpCountDown, value);
		}

		/// <summary>
		/// Current autopilot launch mode. Useful for determining whether the
		/// autopilot is performing a timed launch.
		/// </summary>
		[KRPCProperty]
		public AscentLaunchMode LaunchMode {
			get {
				object active = this.ActiveAutopilot;
				if (active == null || !(bool)AscentBaseAutopilotBinding.timedLaunch.GetValue(active))
					return AscentLaunchMode.Normal;
				if ((bool)AscentSettingsBinding.launchingToRendezvous.GetValue(this.instance))
					return AscentLaunchMode.Rendezvous;
				if ((bool)AscentSettingsBinding.launchingToPlane.GetValue(this.instance))
					return AscentLaunchMode.TargetPlane;
				return AscentLaunchMode.Unknown;
			}
		}

		[KRPCMethod]
		public void AbortTimedLaunch() {
			if (this.LaunchMode == AscentLaunchMode.Unknown)
				throw new InvalidOperationException("There is an unknown timed launch ongoing which can't be aborted");

			AscentSettingsBinding.launchingToPlane.SetValue(this.instance, false);
			AscentSettingsBinding.launchingToRendezvous.SetValue(this.instance, false);
			object active = this.ActiveAutopilot;
			if (active != null)
				AscentBaseAutopilotBinding.timedLaunch.SetValue(active, false);
		}

		private void StartCountdown(double timeOffset) {
			object active = this.ActiveAutopilot;
			if (active == null)
				throw new InvalidOperationException("No active ascent autopilot");
			AscentBaseAutopilotBinding.startCountdown.Invoke(
				active,
				new object[] { MechJeb.vesselState.Time + timeOffset });
		}

		/// <summary>Launch to rendezvous with the selected target.</summary>
		[KRPCMethod]
		public void LaunchToRendezvous() {
			if (!MechJeb.TargetController.NormalTargetExists)
				throw new InvalidOperationException("Invalid target");
			if (this.AscentPathIndex == 1)
				throw new InvalidOperationException("This action can't be performed in PSG path mode");

			this.AbortTimedLaunch();
			try {
				AscentSettingsBinding.launchingToRendezvous.SetValue(this.instance, true);
				this.StartCountdown(LaunchTiming.TimeToPhaseAngle(this.LaunchPhaseAngle));
			}
			catch (Exception) {
				this.AbortTimedLaunch();
				throw;
			}
		}

		/// <summary>Launch into the plane of the selected target.</summary>
		[KRPCMethod]
		public void LaunchToTargetPlane() {
			if (!MechJeb.TargetController.NormalTargetExists)
				throw new InvalidOperationException("Invalid target");

			this.AbortTimedLaunch();
			try {
				Orbit target = MechJeb.TargetController.InternalTargetOrbit;
				AscentSettingsBinding.launchingToPlane.SetValue(this.instance, true);

				Tuple<double, double> item = MathFunctions.MinimumTimeToPlane(target.LAN - this.LaunchLANDifference, target.inclination);
				this.StartCountdown(item.Item1);
				this.DesiredInclination = item.Item2;
			}
			catch (Exception) {
				this.AbortTimedLaunch();
				throw;
			}
		}

		[KRPCEnum(Service = "MechJeb")]
		public enum AscentLaunchMode {
			/// <summary>The autopilot is not performing a timed launch.</summary>
			Normal,
			/// <summary>The autopilot is performing a timed launch to rendezvous with the target.</summary>
			Rendezvous,
			/// <summary>The autopilot is performing a timed launch to target plane.</summary>
			TargetPlane,
			/// <summary>The autopilot is performing an unknown timed launch.</summary>
			Unknown = 99
		}
	}

	/// <summary>
	/// Path-specific bridge classes share the same backing AscentSettings
	/// instance — they bind to subsets of its fields. AscentBase here is
	/// just a marker; the real backing is AscentSettings.
	/// </summary>
	public abstract class AscentBase : ComputerModule { }
}
