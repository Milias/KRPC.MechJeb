// Shared reflection bindings for MechJeb 2.15's ascent subsystem.
//
// MechJeb 2.15 split the old MechJebModuleAscentAutopilot monolith into:
//   - MechJebModuleAscentSettings — all config state (Classic + PSG +
//     shared). EditableDouble fields plus a few bools / ints / properties.
//   - MechJebModuleAscentBaseAutopilot — abstract base for autopilot
//     runtime state (Status, TimedLaunch, StartCountdown).
//   - MechJebModuleAscentClassicAutopilot — concrete Classic implementation.
//   - MechJebModuleAscentPSGAutopilot — concrete PSG implementation (was
//     PVG in 2.14).
//   - MechJebModuleAscentMenu — UI host. Carries the private static
//     TimeToPhaseAngle helper that LaunchTiming used to wrap.
//
// MechJebModuleAscentGuidance and MechJebModuleAirplaneAutopilot were
// removed entirely; MechJebModuleAscentGT was removed (no replacement —
// the "gravity turn" path is folded into Classic).
//
// These static helpers run once at game-load via the InitTypes loop in
// MechJeb.cs. The kRPC-visible AscentAutopilot / AscentClassic /
// AscentPSG bridge classes all share these FieldInfo / PropertyInfo /
// MethodInfo caches.

using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;

namespace KRPC.MechJeb {

	internal static class AscentSettingsBinding {
		internal const string MechJebType = "MuMech.MechJebModuleAscentSettings";

		// Generic config used by AscentAutopilot
		internal static FieldInfo desiredOrbitAltitude;
		internal static FieldInfo desiredInclination;
		internal static FieldInfo correctiveSteering;
		internal static FieldInfo correctiveSteeringGain;
		internal static FieldInfo forceRoll;
		internal static FieldInfo verticalRoll;
		internal static FieldInfo turnRoll;
		internal static FieldInfo autoDeploySolarPanels;
		internal static FieldInfo autoDeployAntennas;
		internal static FieldInfo skipCircularization;
		internal static PropertyInfo autostage;
		internal static FieldInfo limitAoA;
		internal static FieldInfo maxAoA;
		internal static FieldInfo aoALimitFadeoutPressure;
		internal static FieldInfo launchPhaseAngle;
		internal static FieldInfo launchLANDifference;
		internal static FieldInfo warpCountDown;
		internal static FieldInfo launchingToPlane;
		internal static FieldInfo launchingToRendezvous;
		internal static FieldInfo ascentTypeInteger;
		internal static PropertyInfo ascentAutopilotProp;

		// Classic-path config (used by AscentClassic)
		internal static FieldInfo turnStartAltitude;
		internal static FieldInfo turnStartVelocity;
		internal static FieldInfo turnEndAltitude;
		internal static FieldInfo turnEndAngle;
		internal static FieldInfo turnShapeExponent;
		internal static FieldInfo autoPath;
		internal static FieldInfo autoTurnPerc;
		internal static FieldInfo autoTurnSpdFactor;
		internal static PropertyInfo autoTurnStartAltitude;
		internal static PropertyInfo autoTurnStartVelocity;
		internal static PropertyInfo autoTurnEndAltitude;

		// PSG-path config (used by AscentPSG).
		//
		// 2.15.0 field set: PitchStartVelocity / PitchRate / DesiredApoapsis /
		// DesiredAttachAlt / AttachAltFlag / DesiredFPA / MinDeltaV /
		// MaxCoast / MinCoast / CoastStageFlag / CoastStageInternal /
		// CoastStage(prop) / CoastBeforeFlag / SpinupStageFlag /
		// SpinupStageInternal / SpinupStage(prop) / SpinupLeadTime /
		// SpinupAngularVelocity / UnguidedStagesFlag /
		// UnguidedStagesInternal / OptimizeStage(Flag) / OptimizeStageInternal /
		// LastStage / PreStageTime / OptimizerPauseTime / DynamicPressureTrigger /
		// FixedCoast / FixedCoastLength / StagingTrigger / StagingTriggerFlag.
		// Master added Cd / Aref / DesiredArgP / DesiredArgPFlag /
		// CoastLocation / FixedStagesFlag / FixedStagesInternal — none of
		// those exist in 2.15.0, so the bridge doesn't expose them yet.
		internal static FieldInfo pitchStartVelocity;
		internal static FieldInfo pitchRate;
		internal static FieldInfo desiredApoapsis;
		internal static FieldInfo desiredAttachAlt;
		internal static FieldInfo attachAltFlag;
		internal static FieldInfo desiredFPA;
		internal static FieldInfo minDeltaV;
		internal static FieldInfo maxCoast;
		internal static FieldInfo minCoast;
		internal static FieldInfo coastStageFlag;
		internal static FieldInfo coastStageInternal;
		internal static PropertyInfo coastStageProp;
		internal static FieldInfo coastBeforeFlag;
		internal static FieldInfo spinupStageFlag;
		internal static FieldInfo spinupStageInternal;
		internal static PropertyInfo spinupStageProp;
		internal static FieldInfo spinupLeadTime;
		internal static FieldInfo spinupAngularVelocity;
		internal static FieldInfo unguidedStagesFlag;
		internal static FieldInfo unguidedStagesInternal;
		internal static FieldInfo optimizeStageFlag;
		internal static FieldInfo optimizeStageInternal;
		internal static FieldInfo lastStage;
		internal static FieldInfo preStageTime;
		internal static FieldInfo optimizerPauseTime;
		internal static FieldInfo dynamicPressureTrigger;
		internal static FieldInfo fixedCoast;
		internal static FieldInfo fixedCoastLength;
		internal static FieldInfo stagingTrigger;
		internal static FieldInfo stagingTriggerFlag;

		internal static void InitType(Type type) {
			desiredOrbitAltitude    = type.GetCheckedField("DesiredOrbitAltitude");
			desiredInclination      = type.GetCheckedField("DesiredInclination");
			correctiveSteering      = type.GetCheckedField("CorrectiveSteering");
			correctiveSteeringGain  = type.GetCheckedField("CorrectiveSteeringGain");
			forceRoll               = type.GetCheckedField("ForceRoll");
			verticalRoll            = type.GetCheckedField("VerticalRoll");
			turnRoll                = type.GetCheckedField("TurnRoll");
			autoDeploySolarPanels   = type.GetCheckedField("AutodeploySolarPanels");
			autoDeployAntennas      = type.GetCheckedField("AutoDeployAntennas");
			skipCircularization     = type.GetCheckedField("SkipCircularization");
			autostage               = type.GetCheckedProperty("Autostage");
			limitAoA                = type.GetCheckedField("LimitAoA");
			maxAoA                  = type.GetCheckedField("MaxAoA");
			aoALimitFadeoutPressure = type.GetCheckedField("AOALimitFadeoutPressure");
			launchPhaseAngle        = type.GetCheckedField("LaunchPhaseAngle");
			launchLANDifference     = type.GetCheckedField("LaunchLANDifference");
			warpCountDown           = type.GetCheckedField("WarpCountDown");
			launchingToPlane        = type.GetCheckedField("LaunchingToPlane");
			launchingToRendezvous   = type.GetCheckedField("LaunchingToRendezvous");
			ascentTypeInteger       = type.GetCheckedField("AscentTypeInteger");
			ascentAutopilotProp     = type.GetCheckedProperty("AscentAutopilot");

			turnStartAltitude       = type.GetCheckedField("TurnStartAltitude");
			turnStartVelocity       = type.GetCheckedField("TurnStartVelocity");
			turnEndAltitude         = type.GetCheckedField("TurnEndAltitude");
			turnEndAngle            = type.GetCheckedField("TurnEndAngle");
			turnShapeExponent       = type.GetCheckedField("TurnShapeExponent");
			autoPath                = type.GetCheckedField("AutoPath");
			autoTurnPerc            = type.GetCheckedField("AutoTurnPerc");
			autoTurnSpdFactor       = type.GetCheckedField("AutoTurnSpdFactor");
			autoTurnStartAltitude   = type.GetCheckedProperty("AutoTurnStartAltitude");
			autoTurnStartVelocity   = type.GetCheckedProperty("AutoTurnStartVelocity");
			autoTurnEndAltitude     = type.GetCheckedProperty("AutoTurnEndAltitude");

			pitchStartVelocity      = type.GetCheckedField("PitchStartVelocity");
			pitchRate               = type.GetCheckedField("PitchRate");
			desiredApoapsis         = type.GetCheckedField("DesiredApoapsis");
			desiredAttachAlt        = type.GetCheckedField("DesiredAttachAlt");
			attachAltFlag           = type.GetCheckedField("AttachAltFlag");
			desiredFPA              = type.GetCheckedField("DesiredFPA");
			minDeltaV               = type.GetCheckedField("MinDeltaV");
			maxCoast                = type.GetCheckedField("MaxCoast");
			minCoast                = type.GetCheckedField("MinCoast");
			coastStageFlag          = type.GetCheckedField("CoastStageFlag");
			coastStageInternal      = type.GetCheckedField("CoastStageInternal");
			coastStageProp          = type.GetCheckedProperty("CoastStage");
			coastBeforeFlag         = type.GetCheckedField("CoastBeforeFlag");
			spinupStageFlag         = type.GetCheckedField("SpinupStageFlag");
			spinupStageInternal     = type.GetCheckedField("SpinupStageInternal");
			spinupStageProp         = type.GetCheckedProperty("SpinupStage");
			spinupLeadTime          = type.GetCheckedField("SpinupLeadTime");
			spinupAngularVelocity   = type.GetCheckedField("SpinupAngularVelocity");
			unguidedStagesFlag      = type.GetCheckedField("UnguidedStagesFlag");
			unguidedStagesInternal  = type.GetCheckedField("UnguidedStagesInternal");
			optimizeStageFlag       = type.GetCheckedField("OptimizeStageFlag");
			optimizeStageInternal   = type.GetCheckedField("OptimizeStageInternal");
			lastStage               = type.GetCheckedField("LastStage");
			preStageTime            = type.GetCheckedField("PreStageTime");
			optimizerPauseTime      = type.GetCheckedField("OptimizerPauseTime");
			dynamicPressureTrigger  = type.GetCheckedField("DynamicPressureTrigger");
			fixedCoast              = type.GetCheckedField("FixedCoast");
			fixedCoastLength        = type.GetCheckedField("FixedCoastLength");
			stagingTrigger          = type.GetCheckedField("StagingTrigger");
			stagingTriggerFlag      = type.GetCheckedField("StagingTriggerFlag");
		}
	}

	internal static class AscentBaseAutopilotBinding {
		internal const string MechJebType = "MuMech.MechJebModuleAscentBaseAutopilot";

		internal static FieldInfo status;
		internal static FieldInfo timedLaunch;
		/// Private `_mode` enum field on the base autopilot; values are
		/// PRELAUNCH / ASCEND / CIRCULARIZE. Read via reflection.
		internal static FieldInfo mode;
		internal static MethodInfo startCountdown;

		internal static void InitType(Type type) {
			status         = type.GetCheckedField("Status");
			timedLaunch    = type.GetCheckedField("TimedLaunch");
			mode           = type.GetCheckedField("_mode", BindingFlags.NonPublic | BindingFlags.Instance);
			startCountdown = type.GetCheckedMethod("StartCountdown");
		}
	}

	/// Concrete Classic-path autopilot. Holds its own private `_mode`
	/// (VERTICAL_ASCENT / GRAVITY_TURN / COAST_TO_APOAPSIS / EXIT)
	/// independent of the base's PRELAUNCH/ASCEND/CIRCULARIZE.
	internal static class AscentClassicAutopilotBinding {
		internal const string MechJebType = "MuMech.MechJebModuleAscentClassicAutopilot";

		internal static FieldInfo mode;

		internal static void InitType(Type type) {
			mode = type.GetCheckedField("_mode", BindingFlags.NonPublic | BindingFlags.Instance);
		}
	}

	/// Concrete PSG-path autopilot. Holds its own private `_mode`
	/// (VERTICAL_ASCENT / PITCHPROGRAM / ZEROLIFT / GUIDANCE / EXIT).
	/// PSG was renamed from PVG in 2.15 — but the underlying type
	/// name is still `MechJebModuleAscentPVGAutopilot`.
	internal static class AscentPVGAutopilotBinding {
		internal const string MechJebType = "MuMech.MechJebModuleAscentPVGAutopilot";

		internal static FieldInfo mode;

		internal static void InitType(Type type) {
			mode = type.GetCheckedField("_mode", BindingFlags.NonPublic | BindingFlags.Instance);
		}
	}

	internal static class AscentMenuBinding {
		internal const string MechJebType = "MuMech.MechJebModuleAscentMenu";

		internal static MethodInfo timeToPhaseAngle;

		internal static void InitType(Type type) {
			// Private static helper in 2.15. Reflect with NonPublic|Static.
			timeToPhaseAngle = type.GetCheckedMethod(
				"TimeToPhaseAngle",
				BindingFlags.NonPublic | BindingFlags.Static);
		}
	}
}
