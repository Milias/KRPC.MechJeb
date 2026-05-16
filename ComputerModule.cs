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

			internal static void InitType(Type type) {
				usersAdd = type.GetCheckedMethod("Add");
				usersRemove = type.GetCheckedMethod("Remove");
			}
		}
	}

	public abstract class KRPCComputerModule : ComputerModule {
		[KRPCProperty]
		public override bool Enabled {
			get => base.Enabled;
			set => base.Enabled = value;
		}
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
