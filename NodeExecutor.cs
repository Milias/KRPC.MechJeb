using System;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;
using KRPC.Service.Attributes;

namespace KRPC.MechJeb {
	[KRPCClass(Service = "MechJeb")]
	public class NodeExecutor : KRPCComputerModule {
		internal new const string MechJebType = "MuMech.MechJebModuleNodeExecutor";

		// Fields and methods. MechJeb 2.15 dropped the public `tolerance`
		// field on NodeExecutor; the burn-finish criterion is now baked
		// into the executor's internal state machine, no longer a knob.
		private static FieldInfo autowarp;
		private static FieldInfo leadTimeField;

		private static MethodInfo executeOneNode;
		private static MethodInfo executeAllNodes;
		private static MethodInfo abort;

		// Instance objects
		private object leadTime;

		internal static new void InitType(Type type) {
			autowarp = type.GetCheckedField("Autowarp");
			leadTimeField = type.GetCheckedField("LeadTime");

			executeOneNode = type.GetCheckedMethod("ExecuteOneNode");
			executeAllNodes = type.GetCheckedMethod("ExecuteAllNodes");
			abort = type.GetCheckedMethod("Abort");
		}

		protected internal override void InitInstance(object instance) {
			base.InitInstance(instance);

			this.leadTime = leadTimeField.GetInstanceValue(instance);
		}

		[KRPCProperty]
		public override bool Enabled => base.Enabled;

		[KRPCProperty]
		public bool Autowarp {
			get => (bool)autowarp.GetValue(this.instance);
			set => autowarp.SetValue(this.instance, value);
		}

		[KRPCProperty]
		public double LeadTime {
			get => EditableDouble.Get(this.leadTime);
			set => EditableDouble.Set(this.leadTime, value);
		}

		[KRPCMethod]
		public void ExecuteOneNode() {
			executeOneNode.Invoke(this.instance, new object[] { this });
		}

		[KRPCMethod]
		public void ExecuteAllNodes() {
			executeAllNodes.Invoke(this.instance, new object[] { this });
		}

		[KRPCMethod]
		public void Abort() {
			abort.Invoke(this.instance, null);
		}
	}
}
