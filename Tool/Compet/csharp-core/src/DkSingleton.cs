// namespace Tool.Compet.Core {
// 	/// Make singletone for subclass given type T.
// 	/// We use `ScriptableObject` to make our singleton does not be attached to game object.
// 	/// Note that, subclass must provide public empty constructor for initialization.
// 	public class DkSingleton<T> { //: ScriptableObject where T : ScriptableObject {
// 		private static T defaultInstance = default!;

// 		public static T instance {
// 			get {
// 				if (defaultInstance == null) {
// 					var type = typeof(T);

// 					lock (type) {
// 						if (defaultInstance == null) {
// 							defaultInstance = (T)System.Activator.CreateInstance(type)!;

// 							if (DkBuildConfig.DEBUG) {
// 								System.Console.WriteLine($"{type.Name}-singleton~ created defaultInstance: {defaultInstance?.GetType().Name}");
// 							}
// 						}
// 					}
// 				}
// 				return defaultInstance!;
// 			}
// 		}
// 	}
// }
