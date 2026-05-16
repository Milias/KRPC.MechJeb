using System;
using System.Collections.Generic;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;

namespace KRPC.MechJeb {
	public static class EditableDouble {
		internal const string MechJebType = "MuMech.EditableDoubleMult";

		// Fields and methods
		private static PropertyInfo value;

		internal static void InitType(Type type) {
			value = type.GetCheckedProperty("Val");
		}

		public static double Get(object instance) {
			return (double)value.GetValue(instance, null);
		}

		public static void Set(object instance, double value) {
			EditableDouble.value.SetValue(instance, value, null);
		}

		// Helper methods for fields which create a new object every time they are changed in GUI
		public static double Get(FieldInfo field, object instance) {
			return Get(field.GetValue(instance));
		}

		public static void Set(FieldInfo field, object instance, double value) {
			Set(field.GetValue(instance), value);
		}
	}

	public static class EditableInt {
		internal const string MechJebType = "MuMech.EditableInt";

		// Fields and methods. In MechJeb 2.15 both `Val` and `Text` became
		// properties (was: public field `val` + private field `_text`). The
		// Val setter auto-updates the backing TextConfig string, so we
		// don't have to set Text separately.
		private static PropertyInfo value;

		internal static void InitType(Type type) {
			value = type.GetCheckedProperty("Val");
		}

		public static int Get(object instance) {
			return (int)value.GetValue(instance, null);
		}

		public static void Set(object instance, int value) {
			EditableInt.value.SetValue(instance, value, null);
		}
	}

	/// <summary>
	/// MuMech.EditableIntList wraps a `List&lt;int&gt; Val` plus a text-parsing
	/// setter ("1,2,3" / "1-3"). PSG uses this for UnguidedStages and
	/// FixedStages — collections of KSP stage indices that PSG should
	/// treat specially. Read returns the backing List by reference (which
	/// kRPC then serializes); Set replaces its contents in place so the
	/// EditableIntList instance the GUI is watching stays the same.
	/// </summary>
	public static class EditableIntList {
		internal const string MechJebType = "MuMech.EditableIntList";

		// `Val` is a public readonly List<int> field on MuMech.EditableIntList
		// in 2.15 (not a property — different shape from EditableInt/Double).
		private static FieldInfo valField;

		internal static void InitType(Type type) {
			valField = type.GetCheckedField("Val");
		}

		public static IList<int> Get(object instance) {
			return (IList<int>)valField.GetValue(instance);
		}

		public static void Set(object instance, IList<int> values) {
			List<int> backing = (List<int>)valField.GetValue(instance);
			backing.Clear();
			if (values != null) {
				foreach (int v in values)
					backing.Add(v);
			}
		}
	}

	public static class MovingAverage {
		internal const string MechJebType = "MuMech.MovingAverage";

		// Fields and methods
		private static PropertyInfo value;

		internal static void InitType(Type type) {
			value = type.GetCheckedProperty("Value");
		}

		public static double Get(object instance) {
			return (double)value.GetValue(instance, null);
		}
	}
}
