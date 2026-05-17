using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb {
	public abstract class Module {
		protected internal abstract void InitInstance(object instance);
	}

	public abstract class ComputerModule : Module {
		internal const string MechJebType = "MuMech.ComputerModule";

		// Methods needed for correct functionalify
		private static MethodInfo onFixedUpdate;

		// Fields and methods
		// Made `internal` (was `private`) so subclasses that need to
		// read/write enabled/users on a *different* MuMech instance (e.g.
		// AscentAutopilot dispatching to the active concrete autopilot)
		// can share the same reflection cache.
		internal static PropertyInfo enabled;
		internal static FieldInfo usersField;

		// Instance objects
		protected internal object instance;

		protected internal object users;

		internal static void InitType(Type type) {
			onFixedUpdate = type.GetCheckedMethod("OnFixedUpdate");

			enabled = type.GetCheckedProperty("Enabled");
			usersField = type.GetCheckedField("Users");
		}

		protected internal override void InitInstance(object instance) {
			this.instance = instance;

			this.users = usersField.GetInstanceValue(instance);
		}

		public virtual bool Enabled {
			get => (bool)enabled.GetValue(this.instance, null);
			set {
				if(value)
					UserPool.usersAdd.Invoke(this.users, new object[] { this });
				else
					UserPool.usersRemove.Invoke(this.users, new object[] { this });
			}
		}

		internal void OnFixedUpdate() {
			onFixedUpdate.Invoke(this.instance, null);
		}

		internal static class UserPool {
			internal const string MechJebType = "MuMech.UserPool";

			internal static MethodInfo usersAdd;
			internal static MethodInfo usersRemove;
			/// `Count` property — UserPool extends `List<object>`, so this
			/// is the inherited `List<T>.Count` getter. Used by the
			/// observability surface to report how many users a module has.
			internal static PropertyInfo countProp;

			internal static void InitType(Type type) {
				usersAdd = type.GetCheckedMethod("Add");
				usersRemove = type.GetCheckedMethod("Remove");
				countProp = type.GetCheckedProperty("Count");
			}
		}
	}

	public abstract class KRPCComputerModule : ComputerModule {
		[KRPCProperty]
		public override bool Enabled {
			get => base.Enabled;
			set => base.Enabled = value;
		}

		/// <summary>
		/// Number of users requesting this module be active (MechJeb's
		/// `UserPool` semantics — module is enabled iff at least one
		/// user is registered). Useful for diagnosing "I engaged X but
		/// it isn't doing anything" cases: if a downstream dependency
		/// of X has 0 users, the cascade didn't fully wire up.
		/// </summary>
		[KRPCProperty]
		public int UsersCount =>
			this.users == null ? 0 : (int)UserPool.countProp.GetValue(this.users, null);
	}

	public abstract class AutopilotModule : KRPCComputerModule {
		internal new const string MechJebType = "MuMech.AutopilotModule";

		// Fields and methods
		internal static PropertyInfo status;

		[KRPCProperty]
		public string Status => (string)status.GetValue(this.instance, null);

		internal static new void InitType(Type type) {
			status = type.GetCheckedProperty("Status");
		}
	}

	public abstract class DisplayModule : ComputerModule { }
}
