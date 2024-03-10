namespace Tool.Compet.Core {
	using System;
	using System.Reflection;

	public class DkObjects {
		/// Create new instance from given type.
		/// Throws exception if occured.
		public static T NewInstace<T>() where T : class {
			try {
				// Go with this option when no constructor is defined,
				// or parameterless constructor exists.
				return Activator.CreateInstance<T>();
			}
			catch {
				// Go with this option when some constructor is defined,
				// but this will ignore params.
				return (T)Activator.CreateInstance(
					typeof(T),
					BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance | BindingFlags.OptionalParamBinding,
					null,
					new Object[] { Type.Missing },
					null
				)!;
			}
		}
	}
}
