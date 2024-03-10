namespace Tool.Compet.Log {
	using System.Text.Json;

	/// Console + File log for debugging.
	/// This will log message to both Console and File.
	/// About log file path:
	/// - MacOS Editor's log path: ~/Library/Logs/Unity/Editor.log
	/// - MacOS Player's log path: ~/Library/Logs/Company Name/Product Name/Player.log
	/// Ref: https://docs.unity3d.com/Manual/LogFiles.html
	public class DkLogs {
		public static void Debug(object where, string message, object? obj = null) {
			Console.WriteLine(MakeMessage(where, "DEBUG", message, obj));
		}

		public static void Info(object where, string message, object? obj = null) {
			Console.WriteLine(MakeMessage(where, "INFO", message, obj));
		}

		public static void Notice(object where, string message, object? obj = null) {
			Console.WriteLine(MakeMessage(where, "NOTICE", message, obj));
		}

		public static void Warning(object where, string message, object? obj = null) {
			Console.WriteLine(MakeMessage(where, "WARNING", message, obj));
		}

		public static void Error(object where, string message, object? obj = null) {
			Console.WriteLine(MakeMessage(where, "ERROR", message, obj));
		}

		public static void Critical(object where, string message, object? obj = null) {
			Console.WriteLine(MakeMessage(where, "CRITICAL", message, obj));
		}

		private static string MakeMessage(object where, string type, string message, object? obj) {
			var location = (where is string) ? where : where.GetType().Name;
			if (obj != null) {
				message += ", dump_obj: " + JsonSerializer.Serialize(obj, options: new JsonSerializerOptions { WriteIndented = true });
			}
			return "_____[" + type + "] " + location + "~ " + message;
		}
	}
}
